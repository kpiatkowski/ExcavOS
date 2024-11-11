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
        public class TableLayout : Drawable {

            public class TableRow : Drawable {

                private readonly TableLayout ParentTable;
                protected internal readonly List<Drawable> Columns = new List<Drawable>();
                public float Padding = 0.0f;

                public TableRow(TableLayout parent, Drawable item) : base(item.Size) {
                    ParentTable = parent;
                    Columns.Add(item);
                }

                public void AddColumn(Drawable item) {
                    Columns.Add(item);
                    ParentTable.ArrangeRows();
                }

                public float RowHeight() {
                    float height = 0.0f;
                    foreach (var column in Columns) {
                        height = Math.Max(height, column.DesiredSize.Y);
                    }
                    return height;
                }

                public new void SetPositionAndSize(Vector2 position, Vector2 size) {
                    base.SetPositionAndSize(position, size);

                    // set columns position and size
                    float requiredWidth = 0.0f;
                    int columnsToDistribute = Columns.Count;
                    foreach (var column in Columns) {
                        float colWidth = column.DesiredSize.X;
                        if (colWidth > 0.0f) {
                            requiredWidth += colWidth;
                            columnsToDistribute--;
                        }
                    }

                    // what was left
                    float leftOverWidth = Size.X - requiredWidth;
                    if (leftOverWidth < 0.0f) leftOverWidth = 0.0f;

                    // calculate row size
                    float autoColumnWidth = 0.0f;
                    if (columnsToDistribute > 0) {
                        autoColumnWidth = leftOverWidth / columnsToDistribute;
                    }

                    Vector2 pos = Position;
                    foreach (var column in Columns) {
                        float columnWidth = column.DesiredSize.X;
                        Vector2 newColumnSize = new Vector2(columnWidth > 0.0f ? columnWidth : autoColumnWidth, Size.Y);
                        column.SetPositionAndSize(pos, newColumnSize);
                        pos.X += newColumnSize.X;
                    }
                }

                protected override void DrawSprites(IMyTextSurface surface) {
                    foreach (var column in Columns) {
                        column.Draw(surface);
                        AddSprites(column.Sprites);
                    }
                }
            }

            private readonly List<TableRow> Rows = new List<TableRow>();

            public TableLayout() : base(Vector2.Zero) {
            }
            public TableLayout(Vector2 size) : base(size) { 
            }

            public TableLayout(Vector4 sizeAndPosition) : base(sizeAndPosition) {
            }

            public TableRow AddRow(Drawable item) {
                TableRow newRow = new TableRow(this, item);
                Rows.Add(newRow);
                ArrangeRows();
                return newRow;
            }

            public override void SetPositionAndSize(Vector2 position, Vector2 size) {
                base.SetPositionAndSize(position, size);
                ArrangeRows();
            }

            protected internal void ArrangeRows() {
                // ask each row for it's height so leftover space can be distributed
                float requiredHeight = 0.0f;
                int rowsToDistribute = Rows.Count;
                foreach (var row in Rows) {
                    float rowHeight = row.RowHeight();
                    if (rowHeight > 0.0f) {
                        rowsToDistribute--;
                        requiredHeight += rowHeight;
                    }
                }

                // what was left
                float leftOverHeight = Size.Y - requiredHeight;
                if (leftOverHeight < 0.0f) leftOverHeight = 0.0f;

                // calculate row size
                float autoRowHeight = 0.0f;
                if (rowsToDistribute > 0) {
                    autoRowHeight = leftOverHeight / rowsToDistribute;
                }

                Vector2 pos = Position;
                foreach (var row in Rows) {
                    float rowHeight = row.RowHeight();
                    Vector2 rowSize = new Vector2(Size.X, rowHeight > 0.0f ? rowHeight : autoRowHeight);
                    row.SetPositionAndSize(pos, rowSize);
                    pos.Y += rowSize.Y;
                }

            }

            protected override void DrawSprites(IMyTextSurface surface) {
                foreach (var row in Rows) {
                    row.Draw(surface);
                    AddSprites(row.Sprites);
                }
            }
        }
    }
}
