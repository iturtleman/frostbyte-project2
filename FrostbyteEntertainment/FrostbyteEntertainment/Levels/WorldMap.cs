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
    internal class WorldMap
    {
        static readonly TimeSpan RequiredWaitTime = new TimeSpan(0, 0, 0, 0, 0);
        static TimeSpan LevelInitTime = TimeSpan.MinValue;
        private static bool levelCompleted = false;
        private static int visited = 0;

        private static List<Vector2> LevelPositions = null;


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

            if (LevelPositions == null)
            {
                LevelPositions = new List<Vector2>(new Vector2[]{
                    new Vector2(10, 10),
                    new Vector2(50, 25),
                    new Vector2(50, 80),
                    new Vector2(125, 100),
                    new Vector2(285, 40)
                });
            }

            Text title = new Text("titletext", "text", visited.ToString());
            title.CenterOn(new Vector2(v.Width / 2, 100));
            title.Static = true;
            title.DisplayColor = Color.Chartreuse;

            /*if (visited < LevelPositions.Count)
            {
                title.CenterOn(LevelPositions[visited]);
            }*/
            visited++;

            ConcentricCircles c = new ConcentricCircles("cc", 32, 32);
            c.SpawnPoint = new Vector2(v.Width / 2, v.Height / 2);
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
            string nextlevel = LevelFunctions.LoadNextLevel();
            This.Game.SetCurrentLevel(nextlevel);
        }
    }
}
