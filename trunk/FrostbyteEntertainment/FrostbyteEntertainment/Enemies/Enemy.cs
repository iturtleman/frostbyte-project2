using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;
using System.Xml.Linq;
using Microsoft.Xna.Framework.Audio;

namespace Frostbyte
{
    internal abstract partial class Enemy : OurSprite
    {
        #region Variables
        //Movement
        //protected enum movementTypes { Charge, PulseCharge, Ram, StealthCharge, StealthCamp, StealthRetreat, Retreat, TeaseRetreat, Swap, Freeze };
        //protected movementTypes currentMovementType = 0;
        internal TimeSpan movementStartTime = TimeSpan.MaxValue;

        //protected EnemyStatus Status = EnemyStatus.Wander;
        internal IPersonality Personality;
        internal SoundEffectInstance MovementAudio = null;
        #endregion Variables

        #region Properties

        //Elemental Properties
        protected Element ElementType { get; set; }
        #endregion


        public Enemy(string name, Actor actor, float speed, int _health)
            : base(name, actor)
        {
            Personality = new WanderingMinstrelPersonality(this);
            UpdateBehavior = update;
            (This.Game.CurrentLevel as FrostbyteLevel).enemies.Add(this);
            EndBehavior = die;
            Speed = speed;
            Health = _health;
            CollidesWithBackground = true;
        }

        private void die()
        {
            This.Game.CurrentLevel.RemoveSprite(this);
        }

        public void update()
        {
            //necessary for collision
            if(this.CollidesWithBackground)
                previousFootPos = this.GroundPos;

            if (isMovingAllowed)
            {
                updateMovement();

                //perform collision detection with background
                if (this.CollidesWithBackground)
                    checkBackgroundCollisions();

                //update animation facing direction
                switch (Orientation)
                {
                    case Orientations.Down:
                        SetAnimation(0 + 5 * State.GetHashCode());
                        break;
                    case Orientations.Down_Right:
                        Hflip = false;
                        SetAnimation(1 + 5 * State.GetHashCode());
                        break;
                    case Orientations.Down_Left:
                        Hflip = true;
                        SetAnimation(1 + 5 * State.GetHashCode());
                        break;
                    case Orientations.Right:
                        Hflip = false;
                        SetAnimation(2 + 5 * State.GetHashCode());
                        break;
                    case Orientations.Left:
                        Hflip = true;
                        SetAnimation(2 + 5 * State.GetHashCode());
                        break;
                    case Orientations.Up_Right:
                        Hflip = false;
                        SetAnimation(3 + 5 * State.GetHashCode());
                        break;
                    case Orientations.Up_Left:
                        Hflip = true;
                        SetAnimation(3 + 5 * State.GetHashCode());
                        break;
                    case Orientations.Up:
                        SetAnimation(4 + 5 * State.GetHashCode());
                        break;
                }
            }

            updateAttack();

            if (Health <= 0)
            {
                this.EndBehavior();
                return;
            }
        }

        /// \todo what is this for?
        //private override void checkBackgroundCollisions()
        //{
        //    //throw new NotImplementedException();
        //}
        protected abstract void updateMovement();
        protected abstract void updateAttack();

        /// \todo create projectile class (projectiles modify health of enemies/players) 
        /// \todo complete checkBackgroundCollisions
    }
}
