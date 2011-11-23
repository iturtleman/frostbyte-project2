using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Frostbyte;
using Frostbyte.Obstacles;
using Microsoft.Xna.Framework.Graphics;

namespace Frostbyte.Levels
{
    internal static class TitleScreen
    {
        static readonly TimeSpan RequiredWaitTime = new TimeSpan(0, 0, 0, 0, 0);
        static TimeSpan LevelInitTime = TimeSpan.MinValue;

        internal static void Load(Level context)
        {
            FrostbyteLevel l = context as FrostbyteLevel;
            l.Theme = Element.None;
            LevelInitTime = TimeSpan.MinValue;
            
            Viewport v = This.Game.GraphicsDevice.Viewport;

            /** load music */
            This.Game.AudioManager.AddBackgroundMusic("Music/TitleScreenBG");
            This.Game.AudioManager.PlayBackgroundMusic("Music/TitleScreenBG");

            Text title = new Text("titletext", "text", "Welcome. Please Press Enter/Start.");
            title.CenterOn(new Vector2(v.Width / 2, v.Height / 2));
            title.Static = true;
            title.DisplayColor = Color.Chartreuse;

            context.GetTexture("regen");
            RestorePlayerHealthTrigger t = new RestorePlayerHealthTrigger("trigger", v.Width);
            t.SpawnPoint = new Vector2(v.Width / 2, v.Height);
        }

        internal static void Update()
        {
            GameTime gameTime = This.gameTime;
            if (LevelInitTime == TimeSpan.MinValue)
            {
                LevelInitTime = gameTime.TotalGameTime;
            }

            if ((This.Game as FrostbyteGame).GlobalController.Start == ReleasableButtonState.Clicked)
            {
                // Go to next
                (This.Game.CurrentLevel as FrostbyteLevel).LevelCompleted = true;
            }
        }
    }
}
