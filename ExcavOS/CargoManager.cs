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
        public class CargoEntry
        {
            public string typeid;
            public double amount;
        }

        public class CargoManager
        {
            private readonly BlockFinder<IMyTerminalBlock> cargoBlocks;
            private readonly List<MyInventoryItem> items = new List<MyInventoryItem>();
            private Config _config;

            public double CurrentCapacity;
            public double TotalCapacity;
            public IDictionary<string, CargoEntry> cargo = new Dictionary<string, CargoEntry>();

            public CargoManager(Program program, Config config)
            {
                cargoBlocks = new BlockFinder<IMyTerminalBlock>(program);
                _config = config;
            }

            public void QueryData()
            {
                cargoBlocks.FindBlocks(true, block => {                    
                    if (_config.CargoTrackGroupName == "" && block is IMyConveyorSorter)
                    {
                        return false;
                    }
                    return block.HasInventory && block.IsFunctional;
                }, _config.CargoTrackGroupName);
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
                        //if (item.Type.TypeId != "MyObjectBuilder_Ore")
                        //{
                        //    continue;
                        //}
                        //string itemName = item.Type.SubtypeId;
                        string itemName = item.Type.ToString();
                        double amount = (double)item.Amount;
                        if (cargo.ContainsKey(itemName))
                        {
                            CargoEntry ce = cargo[itemName];
                            ce.amount += amount;
                            CargoEntry ce2 = cargo[itemName];
                        } else
                        {
                            CargoEntry ce = new CargoEntry
                            {
                                amount = amount,
                                typeid = item.Type.TypeId
                            };
                            cargo.Add(itemName, ce);
                        }
                    }
                }
            }

            public bool IsEmpty()
            {
                return items.Count() == 0;
            }

            public void IterateCargoDescending(Action<string, CargoEntry> callback)
            {
                foreach (var item in cargo.OrderByDescending(key => key.Value.amount))
                {
                    callback(item.Key, item.Value);
                }
            }

        }
    }
}
