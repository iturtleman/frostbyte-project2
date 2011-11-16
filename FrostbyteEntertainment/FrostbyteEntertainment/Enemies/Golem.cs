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
            Personality = new SentinelPersonality(this);
            ElementType = Element.Normal;
            SpawnPoint = initialPos;
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
                            mAttacks.Add(Attacks.Melee(this, 20, 18, 100).GetEnumerator());
                            attackStartTime = This.gameTime.TotalGameTime;
                            This.Game.AudioManager.PlaySoundEffect("Effects/Golem_Attack");
                        }
                    }
                }
            }
        }
    }
}

