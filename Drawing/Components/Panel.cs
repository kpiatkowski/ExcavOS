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
        public class Panel : Drawable {

            public Color Color;

            public Panel(Color color) : base() {
                Color = color;
            }

            public Panel(Color color, Vector2 size) : base(size) {
                Color = color;
            }

            public Panel(Color color, Vector2 position, Vector2 size) : base(position, size) {
                Color = color;
            }

            protected override void DrawSprites(IMyTextSurface surface) {
                //DrawFrame(Position, Size, Color);
                AddSprite(Painter.DrawRectangle(Position, Size, Color));
            }
        }
    }
}
