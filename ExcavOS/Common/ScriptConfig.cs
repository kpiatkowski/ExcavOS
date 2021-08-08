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
        abstract public class ScriptConfig
        {
            protected readonly MyIni _ini;
            protected readonly string _section;

            public ScriptConfig(MyIni ini, string section)
            {
                _ini = ini;
                _section = section;
            }

            protected MyIniValue GetValue(string key) {
                return _ini.Get(_section, key);
            }

            protected void SetValue(string key, string value)
            {
                _ini.Set(_section, key, value);
            }

            protected void SetValue(string key, float value)
            {
                _ini.Set(_section, key, value);
            }

            protected void SetValue(string key, int value)
            {
                _ini.Set(_section, key, value);
            }

            protected void SetValue(string key, bool value)
            {
                _ini.Set(_section, key, value);
            }

            protected bool KeyExists(string key)
            {
                return _ini.ContainsKey(_section, key);
            }

            public void LoadConfig(string blob)
            {
                MyIniParseResult result;
                if (!_ini.TryParse(blob, _section, out result))
                {
                    return;
                }

                if (!_ini.ContainsSection(_section))
                {
                    return;
                }

                ReadConfig();
            }

            abstract public void SetupDefaults();
            abstract public void ReadConfig();

        }
    }
}
