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
        public class ThrustGroup {
            public List<IMyThrust> thrusters = new List<IMyThrust>();
            public double maxThrust;
            public bool allWorking = false;
            public Vector3D direction;
            public void Update() {
                maxThrust = 0;
                allWorking = true;
                foreach (IMyThrust thrust in thrusters) {
                    if (!thrust.IsWorking) {
                        allWorking = false;
                        continue;
                    }
                    maxThrust += thrust.MaxEffectiveThrust;
                }
                if (thrusters.Count <= 0) return;
                direction = thrusters[0].WorldMatrix.Backward;
            }

            public double AccelerationInDirection(Vector3D direction, Vector3D gravity, double mass) {
                if (!Vector3D.IsUnit(ref direction)) {
                    direction = Vector3D.Normalize(direction);
                }
                double effectiveThrust = 0;
                foreach (IMyThrust thrust in thrusters) {
                    effectiveThrust += Vector3D.Dot(direction, thrust.WorldMatrix.Backward) * thrust.MaxEffectiveThrust;
                }
                
                double effectiveAcceleration = effectiveThrust / mass;
                // Now add the effects of gravity
                return effectiveAcceleration + Vector3D.Dot(gravity, direction);
            }
        }
    }
}
