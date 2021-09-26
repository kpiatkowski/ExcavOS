using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System;
using VRage.Collections;
using VRage.Game.Components;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Game;
using VRage;
using VRageMath;

namespace IngameScript
{
    partial class Program : MyGridProgram
    {
        private ExcavOS _scriptHandler;
        private MyIni _storage;

        public Program()
        {
            _storage = new MyIni();
            _storage.TryParse(Storage);
            _scriptHandler = new ExcavOS(this,_storage);
            Runtime.UpdateFrequency = UpdateFrequency.Update10 | UpdateFrequency.Update100;
        }

        public void Save()
        {
            _scriptHandler.Save();
            Storage = _storage.ToString();
            
        }

        public void Main(string argument, UpdateType updateSource)
        {
            _scriptHandler.Update(argument, updateSource, Runtime.TimeSinceLastRun);            
        }
    }
}
