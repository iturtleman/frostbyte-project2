using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Frostbyte.Obstacles
{
    internal class Door : Obstacle
    {
        internal Door(string name)
            : base(name, new Actor(new Animation("door.anim")))
        {
            ZOrder = int.MinValue;
            This.Game.AudioManager.AddSoundEffect("Effects/Door_Open");
        }

        internal Door(string name, Vector2 initialPos)
            : this(name)
        {
            Pos = initialPos;
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
