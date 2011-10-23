﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Collections;

namespace Frostbyte
{
    internal enum EnemyStatus
    {
        Wander,
        Ram,
        Charge,
        Stealth,
        Frozen,
        Attack
    }

    internal interface IPersonality
    {
        EnemyStatus Status { get; set; }
        void Update();
    }

    internal class ImmobilePersonality : IPersonality
    {
        public EnemyStatus Status { get; set; }
        public void Update()
        {
            // Do Nothing
        }
    }

    internal class WanderingMinstrelPersonality : IPersonality
    {
        public EnemyStatus Status { get; set; }
        private Enemy master;
        private IEnumerator mStates;

        internal WanderingMinstrelPersonality(Enemy master)
        {
            this.master = master;
            mStates = States().GetEnumerator();
        }

        public void Update()
        {
            mStates.MoveNext();
        }

        public IEnumerable States()
        {
            List<Sprite> targets = This.Game.CurrentLevel.GetSpritesByType(typeof(Player));
            float[] transitionDistances = new float[1] { 50f };
            while (true)
            {
                while (!EnemyAI.wander(master, targets, TimeSpan.MaxValue, transitionDistances[0], (float)Math.PI / 8))
                {
                    yield return null;
                }
                while (!EnemyAI.camp(master, targets, 0f, transitionDistances[0]))
                {
                    yield return null;
                }
            }
        }
    }

    internal class AmbushPersonality : IPersonality
    {
        public EnemyStatus Status { get; set; }
        private Enemy master;
        private IEnumerator mStates;

        internal AmbushPersonality(Enemy master)
        {
            this.master = master;
            mStates = States().GetEnumerator();
        }

        public void Update()
        {
            mStates.MoveNext();
        }

        public IEnumerable States()
        {
            List<Sprite> targets = This.Game.CurrentLevel.GetSpritesByType(typeof(Player));
            float[] distances = new float[3] { 1000f, 150f, 100f };
            while (true)
            {
                while (!master.stealthCharge(targets, TimeSpan.MaxValue, distances[1], distances[0], 1f))
                {
                    yield return null;
                }
                while (!master.stealthCamp(targets, distances[2], distances[1]))
                {
                    yield return null;
                }
                while (!master.charge(targets, distances[2], 2))
                {
                    yield return null;
                }
            }
        }
    }

    internal class DontGetNearMePersonality : IPersonality
    {
        public EnemyStatus Status { get; set; }
        private Enemy master;
        private IEnumerator mStates;

        internal DontGetNearMePersonality(Enemy master)
        {
            this.master = master;
            mStates = States().GetEnumerator();
        }

        public void Update()
        {
            mStates.MoveNext();
        }

        public IEnumerable States()
        {
            List<Sprite> targets = This.Game.CurrentLevel.GetSpritesByType(typeof(Player));
            while (true)
            {
                if ( !master.charge(targets, 200.0f, 3.0f) )
                {
                    yield return null;
                }

                if ( !master.charge(targets, float.PositiveInfinity, 0.5f) ) 
                {
                    yield return null;
                }
            }
        }
    }

    internal class DartPersonality : IPersonality
    {
        public EnemyStatus Status { get; set; }
        private Enemy master;
        private IEnumerator mStates;

        internal DartPersonality(Enemy master)
        {
            this.master = master;
            mStates = States().GetEnumerator();
        }

        public void Update()
        {
            mStates.MoveNext();
        }

        public IEnumerable States()
        {
            List<Sprite> targets = This.Game.CurrentLevel.GetSpritesByType(typeof(Player));
            
            while (true)
            {
                TimeSpan snapshot = This.gameTime.TotalGameTime;
                //master.Personality.Status = EnemyStatus.Wander;
                while (!master.dart(targets, 5.0f, 400))
                {   
                    yield return null;
                }

                // Freeze for five seconds
                while (!master.freeze(new TimeSpan(0, 0, 0, 0, 300)))
                {
                    yield return null;
                }
            }

            //}
        }
    }

    internal class CowardlyPersonality : IPersonality
    {
         public EnemyStatus Status { get; set; }
        private Enemy master;
        private IEnumerator mStates;

        internal CowardlyPersonality(Enemy master)
        {
            this.master = master;
            mStates = States().GetEnumerator();
        }

        public void Update()
        {
            mStates.MoveNext();
        }

        public IEnumerable States()
        {
            List<Sprite> targets = This.Game.CurrentLevel.GetSpritesByType(typeof(Player));
            float[] distances = new float[3] { 1000f, 150f, 100f };
            while (true)
            {
                while (!master.charge(targets, distances[0], 5f) && Vector2.Distance(this.master.CenterPos, this.master.GetClosestTarget(targets).CenterPos) > 20)
                {
                    yield return null;
                }
                while (!master.retreat(targets, new TimeSpan(0, 0, 0, 2) , 250f, 15.0f))
                {
                    yield return null;
                }
                //while (!master.charge(targets, distances[2], 2))
                //{
                //    yield return null;
                //}
            }
        }
    }

    internal static class EnemyAI
    {
        //These are only to update position of enemy

        private static Random RNG = new Random();
        private static Vector2 nextHoverPoint = Vector2.Zero;
        private static TimeSpan dartTimeout = new TimeSpan();

        /// <summary>
        /// Update enemy position directly toward target for given duration - complete
        /// </summary>
        internal static bool charge(this Enemy ths, List<Sprite> targets, float aggroDistance, float speedMultiplier)
        {
            Sprite min = ths.GetClosestTarget(targets, aggroDistance);

            if (min == null)  // No targets, so just continue on
            {
                return true;
            }

            float chargeSpeed = ths.Speed * speedMultiplier;
            ths.Direction = min.CenterPos - ths.CenterPos;
            ths.Pos += ths.Direction * chargeSpeed;

            return false;
        }

        /// <summary>
        /// Update enemy position directly toward target with variation of speed (sinusoidal) for given duration - complete
        /// </summary>
        internal static bool pulseCharge(this Enemy ths, List<Sprite> targets, float aggroDistance, float speedMultiplier)
        {
            speedMultiplier = (float)Math.Sin((2 * This.gameTime.TotalGameTime.Milliseconds / 1000.0) * (2 * Math.PI)) + 1.5f;

            return ths.charge(targets, aggroDistance, speedMultiplier);
        }

        /// <summary>
        /// Charge but do not update direction for length of charge - complete
        /// </summary>
        internal static bool ram(this Enemy ths, List<Sprite> targets, TimeSpan duration, float aggroDistance, float speedMultiplier)
        {
            if (ths.Personality.Status != EnemyStatus.Ram)
            {
                ths.movementStartTime = This.gameTime.TotalGameTime;

                Sprite target = ths.GetClosestTarget(targets, aggroDistance);
                if (target != null)
                {
                    ths.Direction = target.CenterPos - ths.CenterPos;
                    ths.Personality.Status = EnemyStatus.Ram;
                }
            }

            float ramSpeed = ths.Speed * speedMultiplier;

            if (ths.Personality.Status == EnemyStatus.Ram)
            {
                if (duration == TimeSpan.MaxValue || This.gameTime.TotalGameTime <= ths.movementStartTime + duration)
                {
                    ths.Pos += ths.Direction * ramSpeed;
                }
                else
                {
                    ths.Personality.Status = EnemyStatus.Wander;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Hide and follow player until certain distance from player - complete
        /// </summary>
        internal static bool stealthCharge(this Enemy ths, List<Sprite> targets, TimeSpan duration, float visibleDistance, float aggroDistance, float speedMultiplier)
        {
            if (ths.Personality.Status != EnemyStatus.Charge)
            {
                ths.movementStartTime = This.gameTime.TotalGameTime;
            }

            Sprite target = ths.GetClosestTarget(targets, aggroDistance);
            if (target != null)
            {
                ths.Personality.Status = EnemyStatus.Charge;
                if (Vector2.DistanceSquared(target.CenterPos, ths.CenterPos) <= visibleDistance * visibleDistance)
                {
                    ths.Personality.Status = EnemyStatus.Wander;
                    ths.Visible = true;
                    return true;
                }
                else
                {
                    ths.Visible = false;
                }
            }

            float chargeSpeed = ths.Speed * speedMultiplier;

            if (ths.Personality.Status == EnemyStatus.Charge)
            {
                if (duration == TimeSpan.MaxValue || This.gameTime.TotalGameTime <= ths.movementStartTime + duration)
                {
                    ths.Direction = target.CenterPos - ths.CenterPos;
                    ths.Pos += ths.Direction * chargeSpeed;
                }
                else
                {
                    ths.Personality.Status = EnemyStatus.Wander;
                    ths.Visible = true;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Be still until a certain distance from a player
        /// </summary>
        internal static bool camp(this Enemy ths, List<Sprite> targets, float aggroDistance, float ignoreDistance)
        {
            Sprite target = ths.GetClosestTarget(targets, aggroDistance);

            if (target != null)
            {
                ths.Personality.Status = EnemyStatus.Wander;
                return true;
            }
            else
            {
                target = ths.GetClosestTarget(targets);
                if (target != null && (Vector2.DistanceSquared(target.CenterPos, ths.CenterPos) >
                    (ignoreDistance * ignoreDistance))){
                        ths.Personality.Status = EnemyStatus.Wander;
                        return true;
                }

                ths.Personality.Status = EnemyStatus.Frozen;
            }

            return false;
        }

        internal static bool retreat(this Enemy ths, List<Sprite> targets, float safeDistance, float speedMultiplier)
        {
            return ths.retreat(targets, TimeSpan.MaxValue, safeDistance, speedMultiplier);
        }

        /// <summary>
        /// Move away until x seconds have passed or you are y distance away
        /// </summary>
        internal static bool retreat(this Enemy ths, List<Sprite> targets, TimeSpan duration, float safeDistance, float speedMultiplier)
        {
            if (ths.Personality.Status != EnemyStatus.Charge)
            {
                ths.movementStartTime = This.gameTime.TotalGameTime;
            }

            Sprite target = ths.GetClosestTarget(targets, safeDistance);
            float fleeSpeed = ths.Speed * speedMultiplier;

            if (target != null)
            {
                ths.Personality.Status = EnemyStatus.Charge;
            }
            else
            {
                ths.Personality.Status = EnemyStatus.Wander;
                return true;
            }

            if (ths.Personality.Status == EnemyStatus.Charge)
            {
                if (This.gameTime.TotalGameTime <= ths.movementStartTime + duration)
                {
                    ths.Direction = target.CenterPos - ths.CenterPos;
                    ths.Pos -= ths.Direction * fleeSpeed;
                }
                else
                {
                    ths.Personality.Status = EnemyStatus.Wander;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Be Invisible and still until certain distance from player
        /// </summary>
        internal static bool stealthCamp(this Enemy ths, List<Sprite> targets, float aggroDistance, float ignoreDistance)
        {
            ths.Visible = camp(ths, targets, aggroDistance, ignoreDistance);
            return ths.Visible;
        }

        /// <summary>
        /// Be Invisible and move away until you are y distance away
        /// </summary>
        internal static bool stealthRetreat(this Enemy ths, List<Sprite> targets, float safeDistance, float speedMultiplier)
        {
            ths.Visible = ths.retreat(targets, safeDistance, speedMultiplier);
            return ths.Visible;
        }

        /// <summary>
        /// Move away when x distance from target until z distance from player
        /// </summary>
        internal static bool teaseRetreat(this Enemy ths, Vector2 P1Coord, Vector2 P2Coord, float aggroDistance, float safeDistance, float speedMultiplier)
        {
            double distToP1 = Vector2.DistanceSquared(P1Coord, ths.CenterPos);
            double distToP2 = Vector2.DistanceSquared(P2Coord, ths.CenterPos);
            float fleeSpeed = ths.Speed * speedMultiplier;
            int playerToFlee = 0;

            // choose which player to run from
            if ((distToP1 <= distToP2) && (distToP1 <= aggroDistance * aggroDistance) || (ths.Personality.Status == EnemyStatus.Charge && (distToP1 < safeDistance * safeDistance)))
            {
                // charge P1
                playerToFlee = 1;
                ths.Personality.Status = EnemyStatus.Charge;

            }

            else if ((distToP2 < distToP1) && (distToP2 <= aggroDistance * aggroDistance) || (ths.Personality.Status == EnemyStatus.Charge && (distToP1 < safeDistance * safeDistance)))
            {
                // charge P2
                playerToFlee = 2;
                ths.Personality.Status = EnemyStatus.Charge;
            }

            else if (Math.Min(distToP1, distToP2) >= safeDistance * safeDistance)
            {
                // isCharging = false;
                // return true;
            }

            if (ths.Personality.Status == EnemyStatus.Charge)
            {

                if ((playerToFlee == 1) && (distToP1 < safeDistance * safeDistance))
                {
                    ths.Direction = P1Coord - ths.CenterPos;
                    ths.Pos -= ths.Direction * fleeSpeed;
                }
                else if ((playerToFlee == 2) && (distToP2 < safeDistance * safeDistance))
                {
                    ths.Direction = P2Coord - ths.CenterPos;
                    ths.Pos -= ths.Direction * fleeSpeed;
                }

            }

            else
            {
                ths.movementStartTime = This.gameTime.TotalGameTime;
            }
            return false;
        }

        /// <summary>
        /// Stop moving for x seconds - complete
        /// </summary>
        internal static bool freeze(this Enemy ths, TimeSpan duration)
        {
            if (ths.Personality.Status != EnemyStatus.Frozen)
            {
                ths.movementStartTime = This.gameTime.TotalGameTime;
                ths.Personality.Status = EnemyStatus.Frozen;
            }

            else if (This.gameTime.TotalGameTime >= ths.movementStartTime + duration)
            {
                ths.Personality.Status = EnemyStatus.Wander;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Wander around for x seconds or until within a certain distance from a target
        /// 
        /// </summary>
        internal static bool wander(this Enemy ths, List<Sprite> targets, TimeSpan duration, float safeDistance, float arcAngle)
        {
            Sprite min = ths.GetClosestTarget(targets, safeDistance);

            if (min != null)  // Near a target, move on to something else
            {
                return true;
            }
            if (RNG.NextDouble() < 0.9)
            {
                double angle = Math.Atan2(ths.Direction.Y, ths.Direction.X) + (2 * RNG.NextDouble() - 1) * arcAngle;
                ths.Direction = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));

                ths.Pos += ths.Direction * ths.Speed / 4;  // Wandering should be *slow*
            }
            return false;
        }

        /// <summary>
        ///  Go quickly to a new location within 100 pixels from the target
        /// </summary
        internal static bool dart(this Enemy ths, List<Sprite> targets, float dartSpeedMultiplier, int flyRadius)
        {
            #region crap!
            //Sprite target = ths.GetClosestTarget(targets);
            //Vector2 nextHoverPoint = Vector2.Zero;

            //float dartSpeed = ths.Speed * dartSpeedMultiplier;


            //if (ths.Personality.Status == EnemyStatus.Wander)
            //{
            //    if (ths.movementStartTime < This.gameTime.TotalGameTime + new TimeSpan(0, 0, 5))
            //        ths.Personality.Status = EnemyStatus.Charge;

            //    nextHoverPoint = new Vector2(
            //           RNG.Next(0, (int)(target.Pos.X)),
            //           RNG.Next(0, (int)(target.Pos.Y))
            //    );

            //    ths.Direction = nextHoverPoint - ths.CenterPos;
            //}

            

            //if (ths.Personality.Status == EnemyStatus.Charge)
            //{
            //    if (ths.Pos != nextHoverPoint && nextHoverPoint != Vector2.Zero)
            //    {
            //        ths.Pos += ths.Direction * dartSpeed;
            //    }

            //    else return true;
            //}
            
            #endregion crap!

            Sprite target = ths.GetClosestTarget(targets);

            if (target == null) return false;

            float dartSpeed = ths.Speed * dartSpeedMultiplier;
            

            if (ths.Personality.Status != EnemyStatus.Charge)
            {
                nextHoverPoint = new Vector2(
                        RNG.Next((int)target.CenterPos.X - flyRadius, (int)target.CenterPos.X + flyRadius),  //(int)(ths.Pos.X + Vector2.Distance(target.Pos, ths.Pos))),
                        RNG.Next((int)target.CenterPos.Y - flyRadius, (int)target.CenterPos.Y + flyRadius)  //(int)(ths.Pos.Y + Vector2.Distance(target.Pos, ths.Pos)))
                );

                ths.Direction = -(ths.Pos - nextHoverPoint);// -ths.CenterPos;
                ths.Direction.Normalize();
                ths.Personality.Status = EnemyStatus.Charge;
                dartTimeout = new TimeSpan(0, 0, 0, 0, 300);
                ths.movementStartTime = This.gameTime.TotalGameTime;
            }

            
            //if we choose a nextHoverPoint thats beyond a wall, we get stuck...
            else if (Vector2.Distance(ths.Pos , nextHoverPoint) > 3f && nextHoverPoint != Vector2.Zero && This.gameTime.TotalGameTime < ths.movementStartTime + dartTimeout)
            {
                ths.Pos += ths.Direction * dartSpeed;
            }

            else 
            {
                ths.Personality.Status = EnemyStatus.Wander;
                nextHoverPoint = new Vector2(
                        RNG.Next((int)target.CenterPos.X - flyRadius, (int)target.CenterPos.X + flyRadius),  //(int)(ths.Pos.X + Vector2.Distance(target.Pos, ths.Pos))),
                        RNG.Next((int)target.CenterPos.Y - flyRadius, (int)target.CenterPos.Y + flyRadius)
                );

                ths.Direction = (ths.Pos - nextHoverPoint);// -ths.CenterPos;
                ths.Direction.Normalize();
                dartTimeout = new TimeSpan(0, 0, 0, 0, 300);
                ths.movementStartTime = This.gameTime.TotalGameTime;
                return true;
            }

            return false;
        }
    }
}