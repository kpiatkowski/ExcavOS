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

        public class BlockCacheModule : ScriptModule {

            #region mdk preserve
            public enum State {
                Waiting, Fetching, Sorting, Invalidating
            }
            #endregion

            protected State currentState = State.Waiting;
            private int testCounter = 1;

            public State CurrentState {
                get { return currentState;  }
            }

            public Dictionary<Type, BlockCache> caches = new Dictionary<Type, BlockCache>();

            protected readonly BlockList<IMyTerminalBlock> blocks;
            protected IEnumerator<bool> sortingCoroutine;

            public BlockCacheModule(ScriptHandler handler) : base(handler) {
                blocks = new BlockList<IMyTerminalBlock>(handler.Program);
                sortingCoroutine = FetchAndSortBlocks();
            }

            public BlockCache RegisterCache<T>() where T : class {
                return RegisterCache<T>(block => block is T);
            }

            public BlockCache RegisterCache<T>(Func<IMyTerminalBlock, bool> filter) where T : class {
                return RegisterCache<T>(new BlockCache(filter));
            }

            public BlockCache RegisterCache<T>(BlockCache cache) where T : class {
                if (caches.ContainsKey(typeof(T))) return caches[typeof(T)];
                caches.Add(typeof(T), cache);
                return caches[typeof(T)];
            }

            public override void Update10() {
                bool hasMoreSteps = sortingCoroutine.MoveNext();
                if (!hasMoreSteps) {
                    sortingCoroutine.Dispose();
                    sortingCoroutine = FetchAndSortBlocks();
                }
                handler.Log($"BlockSorter: {currentState}");
            }

            public void ForEach<T>(Action<T> callback) where T : class, IMyTerminalBlock {
                foreach (var block in caches[typeof(T)].Blocks) {
                    callback?.Invoke(block as T);
                }
            }

            public int Count<T>() {
                return caches[typeof(T)].Blocks.Count;
            }

            protected IEnumerator<bool> FetchAndSortBlocks() {

                currentState = State.Fetching;
                blocks.FindBlocks();
                yield return true;

                currentState = State.Sorting;

                foreach (var cache in caches.Values) {
                    cache.Clear();
                }

                foreach (IMyTerminalBlock block in blocks) {
                    foreach (var cache in caches.Values) {
                        cache.AddBlock(block);
                    }
                }
                yield return true;

                currentState = State.Invalidating;

                foreach (var cache in caches.Values) {
                    cache.NotifyInvalidatedCache();
                    yield return true;
                }
                
                currentState = State.Waiting;
                for (int i = 0; i < Configuration.BLOCK_SORTING_DELAY; i++) {
                    yield return true;
                }
                
            }

            public override void SaveRuntimeVariables(MyIni ini, string section) {
                ini.Set(section, "key", testCounter);
            }

            public override void LoadRuntimeVariables(MyIni ini, string section) {                
                testCounter = ini.Get(section, "key").ToInt32();
                handler.Log($"testCounter: {testCounter}");
                testCounter++;
            }

            public override void LoadUserConfiguration(MyIni ini, string section) {
                
            }

        }
    }
}
