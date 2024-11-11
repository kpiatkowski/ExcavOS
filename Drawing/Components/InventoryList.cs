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
        public class InventoryList : ItemList {

            public delegate string CellValue(int itemIndex);

            public CellValue OnDrawIcon;
            public CellValue OnDrawLabel;
            public CellValue OnDrawValue;

            public InventoryList() : base() { }
            public InventoryList(Vector2 position, Vector2 size) : base(position, size) { }
            public InventoryList(Vector4 sizeAndPosition) : base(new Vector2(sizeAndPosition.Z, sizeAndPosition.W), new Vector2(sizeAndPosition.X, sizeAndPosition.Y)) { }

            protected override IEnumerable<MySprite> DrawSingleRow(IMyTextSurface surface, int itemIndex, Vector2 position, float rowWidth) {
                if (OnDrawLabel != null) {
                    string label = OnDrawLabel(itemIndex);
                    if (OnDrawValue != null) {
                        string value = OnDrawValue(itemIndex);
                        DrawText(new Vector2(position.X + rowWidth, position.Y), value, surface.ScriptForegroundColor, surface.Font, surface, surface.FontSize, TextAlignment.RIGHT);
                    }
                    if (OnDrawIcon != null) {
                        string icon = OnDrawIcon(itemIndex);
                        Vector2 iconSize = new Vector2(textSize.Y, textSize.Y);
                        DrawSprite(position, iconSize, icon, surface.ScriptForegroundColor);
                        position += new Vector2(textSize.Y + style.paddingHorizontal, 0);
                    }
                    DrawText(position, label, surface.ScriptForegroundColor, surface.Font, surface, surface.FontSize);
                }
                return Enumerable.Empty<MySprite>();
            }

        }
    }
}
