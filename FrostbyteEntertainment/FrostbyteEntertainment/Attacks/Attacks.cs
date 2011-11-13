using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections;

namespace Frostbyte
{
    internal static class Attacks
    {
        internal delegate void CreateParticles(OurSprite attacker, Vector2 direction, float projectileSpeed, ParticleEmitter particleEmitter);

        /// <summary>
        /// Sets correctly oriented animation and returns number of frames in animation
        /// </summary>
        /// <returns>returns number of frames in animation</returns>
        private static int setAnimationReturnFrameCount(OurSprite attacker)
        {
            switch (attacker.Orientation)
            {
                case Orientations.Down:
                    attacker.SetAnimation(0 + 5 * attacker.State.GetHashCode());
                    break;
                case Orientations.Down_Right:
                    attacker.Hflip = false;
                    attacker.SetAnimation(1 + 5 * attacker.State.GetHashCode());
                    break;
                case Orientations.Down_Left:
                    attacker.Hflip = true;
                    attacker.SetAnimation(1 + 5 * attacker.State.GetHashCode());
                    break;
                case Orientations.Right:
                    attacker.Hflip = false;
                    attacker.SetAnimation(2 + 5 * attacker.State.GetHashCode());
                    break;
                case Orientations.Left:
                    attacker.Hflip = true;
                    attacker.SetAnimation(2 + 5 * attacker.State.GetHashCode());
                    break;
                case Orientations.Up_Right:
                    attacker.Hflip = false;
                    attacker.SetAnimation(3 + 5 * attacker.State.GetHashCode());
                    break;
                case Orientations.Up_Left:
                    attacker.Hflip = true;
                    attacker.SetAnimation(3 + 5 * attacker.State.GetHashCode());
                    break;
                case Orientations.Up:
                    attacker.SetAnimation(4 + 5 * attacker.State.GetHashCode());
                    break;
            }

            return attacker.FrameCount();
        }

        /// <summary>
        /// Applies damage to target based on attacker
        /// </summary>
        /// <param name="attacker">The attacking object</param>
        /// <param name="target">The target of the attack</param>
        /// <param name="baseDamage">The attack's base damage</param>
        private static void Damage(OurSprite attacker, OurSprite target, int baseDamage = 0)
        {
            int damage = baseDamage;
            foreach (StatusEffect e in attacker.StatusEffects)
            {
                //add effect of elemental buffs
                if (e is ElementalBuff)
                {
                    damage += 2 * baseDamage;
                }
                /// \todo fill this out better
            }
            target.Health -= damage;
        }

        /// <summary>
        /// Performs Melee Attack
        /// </summary>
        /// <returns>returns true when finished</returns>
        public static IEnumerable<bool> Melee(Sprite _target, OurSprite _attacker, int baseDamage, int attackFrame, int attackRange, TimeSpan _minAttackTime)
        {
            OurSprite target = (OurSprite)_target;
            OurSprite attacker = _attacker;
            bool hasAttacked = false;
            TimeSpan minAttackTime = _minAttackTime;
            TimeSpan attackStartTime = This.gameTime.TotalGameTime;

            attacker.State = SpriteState.Attacking;
            int FrameCount = setAnimationReturnFrameCount(attacker);
                
            attacker.Rewind();

            attacker.isMovingAllowed = false;

            bool isLoopOne = false;
            do
            {
                attacker.State = SpriteState.Attacking;
                setAnimationReturnFrameCount(attacker);

                if (attacker.Frame == 0)
                    isLoopOne = !isLoopOne;

                if (!isLoopOne)
                    break;

                if (target != null)
                {
                    Vector2 dirToEnemy = (target.GroundPos - attacker.GroundPos);
                    dirToEnemy.Normalize();

                    if (!hasAttacked &&
                        attacker.Frame == attackFrame &&
                        Vector2.DistanceSquared(target.GroundPos, attacker.GroundPos) < attackRange * attackRange &&
                        Math.Abs(Math.Acos(Vector2.Dot(attacker.Direction, dirToEnemy))) <= Math.PI / 3)
                    {
                        Damage(attacker, target, baseDamage);
                        hasAttacked = true;
                    }
                }
                yield return false;
            } while (isLoopOne);


            attacker.State = SpriteState.Idle;
            setAnimationReturnFrameCount(attacker);

            //wait until minimum amount of time has passed
            while ((This.gameTime.TotalGameTime - attackStartTime) < minAttackTime)
            {
                yield return false;
            }

            attacker.isMovingAllowed = true;

            yield return true;
        }
        
        /// <summary>
        /// Performs Magic Tier 1 Attack
        /// </summary>
        /// <param name="_target">The target for the projectile to attack</param>
        /// <param name="_attacker">The sprite initiating the attack</param>
        /// <param name="_baseDamage">The amount of damage to inflict before constant multiplier for weakness</param>
        /// <param name="_attackFrame">The frame that the attack begins on</param>
        /// <param name="_attackEndTime">The time at which the magic attack should timeout</param>
        /// <param name="_minAttackTime">The minimum time that the attack can take</param>
        /// <param name="_attackRange">The distance from the target that the projectile must come within to be considered a hit</param>
        /// <param name="_projectileSpeed">The speed of the projectile</param>
        /// <param name="_trailLength">The length of the trailing particles as a function of the projectile speed</param>
        /// <returns>Returns true when finished</returns>
        public static IEnumerable<bool> T1Projectile(Sprite _target, OurSprite attacker, int baseDamage, int attackFrame, TimeSpan attackEndTime, TimeSpan minAttackTime, int attackRange, float projectileSpeed, bool isHoming, CreateParticles createParticles, ParticleEmitter _particleEmitter, Element elem = Element.Normal)
        {
            #region Variables
            OurSprite target = (OurSprite)_target;
            Vector2 initialDirection = attacker.Direction;
            attacker.State = SpriteState.Attacking;
            int FrameCount = setAnimationReturnFrameCount(attacker);
            TimeSpan attackStartTime = This.gameTime.TotalGameTime;
            Vector2 direction = new Vector2();
            Tuple<Vector2, Vector2> closestObject = new Tuple<Vector2,Vector2>(new Vector2(), new Vector2());
            Vector2 closestIntersection = new Vector2();
            ParticleEmitter particleEmitter = _particleEmitter;

            bool damageDealt = false;
            #endregion Variables

            particleEmitter.GroundPos = attacker.GroundPos;
            
            attacker.Rewind();

            attacker.isMovingAllowed = false;

            #region Shoot Tier 1 at attackFrame
            while (attacker.Frame < FrameCount)
            {
                if(target != null)
                    attacker.Direction = target.GroundPos - particleEmitter.GroundPos;
                attacker.State = SpriteState.Attacking;
                setAnimationReturnFrameCount(attacker);

                if (attacker.Frame == attackFrame)
                {
                    direction = attacker.Direction;
                    direction.Normalize();
                    attackStartTime = This.gameTime.TotalGameTime;
                    break;
                }

                yield return false;
            }
            #endregion Shoot Tier 1 at attackFrame

            #region Emit Particles until particle hits target or wall or time to live runs out
            while ((This.gameTime.TotalGameTime - attackStartTime) < attackEndTime)
            {
                if (isHoming && target != null)
                {
                    direction = target.GroundPos - particleEmitter.GroundPos;
                    direction.Normalize();
                }

                if (Collision.CollisionData.Count > 0)
                {
                    List<Tuple<CollisionObject, WorldObject, CollisionObject>> collidedWith;
                    Collision.CollisionData.TryGetValue(particleEmitter, out collidedWith);
                    if (collidedWith != null)
                    {
                        foreach (Tuple<CollisionObject, WorldObject, CollisionObject> detectedCollision in collidedWith)
                        {
                            if (((detectedCollision.Item2 is Enemy) && (attacker is Player)) || ((detectedCollision.Item2 is Player) && (attacker is Enemy)))
                            {
                                Damage(attacker, (detectedCollision.Item2 as OurSprite), baseDamage);
                                damageDealt = true;
                                break;
                            }
                            else if ((detectedCollision.Item2 is Player) && (attacker is Player) && (attacker as Player).currentTarget == detectedCollision.Item2)
                            {
                                Player p = (detectedCollision.Item2 as Player);
                                p.StatusEffects.Add(new ElementalBuff(elem));
                                damageDealt=true;
                                break;
                            }
                        }
                    }
                }

                if (damageDealt)
                {
                    break;
                }
                
                //if the attack frame has passed then allow the attacker to move
                if (attacker.Frame >= FrameCount - 1)
                    attacker.isMovingAllowed = true;

                //make sure magic cannot go through walls
                Vector2 previousPosition = particleEmitter.GroundPos;
                particleEmitter.GroundPos += direction * projectileSpeed;
                attacker.detectBackgroundCollisions(particleEmitter.GroundPos, previousPosition, out closestObject, out closestIntersection);
                if (Vector2.DistanceSquared(previousPosition, closestIntersection) <= Vector2.DistanceSquared(previousPosition, particleEmitter.GroundPos))
                {
                    break;
                }

                createParticles(attacker, direction, projectileSpeed, particleEmitter);

                yield return false;
            }
            #endregion Emit Particles until particle hits target or wall or time to live runs out

            attacker.isMovingAllowed = true;

            #region Finish attacking after all particles are dead
            while (particleEmitter.ActiveParticleCount > 0)
            {
                yield return false;
            }
            #endregion Finish attacking after all particles are dead

            particleEmitter.Remove();
            This.Game.CurrentLevel.RemoveSprite(particleEmitter);
            attacker.particleEmitters.Remove(particleEmitter);

            attacker.State = SpriteState.Idle;
            setAnimationReturnFrameCount(attacker);

            while ((This.gameTime.TotalGameTime - attackStartTime) < minAttackTime)
            {
                yield return false;
            }

            yield return true;
        }

    }
}
