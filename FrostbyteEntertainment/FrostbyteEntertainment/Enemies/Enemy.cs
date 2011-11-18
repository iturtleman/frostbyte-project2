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
        internal TimeSpan attackStartTime = TimeSpan.Zero;

        //protected EnemyStatus Status = EnemyStatus.Wander;
        internal IPersonality Personality;
        internal string MovementAudioName;
        internal ProgressBar healthBar;
        private static Vector2 barSize = new Vector2(50, 10);
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
            (This.Game.LoadingLevel as FrostbyteLevel).enemies.Add(this);
            EndBehavior = die;
            Speed = speed;
            Health = _health;
            CollidesWithBackground = true;

            #region HealthBar
            healthBar = new ProgressBar("Health_" + Name, MaxHealth,
                Color.DarkRed, Color.Firebrick, Color.Black, barSize);
            healthBar.Pos = Pos - (This.Game.CurrentLevel as FrostbyteLevel).Camera.Pos + new Vector2(0, -20);
            healthBar.Static = true;
            healthBar.Value = MaxHealth;

            HealthChanged += delegate(object obj, int value)
            {
                healthBar.Value = value;
                if (value == 0)
                {
                    This.Game.CurrentLevel.RemoveSprite(healthBar);
                }
            };
            #endregion
        }

        private void die()
        {
            // Remove Sprite
            This.Game.CurrentLevel.RemoveSprite(this);

            // Remove enemy from target list so that we don't target blank space where an enemy died
            (This.Game.CurrentLevel as FrostbyteLevel).enemies.Remove(this);

        }

        public void update()
        {
            //necessary for collision
            if(this.CollidesWithBackground)
                previousFootPos = this.GroundPos;

            if (isAttackAnimDone)
            {
                PreviousPos = Pos;
                updateMovement();

                //perform collision detection with background
                if (this.CollidesWithBackground)
                    checkBackgroundCollisions();

                #region update animation facing direction
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
                #endregion

                var anim = healthBar.GetAnimation();
                healthBar.Pos = GroundPos - (This.Game.CurrentLevel as FrostbyteLevel).Camera.Pos + new Vector2(Center.X - healthBar.Width / 2, -(healthBar.Height+GetAnimation().Height/2));

            }

            updateAttack();


            if (Health <= 0)
            {
                this.EndBehavior();
                return;
            }
            List<Sprite> targets = (This.Game.CurrentLevel as FrostbyteLevel).allies;
            if (MovementAudioName != null)
            {
                if (GetClosestTarget(targets, This.Game.GraphicsDevice.Viewport.Width * 1.5f) != null)
                {
                    if (PreviousPos != Pos)
                    {
                        This.Game.AudioManager.PlayLoopingSoundEffect(MovementAudioName);
                    }
                }
            }
            State = PreviousPos == Pos ? SpriteState.Idle : SpriteState.Moving;
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

    internal abstract partial class Boss : Enemy
    {
        internal Boss(string name, Actor actor, float speed, int health)
            : base(name, actor, speed, health)
        {
            Enabled = true;
        }

        internal bool Enabled;

        internal override void Update()
        {
            if (Enabled)
            {
                base.Update();
            }
        }
    }
}
