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
        public class RegisteredProvider : ScriptConfig
        {
            private IMyTerminalBlock _block;
            private ExcavOSContext _context;
            private readonly Dictionary<int, ScreenHandler<ExcavOSContext>> _screenHandlers = new Dictionary<int, ScreenHandler<ExcavOSContext>>();
            private readonly Dictionary<int, ScreenHandler<ExcavOSContext>> _immersiveHandlers = new Dictionary<int, ScreenHandler<ExcavOSContext>>();
            public bool WasUnderControl = false;
            private bool EnableImmersion = false;

            public RegisteredProvider(ExcavOSContext context, IMyTerminalBlock block, MyIni ini, string section) : base(ini, section)
            {
                _block = block;
                _context = context;
            }

            public override void ReadConfig()
            {
                EnableImmersion = GetValue("EnableImmersion").ToBoolean(EnableImmersion);
                IMyTextSurfaceProvider surfaceProvider = _block as IMyTextSurfaceProvider;
                for (int n = 0; n < surfaceProvider.SurfaceCount; n++)
                {
                    string key = $"Surface{n}";
                    if (KeyExists(key))
                    {
                        string screen = GetValue(key).ToString();
                        SetScreenHandlerForSurface(screen, n);
                    } else if (_screenHandlers.ContainsKey(n))
                    {
                        ResetScreenHandlerForSurface(n);
                    }
                }
                    
            }

            public override void SetupDefaults()
            {

            }

            public void Update()
            {
                if (!_block.IsWorking) {
                    return;
                }

                IMyTextSurfaceProvider surfaceProvider = _block as IMyTextSurfaceProvider;

                if (EnableImmersion && _block is IMyCockpit)
                {
                    IMyCockpit cockpit = _block as IMyCockpit;
                    if (!WasUnderControl && cockpit.IsUnderControl)
                    {
                        // player entered cockpit
                        WasUnderControl = true;
                        for (int n = 0; n < surfaceProvider.SurfaceCount; n++)
                        {
                            if (_screenHandlers.ContainsKey(n))
                            {
                                _immersiveHandlers[n] = new LoadingScreen(_context);
                            }
                        }
                    }
                    else if (WasUnderControl && !cockpit.IsUnderControl)
                    {
                        // player exited cockpit
                        WasUnderControl = false;
                        for (int n = 0; n < surfaceProvider.SurfaceCount; n++)
                        {
                            if (_screenHandlers.ContainsKey(n))
                            {
                                _immersiveHandlers[n] = new LockScreen(_context);
                            }
                        }
                    }
                }
                

                for (int n = 0; n < surfaceProvider.SurfaceCount; n++)
                {
                    IMyTextSurface surface = surfaceProvider.GetSurface(n);
                    if (_screenHandlers.ContainsKey(n))
                    {
                        surface.Script = "";
                        surface.ContentType = ContentType.SCRIPT;
                        if (_immersiveHandlers.ContainsKey(n))
                        {
                            _immersiveHandlers[n].Draw(surface);
                            if (_immersiveHandlers[n].ShouldDispose())
                            {
                                _immersiveHandlers.Remove(n);
                            }
                        } 
                        else
                        {
                            _screenHandlers[n].Draw(surface);
                        }
                    }
                }
            }

            public void SetScreenHandlerForSurface(string screenName, int surfaceIndex)
            {
                _screenHandlers[surfaceIndex] = ScreenHandlerFactory.GetScreenHandler(screenName, _context);
            }

            public void ResetScreenHandlerForSurface(int surfaceIndex)
            {
                _screenHandlers.Remove(surfaceIndex);
                IMyTextSurfaceProvider surfaceProvider = _block as IMyTextSurfaceProvider;
                IMyTextSurface surface = surfaceProvider.GetSurface(surfaceIndex);
                surface.Script = "";
                surface.ContentType = ContentType.NONE;
            }

            public bool HasSurfaces()
            {
                return _screenHandlers.Count > 0;
            }
                
        }
    }
}
