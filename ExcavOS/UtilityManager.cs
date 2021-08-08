﻿using Sandbox.Game.EntityComponents;
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
            private Program _program;
            private Config _config;
            public string Status;
            private CargoManager _cargoManager;

            private readonly BlockFinder<IMyGyro> _gyros;
            private readonly BlockFinder<IMyShipController> _controllers;
            private readonly BlockFinder<IMyConveyorSorter> _sorters;
            private readonly BlockFinder<IMyBatteryBlock> _batteries;
            private readonly BlockFinder<IMyGasTank> _hydrogenTanks;
            private readonly BlockFinder<IMyReactor> _reactors;
            private readonly List<MyInventoryItemFilter> sorterList = new List<MyInventoryItemFilter>();

            public bool GravityAlign = false;
            public string BatteryCharge = "";
            public string HydrogenCharge = "";
            public double BatteryLevel = 0;
            public double HydrogenLevel = 0;
            public double UraniumLevel = 0;

            public UtilityManager(Program program, Config config, CargoManager cargoManager)
            {
                _program = program;
                _config = config;
                _cargoManager = cargoManager;
                _gyros = new BlockFinder<IMyGyro>(_program);
                _sorters = new BlockFinder<IMyConveyorSorter>(_program);
                _controllers = new BlockFinder<IMyShipController>(_program);
                _batteries = new BlockFinder<IMyBatteryBlock>(_program);
                _hydrogenTanks = new BlockFinder<IMyGasTank>(_program);
                _reactors = new BlockFinder<IMyReactor>(_program);
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
                for (int n = 0; n < _controllers.blocks.Count; n++)
                {
                    if (_controllers.blocks[n].IsWorking)
                    {
                        controller = _controllers.blocks[n];
                        break;
                    }
                }

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
                    DoGravityAlign(controller, _gyros.blocks);
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
                HydrogenCharge = string.Format("{0:0.00}%", (total / capacity) * 100);
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
                    BatteryCharge = string.Format("{0:0.00}%", (stored / max) * 100);
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

            private double DoGravityAlign(IMyShipController controller, List<IMyGyro> gyrosToUse, bool onlyCalculate = false)
            {

                // Thanks to https://forum.keenswh.com/threads/aligning-ship-to-planet-gravity.7373513/#post-1286885461 
                
                double coefficient = 0.9;
                Matrix orientation;
                controller.Orientation.GetMatrix(out orientation);
                Vector3D down = orientation.Down;
                Vector3D gravity = controller.GetNaturalGravity();
                gravity.Normalize();
                
                double offLevel = 0.0;
                
                foreach (var gyro in gyrosToUse)
                {
                    gyro.Orientation.GetMatrix(out orientation);
                    var localDown = Vector3D.Transform(down, MatrixD.Transpose(orientation));
                    var localGrav = Vector3D.Transform(gravity, MatrixD.Transpose(gyro.WorldMatrix.GetOrientation()));

                    var rotation = Vector3D.Cross(localDown, localGrav);
                    double ang = rotation.Length();
                    ang = Math.Atan2(ang, Math.Sqrt(Math.Max(0.0, 1.0 - ang * ang)));

                    if (ang < 0.01)
                    {
                        gyro.GyroOverride = false;
                        continue;
                    }
                    offLevel += ang * 180.0 / 3.14;
                    
                    if (!onlyCalculate)
                    {
                        double controlVelocity = gyro.GetMaximum<float>("Yaw") * (ang / Math.PI) * coefficient;
                        controlVelocity = Math.Min(gyro.GetMaximum<float>("Yaw"), controlVelocity);
                        controlVelocity = Math.Max(0.01, controlVelocity); //Gyros don't work well at very low speeds
                        rotation.Normalize();
                        rotation *= controlVelocity;
                        gyro.SetValueFloat("Pitch", (float)rotation.GetDim(0));
                        gyro.SetValueFloat("Yaw", -(float)rotation.GetDim(1));
                        gyro.SetValueFloat("Roll", -(float)rotation.GetDim(2));
                        gyro.SetValueFloat("Power", 1.0f);
                        gyro.GyroOverride = true;
                    }
                }
                if (gyrosToUse.Count() == 0)
                {
                    return -1000;
                }
                return offLevel / gyrosToUse.Count();
            }

        }
    }
}