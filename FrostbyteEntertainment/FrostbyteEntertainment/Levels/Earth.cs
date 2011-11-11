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

            Characters.Mage mage = new Characters.Mage("Player 1", new Actor(l.GetAnimation("shield_opaque.anim")));
            mage.SpawnPoint = new Microsoft.Xna.Framework.Vector2(108 * Tile.TileSize, 119 * Tile.TileSize);
            mage.Speed = 1;
            mage.Respawn();
            l.HUD.AddPlayer(mage);

            Characters.Mage mage2 = new Characters.Mage("Player 2", new Actor(l.GetAnimation("shield_opaque.anim")));
            mage2.SpawnPoint = new Microsoft.Xna.Framework.Vector2(108 * Tile.TileSize, 121 * Tile.TileSize);
            mage2.Speed = 1;
            mage2.Respawn();
            l.HUD.AddPlayer(mage2);
            mage2.controller = new KeyboardController();

            PartyCrossTrigger t = new PartyCrossTrigger("trigger", Tile.TileSize * 3, Tile.TileSize, l.allies);
            t.Orientation = Orientations.Up;
            t.CenterOn(mage);
            t.Pos.Y -= Tile.TileSize * 5;

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

            Collision.Lists.Add(new KeyValuePair<int, int>(1, 2));
        }

        internal static void Unload()
        {
            string nextlevel = LevelFunctions.LoadNextLevel();
            This.Game.SetCurrentLevel(nextlevel);
        }
    }
}
