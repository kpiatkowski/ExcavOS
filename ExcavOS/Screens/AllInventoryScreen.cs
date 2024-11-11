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
        public class AllInventoryScreen : ScreenHandler {

            private InventoryList inventoryList;
            private List<KeyValuePair<string, CargoEntry>> items;

            public AllInventoryScreen(ExcavOS excavOS, IMyTextSurface surface) : base(excavOS, surface) {
            }

            protected override void SetupComponents() {
                float margin = 5.0f;
                inventoryList = new InventoryList(Layoutable.FullScreenWithMargin(this, margin)) {
                    OnDrawLabel = FormatLabelCell,
                    OnDrawValue = FormatValueCell,
                    OnDrawIcon = LabelIcon
                };
                AddComponent(inventoryList);
            }

            protected override void DrawFrame(MySpriteDrawFrame frame) {
                
                items = ListItems();
                inventoryList.ItemCount = items.Count;

                //sprites.AddRange(labelCaption.Draw("Capacity", true, Surface.FontSize * screenSizeModifier));
                //sprites.AddRange(labelUsage.Draw(string.Format("{0:0.00}%", ExcavOSCPU.CargoMonitor.UsedCapacity * 100), true, Surface.FontSize * screenSizeModifier));

                //sprites.AddRange(capacityUsage.Draw((float)ExcavOSCPU.CargoMonitor.UsedCapacity, true));
                //if (items.Count() == 0) {
                //    sprites.AddRange(emptyPanel.Draw("Cargo empty", EmptyCargoSprite));
                //} else {
                //    sprites.AddRange(inventoryList.Draw(items.Count()));
                //}

            }

            private string LabelIcon(int index) {
                return items[index].Key;
            }

            private string FormatLabelCell(int index) {
                return Formatting.ExtractName(items[index].Key);
            }

            private string FormatValueCell(int index) {
                string amount;
                var entry = items[index].Value;
                if (entry.typeid == "MyObjectBuilder_Ore" || entry.typeid == "MyObjectBuilder_Ingot") {
                    amount = Formatting.FormatWeightWithSuffix(entry.amount);
                } else {
                    amount = string.Format("{0,10:0}", entry.amount);
                }
                return amount;
            }

            protected virtual List<KeyValuePair<string, CargoEntry>> ListItems() {
                return excavOS.GetModule<InventoryModule>().SortedItems;
            }

        }
    }
}
