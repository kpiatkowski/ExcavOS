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
        public class GridLayout : Drawable {

            private readonly List<Drawable> Items = new List<Drawable>();
            public int ItemsPerRow = 4;
            public int Columns = 2;
            public float Gap = 5.0f;

            public GridLayout() : base(Vector2.Zero) {
            }
            public GridLayout(Vector2 size) : base(size) {
            }

            public GridLayout(Vector4 sizeAndPosition) : base(sizeAndPosition) {
            }

            public T AddItem<T>(T item) where T : Drawable {
                Items.Add(item);
                ArrangeItems();
                return item;
            }

            public override void SetPositionAndSize(Vector2 position, Vector2 size) {
                base.SetPositionAndSize(position, size);
                ArrangeItems();
            }

            private void ArrangeItems() {
                float itemWidth = Size.X / ItemsPerRow;
                float itemHeight = Size.Y / Columns;
                Vector2 pos = Position;
                Vector2 itemSize = new Vector2(itemWidth, itemHeight);
                int rowCounter = 0;
                int colCounter = 0;
                foreach (var item in Items) {
                    Vector2 lastRowItemGap = Vector2.Zero;
                    if (rowCounter == ItemsPerRow - 1) {
                        lastRowItemGap.X = Gap;
                    }
                    if (colCounter == Columns - 1) {
                        lastRowItemGap.Y = Gap;
                    }
                    item.SetPositionAndSize(pos + Gap, itemSize - Gap - lastRowItemGap);
                    rowCounter++;
                    if (rowCounter == ItemsPerRow) {
                        rowCounter = 0;
                        pos.Y += itemHeight;
                        pos.X = Position.X;
                        colCounter++;
                        if (colCounter == Columns) {
                            break;
                        }
                    } else {
                        pos.X += itemWidth;
                    }

                }
            }

            protected override void DrawSprites(IMyTextSurface surface) {
                foreach (var item in Items) {
                    item.Draw(surface);
                    AddSprites(item.Sprites);
                }
            }
        }
    }
}
