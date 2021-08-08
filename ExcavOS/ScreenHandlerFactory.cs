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
        public class ScreenHandlerFactory
        {
            private static Dictionary<string, ScreenHandler<ExcavOSContext>> _handlers = new Dictionary<string, ScreenHandler<ExcavOSContext>>();
            public static ScreenHandler<ExcavOSContext> GetScreenHandler(string name, ExcavOSContext context)
            {
                if (_handlers.ContainsKey(name))
                {
                    return _handlers[name];
                }

                ScreenHandler<ExcavOSContext> handler;
                switch (name)
                {
                    case ExcavOSScreen.SCREEN_NAME:
                        handler = new ExcavOSScreen(context);
                        break;
                    case CargoScreen.SCREEN_NAME:
                        handler = new CargoScreen(context);
                        break;
                    case WeightScreen.SCREEN_NAME:
                        handler = new WeightScreen(context);
                        break;
                    case UtilityScreen.SCREEN_NAME:
                        handler = new UtilityScreen(context);
                        break;
                    case AllCargoScreen.SCREEN_NAME:
                        handler = new AllCargoScreen(context);
                        break;
                    default:
                        handler = new BlankScreen(context);
                        break;
                }
                _handlers.Add(name, handler);
                return handler;

            }
        }
    }
}
