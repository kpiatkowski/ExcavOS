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
        public class TestScreen : ScreenHandler {

            private Icon icon;
            private TableLayout tableLayout;

            public TestScreen(ExcavOS excavOS, IMyTextSurface surface) : base(excavOS, surface) {
            }

            protected override void SetupComponents() {
                //var headerBackground = AddComponent(new Panel(PrimaryColor));
                tableLayout = new TableLayout(FullScreenWithMargin(2.0f));
                Vector2 s = TextSize(1.5f, "Refining");
                var header = new TableLayout(new Vector2(0, 64));
                icon = new Icon("Textures\\FactionLogo\\Builders\\BuilderIcon_1.dds", PrimaryColor, new Vector2(64, 64));
                var row = header.AddRow(icon);
                row.AddColumn(new Label("Refining", 1.5f) { DesiredSize = s });
                row.AddColumn(new Label("Working: 1/2", TextAlignment.RIGHT));
                tableLayout.AddRow(header);
                tableLayout.AddRow(new Panel(PrimaryColor, new Vector2(0, 2.0f)));
                //tableLayout.AddRow(new Panel(Color.Red));
                //tableLayout.AddRow(new Panel(Color.Green));
                var grid = new GridLayout {
                    ItemsPerRow = 2,
                    Columns = 4,
                    Gap = 5.0f
                };
                tableLayout.AddRow(grid);
                grid.AddItem(new Panel(Color.Red));
                grid.AddItem(new Panel(Color.Green));
                grid.AddItem(new Panel(Color.White));
                //grid.AddItem(new Panel(Color.Black));
                //grid.AddItem(new Panel(Color.AliceBlue));
                //grid.AddItem(new Panel(Color.BlueViolet));
                //grid.AddItem(new Panel(Color.Chartreuse));
                //grid.AddItem(new Panel(Color.Yellow));
                //grid.AddItem(new Panel(Color.CadetBlue));
                //grid.AddItem(new Panel(Color.BurlyWood));
                //grid.AddItem(new Panel(Color.Azure));
                //grid.AddItem(new Panel(Color.DarkOrchid));
                //grid.AddItem(new Panel(Color.DeepSkyBlue));
                //grid.AddItem(new Panel(Color.Honeydew));
                AddComponent(tableLayout);
                //headerBackground.SetPositionAndSize(header.Position, header.Size);
            }

            protected override void DrawFrame(MySpriteDrawFrame frame) {
                // frame.Add(Painter.DrawRectangle(Center, new Vector2(50, 50), Color.Red));
                //var dm = excavOS.GetModule<DebugModule>();
                //if (icon != null) {
                //    dm.Log($"{icon.Position} x {icon.Size}");
                //}
            }
        }
    }
}
