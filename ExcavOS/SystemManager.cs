using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System;
using System.Collections;
using System.Collections.Generic;
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
        public class SystemManager
        {

            public string Status;
            private Program _program;
            private Config _config;

            private readonly BlockFinder<IMyGyro> _gyros;
            private readonly BlockFinder<IMyShipController> _controllers;
            private IMyShipController _controller;

            private readonly BlockFinder<IMyThrust> _Thrusters;
            private readonly ThrustGroups _ThrustGroups;

            public IMyShipController ActiveController { get { return _controller; } }
            public List<IMyThrust> Thrusters { get { return _Thrusters.blocks; } }
            public ThrustGroups ThrusterGroups { get { return _ThrustGroups; } }
            public SystemManager(Program program, Config config)
            {
                _program = program;
                _config = config;
                _gyros = new BlockFinder<IMyGyro>(_program);
                _controllers = new BlockFinder<IMyShipController>(_program);
                _Thrusters = new BlockFinder<IMyThrust>(_program);
                _ThrustGroups = new ThrustGroups();
            }

            public void Update()
            {
                UpdateController();
                UpdateThrusterGroups();
            }
            private void UpdateController()
            {
                IMyShipController firstWorking = null;
                _controllers.FindBlocks(true, null, "");
                foreach (IMyShipController _controller in _controllers.blocks)
                {
                    if (!_controller.IsWorking) continue;
                    if (firstWorking == null) firstWorking = _controller;
                    if (this._controller == null && _controller.IsUnderControl && _controller.CanControlShip) this._controller = _controller;
                    if (_controller.IsMainCockpit) this._controller = _controller;
                }
                if (_controller == null) _controller = firstWorking;

                if (_controller == null)
                {
                    throw new Exception("Missing Controller!");
                }
            }
            private void UpdateThrusterGroups()
            {
                if (_controller == null) return;

                _ThrustGroups.ClearAll();
                _Thrusters.FindBlocks(true, thruster =>
                {
                    _ThrustGroups.Add(thruster, _controller);
                    return true;
                });
                _ThrustGroups.UpdateAll();
            }
        }
    }
}
