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
        public class ScreenModule : ScriptModule {

            public class ManagedScreen {
                public IMyTerminalBlock parentBlock;
                public IMyTextSurface surface;
                public string config;
                public string handlerName;
                public ScreenHandler handler;
            }

            private readonly MyIni ini = new MyIni();
            private readonly BlockCache surfaceProvidersCache;
            private readonly Dictionary<string, ManagedScreen> screens = new Dictionary<string, ManagedScreen>();

            private readonly Dictionary<string, Func<IMyTextSurface, ScreenHandler>> registeredHandlers = new Dictionary<string, Func<IMyTextSurface, ScreenHandler>>();

            public int ManagedScreenCount {
                get { return screens.Count; }
            }

            public ScreenModule(ExcavOS handler) : base(handler) {

                registeredHandlers.Add("logo", (surface) => new LogoScreen(handler, surface));
                registeredHandlers.Add("name", (surface) => new EmptyScreen(handler, surface));
                registeredHandlers.Add("cargo", (surface) => new AllInventoryScreen(handler, surface));
                registeredHandlers.Add("cargoore", (surface) => new OresInventoryScreen(handler, surface));
                registeredHandlers.Add("cargoingot", (surface) => new IngotsInventoryScreen(handler, surface));
                registeredHandlers.Add("cargocomponents", (surface) => new ComponentsInventoryScreen(handler, surface));
                registeredHandlers.Add("test", (surface) => new TestScreen(handler, surface));
                registeredHandlers.Add("weight", (surface) => new WeightScreen(handler, surface));

                surfaceProvidersCache = handler.GetModule<BlockCacheModule>().RegisterCache<IMyTextSurfaceProvider>(block => {
                    if (!(block is IMyTextSurfaceProvider)) return false;
                    if ((block as IMyTextSurfaceProvider).SurfaceCount == 0) return false;
                    if (handler.Program.Me == block) return false;
                    return MyIni.HasSection(block.CustomData, Constants.SCRIPT_NAME);
                });
                surfaceProvidersCache.OnCacheInvalidated = CheckAndUpdateScreens;
            }

            private void CheckAndUpdateScreens() {
                surfaceProvidersCache.ForEach<IMyTerminalBlock>(provider => {
                    CheckScreenProvider(provider, ini);
                });
            }

            protected void UpdateScreen(ManagedScreen screen) {
                screen.surface.Script = "";
                screen.surface.ContentType = ContentType.SCRIPT;
                screen.handler.Draw();
            }

            public override void Update10() {
                foreach (var screen in screens.Values) {
                    if (screen.handler.UpdateFrequency != UpdateFrequency.Update10) continue;
                    UpdateScreen(screen);
                }
            }

            public override void Update100() {
                foreach (var screen in screens.Values) {
                    if (screen.handler.UpdateFrequency != UpdateFrequency.Update100) continue;
                    UpdateScreen(screen);
                }
            }

            private string KeyForScreen(IMyTerminalBlock block, int surfaceIndex) {
                return $"{block.EntityId}.{surfaceIndex}";
            }

            private void CheckScreenProvider(IMyTerminalBlock block, MyIni ini) {
                if (!ini.TryParse(block.CustomData)) {
                    return;
                }
                IMyTextSurfaceProvider surfaceProvider = block as IMyTextSurfaceProvider;
                for (int n = 0; n < surfaceProvider.SurfaceCount; n++) {
                    string key = $"Screen{n}";
                    if (ini.ContainsKey(Constants.SCRIPT_NAME, key)) {
                        string screenConfig = ini.Get(Constants.SCRIPT_NAME, key).ToString();
                        AddOrUpdateScreenToManage(block, screenConfig, n);
                    } else if (screens.ContainsKey(KeyForScreen(block, n))) {
                        RemoveScreenFromManaged(KeyForScreen(block, n));
                    }
                }
            }

            public void AddOrUpdateScreenToManage(IMyTerminalBlock block, string screenConfig, int surfaceIndex) {
                string key = KeyForScreen(block, surfaceIndex);
                string[] parts = screenConfig.Split(':');
                string screenName = parts[0].ToLower();                
                Log($"Checking screen with key {key} and name {screenName}");
                if (!registeredHandlers.ContainsKey(screenName)) {
                    Log($"No handler for \"{screenName}\" found");
                    return;
                }

                ManagedScreen screen;
                IMyTextSurface surface = (block as IMyTextSurfaceProvider).GetSurface(surfaceIndex);
                if (screens.ContainsKey(key)) {
                    screen = screens[key];
                    screen.config = parts.Length > 1 ? parts[1] : "";
                    if (screen.handlerName != screenName) {
                        screen.handler = registeredHandlers[screenName](surface);
                        screen.handlerName = screenName;
                        screen.handler.Initialize();
                        Log($"Updated config of screen with key {key}");
                    }
                } else {
                    screen = new ManagedScreen() {
                        parentBlock = block,
                        surface = (block as IMyTextSurfaceProvider).GetSurface(surfaceIndex),
                        config = parts.Length > 1 ? parts[1] : "",
                        handler = registeredHandlers[screenName](surface),
                        handlerName = screenName
                    };
                    Log($"New screen {block.CustomName}@{surfaceIndex} with key {key}@{screen.handler.UpdateFrequency}");
                    screens.Add(key, screen);
                    screen.handler.Initialize();
                }                
                
            }

            private void RemoveScreenFromManaged(string key) {
                Log($"Removed screen with key {key}");
                ManagedScreen screen = screens[key];
                screen.surface.Script = "";
                screen.surface.ContentType = ContentType.NONE;
                screens.Remove(key);                
            }

        }
    }
}
