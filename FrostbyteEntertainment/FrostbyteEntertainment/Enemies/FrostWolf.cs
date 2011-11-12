using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace Frostbyte.Enemies
{
    internal partial class FrostWolf : Frostbyte.Enemy
    {

        #region Variables

        bool changeState = false;
        TimeSpan idleTime = new TimeSpan(0, 0, 2);

        static List<Animation> Animations = new List<Animation>(){
            This.Game.CurrentLevel.GetAnimation("antibody.anim"),
        };

        #endregion Variables

        public FrostWolf(string name, Vector2 initialPos)
            : base(name, new Actor(Animations), 1, 100)
        {
            GroundPos = initialPos;
            movementStartTime = new TimeSpan(0, 0, 1);
            Personality = new AmbushPersonality(this);
            ElementType = Element.Water;
            //Scale = .4f;

            startAttackDistance = 20;

            //This.Game.AudioManager.AddSoundEffect("Effects/Wolf_Move");
            //if (MovementAudioName == null)
            //{
            //MovementAudioName = "Effects/Wolf_Move";
            //This.Game.AudioManager.InitializeLoopingSoundEffect(MovementAudioName);
            //}
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
            if (isMovingAllowed)
            {
                float range = 150.0f;
                List<Sprite> targets = This.Game.CurrentLevel.GetSpritesByType(typeof(Player));
                Sprite target = GetClosestTarget(targets, range);
                if (target != null)
                {
                    if (Vector2.DistanceSquared(target.GroundPos, this.GroundPos) < this.startAttackDistance * this.startAttackDistance)
                    {
                        isAttacking = true;
                        isAttackingAllowed = false;
                        mAttacks.Add(Attacks.Melee(target, this, 5, 18, 20, new TimeSpan(0, 0, 0, 0, 250)).GetEnumerator());
                    }
                }
            }
        }

    }
}

