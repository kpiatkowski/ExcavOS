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
        public class WeightScreenHandler : ExcavOSScreenHandler
        {
            private double lastCapacityMeasureTime;
            private double lastCapacityMeasure;
            private double capacityDelta;
            private double ticker;

            public WeightScreenHandler(ExcavOSContext context) : base(context)
            {
                lastCapacityMeasureTime = context.timeAccumulator.TotalSeconds;
                lastCapacityMeasure = context.cargoManager.CurrentCapacity;
                capacityDelta = 0.0;
                ticker = 0;
            }

            public override void Draw(IMyTextSurface surface)
            {
                Painter.SetPaintFromSurface(surface);
                using (var frame = surface.DrawFrame())
                {
                    Painter.frame = frame;
                    if (context.timeAccumulator.TotalSeconds > lastCapacityMeasureTime + 1.0)
                    {
                        capacityDelta = (context.cargoManager.CurrentCapacity - lastCapacityMeasure) / (context.timeAccumulator.TotalSeconds - lastCapacityMeasureTime);
                        lastCapacityMeasureTime = context.timeAccumulator.TotalSeconds;
                        lastCapacityMeasure = context.cargoManager.CurrentCapacity;
                    }
                    double value = context.cargoManager.CurrentCapacity / context.cargoManager.TotalCapacity;
                    double capacityPerSecond = capacityDelta / context.cargoManager.TotalCapacity;
                    string capacityDeltaString = "";
                    if (capacityPerSecond > 0.0)
                    {
                        double fullInSecs = (context.cargoManager.TotalCapacity - context.cargoManager.CurrentCapacity) / capacityDelta;
                        capacityDeltaString = string.Format("+{0,0:0.00}%/s full in {1,0:00}s", capacityPerSecond * 100, fullInSecs);
                    }
                    float margin = 10.0f;
                    float barHeight = 24.0f;
                    float textSize = 0.7f;
                    Vector2 position = new Vector2(margin, margin);
                    Vector2 size = new Vector2(Painter.paint.AvailableWidth - 2 * margin, barHeight);
                    Painter.DrawText(position, Painter.paint.PrimaryColor, "Capacity", textSize, TextAlignment.LEFT);
                    Painter.DrawText(new Vector2(Painter.paint.AvailableWidth - margin, position.Y), Painter.paint.PrimaryColor, string.Format("{0,10:0.00}%", value * 100), textSize, TextAlignment.RIGHT);
                    position.Y += Painter.TEXT_SIZE_PER_UNIT * textSize + margin;
                    Painter.DrawProgressBar(position, size, value, 2.0f);
                    position.Y += barHeight;
                    Painter.DrawText(new Vector2(Painter.paint.AvailableWidth - margin, position.Y), Painter.paint.PrimaryColor, capacityDeltaString, textSize, TextAlignment.RIGHT);

                    position.Y += barHeight + margin;
                    Painter.DrawText(position, Painter.paint.PrimaryColor, "Lift force", textSize, TextAlignment.LEFT);
                    if (context.Mass > 0 && context.liftThrusters.Count() > 0)
                    {
                        value = context.LiftThrustNeeded / context.LiftThrustAvailable;
                        Painter.DrawText(new Vector2(Painter.paint.AvailableWidth - margin, position.Y), Painter.paint.PrimaryColor, string.Format("{0,10:0.00}%", value * 100), textSize, TextAlignment.RIGHT);
                        position.Y += Painter.TEXT_SIZE_PER_UNIT * textSize + margin;
                        Painter.DrawProgressBar(position, size, value, 2.0f);
                        position.Y += barHeight;
                        //Painter.DrawText(new Vector2(Painter.paint.AvailableWidth - margin, position.Y), Painter.paint.PrimaryColor, string.Format("+{0,0:0.00}%/s full in {1}s", capacityPerSecond, fullInSecs), textSize, TextAlignment.RIGHT);
                    } 
                    else
                    {
                        Painter.DrawText(new Vector2(Painter.paint.AvailableWidth - margin, position.Y), Painter.paint.PrimaryColor, "N/A while docked", textSize, TextAlignment.RIGHT);
                        position.Y += Painter.TEXT_SIZE_PER_UNIT * textSize + margin;
                        Painter.DrawProgressBar(position, size, 0.0f, 2.0f);
                        position.Y += barHeight;
                    }

                    if (context.EmergencyLiftPreserver > 0 && context.EmergencyLiftPreserver < 1.0f)
                    {
                        Vector2 p = position + new Vector2(size.X * (float)context.EmergencyLiftPreserver - 8.0f, 0);
                        Painter.DrawSprite(p, new Vector2(16, 16), "Triangle", Painter.paint.PrimaryColor);
                    }

                    if (context.EmergencyLiftPreserverActive)
                    {
                        ticker += 0.2;
                        if ((int)ticker % 2 == 0)
                        {
                            float s = 64.0f;
                            Vector2 p = new Vector2(margin + (Painter.paint.AvailableWidth - 2 * margin - s) / 2, Painter.paint.AvailableHeight / 2 + (Painter.paint.AvailableHeight / 2 - s) / 2);
                            Painter.DrawSprite(p, new Vector2(s, s), "Danger", Painter.paint.PrimaryColor);
                        }
                    }
                }
            }

        }
    }
}
