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
        public class OldPainter
        {
            public const float TEXT_SIZE_PER_UNIT = 18;

            public struct Paint
            {
                public Color PrimaryColor;
                public Color SecondaryColor;
                public Color BackgroundColor;
                public string Font;
                public Vector2 Offset;
                public float AvailableWidth;
                public float AvailableHeight;
                public Vector2 Center;
                //public ScreenSize ScreenSize;
            }

            public static Paint paint;
            public static MySpriteDrawFrame frame;

            public static void SetPaintFromSurface(IMyTextSurface surface, double secondaryColorShade = 0.2f)
            {
                paint.Font = surface.Font;
                paint.PrimaryColor = surface.ScriptForegroundColor;
                paint.BackgroundColor = surface.ScriptBackgroundColor;
                Vector3 hsv = surface.ScriptForegroundColor.ColorToHSV();
                paint.SecondaryColor = (hsv.Z < 0.5f) ? Color.Lighten(paint.PrimaryColor, secondaryColorShade) : Color.Darken(paint.PrimaryColor, secondaryColorShade);
                paint.Offset = new Vector2(0, (surface.TextureSize.Y - surface.SurfaceSize.Y) / 2.0f);
                paint.AvailableWidth = surface.SurfaceSize.X;
                paint.AvailableHeight = surface.SurfaceSize.Y;
                paint.Center = surface.SurfaceSize / 2.0f;
            }

            public static Vector2 TranslateCenterToTopLeftCorner(Vector2 position, Vector2 size)
            {
                return position - size / 2.0f;
            }

            public static Vector2 TranslateTopLeftCornerToCenter(Vector2 position, Vector2 size)
            {
                return position + size / 2.0f;
            }

            public static void DrawText(Vector2 position, Color color, string text, float fontSize = 1.0f, TextAlignment textAlignment = TextAlignment.CENTER)
            {
                MySprite sprite;
                sprite = MySprite.CreateText(text, paint.Font, color, fontSize, textAlignment);
                sprite.Position = position + paint.Offset;
                frame.Add(sprite);
            }

            public static void DrawRectangle(Color color, Vector2 position, Vector2 size, float thickness = 1.0f)
            {
                MySprite sprite;
                sprite = new MySprite(SpriteType.TEXTURE, "SquareSimple", size: new Vector2(size.X, thickness), color: color);
                sprite.Position = TranslateTopLeftCornerToCenter(position, sprite.Size.Value) + paint.Offset;
                frame.Add(sprite);
                sprite = new MySprite(SpriteType.TEXTURE, "SquareSimple", size: new Vector2(size.X, thickness), color: color);
                sprite.Position = TranslateTopLeftCornerToCenter(new Vector2(position.X, position.Y + size.Y - thickness), sprite.Size.Value) + paint.Offset;
                frame.Add(sprite);
                sprite = new MySprite(SpriteType.TEXTURE, "SquareSimple", size: new Vector2(thickness, size.Y), color: color);
                sprite.Position = TranslateTopLeftCornerToCenter(position, sprite.Size.Value) + paint.Offset;
                frame.Add(sprite);
                sprite = new MySprite(SpriteType.TEXTURE, "SquareSimple", size: new Vector2(thickness, size.Y), color: color);
                sprite.Position = TranslateTopLeftCornerToCenter(new Vector2(position.X + size.X - thickness, position.Y), sprite.Size.Value) + paint.Offset;
                frame.Add(sprite);
            }

            public static void DrawFilledRectangle(Color color, Vector2 position, Vector2 size)
            {
                MySprite sprite;
                sprite = new MySprite(SpriteType.TEXTURE, "SquareSimple", size: size, color: color)
                {
                    Position = TranslateTopLeftCornerToCenter(position, size) + paint.Offset
                };
                frame.Add(sprite);
            }

            public static void DrawProgressBar(Vector2 position, Vector2 size, double value, float borderThickness = 1.0f, Color? outerColor = null, Color? innerColor = null)
            {
                if  (!outerColor.HasValue)
                {
                    outerColor = paint.PrimaryColor;
                }
                if (!innerColor.HasValue)
                {
                    innerColor = paint.SecondaryColor;
                }
                DrawRectangle(outerColor.Value, position, size, borderThickness);
                Vector2 barSize = size - borderThickness * 2.0f;
                if (value < 0.0f) value = 0.0f;
                if (value > 1.0f) value = 1.0f;
                barSize.X *= (float)value;
                DrawFilledRectangle(innerColor.Value, position + borderThickness, barSize);
            }

            public static void DrawSprite(Vector2 position, Vector2 size, string spriteName, Color? color = null)
            {
                if (!color.HasValue)
                {
                    color = paint.PrimaryColor;
                }
                MySprite sprite;
                sprite = new MySprite(SpriteType.TEXTURE, spriteName, size: size, color: color.Value);
                sprite.Position = TranslateTopLeftCornerToCenter(position, sprite.Size.Value) + paint.Offset;
                frame.Add(sprite);                
            }

            public static void DrawVerticalTriBar(Vector2 position, Vector2 size, string min, string current, string maxSoft, string maxHard, float value, float softValue)
            {
                float barWidth = size.X * 0.5f;
                Vector2 pos = new Vector2(position.X, position.Y);

                Vector2 barPosition = new Vector2(position.X + (size.X - barWidth) / 2.0f, position.Y);
                Vector2 barSize = new Vector2(barWidth, size.Y);
                DrawFilledRectangle(paint.SecondaryColor, new Vector2(barPosition.X, barPosition.Y + barSize.Y * (1.0f - softValue)), new Vector2(barSize.X, barSize.Y * softValue));
                DrawRectangle(paint.PrimaryColor, barPosition, barSize);

                float barCount = 9;
                float barPadding = 8.0f;
                float barGap = size.Y / (barCount + 1);
                Vector2 lineSize = new Vector2(barSize.X - 2 * barPadding, 1.0f);
                Vector2 linePosition = new Vector2(barPosition.X + barPadding, barPosition.Y + barGap);
                for (int bar = 0; bar < barCount; bar++)
                {
                    DrawFilledRectangle(paint.PrimaryColor, linePosition, lineSize);
                    linePosition.Y += barGap;
                }

                pos.X = position.X + barWidth + (size.X - barWidth) / 2.0f + 10.0f;
                pos.Y = position.Y;
                DrawText(pos, paint.SecondaryColor, maxHard, 1.0f, TextAlignment.LEFT);

                pos.Y = position.Y + size.Y - TEXT_SIZE_PER_UNIT * 1.0f;
                DrawText(pos, paint.SecondaryColor, min, 1.0f,  TextAlignment.LEFT);

                if (maxSoft != maxHard)
                {
                    pos.Y = position.Y + size.Y * (1.0f - softValue) - TEXT_SIZE_PER_UNIT * 1.0f;
                    DrawText(pos, paint.SecondaryColor, maxSoft, 1.0f, TextAlignment.LEFT);
                }

                float markerSize = 20.0f;
                pos.X = position.X + (size.X - barWidth) / 2.0f - 10.0f - markerSize;
                pos.Y = position.Y + size.Y * (1.0f - value) - markerSize / 2.0f;
                MySprite sprite;
                sprite = new MySprite(SpriteType.TEXTURE, "Triangle", size: new Vector2(markerSize), color: paint.PrimaryColor);
                sprite.Position = TranslateTopLeftCornerToCenter(pos, sprite.Size.Value);
                sprite.RotationOrScale = 1.57079633f;
                frame.Add(sprite);

            }

            public static void DrawButton(Vector2 position, Vector2 size, string label, bool toggled, float fontSize = 1.0f)
            {
                Color color = toggled ? paint.PrimaryColor : paint.SecondaryColor;
                DrawFilledRectangle(color, position, size);
                Vector2 labelPosition = position + size / 2.0f;
                labelPosition.Y -= fontSize * TEXT_SIZE_PER_UNIT;
                DrawText(labelPosition, paint.BackgroundColor, label, fontSize);
            }
            
        }
    }
}
