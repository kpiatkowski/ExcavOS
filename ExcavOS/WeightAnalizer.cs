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
using static IngameScript.Program.ThrustGroups;

namespace IngameScript
{
    internal partial class Program
    {
        public class WeightAnalizer
        {
            private Program _program;
            private Config _config;
            public string Status;
            private CargoManager _cargoManager;
            private SystemManager _systemManager;

            public float LiftThrustUsage;
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
                }
                else
                {
                    Status = "";
                }
                float mass = _systemManager.ActiveController.CalculateShipMass().PhysicalMass;
                Vector3D direction = _systemManager.ActiveController.GetShipVelocities().LinearVelocity.Normalized();
                Vector3D gravity = _systemManager.ActiveController.GetNaturalGravity();
                double speed = _systemManager.ActiveController.GetShipSpeed();

                _systemManager.ThrusterGroups.UpdateAll();
                LiftThrustUsage = (float)CalculateLiftThrustUsage(_systemManager.ActiveController, _systemManager.ThrusterGroups);
                CalculateStopDistance(speed, direction, gravity, mass, _systemManager.ThrusterGroups);
            }

            private double CalculateLiftThrustUsage(IMyShipController controller, ThrustGroups thrusterGroups)
            {
                double ThrustUsage = 0;
                float mass = controller.CalculateShipMass().PhysicalMass;
                Vector3D gravity = controller.GetNaturalGravity();
                double gravitationalForce = mass * gravity.Length();
                foreach (ThrustGroup thrustGroup in thrusterGroups.groups)
                {
                    var GravAccel = Vector3D.Dot(gravity, thrustGroup.direction);
                    if (GravAccel > 0) continue;


                    double maxAcceleration = thrustGroup.maxThrust / mass;
                    double effectiveAcceleration = maxAcceleration + GravAccel;

                    float requiredThrust = 1 - (float)(effectiveAcceleration / maxAcceleration);

                    if (ThrustUsage < requiredThrust)
                    {
                        ThrustUsage = requiredThrust;
                    }
                }
                return ThrustUsage;
            }

            private void CalculateStopDistance(double CurrentSpeed, Vector3D Direction, Vector3D Gravity, Double Mass, ThrustGroups thrustGroups)
            {

                double effectiveAcceleration = thrustGroups.AccelerationInDirection(Direction, Gravity, Mass);
                StoppingTime = (float)(-CurrentSpeed / effectiveAcceleration);
                StoppingDistance = (float)(CurrentSpeed * StoppingTime + (effectiveAcceleration * StoppingTime * StoppingTime) / 2.0f);
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
