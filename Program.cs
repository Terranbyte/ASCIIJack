using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Blackjack
{
    // red: ♥ ♦ black: ♠ ♣
    class Card : ConsoleSprite
    {
        public int Value;
        public bool Flipped;
        public string Name;

        public ConsoleSprite BlankCard;

        public Card(int value, bool flipped, SpriteOrigin origin, KeyValuePair<int, int> position, int width = 0, int height = 0, KeyValuePair<ConsoleColor, char>[,] sprite = null, ConsoleColor background = ConsoleColor.Black) : base(origin, position, width, height, sprite, background)
        {
            Value = value;
            Flipped = flipped;
            BlankCard = new ConsoleSprite(origin, position, width, height);
            Program.GetSetSpriteFromFile("../blankcard.consolesprite", ref BlankCard);
        }

        public Card(Card card) : base(card.origin, new KeyValuePair<int, int>(card.X, card.Y), card.Width, card.Height, card.Sprite, card.Background)
        {
            Value = card.Value;
            Flipped = card.Flipped;
            Name = card.Name;
            BlankCard = card.BlankCard;
        }

        public override void RenderSprite()
        {
            int writeX = X, writeY = Y;
            KeyValuePair<ConsoleColor, char>[,] sprite = Sprite;
            if (Flipped)
            {
                sprite = BlankCard.Sprite;
            }

            // Set origin to render from
            switch (origin)
            {
                case SpriteOrigin.Left:
                    writeY = Y - Height / 2;
                    break;
                case SpriteOrigin.BottomLeft:
                    writeY = Y - Height;
                    break;
                case SpriteOrigin.Bottom:
                    writeX = X - Width / 2;
                    writeY = Y - Height;
                    break;
                case SpriteOrigin.BottomRight:
                    writeX = X - Width;
                    writeY = Y - Height;
                    break;
                case SpriteOrigin.Right:
                    writeX = X - Width;
                    writeY = Y - Height / 2;
                    break;
                case SpriteOrigin.TopRight:
                    writeX = X - Width;
                    break;
                case SpriteOrigin.Top:
                    writeX = X - Width / 2;
                    break;
                case SpriteOrigin.Center:
                    writeX = X - Width / 2;
                    writeY = Y - Height / 2;
                    break;
            }

            Console.BackgroundColor = Background;

            // Render it
            for (int y = 0; y < Height; y++)
            {
                Console.SetCursorPosition(writeX, writeY + y);
                for (int x = 0; x < Width; x++)
                {
                    if (sprite[y, x].Key != Console.ForegroundColor)
                    {
                        Console.ForegroundColor = sprite[y, x].Key;
                    }
                    Console.Out.Write(sprite[y, x].Value);
                }
            }
        }

        // Deep copy
        public Card Clone()
        {
            Card sprite = new Card(this);
            return sprite;
        }
    }

    class Program
    {
        // Draws a card from a card pool
        static Card DrawCard(ref Random rng, ref List<string> cardPool, int xOffsetMult, int Y)
        {
            Card card = new Card(0, false, SpriteOrigin.BottomLeft, new KeyValuePair<int, int>(2 + 12 * xOffsetMult, Y));
            
            // Get the sprite
            int i = rng.Next(0, cardPool.Count);
            GetSetSpriteFromFile($"../{cardPool[i]}.consolesprite", ref card);
            
            card.Name = $"{cardPool[i].Split('/')[1]} of {cardPool[i].Split('/')[0]}";
            try
            {
                card.Value = int.Parse(cardPool[i].Split('/')[1]);
            }
            catch
            {
                switch (cardPool[i].Split('/')[1].ToLower())
                {
                    case "ace":
                        card.Value = 11;
                        break;
                    default:
                        card.Value = 10;
                        break;
                }
            }

            cardPool.RemoveAt(i);
            return card;
        }

        static void WriteText(string text, int X, int Y)
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition(X - text.Length, Y);

            Console.SetCursorPosition(X - text.Length / 2, Y);
            Console.Out.WriteAsync(text);
        }

        public static void GetSetSpriteFromFile<T>(string path, ref T refSprite) where T : ConsoleSprite
        {
            if (path.Split('.')[path.Split('.').Length - 1] != "consolesprite")
            {
                throw new FileLoadException($"File \"{path}\" is not a valid console sprite file");
            }

            string[] text;
            int Width, Height, SpriteX = 0, SpriteY = 0;

            try
            {
                using (StreamReader sr = new StreamReader(path))
                {
                    string size = sr.ReadLine();
                    Width = Convert.ToInt32(size.Split(',')[0]);
                    Height = Convert.ToInt32(size.Split(',')[1]);
                    text = new string[Height];
                    for (int i = 0; i < Height; i++)
                    {
                        text[i] = sr.ReadLine().Replace("\r", string.Empty).Replace("\n", string.Empty);
                    }
                }
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
            
            KeyValuePair<ConsoleColor, char>[,] Sprite = new KeyValuePair<ConsoleColor, char>[Height, Width];

            bool setColor = false, repeatColor = true;

            // Loops for setting the sprite
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < text[y].Length; x++)
                {
                    char c = text[y][x];

                    SetColor:
                    if (setColor)
                    {
                        // I can just read the number and cast it as ConsoleColor but then I can't decide what color is what number
                        switch (c)
                        {
                            case '0':
                                Sprite[SpriteY, SpriteX] = new KeyValuePair<ConsoleColor, char>(ConsoleColor.Black, Sprite[SpriteY, SpriteX].Value);
                                break;
                            case '1':
                                Sprite[SpriteY, SpriteX] = new KeyValuePair<ConsoleColor, char>(ConsoleColor.White, Sprite[SpriteY, SpriteX].Value);
                                break;
                            case '2':
                                Sprite[SpriteY, SpriteX] = new KeyValuePair<ConsoleColor, char>(ConsoleColor.Red, Sprite[SpriteY, SpriteX].Value);
                                break;
                            case '3':
                                Sprite[SpriteY, SpriteX] = new KeyValuePair<ConsoleColor, char>(ConsoleColor.Green, Sprite[SpriteY, SpriteX].Value);
                                break;
                            case '4':
                                Sprite[SpriteY, SpriteX] = new KeyValuePair<ConsoleColor, char>(ConsoleColor.Blue, Sprite[SpriteY, SpriteX].Value);
                                break;
                            case '5':
                                Sprite[SpriteY, SpriteX] = new KeyValuePair<ConsoleColor, char>(ConsoleColor.Cyan, Sprite[SpriteY, SpriteX].Value);
                                break;
                            case '6':
                                Sprite[SpriteY, SpriteX] = new KeyValuePair<ConsoleColor, char>(ConsoleColor.DarkBlue, Sprite[SpriteY, SpriteX].Value);
                                break;
                            case '7':
                                Sprite[SpriteY, SpriteX] = new KeyValuePair<ConsoleColor, char>(ConsoleColor.DarkCyan, Sprite[SpriteY, SpriteX].Value);
                                break;
                            case '8':
                                Sprite[SpriteY, SpriteX] = new KeyValuePair<ConsoleColor, char>(ConsoleColor.DarkGray, Sprite[SpriteY, SpriteX].Value);
                                break;
                            case '9':
                                Sprite[SpriteY, SpriteX] = new KeyValuePair<ConsoleColor, char>(ConsoleColor.DarkGreen, Sprite[SpriteY, SpriteX].Value);
                                break;
                            case 'A':
                                Sprite[SpriteY, SpriteX] = new KeyValuePair<ConsoleColor, char>(ConsoleColor.DarkMagenta, Sprite[SpriteY, SpriteX].Value);
                                break;
                            case 'B':
                                Sprite[SpriteY, SpriteX] = new KeyValuePair<ConsoleColor, char>(ConsoleColor.DarkRed, Sprite[SpriteY, SpriteX].Value);
                                break;
                            case 'C':
                                Sprite[SpriteY, SpriteX] = new KeyValuePair<ConsoleColor, char>(ConsoleColor.DarkYellow, Sprite[SpriteY, SpriteX].Value);
                                break;
                            case 'D':
                                Sprite[SpriteY, SpriteX] = new KeyValuePair<ConsoleColor, char>(ConsoleColor.Gray, Sprite[SpriteY, SpriteX].Value);
                                break;
                            case 'E':
                                Sprite[SpriteY, SpriteX] = new KeyValuePair<ConsoleColor, char>(ConsoleColor.Magenta, Sprite[SpriteY, SpriteX].Value);
                                break;
                            case 'F':
                                Sprite[SpriteY, SpriteX] = new KeyValuePair<ConsoleColor, char>(ConsoleColor.Yellow, Sprite[SpriteY, SpriteX].Value);
                                break;
                        }

                        setColor = false;
                        repeatColor = false;
                        continue;
                    }

                    switch (c)
                    {
                        case '&':
                            ++x;
                            c = text[y][x];

                            // I can just read the number and cast it as ConsoleColor but then I can't decide what color is what number
                            switch (c)
                            {
                                case '0':
                                    refSprite.Background = ConsoleColor.Black;
                                    break;
                                case '1':
                                    refSprite.Background = ConsoleColor.White;
                                    break;
                                case '2':
                                    refSprite.Background = ConsoleColor.Red;
                                    break;
                                case '3':
                                    refSprite.Background = ConsoleColor.Green;
                                    break;
                                case '4':
                                    refSprite.Background = ConsoleColor.Blue;
                                    break;
                                case '5':
                                    refSprite.Background = ConsoleColor.Cyan;
                                    break;
                                case '6':
                                    refSprite.Background = ConsoleColor.DarkBlue;
                                    break;
                                case '7':
                                    refSprite.Background = ConsoleColor.DarkCyan;
                                    break;
                                case '8':
                                    refSprite.Background = ConsoleColor.DarkGray;
                                    break;
                                case '9':
                                    refSprite.Background = ConsoleColor.DarkGreen;
                                    break;
                                case 'A':
                                    refSprite.Background = ConsoleColor.DarkMagenta;
                                    break;
                                case 'B':
                                    refSprite.Background = ConsoleColor.DarkRed;
                                    break;
                                case 'C':
                                    refSprite.Background = ConsoleColor.DarkYellow;
                                    break;
                                case 'D':
                                    refSprite.Background = ConsoleColor.Gray;
                                    break;
                                case 'E':
                                    refSprite.Background = ConsoleColor.Magenta;
                                    break;
                                case 'F':
                                    refSprite.Background = ConsoleColor.Yellow;
                                    break;
                            }
                            continue;
                        case '%':
                            setColor = true;
                            continue;
                        default:
                            if (!setColor && repeatColor) 
                            {
                                ConsoleColor old;
                                if (SpriteY > 0 && SpriteX == 0) // If previous color was on the row above the currect
                                {
                                    old = Sprite[SpriteY - 1, Width - 1].Key;
                                    Sprite[SpriteY, SpriteX] = new KeyValuePair<ConsoleColor, char>(old, c);
                                }
                                else if (SpriteX > 0) // if the previous color is on this row
                                {
                                    old = Sprite[SpriteY, SpriteX - 1].Key;
                                    Sprite[SpriteY, SpriteX] = new KeyValuePair<ConsoleColor, char>(old, c);
                                }
                            }

                            if (setColor)
                            {
                                // If the program reaches this point, something messed up and it needs to backtrack.
                                // It's probably redundant but I don't trust this function at all, what the hell was my thought process when writing this.
                                goto SetColor;
                            }

                            if (!repeatColor)
                            {
                                repeatColor = true;
                            }

                            Sprite[SpriteY, SpriteX] = new KeyValuePair<ConsoleColor, char>(Sprite[SpriteY, SpriteX].Key, c);
                            ++SpriteX;
                            continue;
                    }
                }

                SpriteX = 0;
                ++SpriteY;
            }

            refSprite.Sprite = Sprite;
            refSprite.Width = Width;
            refSprite.Height = Height;
        }

        static void Main(string[] args)
        {
            // Top text box center is 121, 4

            // Initialize Console
            const int screenWidth = 240, screenHeight = 132;
            Console.SetBufferSize(screenWidth, screenHeight);
            Console.SetWindowSize(Console.LargestWindowWidth, Console.LargestWindowHeight);
            Console.SetWindowPosition(0, 0);
            Console.CursorVisible = false;

            // Initialize Sprites
            ConsoleSprite GUI = new ConsoleSprite(SpriteOrigin.TopLeft, new KeyValuePair<int, int>(0, 0));
            ConsoleSprite msgBox = new ConsoleSprite(SpriteOrigin.Center, new KeyValuePair<int, int>(screenWidth / 2, screenHeight / 2));
            Card card = new Card(0, false, SpriteOrigin.Center, new KeyValuePair<int, int>(15, 10));

            GetSetSpriteFromFile("../GUI.consolesprite", ref GUI);
            GetSetSpriteFromFile("../Msg Box.consolesprite", ref msgBox);

            // Initialize Game
            Random rng = new Random();
            List<string> CardPoll = new List<string>()
            {
                "Clubs/Ace",
                "Clubs/2",
                "Clubs/3",
                "Clubs/4",
                "Clubs/5",
                "Clubs/6",
                "Clubs/7",
                "Clubs/8",
                "Clubs/9",
                "Clubs/10",
                "Clubs/Jack",
                "Clubs/Queen",
                "Clubs/King",

                "Spades/Ace",
                "Spades/2",
                "Spades/3",
                "Spades/4",
                "Spades/5",
                "Spades/6",
                "Spades/7",
                "Spades/8",
                "Spades/9",
                "Spades/10",
                "Spades/Jack",
                "Spades/Queen",
                "Spades/King",

                "Diamonds/Ace",
                "Diamonds/2",
                "Diamonds/3",
                "Diamonds/4",
                "Diamonds/5",
                "Diamonds/6",
                "Diamonds/7",
                "Diamonds/8",
                "Diamonds/9",
                "Diamonds/10",
                "Diamonds/Jack",
                "Diamonds/Queen",
                "Diamonds/King",

                "Hearts/Ace",
                "Hearts/2",
                "Hearts/3",
                "Hearts/4",
                "Hearts/5",
                "Hearts/6",
                "Hearts/7",
                "Hearts/8",
                "Hearts/9",
                "Hearts/10",
                "Hearts/Jack",
                "Hearts/Queen",
                "Hearts/King",
            };

            List<Card> DealerHand = new List<Card>();
            List<Card> PlayerHand = new List<Card>();

            // Startup
            GUI.RenderSprite(); 
            WriteText("Blackjack", 121, 4);

            msgBox.RenderSprite();
            WriteText("Press any key to start", screenWidth / 2, screenHeight / 2);
            while (!Keyboard.IsAnyKeyDown())
            {

            }

            msgBox.RemoveRender();

            // Draw first cards
            card = DrawCard(ref rng, ref CardPoll, DealerHand.Count, 23);
            card.RenderSprite();
            DealerHand.Add(card.Clone());
            Thread.Sleep(200);

            card = DrawCard(ref rng, ref CardPoll, PlayerHand.Count, 129);
            card.RenderSprite();
            PlayerHand.Add(card.Clone());
            Thread.Sleep(200);

            card = DrawCard(ref rng, ref CardPoll, DealerHand.Count, 23);
            card.Flipped = true;
            card.RenderSprite();
            DealerHand.Add(card.Clone());
            card.Flipped = false;
            Thread.Sleep(200);

            card = DrawCard(ref rng, ref CardPoll, PlayerHand.Count, 129);
            card.RenderSprite();
            PlayerHand.Add(card.Clone());
            Thread.Sleep(200);

            // Check for a Blackjack
            if (PlayerHand[0].Value + PlayerHand[1].Value == 21)
            {
                msgBox.RenderSprite();
                WriteText("Blackjack!", screenWidth / 2, screenHeight / 2 - 1);
                WriteText("You win!", screenWidth / 2, screenHeight / 2);

                Console.SetCursorPosition(0, 0);
                Environment.Exit(0);
            }

            // Player loop
            while (true)
            {
                Hit:
                int temp = 0;

                foreach (Card drawn in PlayerHand)
                {
                    temp += drawn.Value;
                }

                if (temp > 21)
                {
                    msgBox.RenderSprite();
                    WriteText("You lose!", screenWidth / 2, screenHeight / 2);

                    Console.SetCursorPosition(0, 0);
                    Environment.Exit(0);
                }

                msgBox.RenderSprite();
                WriteText("Hit(1) or Stay(2)?", screenWidth / 2, screenHeight / 2);

                while (true)
                {
                    if (Keyboard.IsKeyPressed(ConsoleKey.D1))
                    {
                        msgBox.RemoveRender();

                        card = DrawCard(ref rng, ref CardPoll, PlayerHand.Count, 129);
                        card.RenderSprite();
                        PlayerHand.Add(card.Clone());
                        Thread.Sleep(200);

                        goto Hit;
                    }
                    else if (Keyboard.IsKeyPressed(ConsoleKey.D2))
                    {
                        msgBox.RemoveRender();
                        break;
                    }
                }

                break;
            }

            int PlayerSum = 0;
            foreach (Card drawn in PlayerHand)
            {
                PlayerSum += drawn.Value;
            }

            DealerHand[1].Flipped = false;
            DealerHand[1].RenderSprite();

            // Dealer loop
            while (true)
            {
                Hit:
                int temp = 0;

                foreach (Card drawn in DealerHand)
                {
                    temp += drawn.Value;
                }

                if (temp >= 17)
                {
                    break;
                }

                card = DrawCard(ref rng, ref CardPoll, DealerHand.Count, 23);
                card.RenderSprite();
                DealerHand.Add(card.Clone());
                Thread.Sleep(500 + rng.Next(1, 500));
            }

            int DealerSum = 0;

            foreach (Card drawn in DealerHand)
            {
                DealerSum += drawn.Value;
            }

            int PlayerState = PlayerSum == 21 ? 1 : 0;
            int DealerState = DealerSum == 21 ? 1 : 0;

            if (DealerSum > 21)
            {
                DealerState = 2;
            }

            // Decide winner
            switch (DealerState)
            {
                case 0: // Less than 21
                    switch (PlayerState)
                    {
                        case 0:
                            if (PlayerSum > DealerSum)
                            {
                                msgBox.RenderSprite();
                                WriteText("You win!", screenWidth / 2, screenHeight / 2);
                            }
                            else
                            {
                                msgBox.RenderSprite();
                                WriteText("You lose!", screenWidth / 2, screenHeight / 2);
                            }
                            break;
                        case 1:
                            msgBox.RenderSprite();
                            WriteText("You win!", screenWidth / 2, screenHeight / 2);
                            break;
                    }
                    break;
                case 1: // 21
                    switch (PlayerState)
                    {
                        case 0:
                            msgBox.RenderSprite();
                            WriteText("You lose!", screenWidth / 2, screenHeight / 2);
                            break;
                        case 1:
                            msgBox.RenderSprite();
                            WriteText("You win!", screenWidth / 2, screenHeight / 2);
                            break;
                    }
                    break;
                case 2: // More than 21
                    msgBox.RenderSprite();
                    WriteText("You win!", screenWidth / 2, screenHeight / 2);
                    break;
            }

            Console.SetCursorPosition(0, 0);
            Environment.Exit(0);
        }
    }
}
