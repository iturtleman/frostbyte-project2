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
            XDocument doc = XDocument.Load(@"Content/Level1.xml");
            l.Load(doc);

            l.HUD.LoadCommon();

            Frostbyte.Enemies.ElectricBat ebat = new Frostbyte.Enemies.ElectricBat("EB", new Vector2(269 * 64, 250 * 64));
            Characters.Mage mage = new Characters.Mage("mage", new Actor(l.GetAnimation("shield_opaque.anim")));
            mage.Pos = new Microsoft.Xna.Framework.Vector2(108 * 64, 119 * 64);
            mage.Speed = 1;
            l.HUD.AddPlayer(mage);

            Obstacle rock = new Obstacles.Rock("rock");
            rock.Pos = mage.Pos;
            rock.Pos.Y += 50;

            This.Game.AudioManager.PlayBackgroundMusic("Music/EarthBoss");

            #region loadeffects etc
            l.GetEffect("ParticleSystem");
            #endregion loadeffects etc

            #region load textures
            l.GetTexture("boulder");
            #endregion load textures
        }
    }
}
