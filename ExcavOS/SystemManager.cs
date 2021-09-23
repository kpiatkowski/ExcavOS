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
        public class SystemManager {

            public string Status;
            private Program _program;
            private Config _config;

            private readonly BlockFinder<IMyGyro> _gyros;
            private readonly BlockFinder<IMyShipController> _controllers;
            private IMyShipController _controller;

            private readonly BlockFinder<IMyThrust> _liftThrusters;
            private readonly BlockFinder<IMyThrust> _stopThrusters;

            public IMyShipController ActiveController { get { return _controller; } }
            public List<IMyThrust> LiftThrusters { get { return _liftThrusters.blocks; } }
            public List<IMyThrust> StopThrusters { get { return _stopThrusters.blocks; } }

            public SystemManager(Program program, Config config) {
                _program = program;
                _config = config;
                _gyros = new BlockFinder<IMyGyro>(_program);
                _controllers = new BlockFinder<IMyShipController>(_program);
                _liftThrusters = new BlockFinder<IMyThrust>(_program);
                _stopThrusters = new BlockFinder<IMyThrust>(_program);
                
            }

            public void Update() {
                UpdateController();
                UpdateThrusterGroups();
            }
            private void UpdateController() {
                IMyShipController firstWorking = null;
                _controllers.FindBlocks(true, null, "");
                foreach (IMyShipController _controller in _controllers.blocks) {
                    if (!_controller.IsWorking) continue;
                    if (firstWorking == null) firstWorking = _controller;
                    if (this._controller == null && _controller.IsUnderControl && _controller.CanControlShip) this._controller = _controller;
                    if (_controller.IsMainCockpit) this._controller = _controller;
                }
                if (_controller == null) _controller = firstWorking;

                if (_controller == null) {
                    throw new Exception("Missing Controller!");
                    return;
                }
            }
            private void UpdateThrusterGroups() {
                if (_controller == null) return;
                if (_config.LiftThrustersGroupName != "") {
                    _liftThrusters.FindBlocks(true, null, _config.LiftThrustersGroupName);
                }
                else {
                    _liftThrusters.FindBlocks(true, thruster => {
                        Vector3D thrusterDirection = -thruster.WorldMatrix.Forward;
                        //double forwardDot = Vector3D.Dot(thrusterDirection, _controller.WorldMatrix.Forward);
                        double upDot = Vector3D.Dot(thrusterDirection, -Vector3.Normalize(_controller.GetTotalGravity()));
                        //double leftDot = Vector3D.Dot(thrusterDirection, _controller.WorldMatrix.Left);

                        if (upDot >= 0.5) {
                            return true;
                        }
                        return false;
                    });
                }
                if (_config.StopThrustersGroupName != "") {
                    _stopThrusters.FindBlocks(true, null, _config.StopThrustersGroupName);
                }
                else {
                    _stopThrusters.FindBlocks(true, thruster => {
                        Vector3D thrusterDirection = -thruster.WorldMatrix.Forward;
                        double forwardDot = Vector3D.Dot(thrusterDirection, _controller.GetShipVelocities().LinearVelocity);
                        //double upDot = Vector3D.Dot(thrusterDirection, _controller.WorldMatrix.Up);
                        //double leftDot = Vector3D.Dot(thrusterDirection, _controller.WorldMatrix.Left);

                        if (forwardDot <= -0.7) {
                            return true;
                        }
                        return false;
                    });
                }
            }
        }
    }
}
