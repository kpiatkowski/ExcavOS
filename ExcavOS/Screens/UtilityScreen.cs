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
        
        public class UtilityScreen : ScreenHandler<ExcavOSContext>
        {
            public new const string SCREEN_NAME = "Utility";

            private readonly StringBuilder sb = new StringBuilder();

            public UtilityScreen(ExcavOSContext context) : base(context)
            {
            }

            public override void Draw(IMyTextSurface surface)
            {
                using (var frame = surface.DrawFrame())
                {
                    Painter.SetCurrentSurfaceAndFrame(surface, frame);
                    float margin = Painter.Width >= 512.0f ? 25.0f : 5.0f;
                    float gap = Painter.Width >= 512.0f ? 10.0f : 4.0f;
                    float fontSize = Painter.Width >= 512.0f ? 1.0f : 0.8f;
                    sb.Clear();
                    sb.Append("Xy");
                    Vector2 textSize = surface.MeasureStringInPixels(sb, surface.Font, fontSize);
                    Vector2 position = new Vector2(margin, margin);
                    Vector2 barSize = new Vector2(Painter.Width - margin * 2, Painter.Width >= 512.0f ? 2.0f : 1.0f);

                    Painter.Text(position, "Gravity Align", fontSize, TextAlignment.LEFT);
                    if (_context._utilitymanager.Status == "")
                    {
                        if (_context._utilitymanager.GravityAlign)
                        {                            
                            Painter.Text(new Vector2(Painter.Width - margin, position.Y), "Enabled", fontSize, TextAlignment.RIGHT);
                        }
                        else
                        {
                            Painter.TextEx(new Vector2(Painter.Width - margin, position.Y), Painter.SecondaryColor, "Disabled", fontSize, TextAlignment.RIGHT);
                        }
                    } 
                    else
                    {
                        Painter.TextEx(new Vector2(Painter.Width - margin, position.Y), Painter.SecondaryColor, _context._utilitymanager.Status, fontSize, TextAlignment.RIGHT);
                    }

                    position.Y += textSize.Y + gap;
                    Painter.FilledRectangleEx(position, barSize, Painter.SecondaryColor);
                    position.Y += gap;

                    Painter.Text(position, "Stop", fontSize, TextAlignment.LEFT);
                    if (_context._weightAnalizer.Status == "")
                    {
                        if (_context._weightAnalizer.StoppingDistance >  0)
                        {
                            string w = _context._weightAnalizer.StopThrustersWarning ? " (!)" : "";
                            string s = string.Format("{0:0.00}m @ {1:0.00}s{2}", _context._weightAnalizer.StoppingDistance, _context._weightAnalizer.StoppingTime, w);
                            Painter.TextEx(new Vector2(Painter.Width - margin, position.Y), Painter.SecondaryColor, s, fontSize, TextAlignment.RIGHT);
                        }
                        else
                        {
                            Painter.TextEx(new Vector2(Painter.Width - margin, position.Y), Painter.SecondaryColor, "-", fontSize, TextAlignment.RIGHT);
                        }
                    }
                    else
                    {
                        Painter.TextEx(new Vector2(Painter.Width - margin, position.Y), Painter.SecondaryColor, _context._weightAnalizer.Status, fontSize, TextAlignment.RIGHT);
                    }

                    position.Y += textSize.Y + gap;
                    Painter.FilledRectangleEx(position, barSize, Painter.SecondaryColor);
                    position.Y += gap;

                    Painter.Text(position, "Jettison", fontSize, TextAlignment.LEFT);
                    Painter.TextEx(new Vector2(Painter.Width - margin, position.Y), Painter.SecondaryColor, _context._utilitymanager.GetSortersFilter(), fontSize, TextAlignment.RIGHT);

                    position.Y += textSize.Y + gap;
                    Painter.FilledRectangleEx(position, barSize, Painter.SecondaryColor);
                    position.Y += gap;

                    float maxWidth = (barSize.X - (4 * gap)) / 3;
                    //float maxHeight = Painter.Height - 2 * margin - position.Y;
                    float maxHeight = textSize.Y + gap;
                    position.X += gap;
                    Painter.ProgressBar(position, new Vector2(maxWidth, maxHeight), (float)_context._utilitymanager.BatteryLevel, 1.0f, "IconEnergy");
                    position.X += maxWidth + gap;

                    Painter.ProgressBar(position, new Vector2(maxWidth, maxHeight), (float)_context._utilitymanager.HydrogenLevel, 1.0f, "IconHydrogen");
                    position.X += maxWidth + gap;

                    Painter.ProgressBar(position, new Vector2(maxWidth, maxHeight), (float)_context._utilitymanager.UraniumLevel * 1000, 1.0f, "MyObjectBuilder_Ingot/Uranium");

                    /*
                    Painter.Text(position, "Batteries", fontSize, TextAlignment.LEFT);
                    Painter.TextEx(new Vector2(Painter.Width - margin, position.Y), Painter.SecondaryColor, _context._utilitymanager.BatteryCharge, fontSize, TextAlignment.RIGHT);

                    position.Y += textSize.Y + gap;
                    Painter.FilledRectangleEx(position, barSize, Painter.SecondaryColor);
                    position.Y += gap;

                    Painter.Text(position, "Hydrogen", fontSize, TextAlignment.LEFT);
                    Painter.TextEx(new Vector2(Painter.Width - margin, position.Y), Painter.SecondaryColor, _context._utilitymanager.HydrogenLevel, fontSize, TextAlignment.RIGHT);

                    position.Y += textSize.Y + gap;
                    Painter.FilledRectangleEx(position, barSize, Painter.SecondaryColor);
                    position.Y += gap;

                    Painter.Text(position, "Uranium", fontSize, TextAlignment.LEFT);
                    Painter.TextEx(new Vector2(Painter.Width - margin, position.Y), Painter.SecondaryColor, _context._utilitymanager.HydrogenLevel, fontSize, TextAlignment.RIGHT);

                    position.Y += textSize.Y + gap;
                    Painter.FilledRectangleEx(position, barSize, Painter.SecondaryColor);
                    position.Y += gap;
                    */
                }
            }
        }
    }
}
