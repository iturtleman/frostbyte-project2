using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace Frostbyte.Levels
{
    internal static class Intro
    {
        private static TextScroller scroller;

        internal static void Load(Level context)
        {
            Viewport v = This.Game.GraphicsDevice.Viewport;
            scroller = new TextScroller("intro_text", v.Width * 3 / 4, v.Height * 3 / 4);
            scroller.Pos.X = v.Width / 8;
            scroller.Pos.Y = v.Height / 8;
            scroller.Static = true;
            scroller.ScrollText("this is the intro text");
        }

        internal static bool CompletionCondition()
        {
            return !scroller.Scrolling ||
                (This.Game as FrostbyteGame).GlobalController.Start == ReleasableButtonState.Clicked;
        }

        internal static void Unload()
        {
            string nextlevel = LevelFunctions.LoadNextLevel();
            This.Game.SetCurrentLevel(nextlevel);
        }
    }
}
