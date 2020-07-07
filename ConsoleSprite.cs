using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Blackjack
{
    public enum SpriteOrigin
    {
        TopLeft = 0,
        Left = 1,
        BottomLeft = 3,
        Bottom = 4,
        BottomRight = 5,
        Right = 6,
        TopRight = 7,
        Top = 8,
        Center = 9
    }

    class ConsoleSprite
    {
        protected SpriteOrigin origin;

        public int X, Y, Width, Height;
        public KeyValuePair<ConsoleColor, char>[,] Sprite;
        public ConsoleColor Background;
        public ConsoleSprite(SpriteOrigin origin, KeyValuePair<int, int> position, int width = 0, int height = 0, KeyValuePair<ConsoleColor, char>[,] sprite = null, ConsoleColor background = ConsoleColor.Black)
        {
            this.origin = origin;
            X = position.Key;
            Y = position.Value;
            Width = width;
            Height = height;
            Sprite = sprite;
            Background = background;
        }

        public ConsoleSprite(ConsoleSprite sprite)
        {
            origin = sprite.origin;
            X = sprite.X;
            Y = sprite.Y;
            Width = sprite.Width;
            Height = sprite.Height;
            Sprite = sprite.Sprite;
            Background = sprite.Background;
        }

        public virtual void RenderSprite()
        {
            int writeX = X, writeY = Y;

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
                    if (Sprite[y, x].Key != Console.ForegroundColor)
                    {
                        Console.ForegroundColor = Sprite[y, x].Key;
                    }
                    Console.Out.Write(Sprite[y, x].Value);
                }
            }
        }

        // Same as RenderSprite, but it removes everything in that area
        public void RemoveRender()
        {
            int writeX = X, writeY = Y;

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

            for (int y = 0; y < Height; y++)
            {
                Console.SetCursorPosition(writeX, writeY + y);
                for (int x = 0; x < Width; x++)
                {
                    Console.Out.Write(' ');
                }
            }
        }

        public ConsoleSprite Clone()
        {
            ConsoleSprite sprite = new ConsoleSprite(this);
            return sprite;
        }
    }
}
