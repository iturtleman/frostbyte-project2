using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;


namespace Frostbyte.Enemies
{
    internal partial class FireAnt : Frostbyte.Enemy
    {
        #region Variables

        bool changeState = false;
        TimeSpan idleTime = new TimeSpan(0, 0, 2);

        static List<Animation> Animations = new List<Animation>(){
            This.Game.CurrentLevel.GetAnimation("antibody.anim"),
        };
        #endregion Variables

        public FireAnt(string name, Vector2 initialPos)
            : base(name, new Actor(Animations), 1, 100)
        {
            GroundPos = initialPos;
            movementStartTime = new TimeSpan(0, 0, 1);
            Personality = new ChargePersonality(this);
            startAttackDistance = 10; //in pixels
            //This.Game.AudioManager.AddSoundEffect("Effects/FireAnt_Move");
            //if (MovementAudioName == null)
            //{
                //MovementAudioName = "Effects/FireAnt_Move";
                //This.Game.AudioManager.InitializeLoopingSoundEffect(MovementAudioName);
            //}
        }

        protected override void updateMovement()
        {
            if (changeState)
            {
                movementStartTime = TimeSpan.MaxValue;
            }
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
                float range = 150.0f;
                List<Sprite> targets = This.Game.CurrentLevel.GetSpritesByType(typeof(Player));
                Sprite target = GetClosestTarget(targets, range);
                if (target != null)
                {
                    if (Vector2.DistanceSquared(target.GroundPos, this.GroundPos) < this.startAttackDistance * this.startAttackDistance)
                    {
                        isAttacking = true;
                        isAttackingAllowed = false;
                        mAttack = Attacks.Melee(target, this, 5, 18, 40, TimeSpan.Zero).GetEnumerator();
                    }
                }
            }
        }
    }
}
