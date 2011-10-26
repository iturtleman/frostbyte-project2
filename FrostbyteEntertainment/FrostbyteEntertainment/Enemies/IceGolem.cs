﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Frostbyte.Enemies
{
    internal partial class IceGolem : Golem
    {
        internal IceGolem(string name, float speed, int health, Vector2 initialPos)
            : base(name, health, initialPos, speed)
        {
            ElementType = Element.Water;
        }
    }
}