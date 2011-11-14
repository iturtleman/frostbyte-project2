﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Frostbyte.Obstacles
{
    internal class Door : Obstacle
    {
        public Door(string name)
            : base(name, new Actor(new Animation("door.anim")))
        {
            ZOrder = int.MinValue;
            This.Game.AudioManager.AddSoundEffect("Effects/Door_Open");
        }

        /// <summary>
        /// Constructor for the Level Editor
        /// </summary>
        /// <param name="name">Sprite name of Door</param>
        /// <param name="initialPos">Position of door</param>
        /// <param name="orientation">Door's orientation/direction</param>
        public Door(string name, Vector2 initialPos, Orientations orientation=Orientations.Up)
            : this(name)
        {
            SpawnPoint = initialPos;
            this.Orientation = orientation;
        }

        internal void Open()
        {
            This.Game.AudioManager.PlaySoundEffect("Effects/Door_Open");
            This.Game.CurrentLevel.RemoveSprite(this);
            FrostbyteLevel l = (This.Game.CurrentLevel as FrostbyteLevel);
            if (l != null)
            {
                l.obstacles.Remove(this);
            }
        }
    }
}
