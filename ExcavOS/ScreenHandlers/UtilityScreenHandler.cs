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
        public class UtilityScreenHandler : ExcavOSScreenHandler
        {
            public UtilityScreenHandler(ExcavOSContext context) : base(context)
            {
            }

            public override void Draw(IMyTextSurface surface)
            {
                Painter.SetPaintFromSurface(surface);                
                using (var frame = surface.DrawFrame())
                {
                    Painter.frame = frame;
                    float margin = 4.0f;
                    Painter.DrawRectangle(Painter.paint.PrimaryColor, new Vector2(margin, margin), new Vector2(Painter.paint.AvailableWidth - 2 * margin, Painter.paint.AvailableHeight - 2 * margin), 2.0f);

                    float gap = 5.0f;
                    float fontSize = 0.6f;
                    float textHeight = fontSize * 16.0f;
                    Vector2 point = new Vector2(gap + margin, gap + margin);
                    Vector2 size = new Vector2(Painter.paint.AvailableWidth - 2 * (gap + margin), 1.0f);

                    Painter.DrawSprite(point, new Vector2(textHeight * 2, textHeight * 2), "AH_VelocityVector");
                    Painter.DrawText(new Vector2(point.X + 3 * gap + textHeight, point.Y), Painter.paint.PrimaryColor, "Gravity Align Assist", fontSize, TextAlignment.LEFT);
                    Color gravColor = context.gravityAlignEnabled ? Painter.paint.PrimaryColor : Painter.paint.SecondaryColor;
                    string label = context.gravityAlignEnabled ? "Enabled" : "Disabled";
                    Painter.DrawText(new Vector2(Painter.paint.AvailableWidth - (2 * gap + margin), point.Y), gravColor, label, fontSize, TextAlignment.RIGHT);
                    point.Y += margin + textHeight + gap;
                    Painter.DrawRectangle(Painter.paint.SecondaryColor, point, size);
                    point.Y += gap;

                    label = context.StoppingDistance < 0 ? "Docked" : string.Format("{0,2:0.00}m / {1,2:0.00}s", context.StoppingDistance, context.StoppingTime);
                    Painter.DrawSprite(point, new Vector2(textHeight * 2, textHeight * 2), "AH_BoreSight");
                    Painter.DrawText(new Vector2(point.X + 3 * gap + textHeight, point.Y), Painter.paint.PrimaryColor, "Braking", fontSize, TextAlignment.LEFT);                    
                    Painter.DrawText(new Vector2(Painter.paint.AvailableWidth - (2 * gap + margin), point.Y), Painter.paint.SecondaryColor, label, fontSize, TextAlignment.RIGHT);
                    point.Y += margin + textHeight + gap;
                    Painter.DrawRectangle(Painter.paint.SecondaryColor, point, size);
                    point.Y += gap;

                    //Painter.DrawButton(new Vector2(margin * 2, 60), new Vector2(80, 40), "Gravity Align", true);
                }
            }

        }
    }
}
