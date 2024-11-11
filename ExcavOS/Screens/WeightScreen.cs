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
        public class WeightScreen : ScreenHandler {

            public class MutableKeyValuePair<TKey, TValue> {
                public TKey Key { get; set; }
                public TValue Value { get; set; }
                public MutableKeyValuePair(TKey key, TValue value) {
                    Key = key;
                    Value = value;
                }
            }

            public override UpdateFrequency UpdateFrequency => UpdateFrequency.Update10;
            private InventoryList inventoryList;
            private List<MutableKeyValuePair<string, string>> items = new List<MutableKeyValuePair<string, string>>
            {
                new MutableKeyValuePair<string, string>("Mass", "00kg"),
                new MutableKeyValuePair<string, string>("Thrust", "0%"),
                new MutableKeyValuePair<string, string>("Key3", "Value3"),
            };

            public WeightScreen(ExcavOS excavOS, IMyTextSurface surface) : base(excavOS, surface) {
            }

            protected override void SetupComponents() {
                float margin = 0.0f;
                inventoryList = new InventoryList(Layoutable.FullScreenWithMargin(this, margin)) {
                    OnDrawLabel = FormatLabelCell,
                    OnDrawValue = FormatValueCell
                    //OnDrawIcon = LabelIcon
                };
                AddComponent(inventoryList);
            }

            protected override void DrawFrame(MySpriteDrawFrame frame) {
                WeightModule wm = excavOS.GetModule<WeightModule>();
                items[0].Value = Formatting.FormatWeightWithSuffix(wm.Mass);
                items[1].Value = string.Format("{0:0.00}%", wm.LiftThrustUsage * 100);
                inventoryList.ItemCount = items.Count;
            }

            private string LabelIcon(int index) {
                return items[index].Key;
            }

            private string FormatLabelCell(int index) {
                return Formatting.ExtractName(items[index].Key);
            }

            private string FormatValueCell(int index) {                
                var entry = items[index].Value;
                return entry;
            }

        }
    }
}
