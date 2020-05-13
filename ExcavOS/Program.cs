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
    partial class Program : MyGridProgram
    {

        #region mdk macros
        // This script was deployed at $MDK_DATETIME$
        const string Deployment = "$MDK_DATE$, $MDK_TIME$";
        #endregion

        private class CockpitScreen
        {
            public IMyTextSurface Surface;
            public ExcavOSScreenHandler CurrentHandler;
            public ExcavOSScreenHandler PrimaryHandler;
        }

        private class CockpitDetails
        {
            public IMyCockpit Cockpit;
            public bool WasUnderControl;
            public List<CockpitScreen> Screens = new List<CockpitScreen>();
        }

        private Dictionary<long, CockpitDetails> cockpits = new Dictionary<long, CockpitDetails>();
        private BlockFinder<IMyCockpit> gridCockpits;
        private readonly ExcavOSContext excavOSContext;

        public Program()
        {
            Runtime.UpdateFrequency = UpdateFrequency.Update10;
            gridCockpits = new BlockFinder<IMyCockpit>(this);
            excavOSContext = new ExcavOSContext(this, Me);
            CollectCockpits();
        }

        private void CollectCockpits()
        {
            gridCockpits.FindBlocks();
            gridCockpits.ForEach(cockpit =>
            {
                if (!cockpits.ContainsKey(cockpit.EntityId) && BlockHelper.IsCockpitIndustrial(cockpit))
                {
                    CockpitDetails cockpitDetails = new CockpitDetails
                    {
                        Cockpit = cockpit,
                        WasUnderControl = cockpit.IsUnderControl,                        
                    };
                    cockpitDetails.Screens.Add(new CockpitScreen
                    {
                        Surface = cockpitDetails.Cockpit.GetSurface(0),
                        PrimaryHandler = new CargoScreenHandler(excavOSContext)
                    });
                    cockpitDetails.Screens.Add(new CockpitScreen
                    {
                        Surface = cockpitDetails.Cockpit.GetSurface(1),
                        PrimaryHandler = new WeightScreenHandler(excavOSContext)
                    });
                    cockpitDetails.Screens.Add(new CockpitScreen
                    {
                        Surface = cockpitDetails.Cockpit.GetSurface(2),
                        PrimaryHandler = new UtilityScreenHandler(excavOSContext)
                    });
                    cockpits.Add(cockpit.EntityId, cockpitDetails);

                    //List<string> sprites = new List<string>();
                    //cockpit.GetSurface(0).GetSprites(sprites);
                    //StringBuilder sb = new StringBuilder();
                    //sprites.ForEach(str => sb.Append(str + ","));
                    //Me.CustomData = sb.ToString();
                }
            });
        }

        public void Save()
        {

        }

        public void Main(string argument, UpdateType updateSource)
        {
            excavOSContext.AddTimeSpan(Runtime.TimeSinceLastRun);

            if (argument != "")
            {
                excavOSContext.HandleCommand(argument);
                return;
            }

            foreach (var cockpit in cockpits.Values)
            {
                if (!cockpit.WasUnderControl && cockpit.Cockpit.IsUnderControl)
                {
                    // user entered cockpit
                    cockpit.WasUnderControl = true;
                    cockpit.Screens.ForEach(screen =>
                    {
                        screen.CurrentHandler = new LoadingScreenHandler(excavOSContext);
                    });
                }
                else if (cockpit.WasUnderControl && !cockpit.Cockpit.IsUnderControl)
                {
                    // user exited cockpit
                    cockpit.WasUnderControl = false;
                    cockpit.Screens.ForEach(screen =>
                    {
                        screen.CurrentHandler = new NoUserScreenHandler(excavOSContext);
                    });
                }
                else if (cockpit.Cockpit.IsUnderControl)
                {
                    // user is in cockpit                    
                    cockpit.Screens.ForEach(screen =>
                    {
                        if (screen.CurrentHandler != null && screen.CurrentHandler.ShouldDispose()) {
                            screen.CurrentHandler = screen.PrimaryHandler;
                        }
                    });
                }

                excavOSContext.Update();
                excavOSContext.SetActiveCockpit(cockpit.Cockpit);
                
                cockpit.Screens.ForEach(screen =>
                {
                    if (screen.CurrentHandler != null) {
                        screen.Surface.Script = "";
                        screen.Surface.ContentType = ContentType.SCRIPT;
                        screen.CurrentHandler.Draw(screen.Surface);
                    }                    
                });
            }


            //    if (autoLevelEnabled && gyros.Count() > 0)
            //    {
            //        AutoLevel(managedCockpit.Cockpit, gyros.blocks.Take(gyros.Count() / 2).ToList());
            //    }


        }

        //private string GetPartialText(string text, float amount)
        //{
        //    if (amount > 1.0f) amount = 1.0f;
        //    int x = (int)(amount * text.Length);
        //    if (x == text.Length) return text;
        //    return text.Remove(x);
        //}


        //private void DrawInfoPanels(CockpitDetails cockpitDetails)
        //{
        //    IMyCockpit cockpit = cockpitDetails.Cockpit;
        //    for (int i = 0; i < cockpit.SurfaceCount; i++)
        //    {
        //        IMyTextSurface surface = cockpit.GetSurface(i);
        //        surface.ContentType = ContentType.SCRIPT;
        //        surface.Script = "";
        //        Vector2 center = surface.SurfaceSize / 2.0f;
        //        float offsetTop = (surface.TextureSize.Y - surface.SurfaceSize.Y) / 2.0f;
        //        using (var frame = surface.DrawFrame())
        //        {
        //            Vector3 hsv = surface.ScriptForegroundColor.ColorToHSV();
        //            Color primaryColor = surface.ScriptForegroundColor;
        //            Color secondaryColor = (hsv.Z < 0.5f) ? Color.Lighten(primaryColor, 0.2f) : Color.Darken(primaryColor, 0.2f);
        //            Painter.frame = frame;

        //            MySprite sprite;
        //            center.Y = offsetTop;

        //            if (i == 0)
        //            {
        //                sprite = MySprite.CreateText("--[ Ores ]--", surface.Font, primaryColor, 1.0f, TextAlignment.CENTER);
        //                sprite.Position = center;
        //                frame.Add(sprite);
        //                center.Y += 12.0f;
        //                cargoManager.IterateCargoDescending((name, amount) =>
        //                {
        //                    string affix = " ";
        //                    if (amount > 1024)
        //                    {
        //                        amount /= 1024.0f;
        //                        affix = "k";
        //                    }
        //                    string item = string.Format("{0,-11} {1,10:0.00}{2}\n", name, amount, affix);
        //                    center.Y += 18.0f;
        //                    sprite = MySprite.CreateText(item, surface.Font, primaryColor, 0.6f, TextAlignment.CENTER);
        //                    sprite.Position = center;
        //                    frame.Add(sprite);
        //                });

        //            }
        //            else if (i == 1)
        //            {
        //                sprite = MySprite.CreateText("--[ Cargo ]--", surface.Font, primaryColor, 1.0f, TextAlignment.CENTER);
        //                sprite.Position = center;
        //                frame.Add(sprite);
        //                center.Y += 30.0f;
        //                string cap = string.Format("{0,-11} {1,10:0.00}%\n", "Capacity", (cargoManager.CurrentCapacity / cargoManager.TotalCapacity) * 100);
        //                sprite = MySprite.CreateText(cap, surface.Font, primaryColor, 0.8f, TextAlignment.CENTER);
        //                sprite.Position = center;
        //                frame.Add(sprite);
        //            } else if (i == 2)
        //            {
        //                sprite = MySprite.CreateText("--[ Status ]--", surface.Font, primaryColor, 1.0f, TextAlignment.CENTER);
        //                sprite.Position = center;
        //                frame.Add(sprite);
        //                center.Y += 30.0f;
        //                sprite = MySprite.CreateText($"Align: {AutoLevel(cockpit, gyros.blocks.Take(gyros.Count() / 2).ToList(), true)} / {autoLevelEnabled}", surface.Font, primaryColor, 0.6f, TextAlignment.CENTER);
        //                sprite.Position = center;
        //                frame.Add(sprite);

        //                double gravity = (cockpit.GetNaturalGravity().Length() / 9.81);
        //                double mass = cockpit.CalculateShipMass().PhysicalMass;
        //                double thrustNeeded = mass * (float)gravity / 100;

        //                center.Y += 30.0f;
        //                string s = string.Format("Weight: {0,10:0.00}kg\n", mass);
        //                sprite = MySprite.CreateText(s, surface.Font, primaryColor, 0.6f, TextAlignment.CENTER);
        //                sprite.Position = center;
        //                frame.Add(sprite);

        //                center.Y += 30.0f;
        //                s = string.Format("Thrust needed: {0,10:0.00}kN\n", thrustNeeded);
        //                sprite = MySprite.CreateText(s, surface.Font, primaryColor, 0.6f, TextAlignment.CENTER);
        //                sprite.Position = center;
        //                frame.Add(sprite);

        //                double thrustAvailable = 0;
        //                liftThrusters.ForEach(thruster =>
        //                {
        //                    thrustAvailable += thruster.MaxEffectiveThrust;
        //                });

        //                center.Y += 30.0f;
        //                s = string.Format("Thrust available: {0,10:0.00}kN\n", thrustAvailable / 1000);
        //                sprite = MySprite.CreateText(s, surface.Font, primaryColor, 0.6f, TextAlignment.CENTER);
        //                sprite.Position = center;
        //                frame.Add(sprite);

        //            }
        //        }
        //    }
        //}

        
    }
}
