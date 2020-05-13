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
        public class CargoManager
        {
            private const string KEY_EXCLUDE_FROM_CARGO_TRACKING = "ExcludeFromCargoTracking";
            private BlockFinder<IMyTerminalBlock> cargoBlocks;
            private readonly List<MyInventoryItem> items = new List<MyInventoryItem>();
            public double CurrentCapacity;
            public double TotalCapacity;
            public IDictionary<string, double> cargo = new Dictionary<string, double>();
            private readonly MyIni ini = new MyIni();

            public CargoManager(Program program)
            {
                cargoBlocks = new BlockFinder<IMyTerminalBlock>(program);
            }

            public void QueryData()
            {
                cargoBlocks.FindBlocks(true, block => {
                    if (MyIni.HasSection(block.CustomData, ExcavOSContext.SECTION_EXCAV_OS))
                    {
                        MyIniParseResult result;
                        if (ini.TryParse(block.CustomData, out result))
                        {
                            if (ini.Get(ExcavOSContext.SECTION_EXCAV_OS, KEY_EXCLUDE_FROM_CARGO_TRACKING).ToBoolean())
                            {
                                return false;
                            }
                        }
                    }
                    return block.HasInventory && block.IsFunctional;
                });
                CurrentCapacity = 0;
                TotalCapacity = 0;
                cargo.Clear();
                cargoBlocks.ForEach(ProcessBlock);
            }

            private void ProcessBlock(IMyTerminalBlock block)
            {
                for (int i = 0; i < block.InventoryCount; i++)
                {
                    items.Clear();
                    IMyInventory inventory = block.GetInventory(0);
                    CurrentCapacity += (double)inventory.CurrentVolume;
                    TotalCapacity += (double)inventory.MaxVolume;
                    block.GetInventory(i).GetItems(items);
                    foreach (MyInventoryItem item in items)
                    {
                        if (item.Type.TypeId != "MyObjectBuilder_Ore")
                        {
                            continue;
                        }
                        //string itemName = item.Type.SubtypeId;
                        string itemName = item.Type.ToString();                        
                        double amount = (double)item.Amount;

                        if (cargo.ContainsKey(itemName))
                        {
                            cargo[itemName] += amount;
                        } else
                        {
                            cargo.Add(itemName, amount);
                        }
                    }
                }
            }

            public void IterateCargoDescending(Action<string, double> callback)
            {
                foreach (var item in cargo.OrderByDescending(key => key.Value))
                {
                    callback(item.Key, item.Value);
                }
            }
        }
    }
}
