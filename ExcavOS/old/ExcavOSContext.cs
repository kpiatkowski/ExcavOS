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
            public const string SECTION_EXCAV_OS = "ExcavOS";
            private const string KEY_LIFT_THRUSTERS_GROUP_NAME = "LiftThrustersGroupName";
            private const string KEY_STOP_THRUSTERS_GROUP_NAME = "StopThrustersGroupName";
            private const string KEY_EMERGENCY_LIFT_PRESERVER = "EmergencyLiftPreserver";
            private const string KEY_ALIGN_GYROS = "AlignGyrosGroupName";
            private const string KEY_DISABLE_LOADING = "DisableLoading";

            private string liftThrustersGroupName = "Lift Thrusters";
            private string stopThrustersGroupName = "Stop Thrusters";
            private string alignGyrosGroupName = "Align Gyros";
            private double emergencyLiftPreserver = 0.9f;
            private bool emergencyLiftPreserverActive = false;
            private bool disableLoading = false;
            public readonly Program program;
            private readonly IMyProgrammableBlock me;
            private readonly MyIni ini = new MyIni();

            public bool gravityAlignEnabled = false;
            public TimeSpan timeAccumulator = new TimeSpan();
            public Random random = new Random();
            #region mdk macros
            // This script was deployed at $MDK_DATETIME$
            public const string version = "$MDK_DATE$";
            #endregion

            public readonly BlockFinder<IMyGyro> gyros;
            public readonly BlockFinder<IMyShipDrill> drills;
            public readonly BlockFinder<IMyThrust> liftThrusters;
            public readonly BlockFinder<IMyThrust> stopThrusters;
            public readonly BlockFinder<IMyConveyorSorter> sorters;
            public CargoManager cargoManager;
            private double mass;
            private double liftThrustNeeded;
            private double liftThrustAvailable;
            private double stoppingDistance;
            private double stoppingTime;
            private IMyCockpit activeCockpit;

            public bool IsLoadingScreenDisabled
            {
                get { return disableLoading; }
            }

            public double Mass
            {
                get { return mass; }
            }

            public double LiftThrustNeeded
            {
                get { return liftThrustNeeded; }
            }

            public double LiftThrustAvailable
            {
                get { return liftThrustAvailable; }
            }

            public double StoppingDistance
            {
                get { return stoppingDistance; }
            }

            public double StoppingTime
            {
                get { return stoppingTime; }
            }

            public double EmergencyLiftPreserver
            {
                get { return emergencyLiftPreserver; }
            }

            public bool EmergencyLiftPreserverActive
            {
                get { return emergencyLiftPreserverActive; }
            }

            public ExcavOSContext(Program program, IMyProgrammableBlock me)
            {
                this.program = program;
                this.me = me;

                gyros = new BlockFinder<IMyGyro>(program);
                drills = new BlockFinder<IMyShipDrill>(program);
                liftThrusters = new BlockFinder<IMyThrust>(program);
                stopThrusters = new BlockFinder<IMyThrust>(program);
                sorters = new BlockFinder<IMyConveyorSorter>(program);
                program.Echo($"sorters = {sorters.Count()}");
                cargoManager = new CargoManager(program);
                CollectConfiguration();
                Collect();
            }

            private void SetupDefaultConfig()
            {
                ini.Set(SECTION_EXCAV_OS, KEY_LIFT_THRUSTERS_GROUP_NAME, liftThrustersGroupName);
                ini.Set(SECTION_EXCAV_OS, KEY_STOP_THRUSTERS_GROUP_NAME, stopThrustersGroupName);
                ini.Set(SECTION_EXCAV_OS, KEY_DISABLE_LOADING, disableLoading);
                ini.Set(SECTION_EXCAV_OS, KEY_ALIGN_GYROS, alignGyrosGroupName);
                me.CustomData = ini.ToString();
            }

            private void CollectConfiguration()
            {
                if (!MyIni.HasSection(me.CustomData, SECTION_EXCAV_OS))
                {
                    SetupDefaultConfig();
                }

                MyIniParseResult result;
                if (!ini.TryParse(me.CustomData, out result))
                {
                    return;
                }

                if (!ini.ContainsSection(SECTION_EXCAV_OS))
                {
                    return;
                }

                liftThrustersGroupName = ini.Get(SECTION_EXCAV_OS, KEY_LIFT_THRUSTERS_GROUP_NAME).ToString(liftThrustersGroupName);
                stopThrustersGroupName = ini.Get(SECTION_EXCAV_OS, KEY_STOP_THRUSTERS_GROUP_NAME).ToString(stopThrustersGroupName);
                emergencyLiftPreserver = ini.Get(SECTION_EXCAV_OS, KEY_EMERGENCY_LIFT_PRESERVER).ToDouble(emergencyLiftPreserver);
                alignGyrosGroupName = ini.Get(SECTION_EXCAV_OS, KEY_ALIGN_GYROS).ToString(alignGyrosGroupName);
                disableLoading = ini.Get(SECTION_EXCAV_OS, KEY_DISABLE_LOADING).ToBoolean(disableLoading);
            }

            public void Collect()
            {
                gyros.FindBlocks(true, null, alignGyrosGroupName);
                liftThrusters.FindBlocks(true, null, liftThrustersGroupName);
                stopThrusters.FindBlocks(true, null, stopThrustersGroupName);
                sorters.FindBlocks();
            }

            public void SetActiveCockpit(IMyCockpit cockpit) {

                activeCockpit = cockpit;
                double gravity = (cockpit.GetNaturalGravity().Length() / 9.81);
                mass = cockpit.CalculateShipMass().PhysicalMass;
                liftThrustNeeded = (mass * (float)gravity / 100) * 1000;
               
                liftThrustAvailable = 0;
                liftThrusters.ForEach(thruster =>
                {
                    if (thruster.IsWorking)
                    {
                        liftThrustAvailable += thruster.MaxEffectiveThrust;
                    }
                });

                double stopThrustAvailable = 0;
                stopThrusters.ForEach(thruster =>
                {
                    if (thruster.IsWorking)
                    {
                        stopThrustAvailable += thruster.MaxEffectiveThrust;
                    }
                });

                if (mass > 0)
                {
                    double deacceleration = -stopThrustAvailable / mass;
                    double currentSpeed = cockpit.GetShipSpeed();
                    stoppingTime = -currentSpeed / deacceleration;
                    stoppingDistance = currentSpeed * stoppingTime + (deacceleration * stoppingTime * stoppingTime) / 2;
                } else
                {
                    stoppingTime = -1;
                    stoppingDistance = -1;
                }

            }

            public void AddTimeSpan(TimeSpan time)
            {
                timeAccumulator += time;                
            }

            public void Update()
            {
                cargoManager.QueryData();
                if (gravityAlignEnabled && gyros.Count() > 0 && activeCockpit!= null)
                {
                    GravityAlign(activeCockpit, gyros.blocks);
                } else if (!gravityAlignEnabled && gyros.Count() > 0)
                {
                    gyros.ForEach(gyro => gyro.GyroOverride = false);
                }

                emergencyLiftPreserverActive = emergencyLiftPreserver > 0.0f && emergencyLiftPreserver < 1.0f && (liftThrustNeeded / liftThrustAvailable) > emergencyLiftPreserver;
            }

            public void HandleCommand(string command)
            {
                
                switch(command.Split(' ')[0].ToLower())
                {
                    case "toggle_gaa":
                        gravityAlignEnabled = !gravityAlignEnabled;
                        break;
                    case "dump":
                        SetSortersFilter(command.Split(' ')[1]);
                        break;
                }
            }

            private void SetSortersFilter(string item_id)
            {
                try
                {
                    List<MyInventoryItemFilter> list = new List<MyInventoryItemFilter>
                    {
                        new MyInventoryItemFilter("MyObjectBuilder_Ore/" + item_id)
                    };
                    sorters.ForEach(sorter =>
                    {
                        sorter.SetFilter(MyConveyorSorterMode.Whitelist, list);
                    });
                }
                catch (Exception)
                {
                }
            }

            public string GetSortersFilter()
            {
                List<MyInventoryItemFilter> list = new List<MyInventoryItemFilter>();
                string firstItem = "";
                foreach (var sorter in sorters.blocks)
                {
                    sorter.GetFilterList(list);
                    if (list.Count > 1)
                    {
                        return "Multiple items";                        
                    }
                    else
                    {
                        if (firstItem == "")
                        {
                            firstItem = list[0].ItemId.ToString();
                        }
                        if (firstItem != list[0].ItemId.ToString())
                        {
                            return "Multiple items";
                        }
                    }
                }

                return firstItem.Replace("MyObjectBuilder_Ore/", "");
            }

            private double GravityAlign(IMyShipController controller, List<IMyGyro> gyrosToUse, bool onlyCalculate = false)
            {

                // Thanks to https://forum.keenswh.com/threads/aligning-ship-to-planet-gravity.7373513/#post-1286885461 

                double coefficient = 0.8;
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
                if (gyros.Count() == 0)
                {
                    return -1000;
                }
                return offLevel / gyros.Count();
            }

        }
    }
}
