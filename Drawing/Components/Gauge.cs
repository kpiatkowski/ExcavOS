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
        public class Gauge : Drawable {

            private Label label = new Label("");
            public float Min = 0.0f;
            public float Max = 100.0f;
            public float Value = 0.0f;

            private float Progress() {
                if (Value <= Min) return 0.0f;
                if (Value >= Max) return 1.0f;
                return (Value - Min) / (Max - Min);
            }

            protected override void DrawSprites(IMyTextSurface surface) {
                float borderSize = 2.0f;
                float progress = Progress();

                label.DropShadow = true;
                label.Caption = string.Format("{0:0.00}%", progress * 100.0f);
                label.Position = Position + Size * 0.5f;
                
                DrawFrame(Position, Size, surface.ScriptForegroundColor, borderSize);
                Vector2 fillerPosition = Position + borderSize;
                Vector2 fillerSize = (Size - borderSize * 2.0f) * new Vector2(progress, 1.0f);
                DrawRectangle(fillerPosition, fillerSize, surface.ScriptForegroundColor);

                label.Draw(surface);
                AddSprites(label.Sprites);
            }
        }
    }
}
