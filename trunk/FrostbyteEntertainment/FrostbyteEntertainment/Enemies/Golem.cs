using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace Frostbyte.Enemies
{
    internal partial class Golem : Frostbyte.Enemy
    {
        #region Variables

        bool changeState = false;
        TimeSpan idleTime = new TimeSpan(0, 0, 2);

        static List<Animation> Animations = new List<Animation>(){
                This.Game.CurrentLevel.GetAnimation("golem-idle-down.anim"),
            This.Game.CurrentLevel.GetAnimation("golem-idle-diagdown.anim"),
            This.Game.CurrentLevel.GetAnimation("golem-idle-right.anim"),
            This.Game.CurrentLevel.GetAnimation("golem-idle-diagup.anim"),
            This.Game.CurrentLevel.GetAnimation("golem-idle-up.anim"),
            This.Game.CurrentLevel.GetAnimation("golem-walk-down.anim"),
            This.Game.CurrentLevel.GetAnimation("golem-walk-diagdown.anim"),
            This.Game.CurrentLevel.GetAnimation("golem-walk-right.anim"),
            This.Game.CurrentLevel.GetAnimation("golem-walk-diagup.anim"),
            This.Game.CurrentLevel.GetAnimation("golem-walk-up.anim"),
            This.Game.CurrentLevel.GetAnimation("golem-attack-down.anim"),
            This.Game.CurrentLevel.GetAnimation("golem-attack-diagdown.anim"),
            This.Game.CurrentLevel.GetAnimation("golem-attack-right.anim"),
            This.Game.CurrentLevel.GetAnimation("golem-attack-diagup.anim"),
            This.Game.CurrentLevel.GetAnimation("golem-attack-up.anim"),
        };

        #endregion Variables

        public Golem(string name, Vector2 initialPos, List<Animation> anims = null)
            : base(name, new Actor(anims == null ? Animations : anims), 1, 1000)
        {
            movementStartTime = new TimeSpan(0, 0, 1);
            Personality = new StrictSentinelPersonality(this);
            ElementType = Element.Normal;
            GroundPos = initialPos;
            startAttackDistance = 70; //in pixels
            This.Game.AudioManager.AddSoundEffect("Effects/Golem_Attack");
            This.Game.AudioManager.AddSoundEffect("Effects/Golem_Move");
            MovementAudioName = "Effects/Golem_Move";
            This.Game.AudioManager.InitializeLoopingSoundEffect(MovementAudioName);
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
                        mAttack = Attacks.Melee(target, this, 20, 18, 100, new TimeSpan(0, 0, 0, 1, 500)).GetEnumerator();
                        This.Game.AudioManager.PlaySoundEffect("Effects/Golem_Attack");
                    }
                }
            }
        }
    }
}

