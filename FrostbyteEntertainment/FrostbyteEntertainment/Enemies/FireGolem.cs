using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Frostbyte.Enemies
{
    internal partial class FireGolem : Golem
    {
        internal FireGolem(string name, Vector2 initialPos)
            : base(name, initialPos)
        {
            ElementType = Element.Fire;
        }
    }
}
