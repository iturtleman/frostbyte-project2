using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Frostbyte;
using Microsoft.Xna.Framework.Graphics;

namespace Frostbyte.Levels
{
    internal static class TitleScreen
    {
        static readonly TimeSpan RequiredWaitTime = new TimeSpan(0, 0, 0, 0, 0);
        static TimeSpan LevelInitTime = TimeSpan.MinValue;
        private static bool levelCompleted = false;

        internal static void Load(Level context)
        {
            FrostbyteLevel l = context as FrostbyteLevel;
            l.Theme = Element.DEFAULT;
            LevelInitTime = TimeSpan.MinValue;
            levelCompleted = false;

            Viewport v = This.Game.GraphicsDevice.Viewport;

            /** load music */
            //This.Game.AudioManager.AddBackgroundMusic("title");
            //This.Game.AudioManager.PlayBackgroundMusic("title");

            Text title = new Text("titletext", "text", "Welcome. Please Press Enter/Start.");
            title.CenterOn(new Vector2(v.Width / 2, v.Height / 2));
            title.Static = true;
            title.DisplayColor = Color.Chartreuse;

            context.GetTexture("regen");
            RestorePlayerHealthTrigger t = new RestorePlayerHealthTrigger("trigger", v.Width, v.Height);
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
                levelCompleted = true;
            }
        }

        internal static bool CompletionCondition()
        {
            return levelCompleted;
        }

        internal static void Unload()
        {
            string levelname = FrostbyteLevel.LevelProgression[0];
            This.Game.LoadLevel(levelname);
            This.Game.SetCurrentLevel(levelname);
        }
    }
}
