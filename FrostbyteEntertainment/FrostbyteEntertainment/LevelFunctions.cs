using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Frostbyte
{
    internal static class LevelFunctions
    {
        internal delegate Sprite EnemyFactory();

        internal static readonly Random rand = new Random();

        internal static void DoNothing() { }

        /// <summary>
        /// A behavior that sends the player to the Stage Clear screen once the level is over.
        /// </summary>
        internal static void ToStageClear()
        {
            This.Game.SetCurrentLevel("StageClear");
        }

        internal static string LoadNextLevel()
        {
            FrostbyteLevel l = This.Game.CurrentLevel as FrostbyteLevel;
            int ix = FrostbyteLevel.LevelProgression.IndexOf(l.Name);
            string nextlevel = FrostbyteLevel.LevelProgression[(ix + 1) % FrostbyteLevel.LevelProgression.Count];
            This.Game.LoadLevel(nextlevel);
            return nextlevel;
        }

        internal static void GoToGameOver()
        {
            Condition oldWin = This.Game.CurrentLevel.WinCondition;
            Behavior oldEnd = This.Game.CurrentLevel.EndBehavior;
            This.Game.CurrentLevel.WinCondition = delegate { return true; };
            This.Game.CurrentLevel.EndBehavior = delegate
            {
                // Replace the old win condition
                This.Game.CurrentLevel.WinCondition = oldWin;
                This.Game.CurrentLevel.EndBehavior = oldEnd;
                This.Game.SetCurrentLevel("GameOver");
            };
            This.Game.AudioManager.Stop();
        }

        /// <summary>
        /// Spawns Enemies created by the EnemyFactory at random locations on the screen
        /// </summary>
        /// <param name="constructEnemy">Function to define an enemy</param>
        /// <param name="numEnemies">Number of enemies</param>
        internal static void Spawn(EnemyFactory constructEnemy, int numEnemies)
        {
            if (!This.Cheats.SpawnEnemies.Enabled)
            {
                Level l = This.Game.CurrentLevel;
                for (int i = 0; i < numEnemies; i++)
                {
                    Sprite virus = constructEnemy();
                    virus.Pos = l.Camera.Pos +
                        new Vector2(rand.Next(-500, This.Game.GraphicsDevice.Viewport.Width+500),
                            rand.Next(-500, This.Game.GraphicsDevice.Viewport.Height+500));
                }
            }
        }

        /// <summary>
        /// Spawns Enemies created by the EnemyFactory centered within a radius 
        /// proportional to the number of enemies around a specified position
        /// </summary>
        /// <param name="constructEnemy"></param>
        /// <param name="numEnemies"></param>
        /// <param name="position"></param>
        internal static void Spawn(EnemyFactory constructEnemy, int numEnemies, Vector2 position)
        {
            if (!This.Cheats.SpawnEnemies.Enabled)
            {
                double radius = 160f;
                double angleInc = (1.5 * Math.PI) / numEnemies;
                double startAngle = Math.PI * 2 * rand.NextDouble();
                for (int i = 0; i < numEnemies; i++)
                {
                    Sprite virus = constructEnemy();
                    virus.Pos = position + new Vector2((float)(Math.Cos(angleInc * i + startAngle) * radius * rand.NextDouble()),
                        (float)(Math.Sin(angleInc * i + startAngle) * radius * rand.NextDouble()));
                }
            }
        }
    }
}
