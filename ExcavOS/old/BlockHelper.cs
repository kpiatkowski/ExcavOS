using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System;
using VRage;
using VRage.Collections;
using VRage.Game.Components;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Game;
using VRageMath;

namespace IngameScript
{
    partial class Program
    {
        public class BlockHelper
        {

            private const string LARGE_BLOCK = "LargeBlock";
            private const string SMALL_BLOCK = "SmallBlock";
            private const string TYPEID_WIND_TURBINE = "MyObjectBuilder_WindTurbine";
            private const string TYPEID_HYDROGEN_ENGINE = "MyObjectBuilder_HydrogenEngine";
            private const string TYPEID_OXYGEN_TANK = "MyObjectBuilder_OxygenTank";
            private const string SUBTYPEID_HYDROGEN_TANK = "HydrogenTank";
            private const string SUBTYPEID_COCKPIT_INDUSTRIAL = "CockpitIndustrial";
            private const string SUBTYPEID_COCKPIT_SMALL = "SmallBlockCockpit";
            private const string SUBTYPEID_COCKPIT_LARGE = "LargeBlockCockpitSeat";

            private static string ExtractData(string marker, string value)
            {
                int startPos = value.IndexOf(marker);
                if (startPos == -1)
                {
                    return "";
                }
                string part = value.Substring(startPos + marker.Length);
                int endPos = part.IndexOf('\n');
                if (endPos > 0)
                {
                    return part.Substring(0, endPos);
                }
                return part;
            }

            public static bool IsLargeBlock(IMyTerminalBlock block)
            {
                return block.BlockDefinition.SubtypeId.StartsWith(LARGE_BLOCK);
            }

            public static bool IsSmallBlock(IMyTerminalBlock block)
            {
                return block.BlockDefinition.SubtypeId.StartsWith(SMALL_BLOCK);
            }

            public static bool IsCockpitIndustrial(IMyCockpit cockpit)
            {
                return cockpit.BlockDefinition.SubtypeId.EndsWith(SUBTYPEID_COCKPIT_INDUSTRIAL);
            }

            public static bool IsStandardCockpit(IMyCockpit cockpit)
            {
                return cockpit.BlockDefinition.SubtypeId.EndsWith(SUBTYPEID_COCKPIT_SMALL) || cockpit.BlockDefinition.SubtypeId.EndsWith(SUBTYPEID_COCKPIT_LARGE);
            }

            public static bool IsWindTurbine(IMyTerminalBlock block)
            {
                return block.BlockDefinition.TypeId.ToString() == TYPEID_WIND_TURBINE;
            }

            public static bool IsHydrogenEngine(IMyTerminalBlock block)
            {
                return block.BlockDefinition.TypeId.ToString() == TYPEID_HYDROGEN_ENGINE;
            }

            public static float GetReactorFuelLevel(IMyReactor reactor)
            {
                IMyInventory inventory = reactor.GetInventory(0);
                return (float)inventory.CurrentVolume / (float)inventory.MaxVolume;
            }

            public static float GetHydrogenEngineFuelLevel(IMyPowerProducer hydrogenEngine)
            {
                string filledLine = ExtractData("Filled: ", hydrogenEngine.DetailedInfo);
                string[] fillValues = ExtractData(" (", filledLine).TrimEnd(')').Replace("L", string.Empty).Split('/');
                if (fillValues.Length != 2)
                {
                    return 0.0f;
                }
                float filled = float.Parse(fillValues[0]);
                float max = float.Parse(fillValues[1]);
                return filled / max;
            }

            public static string GetWindTurbineClearance(IMyPowerProducer block)
            {
                return ExtractData("Wind Clearance: ", block.DetailedInfo);
            }

            public static float GetTankFill(IMyGasTank block)
            {
                return (float)(block.FilledRatio * block.Capacity);
            }

            public static bool IsOxygenTank(IMyGasTank block)
            {
                return block.BlockDefinition.TypeId.ToString() == TYPEID_OXYGEN_TANK;
            }

            public static bool IsHydrogenTank(IMyGasTank block)
            {
                return block.BlockDefinition.SubtypeId.ToString().Contains(SUBTYPEID_HYDROGEN_TANK);
            }
        }
    }
}
