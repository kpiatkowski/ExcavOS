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

        public delegate void CommandFunc(string[] args);

        public class ScriptHandler {

            private readonly Dictionary<string, CommandFunc> commands = new Dictionary<string, CommandFunc>();
            private readonly Dictionary<Type, ScriptModule> modules = new Dictionary<Type, ScriptModule>();
            protected readonly Program program;
            protected readonly MyIni runtimeVariables = new MyIni();
            protected readonly MyIni userConfig = new MyIni();
            private readonly string spinner = "|/-\\|/-\\";
            private int tick;

            public Program Program {
                get { return program; }
            }

            public ScriptHandler(Program program, string initialConfig) {
                this.program = program;
                if (runtimeVariables.TryParse(initialConfig)) {
                    Log("Parsed runtime variables config");
                } else {
                    Log("No valid runtime variables config found");
                }
                if (userConfig.TryParse(program.Me.CustomData)) {
                    Log("Parsed user configuration");
                } else {
                    Log("No valid user configuration found");
                }
                RegisterModule(new DebugModule(this));
                RegisterModule(new BlockCacheModule(this));
            }
            
            public void RegisterCommand(string command, CommandFunc executor) {
                commands.Add(command, executor);
            }

            protected ScriptModule RegisterModule(ScriptModule module) {
                modules.Add(module.GetType(), module);
                string moduleName = module.GetType().ToString();
                module.LoadUserConfiguration(userConfig, moduleName);
                module.LoadRuntimeVariables(runtimeVariables, moduleName);
                Log($"Added module {moduleName}");
                return module;
            }

            public void Log(string message) {
                DebugModule debug = GetModule<DebugModule>();
                if (debug == null) {
                    return;
                }
                debug.Log(message);                
            }

            public T GetModule<T>() where T : ScriptModule {
                if (modules.ContainsKey(typeof(T))) return (T)modules[typeof(T)];
                return null;
            }

            public void HandleCommand(string argument) {
                string[] args = argument.Split(' ');
                string cmd = args[0].ToLower();
                if (commands.ContainsKey(cmd)) {
                    Log($"Executing command: {cmd}");
                    CommandFunc executor = commands[cmd];
                    executor(args.Skip(1).ToArray());
                } else {
                    Log($"Unregistered command: {cmd}");
                }
            }

            public string Save() {
                runtimeVariables.Clear();
                foreach (var module in modules.Values) {                    
                    module.SaveRuntimeVariables(runtimeVariables, module.GetType().ToString());
                }
                return runtimeVariables.ToString();
            }

            protected virtual string GetInfo() {
                tick++;
                BlockCacheModule blockCache = GetModule<BlockCacheModule>();
                return $"{Constants.SCRIPT_NAME} {Constants.SCRIPT_VERSION} is running {spinner.Substring(tick % spinner.Length, 1)}\n\nLastRunTimeMs@100T = {Program.Runtime.LastRunTimeMs}\nBlockCache state: {blockCache.CurrentState}\n";
            }

            public void Update(string argument, UpdateType updateSource) {
                if ((updateSource & UpdateType.Update100) != 0) {
                    foreach (var module in modules.Values) {
                        module.Update100();
                    }                    
                } else if ((updateSource & UpdateType.Update10) != 0) {
                    foreach (var module in modules.Values) {
                        module.Update10();
                    }
                    Program.Echo(GetInfo());
                } else if (argument != "") {
                    HandleCommand(argument);
                }
            }

        }
    }
}
