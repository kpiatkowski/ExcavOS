using Sandbox;
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
        abstract public class ScriptModule {

            protected readonly ScriptHandler handler;

            public ScriptModule(ScriptHandler handler) {
                this.handler = handler;
            }

            public virtual void Update10() { }
            public virtual void Update100() { }

            protected void Log(string message) {
                handler.Log(message);
            }

            protected void RegisterCommand(string command, CommandFunc executor) {
                handler.RegisterCommand(command, executor);
            }

            public virtual void SaveRuntimeVariables(MyIni ini, string section) { }
            public virtual void LoadRuntimeVariables(MyIni ini, string section) { }
            public virtual void LoadUserConfiguration(MyIni ini, string section) { }
        }
    }
}
