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
        private static int setAttackAnimation(OurSprite attacker)
        {
            attacker.State = SpriteState.Attacking;

            int FrameCount = 0;

            switch (attacker.Orientation)
            {
                case Orientations.Down:
                    FrameCount = attacker.FrameCount(0 + 5 * attacker.State.GetHashCode());
                    attacker.SetAnimation(0 + 5 * attacker.State.GetHashCode());
                    break;
                case Orientations.Down_Right:
                    attacker.Hflip = false;
                    FrameCount = attacker.FrameCount(1 + 5 * attacker.State.GetHashCode());
                    attacker.SetAnimation(1 + 5 * attacker.State.GetHashCode());
                    break;
                case Orientations.Down_Left:
                    attacker.Hflip = true;
                    FrameCount = attacker.FrameCount(1 + 5 * attacker.State.GetHashCode());
                    attacker.SetAnimation(1 + 5 * attacker.State.GetHashCode());
                    break;
                case Orientations.Right:
                    attacker.Hflip = false;
                    FrameCount = attacker.FrameCount(2 + 5 * attacker.State.GetHashCode());
                    attacker.SetAnimation(2 + 5 * attacker.State.GetHashCode());
                    break;
                case Orientations.Left:
                    attacker.Hflip = true;
                    FrameCount = attacker.FrameCount(2 + 5 * attacker.State.GetHashCode());
                    attacker.SetAnimation(2 + 5 * attacker.State.GetHashCode());
                    break;
                case Orientations.Up_Right:
                    attacker.Hflip = false;
                    FrameCount = attacker.FrameCount(3 + 5 * attacker.State.GetHashCode());
                    attacker.SetAnimation(3 + 5 * attacker.State.GetHashCode());
                    break;
                case Orientations.Up_Left:
                    attacker.Hflip = true;
                    FrameCount = attacker.FrameCount(3 + 5 * attacker.State.GetHashCode());
                    attacker.SetAnimation(3 + 5 * attacker.State.GetHashCode());
                    break;
                case Orientations.Up:
                    FrameCount = attacker.FrameCount(4 + 5 * attacker.State.GetHashCode());
                    attacker.SetAnimation(4 + 5 * attacker.State.GetHashCode());
                    break;
            }

            return FrameCount;
        }


        /// <summary>
        /// Performs Melee Attack
        /// </summary>
        /// <returns>returns true when finished</returns>
        public static IEnumerable<bool> Melee(Sprite _target, OurSprite _attacker, int _baseDamage, int _attackFrame)
        {
            OurSprite target = (OurSprite)_target;
            OurSprite attacker = _attacker;
            int baseDamage = _baseDamage;
            int attackFrame = _attackFrame;
            int FrameCount = setAttackAnimation(attacker);
            bool hasAttacked = false;

            attacker.Rewind();

            attacker.isMovingAllowed = false;

            while (attacker.Frame <= FrameCount - 2)
            {
                if (attacker.Frame == attackFrame && Vector2.DistanceSquared(target.GroundPos, attacker.GroundPos) < attacker.AttackRange * attacker.AttackRange + 50*50 && !hasAttacked)
                {
                    target.Health -= baseDamage;
                    hasAttacked = true;
                }

                yield return false;
            }

            attacker.State = SpriteState.Idle;

            attacker.isMovingAllowed = true;

            yield return true;
        }

        /// <summary>
        /// Performs Magic Earth Tier 1 Attack
        /// </summary>
        /// <returns>returns true when finished</returns>
        public static IEnumerable<bool> EarthT1(Sprite _target, OurSprite _attacker, int _baseDamage, int _attackFrame, Element _weakness)
        {
            #region Variables (Do not Change)
            OurSprite target = (OurSprite)_target;
            OurSprite attacker = _attacker;
            int baseDamage = _baseDamage;
            int attackFrame = _attackFrame;
            Element weakness = _weakness;
            int FrameCount = setAttackAnimation(attacker);
            TimeSpan attackStartTime = This.gameTime.TotalGameTime;
            Vector2 direction = new Vector2();
            Tuple<Vector2, Vector2> closestObject = new Tuple<Vector2,Vector2>(new Vector2(), new Vector2());
            Vector2 closestIntersection = new Vector2();
            #endregion Variables (Do not Change)

            #region Variables (Change)
            //Adjust These
            TimeSpan attackEndTime = new TimeSpan(0,0,0,1,750);
            int hitDistance = 20;  //distance in pixels from target that is considered a hit
            float projectileSpeed = 6f;
            #endregion Variables (Change)

            attacker.particleEmitter.CenterPos = attacker.GroundPos;
            
            attacker.Rewind();

            attacker.isMovingAllowed = false;

            //shoot Earth tier 1
            while (attacker.Frame <= FrameCount - 2)
            {
                if (attacker.Frame == attackFrame)
                {
                    attackStartTime = This.gameTime.TotalGameTime;
                    direction = target.GroundPos - attacker.particleEmitter.CenterPos;
                    direction.Normalize();
                    break;
                }

                yield return false;
            }

            //emmit particles until particle hits target or time to live runs out
            while ((This.gameTime.TotalGameTime - attackStartTime) < attackEndTime)
            {
                if (Vector2.DistanceSquared(target.GroundPos, attacker.particleEmitter.CenterPos) < hitDistance * hitDistance)
                {
                    target.Health -= baseDamage;
                    break;
                }
                
                //if the attack frame has passed then allow the attacker to move
                if (attacker.Frame >= FrameCount - 2)
                    attacker.isMovingAllowed = true;

                //make sure magic cannot go through walls
                Vector2 previousPosition = attacker.particleEmitter.CenterPos;
                attacker.particleEmitter.CenterPos += direction * projectileSpeed;
                attacker.detectBackgroundCollisions(attacker.particleEmitter.CenterPos, previousPosition, out closestObject, out closestIntersection);
                if (Vector2.DistanceSquared(previousPosition, closestIntersection) <= Vector2.DistanceSquared(previousPosition, attacker.CenterPos))
                {
                    break;
                }


                attacker.particleEmitter.Update();
                attacker.particleEmitter.createParticles(direction*projectileSpeed, Vector2.Zero, attacker.particleEmitter.CenterPos, 10, 10);

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
            yield return true;
        }




    }
}
