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
        public class LogoScreen : ScreenHandler {

            private int tick = 0;

            public LogoScreen(ExcavOS excavOS, IMyTextSurface surface) : base(excavOS, surface) {
            }

            protected override void DrawFrame(MySpriteDrawFrame frame) {
                tick++;
                Surface.ScriptBackgroundColor = Color.Black;
                Surface.ScriptForegroundColor = Color.DarkOrange;

                MySprite sprite;
                float iconSize = Math.Min(Surface.SurfaceSize.X, Surface.SurfaceSize.Y);
                sprite = MySprite.CreateSprite("Textures\\FactionLogo\\Miners\\MinerIcon_3.dds", Surface.TextureSize / 2.0f, new Vector2(iconSize, iconSize));
                sprite.Color = Surface.ScriptForegroundColor;
                frame.Add(sprite);

                string title = $"{Constants.SCRIPT_NAME} {Constants.SCRIPT_VERSION}";

                sprite = MySprite.CreateText(title, Surface.Font, Surface.ScriptBackgroundColor, 1.4f);
                sprite.Position = new Vector2(Surface.TextureSize.X / 2.0f, Surface.TextureSize.Y / 2.0f + 80.0f);
                frame.Add(sprite);

                sprite = MySprite.CreateText(title, Surface.Font, Surface.ScriptBackgroundColor, 1.4f);
                sprite.Position = new Vector2(Surface.TextureSize.X / 2.0f - 2.0f, Surface.TextureSize.Y / 2.0f + 78.0f);
                frame.Add(sprite);
                    
                sprite = MySprite.CreateText(title, Surface.Font, Surface.ScriptBackgroundColor, 1.4f);
                sprite.Position = new Vector2(Surface.TextureSize.X / 2.0f + 2.0f, Surface.TextureSize.Y / 2.0f + 78.0f);
                frame.Add(sprite);

                sprite = MySprite.CreateText(title, Surface.Font, Surface.ScriptBackgroundColor, 1.4f);
                sprite.Position = new Vector2(Surface.TextureSize.X / 2.0f - 2.0f, Surface.TextureSize.Y / 2.0f + 82.0f);
                frame.Add(sprite);

                sprite = MySprite.CreateText(title, Surface.Font, Surface.ScriptBackgroundColor, 1.4f);
                sprite.Position = new Vector2(Surface.TextureSize.X / 2.0f + 2.0f, Surface.TextureSize.Y / 2.0f + 82.0f);
                frame.Add(sprite);

                sprite = MySprite.CreateText(title, Surface.Font, Surface.ScriptForegroundColor, 1.4f);
                sprite.Position = new Vector2(Surface.TextureSize.X / 2.0f, Surface.TextureSize.Y / 2.0f + 80.0f);
                frame.Add(sprite);

                BlockCacheModule bcm = excavOS.GetModule<BlockCacheModule>();
                string emote;
                if (bcm.CurrentState == BlockCacheModule.State.Waiting) {
                    emote = "LCD_Emote_Sleepy";
                } else if (bcm.CurrentState == BlockCacheModule.State.Fetching) {
                    emote = "LCD_Emote_Happy";
                } else if (bcm.CurrentState == BlockCacheModule.State.Sorting) {
                    emote = "LCD_Emote_Love";
                } else {
                    emote = tick % 2 == 0 ? "LCD_Emote_Suspicious_Left" : "LCD_Emote_Suspicious_Right";
                }
                sprite = MySprite.CreateSprite(emote, new Vector2(Surface.TextureSize.X - Surface.SurfaceSize.X, Surface.SurfaceSize.Y - 64.0f), new Vector2(64, 64));
                sprite.Position += Offset;
                sprite.Color = Surface.ScriptForegroundColor;
                frame.Add(sprite);

            }
        }
    }
}
