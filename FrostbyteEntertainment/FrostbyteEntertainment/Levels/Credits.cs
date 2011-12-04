using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Frostbyte;

namespace Forstbyte.Levels
{
    internal static class Credits
    {
        static bool levelCompleted = false;

        internal static void Load(Level context)
        {
            levelCompleted = false;
            FrostbyteLevel l = context as FrostbyteLevel;
            l.Theme = Element.DEFAULT;
        }

        internal static void Update()
        {
            GameTime gameTime = This.gameTime;
            Level l = This.Game.CurrentLevel;

            GamePadState currentState = GamePad.GetState(PlayerIndex.One);
            if (currentState.IsConnected)
            {
                if (This.Game.mLastPadStateP1.Buttons.Start == ButtonState.Released &&
                    currentState.Buttons.Start == ButtonState.Pressed)
                {
                    // Go to next
                    // Make awesome sound
                    levelCompleted = true;
                }
            }
            else /* Move with arrow keys */
            {
                KeyboardState keys = Keyboard.GetState();

                if (keys.IsKeyDown(Keys.Enter))
                {
                    // Go to next
                    // Make awesome sound
                    levelCompleted = true;
                }
            }
        }

        internal static void Unload()
        {
            This.Game.SetCurrentLevel("TitleScreen");
        }

        internal static bool CompletionCondition()
        {
            return levelCompleted;
        }
    }
}
