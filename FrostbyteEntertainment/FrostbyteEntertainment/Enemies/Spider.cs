using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Frostbyte.Enemies
{
    internal partial class Spider : Frostbyte.Enemy
    {

        #region Variables

        bool changeState = false;
        TimeSpan idleTime = new TimeSpan(0, 0, 2);

        static List<Animation> Animations = new List<Animation>(){
            This.Game.CurrentLevel.GetAnimation("spider-idle-down.anim"),
            This.Game.CurrentLevel.GetAnimation("spider-idle-diagdown.anim"),
            This.Game.CurrentLevel.GetAnimation("spider-idle-right.anim"),
            This.Game.CurrentLevel.GetAnimation("spider-idle-diagup.anim"),
            This.Game.CurrentLevel.GetAnimation("spider-idle-up.anim"),
            This.Game.CurrentLevel.GetAnimation("spider-idle-down.anim"),
            This.Game.CurrentLevel.GetAnimation("spider-idle-diagdown.anim"),
            This.Game.CurrentLevel.GetAnimation("spider-idle-right.anim"),
            This.Game.CurrentLevel.GetAnimation("spider-idle-diagup.anim"),
            This.Game.CurrentLevel.GetAnimation("spider-idle-up.anim"),
            This.Game.CurrentLevel.GetAnimation("spider-attack-down.anim"),
            This.Game.CurrentLevel.GetAnimation("spider-attack-diagdown.anim"),
            This.Game.CurrentLevel.GetAnimation("spider-attack-right.anim"),
            This.Game.CurrentLevel.GetAnimation("spider-attack-diagup.anim"),
            This.Game.CurrentLevel.GetAnimation("spider-attack-up.anim"),
        };

        #endregion Variables

        internal Spider(string name, Vector2 initialPos)
            : base(name, new Actor(Animations), 1, 100)
        {
            GroundPos = initialPos;
            movementStartTime = new TimeSpan(0, 0, 1);
            Personality = new PulseChargePersonality(this);
            ElementType = Element.Normal;
        }

        protected override void updateMovement()
        {
            if (changeState)
            {
                movementStartTime = TimeSpan.MaxValue;
            }
            List<Sprite> targets = This.Game.CurrentLevel.GetSpritesByType(typeof(Player));
            Personality.Update();
        }

        protected override void updateAttack()
        {
                
        }

    }
}
