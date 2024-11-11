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

        public delegate void CacheInvalidated();
        public class BlockCache {

            protected readonly List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
            public List<IMyTerminalBlock> Blocks { get { return blocks; } }
            public Func<IMyTerminalBlock, bool> filterFunc;
            public CacheInvalidated OnCacheInvalidated;

            public BlockCache(Func<IMyTerminalBlock, bool> filter) {
                filterFunc = filter;
            }

            public void AddBlock<T>(T block) where T : IMyTerminalBlock {
                if (filterFunc(block)) {
                    blocks.Add(block);
                    OnAddedBlock(block);
                }
            }

            protected virtual void OnAddedBlock(IMyTerminalBlock block) { }
            protected virtual void OnBlocksCleared() { }

            public void Clear() {
                blocks.Clear();
                OnBlocksCleared();
            }

            public void ForEach<T>(Action<T> callback) where T : class {
                foreach (var block in blocks) {
                    callback?.Invoke(block as T);
                }
            }

            public void NotifyInvalidatedCache() {
                OnCacheInvalidated?.Invoke();
            }
        }
    }
}
