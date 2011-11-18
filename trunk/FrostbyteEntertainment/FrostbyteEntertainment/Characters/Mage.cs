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
        #region Constructors
        public Mage(string name, Actor actor, Color targetColor)
            : this(name, actor, PlayerIndex.One, targetColor)
        {
            CollidesWithBackground = true;
        }

        internal Mage(string name, Actor actor, PlayerIndex input, Color targetColor)
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
            target = new Sprite("target", new Actor(new Animation("target.anim")));
            target.Visible = false;
            target.Static = true;
            target.mColor = targetColor;
            target.Scale = 1.5f;
            sortType = new DistanceSort(this);

            UpdateBehavior = mUpdate;
            CollidesWithBackground = true;

            This.Game.AudioManager.AddSoundEffect("Effects/Sword_Attack");
            This.Game.AudioManager.AddSoundEffect("Effects/Lightning_Strike");
            This.Game.AudioManager.AddSoundEffect("Effects/Earthquake");

            CollisionList = 3;
        }
        #endregion

        #region Variables
        internal IController controller;
        private Sprite target;
        BasicEffect basicEffect = new BasicEffect(This.Game.GraphicsDevice);
        private IComparer<Sprite> sortType;
        private List<Element> attackCounter = new List<Element>();
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
            Level l = This.Game.CurrentLevel;
            if (isAttackAnimDone)
            {
                if (controller.LaunchAttack == ReleasableButtonState.Clicked)
                {
                    if (attackCounter.Count != 0)
                    {
                        switch (attackCounter.First())
                        {

                            case Element.Earth:
                                if (attackCounter.Count == 1)
                                {
                                    if (Mana >= 10)
                                    {
                                        #region Earth Tier 1

                                        int attackRange = 11;

                                        //Create Earth Tier 1 Particle Emmiter
                                        Effect particleEffect = l.GetEffect("ParticleSystem");
                                        Texture2D boulder = l.GetTexture("boulder");
                                        ParticleEmitter particleEarthTier1 = new ParticleEmitter(1000, particleEffect, boulder);
                                        particleEarthTier1.effectTechnique = "NoSpecialEffect";
                                        particleEarthTier1.blendState = BlendState.AlphaBlend;
                                        (particleEarthTier1.collisionObjects.First() as Collision_BoundingCircle).Radius = attackRange;
                                        (particleEarthTier1.collisionObjects.First() as Collision_BoundingCircle).createDrawPoints();
                                        particleEmitters.Add(particleEarthTier1);

                                        mAttacks.Add(Attacks.T1Projectile(currentTarget,
                                                                  this,
                                                                  20,
                                                                  0,
                                                                  new TimeSpan(0, 0, 0, 1, 150),
                                                                  attackRange,
                                                                  9f,
                                                                  false,
                                                                  delegate(OurSprite attacker, Vector2 direction, float projectileSpeed, ParticleEmitter particleEmitter)
                                                                  {
                                                                      Random randPosition = new Random();
                                                                      particleEmitter.createParticles(direction * projectileSpeed, Vector2.Zero, particleEmitter.GroundPos, 10, 10);
                                                                      Vector2 tangent = new Vector2(-direction.Y, direction.X);
                                                                      for (int i = -5; i < 6; i++)
                                                                      {
                                                                          particleEmitter.createParticles(-direction * projectileSpeed * .75f,
                                                                                                                   tangent * -i * 40,
                                                                                                                   particleEmitter.GroundPos + tangent * i * ParticleEmitter.EllipsePerspectiveModifier + (float)randPosition.NextDouble() * direction * 8f,
                                                                                                                   1.5f,
                                                                                                                   300);
                                                                      }
                                                                  },
                                                                  particleEarthTier1,
                                                                  Element.Earth
                                                                  ).GetEnumerator());
                                        #endregion Earth Tier 1
                                        Mana -= 10;
                                    }
                                }

                                else if (attackCounter.Count == 2)
                                {
                                    if (Mana >= 20)
                                    {
                                        #region Earth Tier 2
                                        mAttacks.Add(Attacks.Earthquake(this, this, 10, 0).GetEnumerator());
                                        #endregion Earth Tier 2
                                        Mana -= 20;
                                    }
                                }

                                else
                                {
                                    if (Mana >= 50 && currentTarget != null && !(currentTarget is Player))
                                    {
                                        #region Earth Tier 3
                                        mAttacks.Add(Attacks.RockShower(currentTarget, this, 10, 0).GetEnumerator());
                                        #endregion Earth Tier 3
                                        Mana -= 50;
                                    }
                                }
                                break;

                            case Element.Lightning:
                                if (attackCounter.Count == 1)
                                {
                                    if (Mana >= 10)
                                    {
                                        #region Lightning Tier 1

                                        int attackRange = 3;

                                        //Create Lightning Tier 1 Particle Emmiter
                                        Effect particleEffect = l.GetEffect("ParticleSystem");
                                        Texture2D lightning = l.GetTexture("sparkball");
                                        ParticleEmitter particleLightningTier1 = new ParticleEmitter(1000, particleEffect, lightning);
                                        particleLightningTier1.effectTechnique = "NoSpecialEffect";
                                        particleLightningTier1.blendState = BlendState.Additive;
                                        (particleLightningTier1.collisionObjects.First() as Collision_BoundingCircle).Radius = attackRange;
                                        (particleLightningTier1.collisionObjects.First() as Collision_BoundingCircle).createDrawPoints();
                                        particleEmitters.Add(particleLightningTier1);

                                        mAttacks.Add(Attacks.T1Projectile(currentTarget,
                                                                  this,
                                                                  20,
                                                                  0,
                                                                  new TimeSpan(0, 0, 0, 1, 250),
                                                                  attackRange,
                                                                  8f,
                                                                  true,
                                                                  delegate(OurSprite attacker, Vector2 direction, float projectileSpeed, ParticleEmitter particleEmitter)
                                                                  {
                                                                      Vector2 tangent = new Vector2(-direction.Y, direction.X);
                                                                      for (int i = -5; i < 6; i++)
                                                                      {
                                                                          particleEmitter.createParticles(-direction * projectileSpeed * 5,
                                                                                                                      tangent * -i * 40,
                                                                                                                      particleEmitter.GroundPos + tangent * i * ParticleEmitter.EllipsePerspectiveModifier - direction * (Math.Abs(i) * 7),
                                                                                                                      4,
                                                                                                                      300);
                                                                      }
                                                                  },
                                                                  particleLightningTier1,
                                                                  Element.Lightning
                                                                  ).GetEnumerator());
                                        #endregion Lightning Tier 1
                                        Mana -= 10;
                                    }
                                }

                                else if (attackCounter.Count == 2)
                                {
                                    if (Mana >= 50)
                                    {
                                        #region Lightning Tier 2
                                        mAttacks.Add(Attacks.LightningStrike(this, this, 10, 0).GetEnumerator());
                                        #endregion Lightning Tier 2
                                        Mana -= 50;

                                        
                                    }
                                }

                                else
                                {
                                    if (Mana >= 50)
                                    {
                                        #region Lightning Tier 3
                                        mAttacks.Add(Attacks.LightningStrike(currentTarget, this, 10, 0).GetEnumerator());
                                        #endregion Lightning Tier 3
                                        Mana -= 50;
                                    }
                                }
                                break;

                            case Element.Water:
                                if (attackCounter.Count == 1)
                                {

                                }

                                else if (attackCounter.Count == 2)
                                {

                                }

                                else
                                {

                                }
                                break;

                            case Element.Fire:
                                if (attackCounter.Count == 1)
                                {
                                    if (Mana >= 10)
                                    {
                                        #region Fire Tier 1

                                        int attackRange = 11;

                                        //Create Fire Tier 1 Particle Emmiter
                                        Effect particleEffect = l.GetEffect("ParticleSystem");
                                        Texture2D fire = l.GetTexture("fire");
                                        ParticleEmitter particleFireTier1 = new ParticleEmitter(3000, particleEffect, fire);
                                        particleFireTier1.effectTechnique = "NoSpecialEffect";
                                        particleFireTier1.blendState = BlendState.AlphaBlend;
                                        (particleFireTier1.collisionObjects.First() as Collision_BoundingCircle).Radius = attackRange;
                                        (particleFireTier1.collisionObjects.First() as Collision_BoundingCircle).createDrawPoints();
                                        particleEmitters.Add(particleFireTier1);


                                        mAttacks.Add(Attacks.T1Projectile(currentTarget,
                                                                  this,
                                                                  30,
                                                                  0,
                                                                  new TimeSpan(0, 0, 0, 0, 750),
                                                                  attackRange,
                                                                  9f,
                                                                  true,
                                                                  delegate(OurSprite attacker, Vector2 direction, float projectileSpeed, ParticleEmitter particleEmitter)
                                                                  {
                                                                      Random rand = new Random();
                                                                      Vector2 tangent = new Vector2(-direction.Y, direction.X);
                                                                      for (int i = -5; i < 6; i++)
                                                                      {
                                                                          float velocitySpeed = rand.Next(30, 55);
                                                                          float accelSpeed = rand.Next(-30, -10);
                                                                          particleEmitter.createParticles(direction * velocitySpeed,
                                                                                          direction * accelSpeed,
                                                                                          particleEmitter.GroundPos,
                                                                                          rand.Next(5, 20),
                                                                                          rand.Next(50, 300));
                                                                      }
                                                                  },
                                                                  particleFireTier1,
                                                                  Element.Fire
                                                                  ).GetEnumerator());
                                        #endregion Fire Tier 1
                                        Mana -= 10;
                                    }
                                }

                                else if (attackCounter.Count == 2)
                                {
                                    if (Mana >= 50)
                                    {
                                        #region Fire Tier 2
                                        mAttacks.Add(Attacks.FireRing(this, this, 1, 0).GetEnumerator());
                                        #endregion Fire Tier 2
                                        Mana -= 50;
                                    }
                                }

                                else
                                {

                                }
                                break;
                        }

                        attackCounter.Clear();
                    }
                }

                else if (controller.Earth == ReleasableButtonState.Clicked)
                {
                    if ((attackCounter.Count == 0 && attackCounter.Count < 3) || attackCounter.First() == Element.Earth)
                    {
                        attackCounter.Add(Element.Earth);
                    }
                    return;
                }

                else if (controller.Fire == ReleasableButtonState.Clicked)
                {
                    if ((attackCounter.Count == 0 && attackCounter.Count < 3) || attackCounter.First() == Element.Fire)
                    {
                        attackCounter.Add(Element.Fire);
                    }
                    return;
                }

                else if (controller.Lightning == ReleasableButtonState.Clicked)
                {
                    if ((attackCounter.Count == 0 && attackCounter.Count < 3) || attackCounter.First() == Element.Lightning)
                    {
                        attackCounter.Add(Element.Lightning);
                    }
                    return;
                }

                else if (controller.Water == ReleasableButtonState.Clicked)
                {
                    if ((attackCounter.Count == 0 && attackCounter.Count < 3) || attackCounter.First() == Element.Water)
                    {
                        attackCounter.Add(Element.Water);
                    }
                }
                if (controller.Sword > 0)
                {
                    #region Start Melee Attack
                    mAttacks.Add(Attacks.Melee(this, 25, 0).GetEnumerator());
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

            if (Health == 0)
            {
                State = SpriteState.Dead;
            }
            else
            {
                State = SpriteState.Idle;
            }

            if (State == SpriteState.Dead)
            {
                return;
            }

            if (controller.IsConnected)
            {
                //necessary for collision
                if (this.CollidesWithBackground)
                    previousFootPos = this.GroundPos;

                #region Targeting
                FrostbyteLevel l = This.Game.CurrentLevel as FrostbyteLevel;
                if (controller.TargetEnemies)
                {
                    if (currentTargetAlignment == TargetAlignment.Ally)
                    {
                        cancelTarget();
                    }
                    else
                    {

                        currentTarget = findMinimum(GetTargetsInRange(
                            l.enemies,
                            This.Game.GraphicsDevice.Viewport.Width / 2));

                        if (currentTarget != null)
                        {
                            currentTargetAlignment = TargetAlignment.Enemy;
                        }
                    }
                }
                else if (controller.TargetAllies)
                {
                    if (currentTargetAlignment == TargetAlignment.Enemy)
                    {
                        cancelTarget();

                    }
                    else
                    {
                        currentTarget = findMinimum(GetTargetsInRange(
                            l.allies.Concat(
                            l.obstacles).ToList(),
                            This.Game.GraphicsDevice.Viewport.Width));
                        if (currentTarget != null)
                        {
                            currentTargetAlignment = TargetAlignment.Ally;
                        }
                    }
                }

                if (controller.CancelTargeting == ReleasableButtonState.Clicked)
                {
                    cancelTarget();

                }

                if (currentTarget != null)
                {
                    target.Visible = true;
                    target.Pos = (target.CenteredOn(currentTarget) - l.Camera.Pos) * l.Camera.Zoom;
                }

                if (!(l.enemies.Contains(currentTarget) ||
                    l.allies.Contains(currentTarget) ||
                    (l.obstacles.Contains(currentTarget) &&
                        currentTarget.GetType().IsAssignableFrom(typeof(TargetableObstacle)))))
                {
                    cancelTarget();
                }
                #endregion Targeting

                #region Movement
                if (isAttackAnimDone)
                {
                    PreviousPos = Pos;

                    Pos.X += Math.Sign(controller.Movement.X) * Math.Min(Math.Abs(controller.Movement.X), .8f) * 1.25f * 3 * Speed;
                    Pos.Y -= Math.Sign(controller.Movement.Y) * Math.Min(Math.Abs(controller.Movement.Y), .8f) * 1.25f * 3 * Speed;

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
        #endregion
    }
}
