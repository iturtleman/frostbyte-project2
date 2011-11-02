﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;


namespace Frostbyte.Enemies
{
    internal abstract class Enemy : Sprite
    {
        #region Variables

        //State
        public float health;
        
        //Elemental Properties
        protected Element elementType = Element.DEFAULT;

        //Movement
        protected enum movementTypes { Charge, PulseCharge, Ram, StealthCharge, StealthCamp, StealthRetreat, Retreat, TeaseRetreat, Swap, Freeze };
        protected movementTypes currentMovementType = 0;
        protected TimeSpan movementStartTime;
        /// <summary>
        /// \todo make this into an enum with bits or w/e it's called (Dan knows)
        /// </summary>
        protected bool isRamming, isCharging, isStealth=false, isFrozen, isAttacking = false;
        protected Vector2 direction;

        #endregion Variables

        public Enemy(string name, Actor actor, float speed, int _health)
            : base(name, actor) 
        {
            UpdateBehavior = update;
            (This.Game.CurrentLevel as FrostbyteLevel).enemies.Add(this);
            Speed = speed;
            health = _health;
        }

        public void update()
        {
            //(This.Game.CurrentLevel as FrostbyteLevel).TileMap
            updateMovement();
            checkBackgroundCollisions();
            updateAttack();            
        }

        /// \todo what is this for?
        //private override void checkBackgroundCollisions()
        //{
        //    //throw new NotImplementedException();
        //}
        protected abstract void updateMovement();
        protected abstract void updateAttack();

        private Sprite GetClosestTarget(List<Sprite> targets)
        {
            return GetClosestTarget(targets, float.PositiveInfinity);
        }

        /// <summary>
        /// Returns a sprite in targets that is closest to the enemy's current position
        /// and within aggroDistance distance from the current position.
        /// </summary>
        /// <param name="targets"></param>
        /// <param name="aggroDistance"></param>
        /// <returns></returns>
        private Sprite GetClosestTarget(List<Sprite> targets, float aggroDistance)
        {
            Sprite min = null;
            foreach (Sprite target in targets)
            {
                if (target == this)
                {
                    continue;
                }
                if (min == null ||
                    Vector2.DistanceSquared(target.Pos, CenterPos) <
                    Vector2.DistanceSquared(min.Pos, CenterPos))
                {
                    if (Vector2.DistanceSquared(target.Pos, CenterPos) <= aggroDistance * aggroDistance)
                    {
                        min = target;
                    }
                }
            }
            return min;
        }

        #region AI Movements
        //These are only to update position of enemy
        
        /// <summary>
        /// Update enemy position directly toward target for given duration - complete
        /// </summary>
        protected bool charge(List<Sprite> targets, float aggroDistance, float speedMultiplier)
        {
            Sprite min = GetClosestTarget(targets, aggroDistance);

            if (min == null)  // No targets, so just continue on
            {
                return true;
            }

            float chargeSpeed = Speed * speedMultiplier;
            direction = min.Pos - CenterPos;
            direction.Normalize();
            Pos += direction * chargeSpeed;

            return false;
        }

        /// <summary>
        /// Update enemy position directly toward target with variation of speed (sinusoidal) for given duration - complete
        /// </summary>
        protected bool pulseCharge(List<Sprite> targets, float aggroDistance, float speedMultiplier)
        {
            speedMultiplier = (float) Math.Sin( (2 * This.gameTime.TotalGameTime.Milliseconds / 1000.0 ) * (2 * Math.PI) ) + 1.5f;

            return charge(targets, aggroDistance, speedMultiplier);
        }

        /// <summary>
        /// Charge but do not update direction for length of charge - complete
        /// </summary>
        protected bool ram(List<Sprite> targets, TimeSpan duration, float aggroDistance, float speedMultiplier)
        {
            if (!isRamming)
            {
                movementStartTime = This.gameTime.TotalGameTime;

                Sprite target = GetClosestTarget(targets, aggroDistance);
                if (target != null)
                {
                    direction = target.Pos - CenterPos;
                    direction.Normalize();
                    isRamming = true;
                }
            }

            float ramSpeed = Speed * speedMultiplier;

            if (isRamming)
            {
                if (This.gameTime.TotalGameTime <= movementStartTime + duration)
                {
                    Pos += direction * ramSpeed;
                }
                else
                {
                    isRamming = false;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Hide and follow player until certain distance from player - complete
        /// </summary>
        protected bool stealthCharge(List<Sprite> targets, TimeSpan duration, float aggroDistance, float visibleDistance, float speedMultiplier)
        {
            if (!isCharging)
            {
                movementStartTime = This.gameTime.TotalGameTime;
            }

            Sprite target = GetClosestTarget(targets, aggroDistance);
            if (target != null)
            {
                isCharging = true;
                if (Vector2.DistanceSquared(target.Pos, CenterPos) <= visibleDistance * visibleDistance)
                {
                    mVisible = true;
                }
                else
                {
                    mVisible = false;
                }
            }

            float chargeSpeed = Speed * speedMultiplier;

            if (isCharging)
            {
                if (This.gameTime.TotalGameTime <= movementStartTime + duration)
                {
                    direction = target.Pos - CenterPos;
                    direction.Normalize();
                    Pos += direction * chargeSpeed;
                }
                else
                {
                    isCharging = false;
                    mVisible = true;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Be Invisible and still until certain distance from player - complete
        /// </summary>
        protected bool stealthCamp(List<Sprite> targets, float aggroDistance)
        {
            Sprite target = GetClosestTarget(targets, aggroDistance);

            if (target != null)
            {
                isFrozen = false;
                mVisible = true;
                return true;
            }
            else
            {
                isFrozen = true;
                mVisible = false;
            }

            return false;
        }

        /// <summary>
        /// Be Invisible and move away until you are y distance away
        /// </summary>
        protected bool stealthRetreat(List<Sprite> targets, float safeDistance, float speedMultiplier)
        {
            mVisible = retreat(targets, safeDistance, speedMultiplier);
            return mVisible;
        }

        protected bool retreat(List<Sprite> targets, float safeDistance, float speedMultiplier)
        {
            return retreat(targets, TimeSpan.MaxValue, safeDistance, speedMultiplier);
        }

        /// <summary>
        /// Move away until x seconds have passed or you are y distance away
        /// </summary>
        protected bool retreat(List<Sprite> targets, TimeSpan duration, float safeDistance, float speedMultiplier)
        {
            if (!isCharging)
            {
                movementStartTime = This.gameTime.TotalGameTime;
            }

            Sprite target = GetClosestTarget(targets, safeDistance);
            float fleeSpeed = Speed * speedMultiplier;

            if (target != null)
            {
                isCharging = true;
            }
            else
            {
                isCharging = false;
                return true;
            }

            if (isCharging)
            {
                if (This.gameTime.TotalGameTime <= movementStartTime + duration)
                {
                    direction = target.Pos - CenterPos;
                    direction.Normalize();
                    Pos -= direction * fleeSpeed;
                }
                else
                {
                    isCharging = false;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Move away when x distance from target until z distance from player
        /// </summary>
        protected bool teaseRetreat(Vector2 P1Coord, Vector2 P2Coord, float aggroDistance, float safeDistance, float speedMultiplier)
        {
            double distToP1 = Vector2.DistanceSquared(P1Coord, CenterPos);
            double distToP2 = Vector2.DistanceSquared(P2Coord, CenterPos);
            float fleeSpeed = Speed * speedMultiplier;
            int playerToFlee = 0;



            // choose which player to run from
            if ((distToP1 <= distToP2) && (distToP1 <= aggroDistance * aggroDistance) || (isCharging && (distToP1 < safeDistance * safeDistance)))
            {
                // charge P1
                playerToFlee = 1;
                isCharging = true;

            }

            else if ((distToP2 < distToP1) && (distToP2 <= aggroDistance * aggroDistance) || (isCharging && (distToP1 < safeDistance * safeDistance)))
            {
                // charge P2
                playerToFlee = 2;
                isCharging = true;
            }

            else if ( Math.Min(distToP1, distToP2) >= safeDistance * safeDistance )
            {
               // isCharging = false;
               // return true;
            }

            if (isCharging)
            {

                if ((playerToFlee == 1) && (distToP1 < safeDistance * safeDistance))
                {
                    direction = P1Coord - CenterPos;
                    direction.Normalize();
                    Pos -= direction * fleeSpeed;
                }
                else if ((playerToFlee == 2) && (distToP2 < safeDistance * safeDistance))
                {
                    direction = P2Coord - CenterPos;
                    direction.Normalize();
                    Pos -= direction * fleeSpeed;
                }

            }

            else
            {
                movementStartTime = This.gameTime.TotalGameTime;
            }
            return false;
        }

        /// <summary>
        /// Stop moving for x seconds - complete
        /// </summary>
        protected bool freeze(TimeSpan duration)
        {
            if (!isFrozen)
            {
                movementStartTime = This.gameTime.TotalGameTime;
                isFrozen = true;
            }

            else if (This.gameTime.TotalGameTime >= movementStartTime + duration)
            {
                isFrozen = false;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Wander around for x seconds or until within a certain distance from a target
        /// 
        /// </summary>
        protected bool wander(List<Sprite> targets, TimeSpan duration, float safeDistance, float arcAngle)
        {
            Sprite min = GetClosestTarget(targets, safeDistance);

            if (min != null)  // Near a target, move on to something else
            {
                return true;
            }
            Random r = new Random();
            double angle = Math.Atan2(direction.Y, direction.X) + (2 * r.NextDouble() - 1) * arcAngle;
            direction = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));

            /*float chargeSpeed = Speed;
            direction = min.Pos - CenterPos;
            direction.Normalize();*/
            Pos += direction * Speed / 2;  // Wandering should be *slow*

            return false;
        }

        /// <summary>
        /// Switch Position with the target
        /// </summary>
        protected bool swap(Vector2 P1Coord, Vector2 P2Coord, float aggroDistance)
        {
            double distToP1 = Vector2.DistanceSquared(P1Coord, Pos);
            double distToP2 = Vector2.DistanceSquared(P2Coord, Pos);
            int playerToSwap = 0;

            // choose which player to run from
            if ((distToP1 <= distToP2) && (distToP1 <= aggroDistance * aggroDistance))
            {
                // charge P1
                playerToSwap = 1;

            }

            else if ((distToP2 < distToP1) && (distToP2 <= aggroDistance * aggroDistance))
            {
                // charge P2
                playerToSwap = 2;
            }

            else
            {
                return false;
            }


            if ((playerToSwap == 1) && (distToP1 < aggroDistance * aggroDistance))
            { 
                This.Game.CurrentLevel.GetSpritesByType("Mage")[0].Pos = Pos;
                Pos = P1Coord;
                return true;
            }
            else if ((playerToSwap == 2) && (distToP2 < aggroDistance * aggroDistance))
            {
                This.Game.CurrentLevel.GetSpritesByType("Mage")[1].Pos = Pos;
                Pos = P2Coord;
                return true;
            }

            return false;
            
        }

        #endregion AI Movements

        

        //todo:
        //fill in AI Movement Functions
        //create projectile class (projectiles modify health of enemies/players) 
        //complete checkBackgroundCollisions
    }
}