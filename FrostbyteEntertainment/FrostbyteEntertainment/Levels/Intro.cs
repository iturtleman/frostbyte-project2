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
            l.Theme = Element.DEFAULT;
            Viewport v = This.Game.GraphicsDevice.Viewport;
            scroller = new TextScroller("intro_text", v.Width * 3 / 4, v.Height * 3 / 4);
            scroller.Pos.X = v.Width / 8;
            scroller.Pos.Y = v.Height / 8;
            scroller.Static = true;
            scroller.ScrollText(@"Long ago, a group of four men attempted to trick the gods into giving each"+ 
            " of them power over one of the elements. The men believed that with control" + 
            " over earth, lightning, water, and fire, they could control the world. The gods," +
            " however, saw the evil in the hearts of these men, and devised a plan of their own." +
            " The men - Solum, Percutio, Madesco, and Exuro - were given the powers they sought, but" +
            " were condemned to an eternity in a domain dominated by their respective elements. Legends" + 
            " tell of these men and their slow descent into madness and corruption at the hands of the gods," +
            " who vowed to never show the men a hint of mercy. Now, centuries later, a reclusive student of" +
            " alchemy and the elements has succeeded where the original organization could not. Through his" +
            " extensive studies, this man - Caelestis - has found a way to harness the power of earth, lightning," +
            " water, and fire without the consent of the gods, and the gods sense evil intentions in his heart." +
            " You have been chosen to seek out and destroy Caelestis before he can succeed in carrying out the" +
            " evil plans the gods have sensed.  If you are to have any hope of surviving, the gods have stated" +
            " that you must also have control over the elements. In order to accomplish this, Solum, Percutio," +
            " Madesco, and Exuro must be sought out in turn and destroyed, for the only way a mortal can gain the" +
            " power of an element is to end the life of the current holder of the god-granted power.");
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
