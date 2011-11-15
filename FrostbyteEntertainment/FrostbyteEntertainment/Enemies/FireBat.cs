﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace Frostbyte.Enemies
{
    internal partial class FireBat : Frostbyte.Enemy
    {
        bool changeState = false;
        TimeSpan idleTime = new TimeSpan(0, 0, 2);

        #region Variables
        static List<Animation> Animations = new List<Animation>(){
            This.Game.CurrentLevel.GetAnimation("bat-down.anim"),
            This.Game.CurrentLevel.GetAnimation("bat-diagdown.anim"),
            This.Game.CurrentLevel.GetAnimation("bat-right.anim"),
            This.Game.CurrentLevel.GetAnimation("bat-diagup.anim"),
            This.Game.CurrentLevel.GetAnimation("bat-up.anim"),
            This.Game.CurrentLevel.GetAnimation("bat-down.anim"),
            This.Game.CurrentLevel.GetAnimation("bat-diagdown.anim"),
            This.Game.CurrentLevel.GetAnimation("bat-right.anim"),
            This.Game.CurrentLevel.GetAnimation("bat-diagup.anim"),
            This.Game.CurrentLevel.GetAnimation("bat-up.anim"),
            This.Game.CurrentLevel.GetAnimation("bat-down.anim"),
            This.Game.CurrentLevel.GetAnimation("bat-diagdown.anim"),
            This.Game.CurrentLevel.GetAnimation("bat-right.anim"),
            This.Game.CurrentLevel.GetAnimation("bat-diagup.anim"),
            This.Game.CurrentLevel.GetAnimation("bat-up.anim"),
        };

        #endregion Variables

        public FireBat(string name, Vector2 initialPos)
            : base(name, new Actor(Animations), 20, 100)
        {
            movementStartTime = new TimeSpan(0, 0, 1);
            ElementType = Element.Fire;
            SpawnPoint = initialPos;
            Personality = new PseudoWanderPersonality(this);
            Scale = .5f;

            This.Game.AudioManager.AddSoundEffect("Effects/Bat_Move");
            if (MovementAudioName == null)
            {
                MovementAudioName = "Effects/Bat_Move";
                This.Game.AudioManager.InitializeLoopingSoundEffect(MovementAudioName);
            }
        }

        protected override void updateMovement()
        {
            if (changeState)
            {
                movementStartTime = TimeSpan.MaxValue;
            }
            Personality.Update();
        }

        protected override void updateAttack()
        {
            if (This.gameTime.TotalGameTime >= attackStartTime + new TimeSpan(0, 0, 0, 0, 750) && isAttackAnimDone)
            {
                float range = 450.0f;
                List<Sprite> targets = (This.Game.CurrentLevel as FrostbyteLevel).allies;
                Sprite target = GetClosestTarget(targets, range);
                if (target != null)
                {
                    attackStartTime = This.gameTime.TotalGameTime;

                    int attackRange = 3;


                    //Create Particle Emmiter
                    Effect particleEffect = This.Game.CurrentLevel.GetEffect("ParticleSystem");
                    Texture2D fire = This.Game.CurrentLevel.GetTexture("fire");
                    ParticleEmitter particleEmitterFire = new ParticleEmitter(1000, particleEffect, fire);
                    particleEmitterFire.effectTechnique = "NoSpecialEffect";
                    particleEmitterFire.blendState = BlendState.Additive;
                    (particleEmitterFire.collisionObjects.First() as Collision_BoundingCircle).Radius = attackRange;
                    (particleEmitterFire.collisionObjects.First() as Collision_BoundingCircle).createDrawPoints();

                    mAttacks.Add(Attacks.T1Projectile(target,
                                              this,
                                              5,
                                              10,
                                              new TimeSpan(0, 0, 0, 1, 750),
                                              attackRange,
                                              6f,
                                              true,
                                              delegate(OurSprite attacker, Vector2 direction, float projectileSpeed, ParticleEmitter particleEmitter)
                                              {
                                                  Random rand = new Random();
                                                  Vector2 tangent = new Vector2(-direction.Y, direction.X);
                                                  for (int i = -5; i < 6; i++)
                                                  {
                                                      float velocitySpeed = rand.Next(30, 55);
                                                      float accelSpeed = rand.Next(-30, -10);
                                                      particleEmitter.createParticles(direction*velocitySpeed,
                                                                          direction*accelSpeed,
                                                                          particleEmitter.GroundPos,
                                                                          rand.Next(5, 20),
                                                                          rand.Next(50, 300));
                                                  }
                                              },
                                              particleEmitterFire).GetEnumerator());
                }
            }
        }
    }
}