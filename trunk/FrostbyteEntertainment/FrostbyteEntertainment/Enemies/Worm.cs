using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework;

namespace Frostbyte.Enemies
{

    internal partial class Worm : Frostbyte.Enemy
    {
        #region Variables
        static List<Animation> Animations = new List<Animation>(){
            This.Game.CurrentLevel.GetAnimation("worm-idle-down.anim"),
            This.Game.CurrentLevel.GetAnimation("worm-idle-diagdown.anim"),
            This.Game.CurrentLevel.GetAnimation("worm-idle-right.anim"),
            This.Game.CurrentLevel.GetAnimation("worm-idle-diagup.anim"),
            This.Game.CurrentLevel.GetAnimation("worm-idle-up.anim"),
            This.Game.CurrentLevel.GetAnimation("worm-spew-down.anim"),
            This.Game.CurrentLevel.GetAnimation("worm-spew-diagdown.anim"),
            This.Game.CurrentLevel.GetAnimation("worm-spew-right.anim"),
            This.Game.CurrentLevel.GetAnimation("worm-spew-diagup.anim"),
            This.Game.CurrentLevel.GetAnimation("worm-spew-up.anim"),
            This.Game.CurrentLevel.GetAnimation("worm-submerge.anim"),
            This.Game.CurrentLevel.GetAnimation("worm-surface-down.anim"),
            This.Game.CurrentLevel.GetAnimation("worm-vomit.anim"),
            This.Game.CurrentLevel.GetAnimation("worm-die.anim"),
        };
        #endregion Variables

        internal Worm(string name, Vector2 initialPos)
            : base(name, new Actor(Animations), 1, 10000)
        {
            GroundPos = initialPos;
            movementStartTime = new TimeSpan(0, 0, 1);
            ElementType = Element.Earth;
        }

        protected override void updateMovement()
        {
        }

        protected override void updateAttack()
        {
        }
    }
}
