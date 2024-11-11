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
        public abstract class Layoutable {

            public Vector2 DesiredSize;
            public Vector2 Position;
            public Vector2 Size;

            public Layoutable() {
                Position = Vector2.Zero;
                Size = Vector2.Zero;
            }

            public Layoutable(Vector2 size) {
                Position = Vector2.Zero;
                Size = size;
                DesiredSize = Size;
            }

            public Layoutable(Vector2 position, Vector2 size) {
                Position = position;
                Size = size;
                DesiredSize = Size;
            }

            public Layoutable(Vector4 sizeAndPosition) {
                Position = new Vector2(sizeAndPosition.Z, sizeAndPosition.W);
                Size = new Vector2(sizeAndPosition.X, sizeAndPosition.Y);
                DesiredSize = Size;
            }

            public virtual void SetPositionAndSize(Vector2 position, Vector2 size) {
                Position = position;
                Size = size;
            }

            public static Vector4 FullScreen(ScreenHandler handler) {
                return FullScreenWithMargin(handler, 0.0f);
            }

            public static Vector4 FullScreenWithMargin(ScreenHandler handler, float margin = 0.0f) {
                return new Vector4(handler.AvailableSize - 2.0f * margin, margin, margin);                
            }

            public static Vector4 FullScreenSquareWithMargin(ScreenHandler handler, float margin = 0.0f) {
                Vector4 sizeAndPosition = FullScreenWithMargin(handler, margin);
                if (sizeAndPosition.X > sizeAndPosition.Y) {
                    sizeAndPosition.X = sizeAndPosition.Y;
                } else {
                    sizeAndPosition.Y = sizeAndPosition.X;
                }
                return sizeAndPosition;
            }

        }

    }
}
