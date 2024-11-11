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
    partial class Program : MyGridProgram
    {
        private readonly ScriptHandler handler;

        public Program() {
            handler = new ExcavOS(this, Storage);
            Runtime.UpdateFrequency = UpdateFrequency.Update10 | UpdateFrequency.Update100;
        }

        public void Save() {
            Storage = handler.Save();
        }

        public void Main(string argument, UpdateType updateSource) {
            handler.Update(argument, updateSource);
        }
    }
}