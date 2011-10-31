using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Frostbyte
{
    internal class Target : Polygon
    {
        internal Target(string name, Color c)
            : this(name, c, 15)
        {
        }

        internal Target(string name, Color color, int size)
            : base(name, new Actor(new DummyAnimation(name, size, size)), color,
                new Vector3(0, 0, 0),
                new Vector3(size, 0, 0),
                new Vector3(size, size, 0),
                new Vector3(0, size, 0),
                new Vector3(0, 0, 0))
        {
            ZOrder = int.MaxValue;
        }
    }
}
