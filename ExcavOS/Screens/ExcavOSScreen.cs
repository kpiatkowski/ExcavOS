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
        public class ExcavOSScreen : ScreenHandler<ExcavOSContext>
        {
            public new const string SCREEN_NAME = "ExcavOS";

            public ExcavOSScreen(ExcavOSContext context) : base(context)
            {
            }

            public override void Draw(IMyTextSurface surface)
            {
                using (var frame = surface.DrawFrame())
                {
                    Painter.SetCurrentSurfaceAndFrame(surface, frame);
                    Painter.SpriteCentered(Painter.Center, new Vector2(Painter.Height * 0.8f, Painter.Height * 0.8f), "Textures\\FactionLogo\\Miners\\MinerIcon_3.dds");
                    Painter.TextEx(new Vector2(Painter.Center.X, Painter.Center.Y - 60.0f), Painter.BackgroundColor, "ExcavOS", 1.6f);
                }
            }
        }
    }
}
