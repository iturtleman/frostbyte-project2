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
            This.Game.AudioManager.AddBackgroundMusic("Music/EarthBoss");
            XDocument doc = XDocument.Load(@"Content/EarthLevel.xml");
            l.Load(doc);

            l.HUD.LoadCommon();

            l.DiaryEntries = LevelFunctions.LoadLevelNotes(l.Name).GetEnumerator();

            Characters.Mage mage = new Characters.Mage("Player 1", new Actor(l.GetAnimation("shield_opaque.anim")), PlayerIndex.One ,new Color(255, 0, 0));
            mage.SpawnPoint = new Microsoft.Xna.Framework.Vector2(108 * Tile.TileSize, 119 * Tile.TileSize);
            //mage.SpawnPoint = new Vector2(7776, 2700);
            mage.Speed = 1;
            l.HUD.AddPlayer(mage);

            Obstacles.DiaryEntry d1 = new Obstacles.DiaryEntry("diary");
            d1.SpawnOn(mage);
            d1.SpawnPoint.Y -= Tile.TileSize;

            Obstacles.DiaryEntry d2 = new Obstacles.DiaryEntry("diary");
            d2.SpawnOn(mage);
            d2.SpawnPoint.Y -= Tile.TileSize;
            d2.SpawnPoint.X -= Tile.TileSize;

            Obstacles.DiaryEntry d3 = new Obstacles.DiaryEntry("diary");
            d3.SpawnOn(mage);
            d3.SpawnPoint.Y -= Tile.TileSize;
            d3.SpawnPoint.X += Tile.TileSize;

            Obstacles.DiaryEntry d4 = new Obstacles.DiaryEntry("diary");
            d4.SpawnOn(mage);
            d4.SpawnPoint.Y -= Tile.TileSize;
            d4.SpawnPoint.Y -= 2*Tile.TileSize;

            /*Characters.Mage mage2 = new Characters.Mage("Player 2", new Actor(l.GetAnimation("shield_opaque.anim")), PlayerIndex.Two, new Color(114, 255, 255));
            mage2.SpawnPoint = new Microsoft.Xna.Framework.Vector2(108 * Tile.TileSize, 121 * Tile.TileSize);
            mage2.Speed = 1;
            l.HUD.AddPlayer(mage2);*/

            /*PartyCrossTrigger t = new PartyCrossTrigger("party", 64, 64, l.allies);
            t.Orientation = Orientations.Up;
            t.SpawnOn(mage);
            t.SpawnPoint.Y -= 128;*/
            

            This.Game.AudioManager.AddBackgroundMusic("Music/GenericBoss");
            This.Game.AudioManager.AddBackgroundMusic("Music/EarthBG");

            This.Game.AudioManager.BackgroundMusicVolume = 0.1f;
            This.Game.AudioManager.PlayBackgroundMusic("Music/EarthBG");

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

        internal static void Unload()
        {
            string nextlevel = LevelFunctions.LoadNextLevel();
            This.Game.SetCurrentLevel(nextlevel);
        }
    }
}
