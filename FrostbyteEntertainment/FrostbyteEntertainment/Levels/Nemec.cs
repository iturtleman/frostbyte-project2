﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Xml.Linq;
using Microsoft.Xna.Framework.Input;
using Frostbyte.Enemies;

namespace Frostbyte.Levels
{
    internal static class Nemec
    {
        internal static void Load()
        {
            Collision.Lists.Add(new KeyValuePair<int, int>(0, 1));

            FrostbyteLevel l = (This.Game.CurrentLevel != This.Game.NextLevel && This.Game.NextLevel != null ? This.Game.NextLevel : This.Game.CurrentLevel) as FrostbyteLevel;

            l.TileMap = new TileList(XDocument.Load(@"Content/Level1.xml"));

            l.AddAnimation(new Animation("shield_opaque.anim"));
            l.AddAnimation(new Animation("antibody.anim"));
            HUD hud = new HUD();

            Characters.Mage mage = new Characters.Mage("mage", new Actor(l.GetAnimation("shield_opaque.anim")));
            mage.Pos = new Microsoft.Xna.Framework.Vector2(200, 200);
            hud.AddPlayer(mage);

            Characters.Mage mage2 = new Characters.Mage("mage2", new Actor(l.GetAnimation("shield_opaque.anim")));
            mage.Pos = new Microsoft.Xna.Framework.Vector2(200, 200);
            hud.AddPlayer(mage2);
        }

        internal static void Update()
        {

        }
    }
}