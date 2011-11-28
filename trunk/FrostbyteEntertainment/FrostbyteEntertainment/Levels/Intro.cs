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
            FrostbyteLevel l = context as FrostbyteLevel;
            l.Theme = Element.None;

            l.DiaryEntries = LevelFunctions.LoadLevelNotes(l.Name).GetEnumerator();
            l.DiaryEntries.MoveNext();

            This.Game.AudioManager.AddBackgroundMusic("Music/TitleScreenBG");
            This.Game.AudioManager.PlayBackgroundMusic("Music/TitleScreenBG", 0.1f);

            Viewport v = This.Game.GraphicsDevice.Viewport;
            scroller = new TextScroller("intro_text", v.Width * 3 / 4, v.Height * 3 / 4);
            scroller.Pos.X = v.Width / 8;
            scroller.Pos.Y = v.Height / 8;
            scroller.Static = true;
            scroller.ScrollText(l.DiaryEntries.Current);
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
