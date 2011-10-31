using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Frostbyte.Enemies
{
    internal partial class ElectricGolem : Golem
    {
        
     #region Variables

        bool changeState = false;
        TimeSpan idleTime = new TimeSpan(0, 0, 2);

        static List<Animation> Animations = new List<Animation>(){
            This.Game.CurrentLevel.GetAnimation("antibody.anim")
        };

        #endregion Variables

        internal ElectricGolem(string name, float speed, int health, Vector2 initialPos)
            : base(name, initialPos)
        {
            movementStartTime = new TimeSpan(0, 0, 1);
            Personality = new StrictSentinelPersonality(this);
            ElementType = Element.Lightning;
            GroundPos = initialPos;
            startAttackDistance = 40; //in pixels
            This.Game.AudioManager.AddSoundEffect("Effects/golem_attack");
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
            if (isAttacking)
            {
                mAttack.MoveNext();
                isAttacking = !mAttack.Current;
            }
            else if (isAttackingAllowed)
            {
                float range = 250.0f;
                List<Sprite> targets = This.Game.CurrentLevel.GetSpritesByType(typeof(Player));
                Sprite target = GetClosestTarget(targets, range);
                if (target != null)
                {
                    if (Vector2.DistanceSquared(target.GroundPos, this.GroundPos) < this.startAttackDistance * this.startAttackDistance)
                    {
                        isAttacking = true;
                        isAttackingAllowed = false;
                        isMovingAllowed = false;
                        mAttack = Attacks.Melee(target, this, 20, 18, 25,new TimeSpan(0,0,0,1,500)).GetEnumerator();
                        This.Game.AudioManager.PlaySoundEffect("Effects/golem_attack");
                    }
                }
            }
        }
    }
}
