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
        public class Painter {

            private static readonly StringBuilder sb = new StringBuilder();

            protected static Vector2 TranslateCenterToTopLeftCorner(Vector2 position, Vector2 size) {
                return position - size / 2.0f;
            }

            protected static Vector2 TranslateTopLeftCornerToCenter(Vector2 position, Vector2 size) {
                return position + size / 2.0f;
            }

            public static MySprite DrawSprite(Vector2 position, Vector2 size, string spriteName, Color color, float rotation = 0.0f) {
                MySprite sprite = new MySprite(SpriteType.TEXTURE, spriteName, size: size, color: color) {
                    Position = position,
                    RotationOrScale = rotation
                };
                sprite.Position = TranslateTopLeftCornerToCenter(position, size);
                return sprite;
            }

            public static MySprite DrawRectangle(Vector2 position, Vector2 size, Color color) {
                MySprite sprite = DrawSprite(position, size, "SquareSimple", color);                
                return sprite;
            }

            public static MySprite DrawText(Vector2 position, string text, Color color, string font, float size = 1.0f, TextAlignment textAlignment = TextAlignment.LEFT) {
                //sb.Clear();
                //sb.Append(text);
                //Vector2 textSize = surface.MeasureStringInPixels(sb, font, size);
                MySprite sprite = MySprite.CreateText(text, font, color, size, textAlignment);
                sprite.Position = position; // - new Vector2(0, textSize.Y * 0.5f)
                return sprite;
            }

        }
    }
}
