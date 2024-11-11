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
        public class DebugModule : ScriptModule {

            private readonly List<string> logLines = new List<string>();

            public DebugModule(ScriptHandler handler): base(handler) {
            }

            public new void Log(string message) {
                logLines.Add(message);
                if (logLines.Count > Constants.MAX_DEBUG_LINES) {
                    logLines.RemoveAt(0);
                }
            }

            public override void Update100() {
                IMyTerminalBlock block = handler.Program.GridTerminalSystem.GetBlockWithName($"[{Constants.SCRIPT_NAME}] {Constants.DEBUG_TAG}");
                if (!(block is IMyTextPanel)) {
                    return;
                }
                IMyTextPanel panel = block as IMyTextPanel;
                panel.WriteText(string.Join("\n", logLines), false);
            }

        }
    }
}
