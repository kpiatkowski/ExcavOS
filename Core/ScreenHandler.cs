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
        abstract public class ScreenHandler {

            public readonly ExcavOS excavOS;
            protected readonly List<Layoutable> components = new List<Layoutable>();
            private readonly StringBuilder sb = new StringBuilder();
            private bool makeSpriteCacheDirty = false;
            protected readonly IMyTextSurface Surface;
            public readonly Vector2 Offset;
            public readonly Vector2 Center;
            public readonly Vector2 AvailableSize;
            public readonly Color PrimaryColor;
            public readonly Color SecondaryColor;
            public readonly Color BackgroundColor;
            public virtual UpdateFrequency UpdateFrequency { get; } = UpdateFrequency.Update100;

            public ScreenHandler(ExcavOS excavOS, IMyTextSurface surface) {
                this.excavOS = excavOS;
                Surface = surface;
                Offset = (Surface.TextureSize - Surface.SurfaceSize) * 0.5f;
                Center = Surface.SurfaceSize * 0.5f;
                AvailableSize = Surface.SurfaceSize;
                PrimaryColor = surface.ScriptForegroundColor;                
                BackgroundColor = surface.ScriptBackgroundColor;
                Vector3 hsv = surface.ScriptForegroundColor.ColorToHSV();
                SecondaryColor = (hsv.Z < 0.5f) ? Color.Lighten(PrimaryColor, 0.3f) : Color.Darken(PrimaryColor, 0.3f);
            }

            public Vector4 FullScreen() {
                return FullScreenWithMargin(0.0f);
            }

            public Vector4 FullScreenWithMargin(float margin = 0.0f) {
                return new Vector4(AvailableSize - 2.0f * margin, margin, margin);
            }

            public Vector4 FullScreenSquareWithMargin(float margin = 0.0f) {
                Vector4 sizeAndPosition = FullScreenWithMargin(margin);
                if (sizeAndPosition.X > sizeAndPosition.Y) {
                    sizeAndPosition.X = sizeAndPosition.Y;
                } else {
                    sizeAndPosition.Y = sizeAndPosition.X;
                }
                return sizeAndPosition;
            }

            public void Initialize() {                
                SetupComponents();
            }

            public Vector2 TextSize(string text = "Qy") {
                return TextSize(Surface.FontSize, text);
            }

            public Vector2 TextSize(float fontSize, string text = "Qy") {
                sb.Clear();
                sb.Append(text);
                Vector2 size = Surface.MeasureStringInPixels(sb, Surface.Font, fontSize);
                return size;
            }

            protected virtual void SetupComponents() { }

            public void Draw() {
                using (var frame = Surface.DrawFrame()) {
                    makeSpriteCacheDirty = !makeSpriteCacheDirty;
                    if (makeSpriteCacheDirty) {
                        frame.Add(new MySprite() {
                            Type = SpriteType.TEXTURE,
                            Data = "SquareSimple",
                            Color = Surface.ScriptBackgroundColor,
                            Position = new Vector2(0, 0),
                            Size = new Vector2(0, 0)
                        });
                    }

                    // frame.Add(new MySprite() {
                    //     Type = SpriteType.TEXTURE,
                    //     Data = "SquareSimple",
                    //     Color = Surface.ScriptForegroundColor,
                    //     Position = new Vector2(0, 0) + Offset,
                    //     Size = AvailableSize
                    // });
                    
                    foreach (Drawable component in components) {
                        component.Draw(Surface);
                        component.Sprites.ForEach(sprite => sprite.Position += Offset);
                        /*
                        component.Sprites.ForEach(sprite => {
                            excavOS.Log($"{sprite} @ {sprite.Position} x {sprite.Size} ");
                        });
                        */
                        frame.AddRange(component.Sprites); 
                    }
                    DrawFrame(frame);

                }
            }

            protected T AddComponent<T>(T component) where T : Drawable {
                components.Add(component);
                return component;
            }

            protected virtual void DrawFrame(MySpriteDrawFrame frame) { }

            public virtual bool ShouldDispose() {
                return false;
            }
        }
    }
}
