using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System;
using VRage.Collections;
using VRage.Game.Components;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Game;
using VRage;
using VRageMath;

namespace IngameScript
{
    partial class Program
    {
        public class Painter
        {
            private static IMyTextSurface _surface;
            private static MySpriteDrawFrame _frame;
            private static Vector2 _offset;

            public static float Width;
            public static float Height;
            public static Vector2 Center;
            public static Vector2 AvailableSize;
            public static Color PrimaryColor;
            public static Color BackgroundColor;
            public static Color SecondaryColor;

            public static void SetCurrentSurfaceAndFrame(IMyTextSurface surface, MySpriteDrawFrame frame)
            {
                _surface = surface;
                _frame = frame;
                _offset = (_surface.TextureSize - _surface.SurfaceSize) / 2.0f;

                Width = _surface.SurfaceSize.X;
                Height = _surface.SurfaceSize.Y;
                Center = new Vector2(Width, Height) / 2.0f + _offset;
                AvailableSize = new Vector2(Width, Height);

                Vector3 hsv = surface.ScriptForegroundColor.ColorToHSV();
                PrimaryColor = _surface.ScriptForegroundColor;
                BackgroundColor = _surface.ScriptBackgroundColor;
                SecondaryColor = (hsv.Z < 0.5f) ? Color.Lighten(PrimaryColor, 0.3f) : Color.Darken(PrimaryColor, 0.3f);
            }

            private static Vector2 TranslateCenterToTopLeftCorner(Vector2 position, Vector2 size)
            {
                return position - size / 2.0f;
            }

            private static Vector2 TranslateTopLeftCornerToCenter(Vector2 position, Vector2 size)
            {
                return position + size / 2.0f;
            }

            public static void RectangleEx(Vector2 position, Vector2 size, float borderThickness = 1.0f, Color? color = null)
            {
                if (!color.HasValue)
                {
                    color = _surface.ScriptForegroundColor;
                }
                MySprite sprite;
                sprite = new MySprite(SpriteType.TEXTURE, "SquareSimple", size: new Vector2(size.X, borderThickness), color: color);
                sprite.Position = TranslateTopLeftCornerToCenter(position, sprite.Size.Value) + _offset;
                _frame.Add(sprite);
                sprite = new MySprite(SpriteType.TEXTURE, "SquareSimple", size: new Vector2(size.X, borderThickness), color: color);
                sprite.Position = TranslateTopLeftCornerToCenter(new Vector2(position.X, position.Y + size.Y - borderThickness), sprite.Size.Value) + _offset;
                _frame.Add(sprite);
                sprite = new MySprite(SpriteType.TEXTURE, "SquareSimple", size: new Vector2(borderThickness, size.Y), color: color);
                sprite.Position = TranslateTopLeftCornerToCenter(position, sprite.Size.Value) + _offset;
                _frame.Add(sprite);
                sprite = new MySprite(SpriteType.TEXTURE, "SquareSimple", size: new Vector2(borderThickness, size.Y), color: color);
                sprite.Position = TranslateTopLeftCornerToCenter(new Vector2(position.X + size.X - borderThickness, position.Y), sprite.Size.Value) + _offset;
                _frame.Add(sprite);
            }

            public static void FilledRectangleEx(Vector2 position, Vector2 size, Color color, float rotation = 0.0f)
            {
                MySprite sprite;
                sprite = new MySprite(SpriteType.TEXTURE, "SquareSimple", size: size, color: color)
                {
                    Position = TranslateTopLeftCornerToCenter(position, size) + _offset,
                    RotationOrScale = rotation
                    
                };
                _frame.Add(sprite);
            }

            public static void Rectangle(Vector2 position, Vector2 size, float borderThickness = 1.0f)
            {
                RectangleEx(position, size, borderThickness, _surface.ScriptForegroundColor);
            }

            public static void FilledRectangle(Vector2 position, Vector2 size, float rotation = 0.0f)
            {
                FilledRectangleEx(position, size, _surface.ScriptForegroundColor, rotation);
            }

            public static void SpriteCentered(Vector2 position, Vector2 size, string spriteName, Color? color = null, float rotation = 0.0f)
            {
                if (!color.HasValue)
                {
                    color = _surface.ScriptForegroundColor;
                }
                MySprite sprite;
                sprite = new MySprite(SpriteType.TEXTURE, spriteName, size: size, color: color.Value)
                {
                    Position = position,
                    RotationOrScale = rotation
                };
                _frame.Add(sprite);
            }

            public static void Sprite(Vector2 position, Vector2 size, string spriteName, Color? color = null, float rotation = 0.0f)
            {
                SpriteCentered(TranslateTopLeftCornerToCenter(position, size) + _offset, size, spriteName, color, rotation);
            }

            public static void TextEx(Vector2 position, Color color, string text, float fontSize = 1.0f, TextAlignment textAlignment = TextAlignment.CENTER)
            {
                MySprite sprite;
                sprite = MySprite.CreateText(text, _surface.Font, color, fontSize, textAlignment);
                sprite.Position = position + _offset;
                _frame.Add(sprite);
            }

            public static void Text(Vector2 position, string text, float fontSize = 1.0f, TextAlignment textAlignment = TextAlignment.CENTER)
            {
                TextEx(position, _surface.ScriptForegroundColor, text, fontSize, textAlignment);
            }

            public static void Radial(Vector2 position, Vector2 size, float value, string subText = "", int bars = 20, bool flip = false)
            {
                if (value < 0.0f) value = 0;
                if (value > 1.0f) value = 1.0f;
                Color secondary = new Color(SecondaryColor, 0.1f);                
                Vector2 barPosition, barSize;
                MySprite sprite;
                barSize = new Vector2(size.X / 256 * 20.0f, size.X / 128 * 4.0f);
                float radius = (size.X - barSize.X) / 2.0f;
                float fontSize = 0.5f + size.X / 256.0f;
                Vector2 origin = new Vector2(position.X + radius, flip ? position.Y + barSize.Y : position.Y + size.Y);
                string text = string.Format("{0:0.00}%", value * 100);
                StringBuilder sb = new StringBuilder();
                sb.Append(text);
                Vector2 mainTextSize = _surface.MeasureStringInPixels(sb, _surface.Font, fontSize);
                sb.Clear();
                sb.Append(subText);
                Vector2 subTextSize = _surface.MeasureStringInPixels(sb, _surface.Font, fontSize / 2.0f);
                for (int n = 0; n <= bars; n++)
                {
                    float angle = -(float)Math.PI / 2.0f + (flip ? -n : n) * ((float)Math.PI / bars);
                    float v = (float)n / bars;
                    float barScale = 0.2f + v * 0.8f;
                    barPosition = new Vector2((float)(radius * Math.Sin(angle)) + barSize.X / 2.0f, -(float)(radius * Math.Cos(angle)) - barSize.Y / 2.0f);
                    sprite = new MySprite(SpriteType.TEXTURE, "SquareSimple", size: new Vector2(barSize.X, barSize.Y * barScale), color: value > v ? _surface.ScriptForegroundColor : secondary)
                    {
                        Position = origin + barPosition + _offset,
                        RotationOrScale = angle + (float)Math.PI / 2.0f
                    };
                    _frame.Add(sprite);
                    Vector2 textPosition = new Vector2(position.X + size.X / 2.0f, flip ? position.Y : origin.Y - mainTextSize.Y);
                    Text(textPosition, text, fontSize);
                    textPosition.Y += flip ? mainTextSize.Y : -subTextSize.Y;
                    TextEx(textPosition, SecondaryColor, subText, fontSize / 2.0f);
                }
            }

            public static void ProgressBar(Vector2 position, Vector2 size, float value, float borderThickness = 1.0f, string sprite = "")
            {
                if (value < 0.0f) value = 0.0f;
                if (value > 1.0f) value = 1.0f;
                RectangleEx(position, size, borderThickness, SecondaryColor);
                size -= 2 * borderThickness;
                FilledRectangle(position + borderThickness, new Vector2(size.X * value, size.Y));
                if (sprite != "")
                {
                    Vector2 spriteSize = new Vector2(size.Y, size.Y);
                    Vector2 center = position + size / 2.0f - spriteSize / 2.0f;
                    Sprite(center, spriteSize, sprite, SecondaryColor);
                }                
            }

            public static void ProgressBarVertical(Vector2 position, Vector2 size, float value, float borderThickness = 1.0f, string sprite = "")
            {
                if (value < 0.0f) value = 0.0f;
                if (value > 1.0f) value = 1.0f;
                RectangleEx(position, size, borderThickness, SecondaryColor);
                size -= 2 * borderThickness;
                Vector2 spriteSize = new Vector2(size.X / 2.0f, (size.X / 2.0f));
                Vector2 center = position + size / 2.0f - spriteSize / 2.0f;

                position += borderThickness;
                FilledRectangle(new Vector2(position.X, position.Y + size.Y * (1.0f - value)), new Vector2(size.X, size.Y * value));
                if (sprite != "")
                {
                    Sprite(center, spriteSize, sprite, SecondaryColor);
                }
            }

        }
    }
}
