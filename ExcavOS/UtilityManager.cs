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
        public class UtilityManager
        {
            private string sectionKey = "UtilMan";
            private Program _program;
            private Config _config;
            private MyIni _storage;
            public string Status;
            private CargoManager _cargoManager;
            private SystemManager _systemManager;

            private readonly BlockFinder<IMyGyro> _gyros;
            private readonly BlockFinder<IMyShipController> _controllers;
            private readonly BlockFinder<IMyConveyorSorter> _sorters;
            private readonly BlockFinder<IMyBatteryBlock> _batteries;
            private readonly BlockFinder<IMyGasTank> _hydrogenTanks;
            private readonly BlockFinder<IMyReactor> _reactors;
            private readonly List<MyInventoryItemFilter> sorterList = new List<MyInventoryItemFilter>();
            public readonly PIDController thrustPID = new PIDController(1.0 / 60.0);

            private const double ThrustKp = 0.5;
            private const double ThrustTi = 0.1;
            private const double ThrustTd = 0.0;

            public bool GravityAlign = false;
            public bool CruiseEnabled = false;
            private bool _GravityAlignActive = false;
            public float GravityAlignPitch = 0;
            public float CruiseTarget = 0;
            public string BatteryCharge = "";
            public string HydrogenCharge = "";
            public double BatteryLevel = 0;
            public double HydrogenLevel = 0;
            public double UraniumLevel = 0;

            public UtilityManager(Program program, Config config, CargoManager cargoManager, SystemManager systemManager, MyIni storage)
            {
                _program = program;
                _config = config;
                _storage = storage;
                _cargoManager = cargoManager;
                _systemManager = systemManager;
                _gyros = new BlockFinder<IMyGyro>(_program);
                _sorters = new BlockFinder<IMyConveyorSorter>(_program);
                _controllers = new BlockFinder<IMyShipController>(_program);
                _batteries = new BlockFinder<IMyBatteryBlock>(_program);
                _hydrogenTanks = new BlockFinder<IMyGasTank>(_program);
                _reactors = new BlockFinder<IMyReactor>(_program);
                thrustPID.Kp = ThrustKp;
                thrustPID.Ti = ThrustTi;
                thrustPID.Td = ThrustTd;
                Initialize();
            }

            public void Save() {
                _storage.Set(sectionKey, "GAP", GravityAlignPitch);
                _storage.Set(sectionKey, "CruiseTarget", CruiseTarget);
            }

            protected void Initialize() {
                GravityAlignPitch = (float)_storage.Get(sectionKey, "GAP").ToDouble(0);
                CruiseTarget = (float)_storage.Get(sectionKey, "CruiseTarget").ToDouble(0);
            }

            public void Update()
            {
                _gyros.FindBlocks(true, null, _config.AlignGyrosGroupName);
                _sorters.FindBlocks(true, null, _config.DumpSortersGroupName);
                _controllers.FindBlocks(true, null);
                _batteries.FindBlocks(true, null);
                _reactors.FindBlocks(true, null);
                _hydrogenTanks.FindBlocks(true, tank => {
                    return BlockHelper.IsHydrogenTank(tank);
                });
                CalculateCharge();
                CalculateHydrogen();
                CalculateUranium();

                IMyShipController controller = null;
                IMyShipController firstWorking = null;
                foreach ( IMyShipController _controller in _controllers.blocks) {
                    if (!_controller.IsWorking) continue;
                    if (firstWorking == null) firstWorking = _controller;
                    if (controller == null && _controller.IsUnderControl && _controller.CanControlShip) controller = _controller;
                    if (_controller.IsMainCockpit) controller = _controller;
                }
                if (controller == null) controller = firstWorking;

                if (controller == null)
                {
                    Status = "Missing controller";
                    return;
                }

                if (_gyros.Count() == 0)
                {
                    Status = "Missing gyros";
                    return;
                }

                Status = "";

                if (GravityAlign)
                {
                    _GravityAlignActive = true;
                    DoGravityAlign(controller, _gyros.blocks,GravityAlignPitch);
                }
                if(!GravityAlign && _GravityAlignActive) {
                    ReleaseGyros(_gyros.blocks);
                }

                if (CruiseEnabled)
                {
                    double currentSpeed = controller.GetShipSpeed();

                    var error = CruiseTarget - currentSpeed;
                    var force = thrustPID.Compute(error);
                    _systemManager.CruiseThrusters.ForEach(thruster =>
                    {
                        if (Math.Abs(error) < 0.02f * CruiseTarget)
                        {
                            thruster.ThrustOverridePercentage = 0.0f;
                        } else if (force > 0.0)
                        {
                            thruster.Enabled = true;
                            thruster.ThrustOverridePercentage = (float)force * 0.1f;                            
                        } else
                        {
                            thruster.ThrustOverridePercentage = 0.0f;
                            thruster.Enabled = false;
                        }
                    });

                    _systemManager.CruiseReverseThrusters.ForEach(thruster =>
                    {
                        if (Math.Abs(error) < 0.02f * CruiseTarget)
                        {
                            thruster.ThrustOverridePercentage = 0.0f;
                            thruster.Enabled = false;
                        }
                        else if (force > 0.0)
                        {
                            thruster.ThrustOverridePercentage = 0.0f;
                            thruster.Enabled = false;
                        }
                        else
                        {
                            thruster.Enabled = true;
                            thruster.ThrustOverridePercentage = -(float)force * 0.1f;
                        }
                    });

                }
            }

            public void SetSortersFilter(string item_id)
            {
                try
                {
                    List<MyInventoryItemFilter> currentList = new List<MyInventoryItemFilter>();
                    MyInventoryItemFilter item = new MyInventoryItemFilter("MyObjectBuilder_Ore/" + item_id);
                    _sorters.ForEach(sorter =>
                    {
                        sorter.GetFilterList(currentList);
                        if (currentList.Contains(item))
                        {
                            currentList.Remove(item);
                        } else
                        {
                            currentList.Add(item);
                        }
                        sorter.SetFilter(MyConveyorSorterMode.Whitelist, currentList);
                    });
                }
                catch (Exception)
                {
                }
            }

            public string GetSortersFilter()
            {
                if (_sorters.Count() == 0)
                {
                    return "No sorters";
                }

                string firstItem = "";
                foreach (var sorter in _sorters.blocks)
                {
                    sorter.GetFilterList(sorterList);
                    if (sorterList.Count == 0)
                    {
                        continue;
                    }
                    if (sorterList.Count > 1)
                    {
                        return "Multiple items";
                    }
                    else
                    {
                        if (firstItem == "")
                        {
                            firstItem = sorterList[0].ItemId.ToString();
                        }
                        if (firstItem != sorterList[0].ItemId.ToString())
                        {
                            return "Multiple items";
                        }
                    }
                }
                if (firstItem == "")
                {
                    return "No filters";
                }
                return firstItem.Replace("MyObjectBuilder_Ore/", "");
            }

            private void CalculateHydrogen()
            {
                if (_hydrogenTanks.Count() == 0)
                {
                    HydrogenCharge = "N/A";
                    HydrogenLevel = 0;
                    return;
                }
                double total = 0;
                double capacity = 0;
                _hydrogenTanks.ForEach(tank =>
                {
                    total += tank.FilledRatio * tank.Capacity;
                    capacity += tank.Capacity;
                });
                HydrogenLevel = total / capacity;
                HydrogenCharge = string.Format("{0:0.0}%", (total / capacity) * 100);
            }

            private void CalculateCharge()
            {
                float stored = 0;
                float max = 0;
                float delta = 0;
                _batteries.ForEach(battery =>
                {
                    if (!battery.IsWorking)
                    {
                        return;
                    }

                    delta += battery.CurrentInput - battery.CurrentOutput;
                    stored += battery.CurrentStoredPower;
                    max += battery.MaxStoredPower;
                });

                if (max > 0)
                {
                    BatteryCharge = string.Format("{0:0.0}%", (stored / max) * 100);
                    BatteryLevel = stored / max;
                } else
                {
                    BatteryCharge = "N/A";
                    BatteryLevel = 0;
                }
            }

            private void CalculateUranium()
            {
                if (_reactors.Count() == 0)
                {
                    //ReactorsCharge = "N/A";
                    UraniumLevel = 0;
                    return;
                }
                double total = 0;
                _reactors.ForEach(reactor =>
                {
                    total += BlockHelper.GetReactorFuelLevel(reactor);
                });
                UraniumLevel = total / _reactors.Count();
            }
            private double DoGravityAlign(IMyShipController controller, List<IMyGyro> gyrosToUse, float pitch = 0f, bool onlyCalculate = false)
            {
                // Thanks to https://forum.keenswh.com/threads/aligning-ship-to-planet-gravity.7373513/#post-1286885461 

                double coefficient = 0.9;
                Matrix orientation;
                controller.Orientation.GetMatrix(out orientation);

                Vector3D down = orientation.Down;
                if (pitch < 0)
                {
                    down = Vector3D.Lerp(orientation.Down, orientation.Forward, -pitch / 90);
                }
                else if (pitch > 0)
                {
                    down = Vector3D.Lerp(orientation.Down, -orientation.Forward, pitch / 90);
                }

                Vector3D gravity = controller.GetNaturalGravity();
                gravity.Normalize();

                double offLevel = 0.0;

                Vector3 mouse = new Vector3(controller.RotationIndicator * 0.1f, controller.RollIndicator * 9);
                mouse *= _config.MouseSensitivity;

                foreach (var gyro in gyrosToUse)
                {
                    gyro.Orientation.GetMatrix(out orientation);
                    var localDown = Vector3D.Transform(down, MatrixD.Transpose(orientation));
                    var localGrav = Vector3D.Transform(gravity, MatrixD.Transpose(gyro.WorldMatrix.GetOrientation()));

                    var rotation = Vector3D.Cross(localDown, localGrav);
                    double ang = rotation.Length();
                    ang = Math.Atan2(ang, Math.Sqrt(Math.Max(0.0, 1.0 - ang * ang)));

                    /*
                    if (ang < 0.01)
                    {
                        gyro.GyroOverride = false;
                        continue;
                    }*/
                    offLevel += ang * 180.0 / 3.14;

                    if (!onlyCalculate)
                    {
                        double controlVelocity = gyro.GetMaximum<float>("Yaw") * (ang / Math.PI) * coefficient;
                        controlVelocity = Math.Min(gyro.GetMaximum<float>("Yaw"), controlVelocity);
                        controlVelocity = Math.Max(0.01, controlVelocity); //Gyros don't work well at very low speeds
                        rotation.Normalize();
                        rotation *= controlVelocity;
                        gyro.SetValueFloat("Pitch", (float)rotation.GetDim(0) + mouse.X);
                        gyro.SetValueFloat("Yaw", -((float)rotation.GetDim(1) - mouse.Y));
                        gyro.SetValueFloat("Roll", -(float)rotation.GetDim(2) - mouse.Z);
                        //gyro.SetValueFloat("Power", 1.0f);
                        gyro.GyroOverride = true;
                    }
                }
                if (gyrosToUse.Count() == 0)
                {
                    return -1000;
                }
                return offLevel / gyrosToUse.Count();
            }
            private void ReleaseGyros(List<IMyGyro> gyros) {
                foreach (IMyGyro gyro in gyros) {
                    gyro.SetValueFloat("Pitch", 0f);
                    gyro.SetValueFloat("Yaw", 0f);
                    gyro.SetValueFloat("Roll", 0f);
                    gyro.SetValueFloat("Power", 1.0f);
                    gyro.GyroOverride = false;
                }
            }
        }
    }
}
