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

        public class CargoEntry {
            public string typeid;
            public double amount;
        }

        public class InventoryModule : ScriptModule {

            private readonly BlockCache inventoryBlocks;
            private readonly List<MyInventoryItem> items = new List<MyInventoryItem>();
            private readonly Dictionary<string, CargoEntry> cargo = new Dictionary<string, CargoEntry>();
            private readonly List<KeyValuePair<string, CargoEntry>> sortedItems = new List<KeyValuePair<string, CargoEntry>>();
            private readonly List<KeyValuePair<string, CargoEntry>> sortedOreItems = new List<KeyValuePair<string, CargoEntry>>();
            private readonly List<KeyValuePair<string, CargoEntry>> sortedIngotItems = new List<KeyValuePair<string, CargoEntry>>();
            private readonly List<KeyValuePair<string, CargoEntry>> sortedComponentItems = new List<KeyValuePair<string, CargoEntry>>();

            private double currentCapacity;
            private double totalCapacity;

            public double CurrentCapacity {
                get { return currentCapacity; }
            }

            public double TotalCapacity {
                get { return totalCapacity; }
            }
            public double UsedCapacity {
                get { return totalCapacity > 0 ? currentCapacity / totalCapacity : 0; }
            }

            public int TotalItems {
                get { return items.Count; }
            }

            public List<KeyValuePair<string, CargoEntry>> SortedItems {
                get {
                    return sortedItems;
                }
            }

            public List<KeyValuePair<string, CargoEntry>> SortedOres {
                get {
                    return sortedOreItems;
                }
            }

            public List<KeyValuePair<string, CargoEntry>> SortedIngots {
                get {
                    return sortedIngotItems;
                }
            }

            public List<KeyValuePair<string, CargoEntry>> SortedComponents {
                get {
                    return sortedComponentItems;
                }
            }


            public InventoryModule(ExcavOS handler) : base(handler) {
                inventoryBlocks = handler.GetModule<BlockCacheModule>().RegisterCache<IMyTerminalBlock>(block => {
                    return block.HasInventory && block.GetInventory(0) != null && block.IsFunctional;
                });
            }

            public override void Update100() {
                currentCapacity = 0;
                totalCapacity = 0;
                cargo.Clear();
                inventoryBlocks.ForEach<IMyTerminalBlock>(cargoBlock => {                    
                    for (int i = 0; i < cargoBlock.InventoryCount; i++) {
                        items.Clear();
                        IMyInventory inventory = cargoBlock.GetInventory(i);
                        currentCapacity += (double)inventory.CurrentVolume;
                        totalCapacity += (double)inventory.MaxVolume;
                        inventory.GetItems(items);
                        foreach (MyInventoryItem item in items) {
                            string itemName = item.Type.ToString();
                            double amount = (double)item.Amount;
                            if (cargo.ContainsKey(itemName)) {
                                CargoEntry ce = cargo[itemName];
                                ce.amount += amount;
                            } else {
                                CargoEntry ce = new CargoEntry {
                                    amount = amount,
                                    typeid = item.Type.TypeId
                                };
                                cargo.Add(itemName, ce);
                            }
                        }
                    }
                });

                sortedItems.Clear();
                sortedOreItems.Clear();
                sortedIngotItems.Clear();
                sortedComponentItems.Clear();

                foreach (var item in cargo.OrderByDescending(key => key.Value.amount).ThenBy(key => Formatting.ExtractName(key.Key))) {
                    sortedItems.Add(item);
                    if (item.Value.typeid == "MyObjectBuilder_Ore") {
                        sortedOreItems.Add(item);
                    } else if (item.Value.typeid == "MyObjectBuilder_Ingot") {
                        sortedIngotItems.Add(item);
                    } else {
                        sortedComponentItems.Add(item);
                    }
                }

            }
        }
    }
}
