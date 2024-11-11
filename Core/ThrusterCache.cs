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
        public class ThrusterCache : BlockCache {

            protected readonly ThrustGroup[] sortedThrusters = new ThrustGroup[6];
            public ThrustGroup[] Thrusters {
                get { return sortedThrusters; }
            }

            public ThrusterCache(Func<IMyTerminalBlock, bool> filter) : base(filter) {
                for (int i = 0; i < sortedThrusters.Length; i++) {
                    sortedThrusters[i] = new ThrustGroup();
                }
            }

            protected override void OnAddedBlock(IMyTerminalBlock block) {
                var index = (int)block.Orientation.TransformDirection(Base6Directions.Direction.Forward);
                sortedThrusters[index].thrusters.Add(block as IMyThrust);
            }

            protected override void OnBlocksCleared() {
                for (int i = 0; i < sortedThrusters.Length; i++) {
                    sortedThrusters[i].thrusters.Clear();
                }
            }

            public void Update() {
                for (int i = 0; i < sortedThrusters.Length; i++) {
                    sortedThrusters[i].Update();
                }
            }

            public double AccelerationInDirection(Vector3D direction, Vector3D gravity, double mass) {
                double totalAccel = 0;
                for (int i = 0; i < sortedThrusters.Length; i++) {
                    if (Vector3D.Dot(sortedThrusters[i].direction, direction) < -0.01) {
                        totalAccel += sortedThrusters[i].AccelerationInDirection(direction, gravity, mass);
                    }
                }
                return totalAccel;
            }
        }
    }
}
