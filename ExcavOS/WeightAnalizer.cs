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
        public class WeightAnalizer
        {
            private Program _program;
            private Config _config;
            public string Status;
            private CargoManager _cargoManager;
            private SystemManager _systemManager;
            
            public float LiftThrustNeeded;
            public float LiftThrustAvailable;
            public float StoppingDistance;
            public float StoppingTime;
            public float CapacityDelta;
            public bool StopThrustersWarning = false;

            protected struct WeightPoint
            {
                public double time;
                public double capacity;
            }
            private const int MaxWeightPoints = 20;
            private WeightPoint[] _weightPoints = new WeightPoint[MaxWeightPoints];
            private int addedWeightPoints = 0;

            public WeightAnalizer(Program program, Config config, CargoManager cargoManager, SystemManager systemManager)
            {
                _program = program;
                _config = config;
                _cargoManager = cargoManager;
                _systemManager = systemManager;
            }

            public void QueryData(TimeSpan time)
            {
                Calculate();
                CalculateCapacityDelta(time);
            }

            public float GetLiftThresholdWarning()
            {
                return _config.LiftThresholdWarning;
            }

            private void Calculate()
            {

                if (_systemManager.ActiveController == null)
                {
                    Status = "Missing controller";
                    return;
                }

                if (_systemManager.ActiveController.CalculateShipMass().PhysicalMass == 0)
                {
                    Status = "Grid is static";
                    LiftThrustNeeded = 0;
                    LiftThrustAvailable = 0;
                    StoppingTime = 0;
                    StoppingDistance = 0;
                    return;
                }

                Status = "";
                CalculateLiftThrustUsage(_systemManager.ActiveController, _systemManager.LiftThrusters);
                CalculateStopDistance(_systemManager.ActiveController, _systemManager.StopThrusters);
            }

            private void CalculateLiftThrustUsage(IMyShipController controller, List<IMyThrust> thrusters)
            {
                /*
                float mass = controller.CalculateShipMass().PhysicalMass;
                float gravityStrength = (float)(controller.GetNaturalGravity().Length() / 9.81);                
                LiftThrustNeeded = (mass * (float)gravityStrength / 100) * 1000;

                Vector3 gravity = controller.GetNaturalGravity();


                LiftThrustAvailable = 0;
                thrusters.ForEach(thruster =>
                {
                    if (thruster.IsWorking)
                    {
                        Vector3D thrusterDirection = thruster.WorldMatrix.Forward;
                        double upDot = Vector3D.Dot(thrusterDirection, Vector3.Normalize(gravity));
                        LiftThrustAvailable += (thruster.MaxEffectiveThrust*(float)upDot);
                    }
                });
                */
                LiftThrustNeeded = 0;
                thrusters.ForEach(thruster =>
                {
                    if (thruster.IsWorking)
                    {
                        if (thruster.CurrentThrustPercentage > LiftThrustNeeded)
                        {
                            LiftThrustNeeded = thruster.CurrentThrustPercentage;
                        }
                    }
                });
                LiftThrustNeeded *= 0.01f;
                LiftThrustAvailable = (float)MathHelper.Lerp(LiftThrustAvailable, LiftThrustNeeded, 0.5);
            }

            private void CalculateStopDistance(IMyShipController controller, List<IMyThrust> thrusters) 
            {
                float mass = controller.CalculateShipMass().PhysicalMass;
                double stopThrustAvailable = 0;
                int disabledThrusters = 0;
                thrusters.ForEach(thruster =>
                {
                    if (!thruster.IsWorking) disabledThrusters++;
                    if (thruster.IsFunctional)
                    {
                        stopThrustAvailable += thruster.MaxEffectiveThrust;
                    }
                });
                StopThrustersWarning = disabledThrusters > 0;
                double deacceleration = -stopThrustAvailable / mass;
                double currentSpeed = controller.GetShipSpeed();
                StoppingTime = (float)(-currentSpeed / deacceleration);
                StoppingDistance = (float)(currentSpeed * StoppingTime + (deacceleration * StoppingTime * StoppingTime) / 2.0f);
            }

            private void CalculateCapacityDelta(TimeSpan time)
            {
                WeightPoint wp = new WeightPoint
                {
                    time = time.TotalSeconds,
                    capacity = _cargoManager.CurrentCapacity / _cargoManager.TotalCapacity
                };

                if (addedWeightPoints < MaxWeightPoints)
                {
                    _weightPoints[addedWeightPoints] = wp;
                    addedWeightPoints++;
                    CapacityDelta = 0;
                }
                else
                {
                    for (int n = 1; n < MaxWeightPoints; n++)
                    {
                        _weightPoints[n - 1] = _weightPoints[n];
                    }
                    _weightPoints[MaxWeightPoints - 1] = wp;

                    float capacityIncrease = (float)(_weightPoints[MaxWeightPoints - 1].capacity - _weightPoints[0].capacity);
                    float timeIncrease = (float)(_weightPoints[MaxWeightPoints - 1].time - _weightPoints[0].time);
                    CapacityDelta = capacityIncrease / timeIncrease;
                }

            }
        }
    }
}
