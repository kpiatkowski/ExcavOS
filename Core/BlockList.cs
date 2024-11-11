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

namespace IngameScript
{
    partial class Program
    {
        public class BlockList<T> : List<T> where T : class {

            protected readonly Program program;

            public BlockList(Program program) {
                this.program = program;
            }

            private Func<T, bool> BlockFilter(bool sameConstruct = true, Func<T, bool> filter = null) {
                return block => {
                    bool constructCheck = true;
                    if (block is IMyTerminalBlock) {
                        if (sameConstruct) {
                            constructCheck = (block as IMyTerminalBlock).IsSameConstructAs(program.Me);
                        } else {
                            constructCheck = !(block as IMyTerminalBlock).IsSameConstructAs(program.Me);
                        }
                    }
                    return constructCheck && (filter == null || filter(block));
                };
            }

            public void FindBlocks(bool sameConstruct = true, Func<T, bool> filter = null, string groupName = null) {
                Clear();
                if (groupName != null && groupName != "") {
                    IMyBlockGroup group = program.GridTerminalSystem.GetBlockGroupWithName(groupName);
                    if (group != null) {
                        group.GetBlocksOfType(this, BlockFilter(sameConstruct, filter));
                    }
                } else {
                    program.GridTerminalSystem.GetBlocksOfType(this, BlockFilter(sameConstruct, filter));
                }
            }

        }

    }
}
