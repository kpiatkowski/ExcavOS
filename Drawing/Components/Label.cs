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
        public class Label : Drawable {

            public string Caption {
                get { return caption; }
                set { caption = value; sb.Clear(); sb.Append(value); }
            }
            public float FontSize = 1.0f;

            public bool DropShadow;
            public TextAlignment TextAlignment;

            private readonly StringBuilder sb = new StringBuilder();
            private string caption;
            private readonly Vector2[] shadowOffsets = new Vector2[] {
                new Vector2(-1.0f, -1.0f), new Vector2(1.0f, -1.0f), new Vector2(-1.0f, 1.0f), new Vector2(1.0f, 1.0f),
            };

            public Label(string caption, TextAlignment textAlignment = TextAlignment.CENTER) : base() {
                Caption = caption;
                TextAlignment = textAlignment;
            }

            public Label(string caption, float fontSize, TextAlignment textAlignment = TextAlignment.CENTER) : base() {
                Caption = caption;
                TextAlignment = textAlignment;
                FontSize = fontSize;
            }

            public Label(string caption, float fontSize) : base() {
                Caption = caption;
                FontSize = fontSize;
            }

            public Label(string caption, Vector2 position, Vector2 size) : base(position, size) {
                Caption = caption;
            }

            protected override void DrawSprites(IMyTextSurface surface) {
                //DrawFrame(Position, Size, Color.Red);
                Vector2 textSize = surface.MeasureStringInPixels(sb, surface.Font, surface.FontSize * FontSize);
                Vector2 pos = Position;
                if (TextAlignment == TextAlignment.LEFT) {
                    pos.Y += Size.Y / 2.0f;
                } else if (TextAlignment == TextAlignment.RIGHT) {
                    pos.Y += Size.Y / 2.0f;
                    pos.X += Size.X;
                } else {
                    pos = Position + Size / 2.0f;
                }
                if (DropShadow) {
                    float shadowSpread = 2.0f;
                    foreach (Vector2 shadowOffset in shadowOffsets) {
                        MySprite shadowSprite = CreateTextSprite(Caption, surface.Font, surface.ScriptBackgroundColor, surface.FontSize * FontSize);
                        shadowSprite.Position = pos - new Vector2(0, textSize.Y / 2.0f) + shadowOffset * shadowSpread;
                        AddSprite(shadowSprite);
                    }
                }

                MySprite sprite = CreateTextSprite(Caption, surface.Font, surface.ScriptForegroundColor, surface.FontSize * FontSize);
                sprite.Position = pos - new Vector2(0, textSize.Y / 2.0f);                
                AddSprite(sprite);
            }

            private MySprite CreateTextSprite(string caption, string font, Color color, float fontSize) {
                MySprite sprite = MySprite.CreateText(caption, font, color, fontSize);
                sprite.Alignment = TextAlignment;
                return sprite;
            }
        }
    }
}
