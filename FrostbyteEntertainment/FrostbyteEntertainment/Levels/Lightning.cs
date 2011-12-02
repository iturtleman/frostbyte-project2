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

            l.DiaryEntries = LevelFunctions.LoadLevelNotes(l.Name).GetEnumerator();

            Characters.Mage mage = new Characters.Mage("Player 1", PlayerIndex.One, new Color(255, 0, 0));
            mage.SpawnPoint = new Microsoft.Xna.Framework.Vector2(70 * 64, 8 * 64);
            //mage.SpawnPoint = new Vector2(4576, 3184);
            mage.Speed = 1;
            mage.Scale = 0.7f;
            l.HUD.AddPlayer(mage);

            /*Characters.Mage mage2 = new Characters.Mage("Player 2", PlayerIndex.Two, new Color(114, 255, 255));
            mage2.SpawnPoint = new Microsoft.Xna.Framework.Vector2(72 * 64, 8 * 64);
            mage2.Speed = 1;
            mage2.Scale = 0.7f;
            l.HUD.AddPlayer(mage2);*/

            This.Game.AudioManager.PlayBackgroundMusic("Music/LightningBG", 0.3f);

            l.HUD.FadeText("The Caves");

            #region loadeffects etc
            l.GetEffect("ParticleSystem");
            #endregion loadeffects etc

            #region load textures
            l.GetTexture("boulder");
            #endregion load textures

            #region add applicable spells
            Characters.Mage.UnlockedSpells = Spells.EarthOne;
            #endregion add applicable spells

            Collision.Lists.Add(new KeyValuePair<int, int>(1, 2));
            Collision.Lists.Add(new KeyValuePair<int, int>(1, 3));
            Collision.Lists.Add(new KeyValuePair<int, int>(2, 3));
        }
    }
}
