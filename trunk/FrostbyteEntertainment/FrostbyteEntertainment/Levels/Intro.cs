using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
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

            Characters.Mage mage = new Characters.Mage("Player 1", PlayerIndex.One, new Color(255, 0, 0));
            mage.Visible = false;
            mage.Speed = 0;
            Characters.Mage mage2 = new Characters.Mage("Player 2", PlayerIndex.Two, new Color(114, 255, 255));
            mage2.Visible = false;
            mage2.Speed = 0;
        }

        internal static bool CompletionCondition()
        {
            bool PlayerPressedStart = false;
            foreach(Sprite sp in (This.Game.CurrentLevel as FrostbyteLevel).allies)
                if((sp as Frostbyte.Characters.Mage).controller.Start == ReleasableButtonState.Clicked)
                    PlayerPressedStart = true;

            return !scroller.Scrolling ||
                (This.Game as FrostbyteGame).GlobalController.Start == ReleasableButtonState.Clicked ||
                PlayerPressedStart;
        }

        internal static void Unload()
        {
            string nextlevel = LevelFunctions.LoadNextLevel();
            This.Game.SetCurrentLevel(nextlevel);
        }
    }
}
