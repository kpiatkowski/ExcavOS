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
        public class Formatting {
            public static string ExtractName(string itemType) {
                return itemType.Split('/').Last();
            }

            public static string FormatWeightWithSuffix(double amount) {
                if (amount >= 1000000000) {
                    return string.Format("{0:0.00}Mt", amount / 1000000000);
                } else if (amount >= 1000000) {
                    return string.Format("{0:0.00}kt", amount / 1000000);
                } else if (amount >= 1000) {
                    return string.Format("{0:0.00}t", amount / 1000);
                }
                return string.Format("{0:0.00}kg", amount);
            }

            public static string FormatForceWithSuffix(double amount) {
                if (amount >= 1000000000) {
                    return string.Format("{0:0.00}GN", amount / 1000000000);
                } else if (amount >= 1000000) {
                    return string.Format("{0:0.00}MN", amount / 1000000);
                } else if (amount >= 1000) {
                    return string.Format("{0:0.00}kN", amount / 1000);
                }
                return string.Format("{0:0.00}N", amount);
            }

        }
    }
}
