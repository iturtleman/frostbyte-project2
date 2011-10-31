using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Frostbyte.Obstacles
{
    internal class Rock : Obstacle
    {
        internal Rock(string name)
            : base(name, new Actor(This.Game.LoadingLevel.GetAnimation("rock.anim")))
        {
            ZOrder = int.MaxValue;
        }
    }
}
