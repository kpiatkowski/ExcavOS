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
        public class LoadingScreenHandler : ExcavOSScreenHandler
        {
            private readonly double loadingStart;
            private readonly double loadingTime;
            private readonly int quotesPerLoading;
            private int currentQuote = 0;
            private string quote;

            public LoadingScreenHandler(ExcavOSContext context) : base(context)
            {
                loadingStart = context.timeAccumulator.TotalSeconds;                
                loadingTime = 1.0 + context.random.Next(1000, 2000) / 1000.0;
                quotesPerLoading = context.random.Next(2, 8);
            }

            private string GetInitializationSimsLikeText(double progress)
            {
                int quoteNumber = (int)(progress / (1.0 / quotesPerLoading));
                if (quoteNumber == 0)
                {
                    return "Booting";
                }

                string[] quotes = { 
                    "Praying to Clang",
                    "Couting subgrids",
                    "Something something",
                    "Calming Clang Wrath",
                    "Counting stones in cargo",
                    "Doing important stuff",
                    "Formatting drive",
                    "Generating phantom forces",
                    "Connecting dots"
                };
                if (currentQuote != quoteNumber)
                {
                    int quoteIndex = context.random.Next(0, quotes.Length);
                    quote = quotes[quoteIndex];
                }
                
                currentQuote = quoteNumber;
                return quote;
            }

            public override void Draw(IMyTextSurface surface)
            {
                Painter.SetPaintFromSurface(surface);                
                using (var frame = surface.DrawFrame())
                {
                    Painter.frame = frame;
                    double value = (context.timeAccumulator.TotalSeconds - loadingStart) / loadingTime;
                    float margin = 20.0f;
                    float bottomYPos = Painter.paint.AvailableHeight - margin;
                    Painter.DrawProgressBar(new Vector2(margin, bottomYPos), new Vector2(Painter.paint.AvailableWidth - margin * 2, 10), value, 2.0f);
                    Painter.DrawText(new Vector2(Painter.paint.AvailableWidth / 2, margin), Painter.paint.PrimaryColor, "ExcavOS");
                    Painter.DrawSprite(new Vector2((Painter.paint.AvailableWidth - 80) / 2, margin * 2.5f), new Vector2(80, 80), "Textures\\FactionLogo\\Miners\\MinerIcon_3.dds");

                    Painter.DrawText(new Vector2(margin, bottomYPos - margin), Painter.paint.PrimaryColor, GetInitializationSimsLikeText(value) + "...", 0.5f, TextAlignment.LEFT);
                }
            }

            public override bool ShouldDispose()
            {
                return context.timeAccumulator.TotalSeconds - loadingStart > loadingTime;
            }
        }
    }
}
