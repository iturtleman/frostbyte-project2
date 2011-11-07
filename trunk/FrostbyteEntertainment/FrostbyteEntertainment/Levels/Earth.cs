using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Microsoft.Xna.Framework;

namespace Frostbyte.Levels
{
    internal static class Earth
    {
        internal static void Load(Level context)
        {
            FrostbyteLevel l = context as FrostbyteLevel;
            This.Game.AudioManager.AddBackgroundMusic("Music/EarthBoss");
            XDocument doc = XDocument.Load(@"Content/EarthLevel.xml");
            l.Load(doc);
            l.Theme = Element.Earth;

            l.HUD.LoadCommon();

            Characters.Mage mage = new Characters.Mage("Player 1", new Actor(l.GetAnimation("shield_opaque.anim")));
            mage.Pos = new Microsoft.Xna.Framework.Vector2(108 * 64, 119 * 64);
            mage.Speed = 1;
            l.HUD.AddPlayer(mage);

            Characters.Mage mage2 = new Characters.Mage("Player 2", new Actor(l.GetAnimation("shield_opaque.anim")));
            mage2.Pos = new Microsoft.Xna.Framework.Vector2(108 * 64, 121 * 64);
            mage2.Speed = 1;
            l.HUD.AddPlayer(mage2);
            mage2.controller = new KeyboardController();

            This.Game.AudioManager.AddBackgroundMusic("Music/GenericBoss");
            This.Game.AudioManager.AddBackgroundMusic("Music/EarthBG");

            This.Game.AudioManager.BackgroundMusicVolume = 0.1f;
            This.Game.AudioManager.PlayBackgroundMusic("Music/EarthBoss");

            #region loadeffects etc
            l.GetEffect("ParticleSystem");
            #endregion loadeffects etc

            #region load textures
            l.GetTexture("boulder");
            #endregion load textures
        }

        internal static void Unload()
        {
            string nextlevel = LevelFunctions.LoadNextLevel();
            This.Game.SetCurrentLevel(nextlevel);
        }
    }
}
