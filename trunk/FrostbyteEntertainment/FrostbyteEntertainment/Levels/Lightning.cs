using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Microsoft.Xna.Framework;

namespace Frostbyte.Levels
{
    internal static class Lightning
    {
        internal static void Load(Level context)
        {
            FrostbyteLevel l = context as FrostbyteLevel;
            l.Theme = Element.Lightning;
            This.Game.AudioManager.AddBackgroundMusic("Music/LightningBG");
            XDocument doc = XDocument.Load(@"Content/LightningLevel.xml");
            l.Load(doc);

            l.HUD.LoadCommon();

            Characters.Mage mage = new Characters.Mage("Player 1", new Actor(l.GetAnimation("shield_opaque.anim")));
            mage.SpawnPoint = new Microsoft.Xna.Framework.Vector2(70 * 64, 8 * 64);
            mage.Speed = 1;
            l.HUD.AddPlayer(mage);

            Characters.Mage mage2 = new Characters.Mage("Player 2", new Actor(l.GetAnimation("shield_opaque.anim")));
            mage2.SpawnPoint = new Microsoft.Xna.Framework.Vector2(72 * 64, 8 * 64);
            mage2.Speed = 1;
            l.HUD.AddPlayer(mage2);
            mage2.controller = new KeyboardController();

            This.Game.AudioManager.PlayBackgroundMusic("Music/LightningBG");

            #region loadeffects etc
            l.GetEffect("ParticleSystem");
            #endregion loadeffects etc

            #region load textures
            l.GetTexture("boulder");
            #endregion load textures

            Collision.Lists.Add(new KeyValuePair<int, int>(1, 2));
        }
    }
}
