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
        /// Performs Melee Attack
        /// </summary>
        /// <returns>returns true when finished</returns>
        public static IEnumerable<bool> Melee(Sprite _target, OurSprite _attacker, int _baseDamage, int _attackFrame, int attackRange, TimeSpan _minAttackTime)
        {
            OurSprite target = (OurSprite)_target;
            OurSprite attacker = _attacker;
            int baseDamage = _baseDamage;
            int attackFrame = _attackFrame;
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
                attacker.Direction = target.GroundPos - attacker.GroundPos;
                attacker.State = SpriteState.Attacking;
                setAnimationReturnFrameCount(attacker);

                if (attacker.Frame == 0)
                    isLoopOne = !isLoopOne;

                if (!isLoopOne)
                    break;

                if (attacker.Frame == attackFrame && Vector2.DistanceSquared(target.GroundPos, attacker.GroundPos) < attackRange * attackRange && !hasAttacked)
                {
                    target.Health -= baseDamage;
                    hasAttacked = true;
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
        /// Performs Magic Earth Tier 1 Attack
        /// </summary>
        /// <param name="_target">The target for the projectile to attack</param>
        /// <param name="_attacker">The sprite initiating the attack</param>
        /// <param name="_baseDamage">The amount of damage to inflict before constant multiplier for weakness</param>
        /// <param name="_attackFrame">The frame that the attack begins on</param>
        /// <param name="_attackEndTime">The time at which the magic attack should timeout</param>
        /// <param name="_minAttackTime">The minimum time that the attack can take</param>
        /// <param name="_attackRange">The distance from the target that the projectile must come within to be considered a hit</param>
        /// <param name="_projectileSpeed">The speed of the projectile</param>
        /// <returns>Returns true when finished</returns>
        public static IEnumerable<bool> T1Projectile(Sprite _target, OurSprite _attacker, int _baseDamage, int _attackFrame, TimeSpan _attackEndTime, TimeSpan _minAttackTime, int _attackRange, float _projectileSpeed, bool _isHoming)
        {
            #region Variables
            OurSprite target = (OurSprite)_target;
            OurSprite attacker = _attacker;
            int baseDamage = _baseDamage;
            int attackFrame = _attackFrame;
            attacker.State = SpriteState.Attacking;
            int FrameCount = setAnimationReturnFrameCount(attacker);
            TimeSpan attackStartTime = This.gameTime.TotalGameTime;
            Vector2 direction = new Vector2();
            Tuple<Vector2, Vector2> closestObject = new Tuple<Vector2,Vector2>(new Vector2(), new Vector2());
            Vector2 closestIntersection = new Vector2();
            TimeSpan attackEndTime = _attackEndTime;
            TimeSpan minAttackTime = _minAttackTime;
            int attackRange = _attackRange;  //distance in pixels from target that is considered a hit
            float projectileSpeed = _projectileSpeed;
            bool isHoming = _isHoming;
            #endregion Variables

            attacker.particleEmitter.GroundPos = attacker.GroundPos;
            
            attacker.Rewind();

            attacker.isMovingAllowed = false;

            //shoot Earth tier 1
            while (attacker.Frame < FrameCount)
            {
                attacker.Direction = target.GroundPos - attacker.particleEmitter.GroundPos;
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

            //emmit particles until particle hits target or time to live runs out
            while ((This.gameTime.TotalGameTime - attackStartTime) < attackEndTime)
            {
                if (isHoming)
                {
                    direction = target.GroundPos - attacker.particleEmitter.GroundPos;
                    direction.Normalize();
                }

                if (Vector2.DistanceSquared(target.GroundPos, attacker.particleEmitter.GroundPos) < attackRange * attackRange)
                {
                    target.Health -= baseDamage;
                    break;
                }
                
                //if the attack frame has passed then allow the attacker to move
                if (attacker.Frame >= FrameCount - 1)
                    attacker.isMovingAllowed = true;

                //make sure magic cannot go through walls
                Vector2 previousPosition = attacker.particleEmitter.GroundPos;
                attacker.particleEmitter.GroundPos += direction * projectileSpeed;
                attacker.detectBackgroundCollisions(attacker.particleEmitter.GroundPos, previousPosition, out closestObject, out closestIntersection);
                if (Vector2.DistanceSquared(previousPosition, closestIntersection) <= Vector2.DistanceSquared(previousPosition, attacker.particleEmitter.GroundPos))
                {
                    break;
                }


                attacker.particleEmitter.Update();
                attacker.particleEmitter.createParticles(direction*projectileSpeed, Vector2.Zero, attacker.particleEmitter.GroundPos, 10, 10);
                Vector2 tangent = new Vector2(-direction.Y, direction.X);
                for (int i = -5; i < 6; i++)
                {
                    attacker.particleEmitter.createParticles(-direction * projectileSpeed * 5,
                                                             tangent * -i * 40,
                                                             attacker.particleEmitter.GroundPos + tangent*i*1.7f,// + -direction*i*2,
                                                             1.5f,
                                                             300);
                }

                yield return false;
            }

            attacker.isMovingAllowed = true;

            //finished attacking after all particles are dead
            while (attacker.particleEmitter.ActiveParticleCount > 0)
            {
                attacker.particleEmitter.Update();
                yield return false;
            }

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
