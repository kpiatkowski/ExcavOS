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
        public class ExcavOS : ScriptHandler {
            public ExcavOS(Program program, string initialConfig) : base(program, initialConfig) {
                RegisterModule(new SystemModule(this));
                RegisterModule(new InventoryModule(this));
                RegisterModule(new FlightModule(this));                
                RegisterModule(new ScreenModule(this));
                RegisterModule(new WeightModule(this));

                // register PB block to show ExcavOS logo while running
                GetModule<ScreenModule>().AddOrUpdateScreenToManage(program.Me, "Logo", 0);
            }

            protected override string GetInfo() {
                ScreenModule sm = GetModule<ScreenModule>();
                return base.GetInfo() + $"Managed screens: {sm.ManagedScreenCount}";
            }
        }
    }
}
