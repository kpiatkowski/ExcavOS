using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System;
using System.Collections;
using System.Collections.Generic;
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
        #region mdk preserve
        public enum State {
            notInitialized,
            noController,
            otherError,
            allOk
        }
        public enum ShipState {
            isDocked,
            isControlled,
            isIdle,
            isStatic
        }
        #endregion

        public class SystemModule : ScriptModule {

            public readonly ThrusterCache thrusters;
            private readonly BlockCache controllers;
            private readonly BlockCache gyros;
            private readonly BlockCache parachutes;
            private readonly BlockCache gasTanks;
            private readonly BlockCache batteries;
            private readonly BlockCache connectors;

            private IMyShipController controller;

            public State State { get { return state; } }
            private State state = State.notInitialized;

            public ShipState ShipState { get { return shipState; } }
            private ShipState shipState = ShipState.isIdle;

            public IMyShipController ActiveController { get { return controller; } }

            public SystemModule(ExcavOS handler) : base(handler) {
                thrusters = (ThrusterCache)handler.GetModule<BlockCacheModule>().RegisterCache<IMyThrust>(new ThrusterCache(block => block is IMyThrust));

                controllers = handler.GetModule<BlockCacheModule>().RegisterCache<IMyShipController>(new BlockCache(block => block is IMyShipController));
                controllers.OnCacheInvalidated += FindController;

                gyros = handler.GetModule<BlockCacheModule>().RegisterCache<IMyGyro>(new BlockCache(block => block is IMyGyro));
                parachutes = handler.GetModule<BlockCacheModule>().RegisterCache<IMyParachute>(new BlockCache(block => block is IMyParachute));
                gasTanks = handler.GetModule<BlockCacheModule>().RegisterCache<IMyGasTank>(new BlockCache(block => block is IMyGasTank));
                batteries = handler.GetModule<BlockCacheModule>().RegisterCache<IMyBatteryBlock>(new BlockCache(block => block is IMyBatteryBlock));
                connectors = handler.GetModule<BlockCacheModule>().RegisterCache<IMyShipConnector>(new BlockCache(block => block is IMyShipConnector));
            }

            private void FindController() {
                IMyShipController firstWorking = null;
                controllers.ForEach<IMyShipController>(controller => {
                    if (!controller.IsWorking) return;
                    if (firstWorking == null) firstWorking = controller;
                    if (this.controller == null && controller.IsUnderControl && controller.CanControlShip) this.controller = controller;
                    if (controller.IsMainCockpit) this.controller = controller;
                });
                if (controller == null) controller = firstWorking;

                if (controller == null) {
                    state = State.noController;
                } else {
                    state = State.allOk;
                }
            }

            public override void Update10() {
                thrusters.Update();
            }

            public override void Update100() {
                
            }

        }
    }
}