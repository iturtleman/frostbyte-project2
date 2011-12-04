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
            l.Theme = Element.Earth;
            XDocument doc = XDocument.Load(@"Content/EarthLevel.xml");
            l.Load(doc);

            l.HUD.LoadCommon(new EarthTheme());

            l.DiaryEntries = LevelFunctions.LoadLevelNotes(l.Name).GetEnumerator();

            l.ExitPortalSpawnPoint = new Vector2(7776, 2700);

            Characters.Mage mage = new Characters.Mage("Player 1", PlayerIndex.One, new Color(255, 0, 0));
            //mage.SpawnPoint = new Microsoft.Xna.Framework.Vector2(108 * Tile.TileSize, 119 * Tile.TileSize);
            mage.SpawnPoint = new Vector2(7776, 2684);
            mage.Speed = 1;
            mage.Scale = 0.7f;
            l.HUD.AddPlayer(mage);

            Characters.Mage mage2 = new Characters.Mage("Player 2", PlayerIndex.Two, new Color(114, 255, 255));
            //mage2.SpawnPoint = new Microsoft.Xna.Framework.Vector2(108 * Tile.TileSize, 121 * Tile.TileSize);
            mage2.SpawnPoint = new Vector2(7756, 2684);
            mage2.Speed = 1;
            mage2.Scale = 0.7f;
            l.HUD.AddPlayer(mage2);
            
            This.Game.AudioManager.AddBackgroundMusic("Music/EarthBG");
            This.Game.AudioManager.PlayBackgroundMusic("Music/EarthBG", 0.1f);

            l.isPauseEnabled = true;

            l.HUD.FadeText("Chapter I: Solum's Forest");

            #region loadeffects etc
            l.GetEffect("ParticleSystem");
            #endregion loadeffects etc

            #region load textures
            l.GetTexture("boulder");
            #endregion load textures

            Collision.Lists.Add(new KeyValuePair<int, int>(1, 2));
            Collision.Lists.Add(new KeyValuePair<int, int>(1, 3));
            Collision.Lists.Add(new KeyValuePair<int, int>(2, 3));

        }
    }
}
