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
using System.Runtime.InteropServices;

namespace IngameScript
{
    partial class Program
    {
        public class CargoScreenHandler : ExcavOSScreenHandler
        {
            public CargoScreenHandler(ExcavOSContext context) : base(context)
            {
            }

            private string ExtractName(string itemType)
            {
                return itemType.Split('/').Last();
            }

            private string FormatWithSuffix(double amount)
            {
                if (amount > 100000)
                {
                    return string.Format("{0,10:0.00}Mt", amount / 100000);
                } else if (amount > 1000)
                {
                    return string.Format("{0,10:0.00}t", amount / 100);
                }
                return string.Format("{0,10:0.00}kg", amount);
            }

            public override void Draw(IMyTextSurface surface)
            {
                Painter.SetPaintFromSurface(surface);
                using (var frame = surface.DrawFrame())
                {
                    Painter.frame = frame;
                    
                    float margin = 4.0f;
                    float gap = 5.0f;
                    float fontSize = 0.6f;
                    float textHeight = fontSize * 16.0f;
                    Color backbar = Painter.paint.PrimaryColor.Alpha(0.2f);
                    Vector2 point = new Vector2(gap + margin, gap + margin);
                    //Vector2 size = new Vector2(Painter.paint.AvailableWidth - 2 * (gap + margin), textHeight + 2 * gap);
                    Vector2 size = new Vector2(Painter.paint.AvailableWidth - 2 * (gap + margin), 1.0f);                    
                    Painter.DrawRectangle(Painter.paint.PrimaryColor, new Vector2(margin, margin), new Vector2(Painter.paint.AvailableWidth - 2 * margin, Painter.paint.AvailableHeight - 2 * margin), 2.0f);
                    context.cargoManager.IterateCargoDescending((name, amount) => {
                        //Painter.DrawProgressBar(point, size, amount / (context.cargoManager.TotalCapacity * 1000), 1.0f, Painter.paint.PrimaryColor, backbar);
                        Painter.DrawSprite(point, new Vector2(textHeight * 2, textHeight * 2), name);
                        Painter.DrawText(new Vector2(point.X + 2 * gap + textHeight, point.Y), Painter.paint.PrimaryColor, ExtractName(name), fontSize, TextAlignment.LEFT);
                        Painter.DrawText(new Vector2(Painter.paint.AvailableWidth - (2 * gap + margin), point.Y), Painter.paint.PrimaryColor, FormatWithSuffix(amount), fontSize, TextAlignment.RIGHT);
                        point.Y += margin + textHeight + gap;
                        Painter.DrawRectangle(Painter.paint.SecondaryColor, point, size);
                        point.Y += gap;
                    });
                }
            }
        }
    }
}
