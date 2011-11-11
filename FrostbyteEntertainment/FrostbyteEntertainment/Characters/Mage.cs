using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

namespace Frostbyte.Characters
{
    class Mage : Player
    {
        enum TargetAlignment
        {
            Ally,
            Enemy,
            None
        }

        #region Constructors
        public Mage(string name, Actor actor)
            : this(name, actor, PlayerIndex.One)
        {
            CollidesWithBackground = true;
        }

        internal Mage(string name, Actor actor, PlayerIndex input)
            : base(name, actor)
        {
            if (GamePad.GetState(input).IsConnected)
            {
                controller = new GamePadController(input);
            }
            else
            {
                controller = new KeyboardController();
            }
            currentTargetAlignment = TargetAlignment.None;
            This.Game.LoadingLevel.AddAnimation(new Animation("target.anim"));
            target = new Sprite("target", new Actor(new Animation("target.anim")));//new Target("target", Color.Red);
            target.Visible = false;
            sortType = new DistanceSort(this);

            UpdateBehavior = mUpdate;
            CollidesWithBackground = true;

            This.Game.AudioManager.AddSoundEffect("Effects/Sword_Attack");

            //Create Earth Tier 1 Particle Emmiter
            Effect particleEffect = This.Game.CurrentLevel.GetEffect("ParticleSystem");
            Texture2D boulder = This.Game.CurrentLevel.GetTexture("boulder");
            particleEarthTier1 = new ParticleEmitter(1000, particleEffect, boulder);
            particleEarthTier1.effectTechnique = "NoSpecialEffect";
            particleEarthTier1.blendState = BlendState.AlphaBlend;

            //Create Lightning Tier 1 Particle Emmiter
            Texture2D lightning = This.Game.CurrentLevel.GetTexture("sparkball");
            particleLightningTier1 = new ParticleEmitter(1000, particleEffect, lightning);
            particleLightningTier1.effectTechnique = "NoSpecialEffect";
            particleLightningTier1.blendState = BlendState.Additive;

            //Create Fire Tier 1 Particle Emmiter
            Texture2D fire = This.Game.CurrentLevel.GetTexture("fire");
            particleFireTier1 = new ParticleEmitter(3000, particleEffect, fire);
            particleFireTier1.effectTechnique = "NoSpecialEffect";
            particleFireTier1.blendState = BlendState.AlphaBlend;
        }
        #endregion

        #region Variables
        private Sprite currentTarget = null;
        private TargetAlignment currentTargetAlignment;
        internal IController controller;
        private Sprite target;
        BasicEffect basicEffect = new BasicEffect(This.Game.GraphicsDevice);
        private IComparer<Sprite> sortType;
        private int spellManaCost = 10;

        private ParticleEmitter particleEarthTier1;
        private ParticleEmitter particleLightningTier1;
        private ParticleEmitter particleFireTier1;
        #endregion

        #region Methods
        /// <summary>
        /// Finds the closest enemy sprite to the player that's further than the current target
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private Sprite findMinimum(List<Sprite> list)
        {
            if (list.Contains(this))
            {
                list.Remove(this);
            }
            list.Sort(sortType);

            int next = list.IndexOf(currentTarget);
            for (int x = 0; x < list.Count; x++)
            {
                Sprite target = list[(next + 1 + x) % list.Count];
                if (target.Visible)
                {
                    return target;
                }
            }

            cancelTarget(); // Every sprite in list is invisible, there's nothing to target
            return null;
        }

        private void cancelTarget()
        {
            target.Visible = false;
            currentTarget = null;
            currentTargetAlignment = TargetAlignment.None;
        }

        /// <summary>
        /// Chooses and executes the attack.
        /// Ensures that only one attack is performed per update (eg. no sword *and* magic)
        /// </summary>
        private void attack()
        {
            if (isAttacking)
            {
                mAttack.MoveNext();
                isAttacking = !mAttack.Current;
            }
            else if (isAttackingAllowed)
            {
                if (Mana >= spellManaCost)
                {
                    if (controller.Earth == ReleasableButtonState.Clicked)
                    {
                        #region Earth Tier 1
                        particleEmitter = particleEarthTier1;
                        isAttacking = true;

                        //particle emitter is created in constructor

                        int attackRange = 11;

                        (particleEarthTier1.collisionObjects.First() as Collision_BoundingCircle).Radius = attackRange;
                        (particleEarthTier1.collisionObjects.First() as Collision_BoundingCircle).createDrawPoints();


                        mAttack = Attacks.T1Projectile(currentTarget,
                                                  this,
                                                  20,
                                                  0,
                                                  new TimeSpan(0, 0, 0, 1, 150),
                                                  new TimeSpan(0, 0, 0, 0, 100),
                                                  attackRange,
                                                  9f,
                                                  false,
                                                  delegate(OurSprite attacker, Vector2 direction, float projectileSpeed)
                                                  {
                                                      Random randPosition = new Random();
                                                      attacker.particleEmitter.createParticles(direction * projectileSpeed, Vector2.Zero, attacker.particleEmitter.GroundPos, 10, 10);
                                                      Vector2 tangent = new Vector2(-direction.Y, direction.X);
                                                      for (int i = -5; i < 6; i++)
                                                      {
                                                          attacker.particleEmitter.createParticles(-direction * projectileSpeed * .75f,
                                                                                                   tangent * -i * 40,
                                                                                                   attacker.particleEmitter.GroundPos + tangent * i * 1.7f + (float)randPosition.NextDouble() * direction * 8f,
                                                                                                   1.5f,
                                                                                                   300);
                                                      }
                                                  }
                                                  ).GetEnumerator();





                        Mana -= spellManaCost;
                        #endregion Earth Tier 1
                        return;
                    }
                    else if (controller.Fire == ReleasableButtonState.Clicked)
                    {
                        #region Fire Tier 1
                        particleEmitter = particleFireTier1;
                        isAttacking = true;

                        //particle emitter is created in constructor

                        int attackRange = 11;

                        (particleEarthTier1.collisionObjects.First() as Collision_BoundingCircle).Radius = attackRange;
                        (particleEarthTier1.collisionObjects.First() as Collision_BoundingCircle).createDrawPoints();


                        mAttack = Attacks.T1Projectile(currentTarget,
                                                  this,
                                                  30,
                                                  0,
                                                  new TimeSpan(0, 0, 0, 0, 750),
                                                  new TimeSpan(0, 0, 0, 0, 100),
                                                  attackRange,
                                                  9f,
                                                  true,
                                                  delegate(OurSprite attacker, Vector2 direction, float projectileSpeed)
                                                  {
                                                      Random rand = new Random();
                                                      Vector2 tangent = new Vector2(-direction.Y, direction.X);
                                                      for (int i = -5; i < 6; i++)
                                                      {
                                                          float velocitySpeed = rand.Next(30, 55);
                                                          float accelSpeed = rand.Next(-30, -10);
                                                          attacker.particleEmitter.createParticles(direction*velocitySpeed,
                                                                          direction*accelSpeed,
                                                                          attacker.particleEmitter.GroundPos,
                                                                          rand.Next(5, 20),
                                                                          rand.Next(50, 300));
                                                      }
                                                  }
                                                  ).GetEnumerator();
                        Mana -= spellManaCost;
                        #endregion Fire Tier 1
                        return;
                    }
                    else if (controller.Lightning == ReleasableButtonState.Clicked)
                    {
                        #region Lightning Tier 1
                        particleEmitter = particleLightningTier1;

                        isAttacking = true;

                        //particle emitter is created in constructor

                        int attackRange = 3;

                        (particleEmitter.collisionObjects.First() as Collision_BoundingCircle).Radius = attackRange;
                        (particleEmitter.collisionObjects.First() as Collision_BoundingCircle).createDrawPoints();

                        mAttack = Attacks.T1Projectile(currentTarget,
                                                  this,
                                                  20,
                                                  0,
                                                  new TimeSpan(0, 0, 0, 1, 250),
                                                  new TimeSpan(0, 0, 0, 0, 150),
                                                  attackRange,
                                                  8f,
                                                  true,
                                                  delegate(OurSprite attacker, Vector2 direction, float projectileSpeed)
                                                  {
                                                      Vector2 tangent = new Vector2(-direction.Y, direction.X);
                                                      for (int i = -5; i < 6; i++)
                                                      {
                                                          attacker.particleEmitter.createParticles(-direction * projectileSpeed * 5,
                                                                                                      tangent * -i * 40,
                                                                                                      attacker.particleEmitter.GroundPos + tangent * i * 1.7f - direction * (Math.Abs(i) * 7),
                                                                                                      4,
                                                                                                      300);
                                                      }
                                                  }).GetEnumerator();
                        Mana -= spellManaCost;
                        #endregion Lightning Tier 1
                        return;
                    }
                    else if (controller.Water == ReleasableButtonState.Clicked)
                    {
                        Item i = new Key("key");
                        PickUpItem(i);
                        Mana -= spellManaCost;
                        return;
                    }
                }
                if (controller.Sword > 0)
                {
                    #region Start Melee Attack
                    float range = 450.0f;
                    List<Sprite> targets = This.Game.CurrentLevel.GetSpritesByType(typeof(Enemy));
                    Sprite target = GetClosestTarget(targets, range);
                    isAttacking = true;
                    isMovingAllowed = false;
                    mAttack = Attacks.Melee(target, this, 25, 0, 50, TimeSpan.Zero).GetEnumerator();
                    This.Game.AudioManager.PlaySoundEffect("Effects/Sword_Attack");
                    #endregion Start Melee Attack
                    return;
                }
            }
        }

        private void interact()
        {
            List<Sprite> obstacles = (This.Game.CurrentLevel as FrostbyteLevel).obstacles;
            float distance = Math.Max(GetAnimation().Height, GetAnimation().Width) * 1.5f;
            if (obstacles != null)
            {
                List<Sprite> targets = GetTargetsInRange(obstacles, distance);
                foreach (Sprite target in targets)
                {
                    if (target is Obstacles.Door)
                    {
                        for (int x = 0; x < ItemBag.Count; x++)
                        {
                            if (ItemBag[x] is Key)
                            {
                                ItemBag.RemoveAt(x);
                                (target as Obstacles.Door).Open();
                                return;
                            }
                        }
                    }
                    else if (target is Obstacles.Chest)
                    {
                        Item i = (target as Obstacles.Chest).Open();
                        PickUpItem(i);
                    }
                }
            }
        }

        public void mUpdate()
        {
            controller.Update();

            if (currentTarget != null && !currentTarget.Visible)
            {
                cancelTarget();
            }

            /*if (State == SpriteState.Dead)
            {
                FrostbyteLevel l = This.Game.CurrentLevel as FrostbyteLevel;
                if (l.allies.Count > 0)
                {

                }
                return;
            }*/

            if (controller.IsConnected)
            {
                //necessary for collision
                if (this.CollidesWithBackground)
                    previousFootPos = this.GroundPos;

                #region Targeting
                if (controller.TargetEnemies)
                {
                    if (currentTargetAlignment != TargetAlignment.Enemy)
                    {
                        currentTarget = null;
                    }

                    currentTarget = findMinimum(GetTargetsInRange(
                        (This.Game.CurrentLevel as FrostbyteLevel).enemies,
                        This.Game.GraphicsDevice.Viewport.Width));

                    if (currentTarget != null)
                    {
                        currentTargetAlignment = TargetAlignment.Enemy;
                    }
                }
                else if (controller.TargetAllies)
                {
                    if (currentTargetAlignment != TargetAlignment.Ally)
                    {
                        currentTarget = null;
                    }

                    currentTarget = findMinimum(GetTargetsInRange(
                        (This.Game.CurrentLevel as FrostbyteLevel).allies.Concat(
                        (This.Game.CurrentLevel as FrostbyteLevel).obstacles).ToList(),
                        This.Game.GraphicsDevice.Viewport.Width));
                    if (currentTarget != null)
                    {
                        currentTargetAlignment = TargetAlignment.Ally;
                    }
                }

                if (controller.CancelTargeting == ReleasableButtonState.Clicked)
                {
                    cancelTarget();
                }

                if (currentTarget != null)
                {
                    target.Visible = true;
                    target.CenterOn(currentTarget);
                }

                if (!(This.Game.CurrentLevel as FrostbyteLevel).enemies.Contains(currentTarget))
                {
                    cancelTarget();
                }
                #endregion Targeting

                #region Movement
                if (isMovingAllowed)
                {
                    PreviousPos = Pos;

                    Pos.X += controller.Movement.X * 3 * Speed;
                    Pos.Y -= controller.Movement.Y * 3 * Speed;

                    Vector2 newDirection = controller.Movement;
                    newDirection.Y *= -1;
                    Direction = newDirection;

                    State = PreviousPos == Pos ? SpriteState.Idle : SpriteState.Moving;
                }
                #endregion Movement

                //perform collision detection with background
                if (this.CollidesWithBackground)
                    checkBackgroundCollisions();

                attack();

                if (controller.Interact == ReleasableButtonState.Clicked)
                {
                    interact();
                }
            }
        }

        /// <summary>
        /// Respawns the player at their spawn point with their default attributes
        /// </summary>
        internal void Respawn()
        {
            Pos = SpawnPoint;
            Health = MaxHealth;
            Mana = MaxMana;
            Rewind();
            StartAnim();
        }
        #endregion


    }
}
