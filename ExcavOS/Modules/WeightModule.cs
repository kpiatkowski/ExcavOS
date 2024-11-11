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
        public class WeightModule : ScriptModule {

            private readonly SystemModule systemModule;
            public double LiftThrustUsage;
            public double StoppingDistance;
            public double StoppingTime;

            public WeightModule(ExcavOS handler) : base(handler) {
                systemModule = handler.GetModule<SystemModule>();
            }

            public override void Update10() {
                if (systemModule.State != State.allOk) {
                    return;
                }
                LiftThrustUsage = CalculateLiftThrustUsage(systemModule.ActiveController, systemModule.thrusters);                
                CalculateStopDistance(systemModule.ActiveController, systemModule.thrusters);
            }
            public double Mass {
                get {
                    if (systemModule.ActiveController == null) {
                        return 0.0;
                    }                    
                    return systemModule.ActiveController.CalculateShipMass().PhysicalMass;
                }
            }

            private double CalculateLiftThrustUsage(IMyShipController controller, ThrusterCache thrusterGroups) {
                double ThrustUsage = 0;
                float mass = controller.CalculateShipMass().PhysicalMass;
                Vector3D gravity = controller.GetNaturalGravity();
                double gravitationalForce = mass * gravity.Length();
                foreach (ThrustGroup thrustGroup in thrusterGroups.Thrusters) {
                    var GravAccel = Vector3D.Dot(gravity, thrustGroup.direction);
                    if (GravAccel > 0) continue;

                    double maxAcceleration = thrustGroup.maxThrust / mass;
                    double effectiveAcceleration = maxAcceleration + GravAccel;

                    float requiredThrust = 1 - (float)(effectiveAcceleration / maxAcceleration);

                    if (ThrustUsage < requiredThrust) {
                        ThrustUsage = requiredThrust;
                    }
                }
                return ThrustUsage;
            }

            private void CalculateStopDistance(IMyShipController controller, ThrusterCache thrusterGroups) {
                float mass = controller.CalculateShipMass().PhysicalMass;
                Vector3D direction = controller.GetShipVelocities().LinearVelocity.Normalized();
                Vector3D gravity = controller.GetNaturalGravity();
                double speed = controller.GetShipSpeed();
                double effectiveAcceleration = thrusterGroups.AccelerationInDirection(direction, gravity, mass);
                StoppingTime = (float)(-speed / effectiveAcceleration);
                StoppingDistance = (float)(speed * StoppingTime + (effectiveAcceleration * StoppingTime * StoppingTime) / 2.0f);
            }

        }
    }
}
