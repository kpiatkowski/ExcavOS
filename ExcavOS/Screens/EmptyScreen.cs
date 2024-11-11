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
        public class EmptyScreen : ScreenHandler {

            private FlexLayout layout;
            private FlexLayout layout2;
            private FlexLayout layout3;

            private Gauge g1;
            private Gauge g2;
            private InventoryList il1;

            private List<KeyValuePair<string, CargoEntry>> items;

            public EmptyScreen(ExcavOS excavOS, IMyTextSurface surface) : base(excavOS, surface) {
            }

            protected override void SetupComponents() {
                
                Label l1 = AddComponent(new Label("Something"));
                l1.DropShadow = true;
                Label l2 = AddComponent(new Label("Other stuff"));
                Label l3 = AddComponent(new Label("Testy", 2.0f));
                l3.DropShadow = true;
                layout = new FlexLayout(10.0f);

                g1 = AddComponent(new Gauge());
                g2 = AddComponent(new Gauge());
                //il1 = AddComponent(new InventoryList());
                il1 = new InventoryList() {
                    OnDrawLabel = FormatLabelCell,
                    OnDrawValue = FormatValueCell,
                    OnDrawIcon = LabelIcon
                };
                AddComponent(il1);
                layout2 = new FlexLayout(10.0f);                

                layout3 = new FlexLayout(Layoutable.FullScreenWithMargin(this, 10.0f), 10.0f, true);
                layout3.AddComponent(layout);
                layout3.AddComponent(layout2);

                layout.AddComponent(l1);
                layout.AddComponent(l2);
                layout.AddComponent(l3);

                layout2.AddComponent(g1);
                layout2.AddComponent(g2);
                layout2.AddComponent(il1);
            }

            protected override void DrawFrame(MySpriteDrawFrame frame) {
                g1.Value += 1.0f;
                g2.Value += 1.0f;                
                if (g1.Value > g1.Max) g1.Value = g1.Min;
                if (g2.Value > g2.Max) g2.Value = g2.Min;
                items = ListItems();
                il1.ItemCount = items.Count;
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
