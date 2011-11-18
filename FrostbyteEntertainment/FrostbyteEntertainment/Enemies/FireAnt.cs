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
            SpawnPoint = initialPos;
            movementStartTime = new TimeSpan(0, 0, 1);
            Personality = new ChargePersonality(this);
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

            List<Sprite> targets = (This.Game.CurrentLevel as FrostbyteLevel).allies;
            Sprite target = GetClosestTarget(targets, float.MaxValue);
            if (target != null)
            {
                float attackRadius = ((target.GetCollision()[0] as Collision_BoundingCircle).Radius + (this.GetCollision()[0] as Collision_BoundingCircle).Radius) * .92f;
                if (Vector2.DistanceSquared(target.GroundPos, this.GroundPos) > attackRadius * attackRadius)
                {
                    Personality.Update();
                }
            }
        }

        protected override void updateAttack()
        {
            if (This.gameTime.TotalGameTime >= attackStartTime + new TimeSpan(0, 0, 2) && isAttackAnimDone)
            {
                List<Tuple<CollisionObject, WorldObject, CollisionObject>> collidedWith;
                Collision.CollisionData.TryGetValue(this, out collidedWith);
                if (collidedWith != null)
                {
                    foreach (Tuple<CollisionObject, WorldObject, CollisionObject> detectedCollision in collidedWith)
                    {
                        if (detectedCollision.Item2 is Player)
                        {
                            mAttacks.Add(Attacks.Melee(this, 5, 18).GetEnumerator());
                            attackStartTime = This.gameTime.TotalGameTime;
                            break;
                        }
                    }
                }
            }
        }
    }
}
