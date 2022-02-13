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
        public class ExcavOSContext
        {
            public Program _program;
            private Config _config;
            public MyIni _storage;

            public TimeSpan TimeAccumulator;
            public Random Randomizer = new Random();
            public CargoManager _cargoManager;
            public WeightAnalizer _weightAnalizer;
            public UtilityManager _utilitymanager;
            public SystemManager _systemmanager;

            public int tick;

            public ExcavOSContext(Program program, Config config, MyIni storage)
            {
                _program = program;
                _config = config;
                _storage = storage;
                _cargoManager = new CargoManager(_program, _config);
                _systemmanager = new SystemManager(_program, _config);
                _weightAnalizer = new WeightAnalizer(_program, _config, _cargoManager, _systemmanager);
                _utilitymanager = new UtilityManager(_program, _config, _cargoManager, _systemmanager, _storage);
            }

            public void Save() {
                _utilitymanager.Save();
            }

            public void Update(TimeSpan time)
            {
                TimeAccumulator = time;
                _cargoManager.QueryData();
                _weightAnalizer.QueryData(time);
                _utilitymanager.Update();
                _systemmanager.Update();
                tick++;
            }

            public void HandleCommand(string argument)
            {
                string[] args = argument.Split(' ');
                switch (args[0].ToLower()) {
                    case "toggle_gaa":
                        _utilitymanager.GravityAlign = !_utilitymanager.GravityAlign;
                        break;
                    case "set_gaa_pitch":
                        char modifier = args[1][0];
                        float pitch = float.Parse(args[1]);
                        if (modifier.ToString() == "+" || modifier.ToString() == "-") {
                            _utilitymanager.GravityAlignPitch += pitch;
                        }
                        else if (!float.IsNaN(pitch)) {
                            _utilitymanager.GravityAlignPitch = pitch;
                        }
                        _utilitymanager.GravityAlignPitch = MathHelper.Clamp(_utilitymanager.GravityAlignPitch, -90, 90);
                        break;
                    case "toggle_cruise":
                        _utilitymanager.CruiseEnabled = !_utilitymanager.CruiseEnabled;
                        if (_utilitymanager.CruiseEnabled)
                        {
                            _utilitymanager.thrustPID.Reset();
                        } else
                        {
                            _systemmanager.CruiseThrusters.ForEach(thruster => thruster.ThrustOverridePercentage = 0.0f);
                            _systemmanager.CruiseReverseThrusters.ForEach(thruster => {
                                thruster.ThrustOverridePercentage = 0.0f;
                                thruster.Enabled = true;
                            });
                        }
                        break;
                    case "set_cruise":
                        modifier = args[1][0];
                        float speed = float.Parse(args[1]);
                        if (modifier.ToString() == "+" || modifier.ToString() == "-")
                        {
                            _utilitymanager.CruiseTarget += speed;
                        }
                        else if (!float.IsNaN(speed))
                        {
                            _utilitymanager.CruiseTarget = speed;
                        }                        
                        break;
                    case "dump":
                        _utilitymanager.SetSortersFilter(args[1]);
                        break;
                }
            }
        }
    }
}