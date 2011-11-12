using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Frostbyte
{
    internal class ElementalBuff : StatusEffect
    {
        internal ElementalBuff(Element e)
            :base(new TimeSpan(0,0,42), LevelFunctions.DoNothing)
        {
            Element = e;
        }

        Element Element = Element.Normal;
    }
}
