﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Collections;
using Microsoft.Xna.Framework.Graphics;

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
            List<Sprite> targets = (This.Game.CurrentLevel as FrostbyteLevel).allies;
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

                yield return null;
            }
        }
    }

    internal class PseudoWanderPersonality : IPersonality
    {
        public EnemyStatus Status { get; set; }
        private Enemy master;
        private IEnumerator mStates;

        internal PseudoWanderPersonality(Enemy master)
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
            List<Sprite> targets = (This.Game.CurrentLevel as FrostbyteLevel).allies;
            float[] transitionDistances = new float[1] { 0f };
            while (true)
            {
                Sprite closestTarget = master.GetClosestTarget(targets, 500.0f);
                
                if(closestTarget != null && Vector2.DistanceSquared(master.GroundPos, closestTarget.GroundPos) <= 500*500)
                {
                    EnemyAI.rangeWander(master, targets, TimeSpan.MaxValue, (float)Math.PI / 8, 400f);
                }

                yield return null;
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
            List<Sprite> targets = (This.Game.CurrentLevel as FrostbyteLevel).allies;
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

                yield return null;
            }
        }
    }

    internal class SentinelPersonality : IPersonality
    {
        public EnemyStatus Status { get; set; }
        private Enemy master;
        private IEnumerator mStates;

        internal SentinelPersonality(Enemy master)
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
            List<Sprite> targets = (This.Game.CurrentLevel as FrostbyteLevel).allies;
            Vector2 guardPosition = master.GroundPos;
            while (true)
            {
                Sprite closestTarget = master.GetClosestTarget(targets, 300.0f);


                if (closestTarget != null && Vector2.DistanceSquared(closestTarget.GroundPos, master.GroundPos) > 200 * 200)
                {
                    master.charge(closestTarget.GroundPos, 1.4f);
                }
                else if (closestTarget != null)
                {
                    master.charge(closestTarget.GroundPos, 3.0f);
                }
                else if (closestTarget == null && Vector2.DistanceSquared(guardPosition, master.GroundPos) > 20 * 20)
                {
                    master.charge(guardPosition, 3.0f);
                }
                else if (closestTarget == null && Vector2.DistanceSquared(guardPosition, master.GroundPos) <= 20 * 20)
                {
                    master.State = SpriteState.Idle;
                }

                yield return null;
            }
        }
    }

    internal class StrictSentinelPersonality : IPersonality
    {
        public EnemyStatus Status { get; set; }
        private Enemy master;
        private IEnumerator mStates;

        internal StrictSentinelPersonality(Enemy master)
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
            List<Sprite> targets = (This.Game.CurrentLevel as FrostbyteLevel).allies;
            Vector2 guardPosition = master.GroundPos;
            while (true)
            {
                if (Vector2.DistanceSquared(guardPosition, master.GroundPos) <= 20 * 20)
                {
                    master.State = SpriteState.Idle;
                }

                yield return null;
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
            List<Sprite> targets = (This.Game.CurrentLevel as FrostbyteLevel).allies;
            while (true)
            {
                TimeSpan snapshot = This.gameTime.TotalGameTime;
                //master.Personality.Status = EnemyStatus.Wander;
                Sprite closestTarget = this.master.GetClosestTarget(targets);
                while (!master.dart(targets, 5.0f, 400) && closestTarget != null &&
                    Vector2.Distance(this.master.GroundPos, closestTarget.GroundPos) < 500)
                {
                    yield return null;
                    closestTarget = this.master.GetClosestTarget(targets);
                }

                // Freeze for five seconds
                while (!master.freeze(new TimeSpan(0, 0, 0, 0, 300)))
                {
                    yield return null;
                }

                yield return null;
            }

            //}
        }
    }

    internal class PulseChargePersonality : IPersonality
    {
        public EnemyStatus Status { get; set; }
        private Enemy master;
        private IEnumerator mStates;

        internal PulseChargePersonality(Enemy master)
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
            List<Sprite> targets = (This.Game.CurrentLevel as FrostbyteLevel).allies;
            while (true)
            {
                TimeSpan snapshot = This.gameTime.TotalGameTime;
                while (!master.pulseCharge(targets, 500, 3.2f))
                {
                    yield return null;
                }

                yield return null;
            }
        }
    }

    internal class ChargePersonality : IPersonality
    {
        public EnemyStatus Status { get; set; }
        private Enemy master;
        private IEnumerator mStates;

        internal ChargePersonality(Enemy master)
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
            List<Sprite> targets = (This.Game.CurrentLevel as FrostbyteLevel).allies;
            while (true)
            {
                master.charge(targets, 500, 2.94f);
                yield return null;
            }
        }
    }

    #region Bosses
    internal class DiePersonality : IPersonality
    {
        public EnemyStatus Status { get; set; }

        private Boss master;
        private int deathAnimation;
        private IEnumerator mStates;

        internal DiePersonality(Boss master, int DeathAnimation)
        {
            this.master = master;
            deathAnimation = DeathAnimation;
            mStates = States().GetEnumerator();
        }

        public void Update()
        {
            mStates.MoveNext();
        }

        public IEnumerable States()
        {
            master.Rewind();
            while (master.Frame != master.FrameCount() - 1)
            {
                master.SetAnimation(deathAnimation);
                yield return null;
            }
            master.StopAnim();
            while (true)
            {
                yield return null;
            }
        }
    }


    internal class UndergroundAttackPersonality : IPersonality
    {
        public EnemyStatus Status { get; set; }
        private Enemies.Worm master;
        private IEnumerator mStates;

        internal UndergroundAttackPersonality(Enemies.Worm master)
        {
            this.master = master;
            mStates = States().GetEnumerator();
            This.Game.AudioManager.AddSoundEffect("Effects/Worm_Spawn");
            This.Game.AudioManager.AddBackgroundMusic("Music/OldEarthBoss");
        }

        public void Update()
        {
            mStates.MoveNext();
        }

        public IEnumerable Surface()
        {
            This.Game.AudioManager.PlaySoundEffect("Effects/Worm_Spawn", .05f);
            master.Visible = true;
            master.SetAnimation(17);
            master.Rewind();
            while (master.Frame != master.FrameCount() - 1)
            {
                master.SetAnimation(17);
                yield return null;
            }
            master.SetAnimation(0);
            while (!master.freeze(new TimeSpan(0, 0, 1)))
            {
                yield return null;
            }
            master.HasVomited = false;
            master.IsSubmerged = false;
        }

        private IEnumerable Submerge()
        {
            master.IsSubmerged = true;
            master.StopAttacks();
            master.SetAnimation(16);
            master.Rewind();
            while (master.Frame != master.FrameCount() - 1)
            {
                master.SetAnimation(16);
                yield return null;
            }
            master.Visible = false;
            master.Pos = Vector2.Zero;
        }

        public IEnumerable States()
        {
            List<Sprite> targets = (This.Game.CurrentLevel as FrostbyteLevel).allies;

            while (!master.camp(targets, 100, float.PositiveInfinity) && !master.AtArms)
            {
                yield return null;
            }
            master.setAtArms();
            This.Game.AudioManager.PlayBackgroundMusic("Music/OldEarthBoss");
            This.Game.AudioManager.BackgroundMusicVolume = 0.05f;

            foreach (Object o in Surface()) { yield return null; }

            while (true)
            {
                if (master.IsSubmerged)
                {
                    Camera cam = This.Game.CurrentLevel.Camera;
                    Viewport v = This.Game.GraphicsDevice.Viewport;
                    while (!master.delayedTeleport(new TimeSpan(0, 0, 2),
                        new Rectangle(
                            (int)(cam.Pos.X * cam.Zoom),
                            (int)(cam.Pos.Y * cam.Zoom),
                            (int)(v.Width * cam.Zoom),
                            (int)(v.Height * cam.Zoom))))
                    {
                        yield return null;
                    }

                    foreach (Object o in Surface()) { yield return null; }
                }
                else
                {
                    while (!master.freeze(new TimeSpan(0, 0, 5))) { yield return null; }

                    foreach (Object o in Submerge()) { yield return null; }
                }
                yield return null;
            }
        }
    }

    internal class ShiningPersonality : IPersonality
    {
        public EnemyStatus Status { get; set; }
        private Enemies.CrystalMan master;
        private Enemies.Crystal currentCrystal;
        private IEnumerator mStates;
        private static Random rng = new Random();

        private int EMPTY_CRYSTAL { get { return 19; } }

        internal ShiningPersonality(Enemies.CrystalMan master)
        {
            this.master = master;
            mStates = States().GetEnumerator();
        }

        public void Update()
        {
            mStates.MoveNext();
        }

        public IEnumerable TeleportIn()
        {
            currentCrystal.SetAnimation(5);
            currentCrystal.Rewind();
            while (currentCrystal.Frame != currentCrystal.FrameCount() - 1)
            {
                currentCrystal.SetAnimation(5);
                yield return null;
            }
            currentCrystal.Rewind();
            currentCrystal.SetAnimation(0);
            while (!master.freeze(new TimeSpan(0, 0, 1)))
            {
                yield return null;
            }
            master.attackWait = This.gameTime.TotalGameTime + new TimeSpan(0, 0, rng.Next(5));
        }

        public IEnumerable TeleportOut()
        {
            master.attackWait = TimeSpan.MaxValue;
            currentCrystal.Rewind();
            currentCrystal.SetAnimation(6);
            while (currentCrystal.Frame != currentCrystal.FrameCount() - 1)
            {
                currentCrystal.SetAnimation(6);
                yield return null;
            }
            currentCrystal.Rewind();
            currentCrystal.SetAnimation(EMPTY_CRYSTAL);
            while (!master.freeze(new TimeSpan(0, 0, 1)))
            {
                yield return null;
            }
        }

        public IEnumerable States()
        {
            List<Sprite> targets = (This.Game.CurrentLevel as FrostbyteLevel).allies;
            Random rng = new Random();

            while (master.Crystals == null || master.Crystals.Count == 0)
            {
                yield return null;
            }
            currentCrystal = master.Crystals[0];

            int initialWaitDistance = 300;
            while (!master.camp(targets, initialWaitDistance, float.PositiveInfinity) && !master.AtArms)
            {
                foreach (Enemies.Crystal crystal in master.Crystals)
                {
                    currentCrystal.Rewind();
                    crystal.SetAnimation(EMPTY_CRYSTAL);
                }
                yield return null;
            }
            master.setAtArms();
            This.Game.AudioManager.PlayBackgroundMusic("Music/OldEarthBoss");
            This.Game.AudioManager.BackgroundMusicVolume = 0.05f;

            while (true)
            {
                foreach (Object o in TeleportIn())
                {
                    if (currentCrystal.State == SpriteState.Dead)
                    {
                        break;
                    }
                    yield return null;
                }

                while (!master.freeze(new TimeSpan(0, 0, 5)))
                {
                    if (currentCrystal.State == SpriteState.Dead)
                    {
                        break;
                    }
                    yield return null;
                }

                foreach (Object o in TeleportOut())
                {
                    if (currentCrystal.State == SpriteState.Dead)
                    {
                        break;
                    }
                    yield return null;
                }

                if (currentCrystal.State == SpriteState.Dead)
                {
                    master.Crystals.Remove(currentCrystal);
                }

                if (master.Crystals.Count == 0)
                {
                    break;
                }

                currentCrystal = master.Crystals.GetRandomElement();//[rng.Next(master.Crystals.Count)];

                yield return null;
            }

            master.Health = 0;
        }
    }

    #endregion

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

            charge(ths, min.GroundPos, speedMultiplier);

            return false;
        }

        internal static void charge(this Enemy ths, Vector2 targetPos, float speedMultiplier = 1)
        {
            float chargeSpeed = ths.Speed * speedMultiplier;
            ths.GroundPos += ths.Direction * chargeSpeed;
            //This must be set after because determining the animation is dependent on the new position ( I know it's not optimal but I'm not sure where to put it)
            ths.Direction = targetPos - ths.GroundPos;
        }

        /// <summary>
        /// Update enemy position directly toward target with variation of speed (sinusoidal) for given duration - complete
        /// </summary>
        internal static bool pulseCharge(this Enemy ths, List<Sprite> targets, float aggroDistance, float speedMultiplier)
        {
            speedMultiplier = 2*(float)Math.Sin((2 * This.gameTime.TotalGameTime.Milliseconds / 1000.0) * (2 * Math.PI)) + 2.5f;

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
                    ths.Direction = target.GroundPos - ths.GroundPos;
                    ths.Personality.Status = EnemyStatus.Ram;
                }
            }

            float ramSpeed = ths.Speed * speedMultiplier;

            if (ths.Personality.Status == EnemyStatus.Ram)
            {
                if (duration == TimeSpan.MaxValue || This.gameTime.TotalGameTime <= ths.movementStartTime + duration)
                {
                    ths.GroundPos += ths.Direction * ramSpeed;
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
                if (Vector2.DistanceSquared(target.GroundPos, ths.GroundPos) <= visibleDistance * visibleDistance)
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
            else
            {
                return false;
            }

            float chargeSpeed = ths.Speed * speedMultiplier;

            if (ths.Personality.Status == EnemyStatus.Charge)
            {
                if (duration == TimeSpan.MaxValue || This.gameTime.TotalGameTime <= ths.movementStartTime + duration)
                {
                    ths.Direction = target.GroundPos - ths.GroundPos;
                    ths.GroundPos += ths.Direction * chargeSpeed;
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
        /// Be still until a certain distance from a player (between aggroDistance and ignoreDistance)
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
                if (target != null && (Vector2.DistanceSquared(target.GroundPos, ths.GroundPos) >
                    (ignoreDistance * ignoreDistance)))
                {
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
                    ths.Direction = target.GroundPos - ths.GroundPos;
                    ths.GroundPos -= ths.Direction * fleeSpeed;
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
            double distToP1 = Vector2.DistanceSquared(P1Coord, ths.GroundPos);
            double distToP2 = Vector2.DistanceSquared(P2Coord, ths.GroundPos);
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
                    ths.Direction = P1Coord - ths.GroundPos;
                    ths.GroundPos -= ths.Direction * fleeSpeed;
                }
                else if ((playerToFlee == 2) && (distToP2 < safeDistance * safeDistance))
                {
                    ths.Direction = P2Coord - ths.GroundPos;
                    ths.GroundPos -= ths.Direction * fleeSpeed;
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

                ths.GroundPos += ths.Direction * ths.Speed / 4;  // Wandering should be *slow*
            }
            return false;
        }


        /// <summary>
        /// Wander around within a certain range from closest enemy
        /// </summary>
        internal static bool rangeWander(this Enemy ths, List<Sprite> targets, TimeSpan duration, float arcAngle, float wanderRadius)
        {
            Sprite target = ths.GetClosestTarget(targets);
            if (target == null) return false;

            if (Vector2.DistanceSquared(ths.GroundPos, target.GroundPos) >= wanderRadius * wanderRadius)
            {
                ths.Direction = target.GroundPos - ths.GroundPos;
                ths.Direction.Normalize();
                ths.GroundPos += ths.Direction * ths.Speed / 4;
            }


            if (RNG.NextDouble() < 0.9)
            {
                double angle = Math.Atan2(ths.Direction.Y, ths.Direction.X) + (2 * RNG.NextDouble() - 1) * arcAngle;
                ths.Direction = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));

                ths.GroundPos += ths.Direction * ths.Speed / 4;  // Wandering should be *slow*
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

            //    ths.Direction = nextHoverPoint - ths.GroundPos;
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
                        RNG.Next((int)target.GroundPos.X - flyRadius, (int)target.GroundPos.X + flyRadius),  //(int)(ths.Pos.X + Vector2.Distance(target.Pos, ths.Pos))),
                        RNG.Next((int)target.GroundPos.Y - flyRadius, (int)target.GroundPos.Y + flyRadius)  //(int)(ths.Pos.Y + Vector2.Distance(target.Pos, ths.Pos)))
                );

                ths.Direction = -(ths.GroundPos - nextHoverPoint);// -ths.GroundPos;
                ths.Direction.Normalize();
                ths.Personality.Status = EnemyStatus.Charge;
                dartTimeout = new TimeSpan(0, 0, 0, 0, 300);
                ths.movementStartTime = This.gameTime.TotalGameTime;
            }


            //if we choose a nextHoverPoint thats beyond a wall, we get stuck...
            else if (Vector2.Distance(ths.GroundPos, nextHoverPoint) > 3f && nextHoverPoint != Vector2.Zero && This.gameTime.TotalGameTime < ths.movementStartTime + dartTimeout)
            {
                ths.GroundPos += ths.Direction * dartSpeed;
            }

            else
            {
                ths.Personality.Status = EnemyStatus.Wander;
                nextHoverPoint = new Vector2(
                        RNG.Next((int)target.GroundPos.X - flyRadius, (int)target.GroundPos.X + flyRadius),  //(int)(ths.Pos.X + Vector2.Distance(target.Pos, ths.Pos))),
                        RNG.Next((int)target.GroundPos.Y - flyRadius, (int)target.GroundPos.Y + flyRadius)
                );

                ths.Direction = (ths.GroundPos - nextHoverPoint);// -ths.GroundPos;
                ths.Direction.Normalize();
                dartTimeout = new TimeSpan(0, 0, 0, 0, 300);
                ths.movementStartTime = This.gameTime.TotalGameTime;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Move off-screen for `duration` seconds, then teleport to a random point within a rectangle
        /// </summary>
        internal static bool delayedTeleport(this Enemy ths, TimeSpan wait, Rectangle bounds)
        {
            if (ths.Personality.Status != EnemyStatus.Frozen)
            {
                ths.movementStartTime = This.gameTime.TotalGameTime;
                ths.Personality.Status = EnemyStatus.Frozen;
            }

            else if (This.gameTime.TotalGameTime >= ths.movementStartTime + wait)
            {
                ths.Personality.Status = EnemyStatus.Wander;
                ths.GroundPos = new Vector2(bounds.X + RNG.Next(bounds.Width), bounds.Y + RNG.Next(bounds.Height));
                return true;
            }

            return false;
        }
    }
}
