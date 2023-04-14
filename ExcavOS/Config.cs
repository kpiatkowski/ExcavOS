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
    partial class Program
    {
        public class Config : ScriptConfig
        {

            public string LiftThrustersGroupName = "";
            public string StopThrustersGroupName = "";
            public string CargoTrackGroupName = "";
            public string AlignGyrosGroupName = "";
            public string DumpSortersGroupName = "";
            public float LiftThresholdWarning = 0.9f;
            public float MouseSensitivity = 1f;

            public Config(MyIni ini, string section) : base(ini, section)
            {
            }

            public override void SetupDefaults()
            {
                _ini.Set(_section, "LiftThrustersGroupName", LiftThrustersGroupName);
                _ini.Set(_section, "StopThrustersGroupName", StopThrustersGroupName);
                _ini.Set(_section, "CargoTrackGroupName", CargoTrackGroupName);
                _ini.Set(_section, "AlignGyrosGroupName", AlignGyrosGroupName);
                _ini.Set(_section, "DumpSortersGroupName", DumpSortersGroupName);
                _ini.Set(_section, "MouseSensitivity", MouseSensitivity);
                _ini.Set(_section, "LiftThresholdWarning", LiftThresholdWarning);
            }

            public override void ReadConfig()
            {
                LiftThrustersGroupName = GetValue("LiftThrustersGroupName").ToString(LiftThrustersGroupName);
                StopThrustersGroupName = GetValue("StopThrustersGroupName").ToString(StopThrustersGroupName);
                CargoTrackGroupName = GetValue("CargoTrackGroupName").ToString(CargoTrackGroupName);
                AlignGyrosGroupName = GetValue("AlignGyrosGroupName").ToString(AlignGyrosGroupName);
                DumpSortersGroupName = GetValue("DumpSortersGroupName").ToString(DumpSortersGroupName);
                LiftThresholdWarning = GetValue("LiftThresholdWarning").ToSingle(LiftThresholdWarning);
                MouseSensitivity = GetValue("MouseSensitivity").ToSingle(MouseSensitivity);
            }
        }
    }
}
