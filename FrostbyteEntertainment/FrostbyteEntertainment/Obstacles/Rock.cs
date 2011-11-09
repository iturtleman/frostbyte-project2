using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Frostbyte.Obstacles
{
    internal class Rock : Obstacle
    {
        internal Rock(string name)
            : base(name, new Actor(This.Game.LoadingLevel.GetAnimation("rock.anim")))
        {
            ZOrder = int.MaxValue;
        }

        internal Rock(string name, Vector2 initialPosition)
            : this(name)
        {
            Pos = initialPosition;
        }
    }

    internal class PartialRock : Obstacle
    {
        internal PartialRock(string name)
            : base(name, new Actor(This.Game.LoadingLevel.GetAnimation("rock.anim")))
        {
            ZOrder = int.MaxValue;
        }

        internal PartialRock(string name, Vector2 initialPosition)
            : this(name)
        {
            Pos = initialPosition;
        }
    }
}
