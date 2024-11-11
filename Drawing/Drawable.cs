using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using VRage;
using VRage.Collections;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRageMath;

namespace IngameScript {
    partial class Program {
        public abstract class Drawable : Layoutable {

            protected readonly List<MySprite> sprites = new List<MySprite>();
            private readonly StringBuilder sb = new StringBuilder();
            protected IMyTextSurface surface;

            public List<MySprite> Sprites {
                get { return sprites; }
            }

            public Drawable() : base() { }

            public Drawable(Vector2 size) : base(size) { }

            public Drawable(Vector4 sizeAndPosition) : base(sizeAndPosition) { }

            public Drawable(Vector2 position, Vector2 size) : base(position, size) { }

            protected void AddSprite(MySprite sprite) {
                sprites.Add(sprite);
            }

            protected void AddSprites(IEnumerable<MySprite> sprites) {
                this.sprites.AddRange(sprites);
            }

            public void Draw(IMyTextSurface surface) {
                this.surface = surface;
                sprites.Clear();
                DrawSprites(surface);
            }

            protected abstract void DrawSprites(IMyTextSurface surface);

            protected Vector2 TranslateCenterToTopLeftCorner(Vector2 position, Vector2 size) {
                return position - size / 2.0f;
            }

            protected Vector2 TranslateTopLeftCornerToCenter(Vector2 position, Vector2 size) {
                return position + size / 2.0f;
            }

            protected void DrawRectangle(Vector2 position, Vector2 size, Color color) {
                MySprite sprite = new MySprite(SpriteType.TEXTURE, "SquareSimple", size: size, color: color);
                sprite.Position = TranslateTopLeftCornerToCenter(position, sprite.Size.Value);
                AddSprite(sprite);
            }

            protected void DrawLine(Vector2 point1, Vector2 point2, float width, Color color) {
                Vector2 position = 0.5f * (point1 + point2);
                Vector2 diff = point1 - point2;
                float length = diff.Length();
                if (length > 0) diff /= length;

                Vector2 size = new Vector2(length, width);
                float angle = (float)Math.Acos(Vector2.Dot(diff, Vector2.UnitX));
                angle *= Math.Sign(Vector2.Dot(diff, Vector2.UnitY));

                MySprite sprite = MySprite.CreateSprite("SquareSimple", position, size);
                sprite.RotationOrScale = angle;
                sprite.Color = color;
                AddSprite(sprite);
            }

            protected void DrawText(Vector2 position, string text, Color color, string font, IMyTextSurface surface, float size = 1.0f, TextAlignment textAlignment = TextAlignment.LEFT) {
                sb.Clear();
                sb.Append(text);
                Vector2 textSize = surface.MeasureStringInPixels(sb, font, size);
                MySprite sprite = MySprite.CreateText(text, font, color, size, textAlignment);
                sprite.Position = position; // - new Vector2(0, textSize.Y * 0.5f)
                AddSprite(sprite);
            }

            protected void DrawFrame(Vector2 position, Vector2 size, Color color, float borderThickness = 2.0f) {
                DrawRectangle(position, new Vector2(size.X, borderThickness), color);
                DrawRectangle(new Vector2(position.X, position.Y + size.Y - borderThickness), new Vector2(size.X, borderThickness), color);
                DrawRectangle(new Vector2(position.X, position.Y), new Vector2(borderThickness, size.Y), color);
                DrawRectangle(new Vector2(position.X + size.X - borderThickness, position.Y), new Vector2(borderThickness, size.Y), color);
            }

            protected void DrawSpriteCentered(Vector2 position, Vector2 size, string spriteName, Color color, float rotation = 0.0f) {                
                MySprite sprite;
                sprite = new MySprite(SpriteType.TEXTURE, spriteName, size: size, color: color) {
                    Position = position,
                    RotationOrScale = rotation
                };
                AddSprite(sprite);
            }

            public void DrawSprite(Vector2 position, Vector2 size, string spriteName, Color color, float rotation = 0.0f) {
                DrawSpriteCentered(TranslateTopLeftCornerToCenter(position, size), size, spriteName, color, rotation);
            }
        }
    }
}
