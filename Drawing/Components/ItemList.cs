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
        public class ItemList : Drawable {

            public class ComponentStyle {
                public float paddingVertical;
                public float paddingHorizontal;
                public float itemGap;
                public float borderSize;
                public bool border;
                public bool itemSeparator;
                public bool alignContentVertically;
                public bool drawScrollBar;
            }

            public static readonly ComponentStyle CompactStyle = new ComponentStyle {
                border = false,
                itemGap = 2.0f,
                paddingHorizontal = 4.0f,
                paddingVertical = 2.0f,
                itemSeparator = false,
                alignContentVertically = true,
                drawScrollBar = true,
            };

            public static readonly ComponentStyle RelaxedStyle = new ComponentStyle {
                border = true,
                itemGap = 1.0f,
                paddingHorizontal = 8.0f,
                paddingVertical = 2.0f,
                itemSeparator = true,
                alignContentVertically = false,
                drawScrollBar = true
            };

            public delegate List<MySprite> DrawRow(IMyTextSurface surface, int itemIndex, Vector2 position, float rowWidth);
            public DrawRow OnDrawRow;
            private readonly StringBuilder sb = new StringBuilder().Append("Qy");
            protected Vector2 textSize;
            protected ComponentStyle style;
            private readonly int TicksPerPage = 10;
            private int tickCounter = 0;
            public int ItemCount = 0;

            public ItemList() : base() { }
            public ItemList(Vector2 position, Vector2 size) : base(position, size) { }
            public ItemList(Vector4 sizeAndPosition) : base(new Vector2(sizeAndPosition.Z, sizeAndPosition.W), new Vector2(sizeAndPosition.X, sizeAndPosition.Y)) { }

            protected virtual IEnumerable<MySprite> DrawSingleRow(IMyTextSurface surface, int itemIndex, Vector2 position, float rowWidth) {
                if (OnDrawRow != null) {
                    AddSprites(OnDrawRow(surface, itemIndex, position, rowWidth));
                }
                return Enumerable.Empty<MySprite>();
            }

            protected override void DrawSprites(IMyTextSurface surface) {
                style = (Size.Y < 512 ? CompactStyle : RelaxedStyle);
                style = RelaxedStyle;
                textSize = surface.MeasureStringInPixels(sb, surface.Font, surface.FontSize);

                float rowHeight = textSize.Y + style.itemGap + style.paddingVertical;
                float rowWidth = Size.X - style.paddingHorizontal * 2.0f;
                int maxItemsOnScreen = (int)(Size.Y / rowHeight);
                float leftoverSpace = 0.0f;
                if (style.alignContentVertically) {
                    leftoverSpace = Size.Y - rowHeight * maxItemsOnScreen;
                }
                int itemsOnPage = Math.Min(ItemCount, maxItemsOnScreen);
                int totalPages = (int)Math.Ceiling(ItemCount / (float)itemsOnPage);
                int currentPage = (tickCounter / TicksPerPage) % totalPages;
                bool hasScrollBar = style.drawScrollBar && totalPages > 1;

                if (hasScrollBar) {
                    float scrollBorder = 1.0f;
                    float scrollSize = 6.0f;
                    rowWidth -= style.paddingHorizontal + scrollSize + 2.0f * scrollBorder;
                    Vector2 scrollPosition = new Vector2(Position.X + rowWidth + 2.0f * style.paddingHorizontal, Position.Y + style.paddingVertical);
                    // Components.Frame.Draw(scrollPosition, new Vector2(scrollSize, size.Y - 2.0f * style.paddingVertical), PrimaryColor, scrollBorder);
                    float scrollHeight = Size.Y - 2.0f * (style.paddingVertical + scrollBorder);
                    float trackerHeight = scrollHeight / totalPages;
                    scrollPosition += scrollBorder;
                    scrollPosition += new Vector2(0, currentPage * trackerHeight);
                    DrawRectangle(scrollPosition, new Vector2(scrollSize - 2.0f * scrollBorder, trackerHeight), surface.ScriptForegroundColor);
                }

                Vector2 itemOffset = Position + new Vector2(style.paddingHorizontal, style.paddingVertical + leftoverSpace * 0.5f);
                int startItem = currentPage * itemsOnPage;
                int endItem = startItem + itemsOnPage;
                if (endItem > ItemCount) endItem = ItemCount;
                for (int itemIndex = startItem; itemIndex < endItem; itemIndex++) {
                    DrawSingleRow(surface, itemIndex, itemOffset, rowWidth);
                    // DrawLabel(itemOffset + new Vector2(textSize.Y + style.paddingHorizontal, 0), label);
                    // DrawItemValue(new Vector2(itemOffset.X + rowWidth, itemOffset.Y), itemIndex);
                    // DrawLabel(new Vector2(itemOffset.X + rowWidth, itemOffset.Y), value, PrimaryColor, Surface.FontSize, TextAlignment.RIGHT);
                    itemOffset.Y += textSize.Y + style.paddingVertical * 2.0f;
                    if (style.itemSeparator) {
                        DrawRectangle(itemOffset - new Vector2(0, style.itemGap * 2.0f), new Vector2(rowWidth, style.itemGap), surface.ScriptForegroundColor);
                    }
                }

                tickCounter++;
            }
        }
    }
}
