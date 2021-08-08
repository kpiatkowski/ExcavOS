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

            private readonly BlockFinder<IMyThrust> stopThrusters;
            private readonly BlockFinder<IMyThrust> liftThrusters;
            private readonly BlockFinder<IMyShipController> controllers;

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

            public WeightAnalizer(Program program, Config config, CargoManager cargoManager)
            {
                _program = program;
                _config = config;
                _cargoManager = cargoManager;
                liftThrusters = new BlockFinder<IMyThrust>(_program);
                stopThrusters = new BlockFinder<IMyThrust>(_program);
                controllers = new BlockFinder<IMyShipController>(_program);
            }

            public void QueryData(TimeSpan time)
            {
                liftThrusters.FindBlocks(true, null, _config.LiftThrustersGroupName);
                stopThrusters.FindBlocks(true, null, _config.StopThrustersGroupName);
                controllers.FindBlocks(true, null);
                Calculate();
                CalculateCapacityDelta(time);
            }

            public float GetLiftThresholdWarning()
            {
                return _config.LiftThresholdWarning;
            }

            private void Calculate()
            {
                IMyShipController controller = null;
                for (int n = 0; n < controllers.blocks.Count; n++)
                {
                    if (controllers.blocks[n].IsWorking)
                    {
                        controller = controllers.blocks[n];
                        break;
                    }
                }

                if (controller == null)
                {
                    Status = "Missing controller";
                    return;
                }

                if (controller.CalculateShipMass().PhysicalMass == 0)
                {
                    Status = "Grid is static";
                    LiftThrustNeeded = 0;
                    LiftThrustAvailable = 0;
                    StoppingTime = 0;
                    StoppingDistance = 0;
                    return;
                }

                Status = "";
                CalculateLiftThrustUsage(controller);
                CalculateStopDistance(controller);
            }

            private void CalculateLiftThrustUsage(IMyShipController controller)
            {
                float mass = controller.CalculateShipMass().PhysicalMass;
                float gravity = (float)(controller.GetNaturalGravity().Length() / 9.81);                
                LiftThrustNeeded = (mass * (float)gravity / 100) * 1000;

                LiftThrustAvailable = 0;
                liftThrusters.ForEach(thruster =>
                {
                    if (thruster.IsWorking)
                    {
                        LiftThrustAvailable += thruster.MaxEffectiveThrust;
                    }
                });
                _program.Echo($"LiftThrustAvailable = {LiftThrustAvailable}");
                _program.Echo($"Mass = {mass}");
            }

            private void CalculateStopDistance(IMyShipController controller) 
            {
                float mass = controller.CalculateShipMass().PhysicalMass;
                double stopThrustAvailable = 0;
                int disabledThrusters = 0;
                stopThrusters.ForEach(thruster =>
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
