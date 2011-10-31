using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Frostbyte;

namespace Frostbyte.Levels
{
    internal static class TitleScreen
    {
        static readonly TimeSpan RequiredWaitTime = new TimeSpan(0, 0, 0, 0, 0);
        static TimeSpan LevelInitTime = TimeSpan.MinValue;
        private static bool levelCompleted = false;
        private static IController controller;

        internal static void Load(Level context)
        {
            LevelInitTime = TimeSpan.MinValue;
            levelCompleted = false;

            /** load music */
            //This.Game.AudioManager.AddBackgroundMusic("title");
            //This.Game.AudioManager.PlayBackgroundMusic("title");

            Text title = new Text("titletext", "text", "Welcome. Please Press Enter/Start.");
            title.Pos = new Vector2(400, 50);
            title.Static = true;
            title.DisplayColor = Color.Chartreuse;

            if (GamePad.GetState(PlayerIndex.One).IsConnected)
            {
                controller = new GamePadController(PlayerIndex.One);
            }
            else
            {
                controller = new KeyboardController();
            }
        }

        internal static void Update()
        {
            GameTime gameTime = This.gameTime;
            if (LevelInitTime == TimeSpan.MinValue)
            {
                LevelInitTime = gameTime.TotalGameTime;
            }

            controller.Update();
            if (controller.Start == ReleasableButtonState.Clicked)
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
