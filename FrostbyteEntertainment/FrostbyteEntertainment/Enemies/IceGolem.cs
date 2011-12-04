﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace Frostbyte.Enemies
{
    internal partial class IceGolem : Golem
    {
        #region Variables
        static List<String> Animations = new List<String>(){
           "crystalgolem-idle-down.anim",
           "crystalgolem-idle-diagdown.anim",
           "crystalgolem-idle-right.anim",
           "crystalgolem-idle-diagup.anim",
           "crystalgolem-idle-up.anim",
           "crystalgolem-walk-down.anim",
           "crystalgolem-walk-diagdown.anim",
           "crystalgolem-walk-right.anim",
           "crystalgolem-walk-diagup.anim",
           "crystalgolem-walk-up.anim",
           "crystalgolem-attack-down.anim",
           "crystalgolem-attack-diagdown.anim",
           "crystalgolem-attack-right.anim",
           "crystalgolem-attack-diagup.anim",
           "crystalgolem-attack-up.anim",
        };
        #endregion Variables

        public IceGolem(string name, Vector2 initialPos)
            : base(name, initialPos, 200, Animations)
        {
            ElementType = Element.Water;
            Personality = new WalkingSentinelPersonality(this);
        }

        protected override void updateAttack()
        {
            if (This.gameTime.TotalGameTime >= attackStartTime + new TimeSpan(0, 0, 2) && isAttackAnimDone)
            {
                float range = 450.0f;
                List<Sprite> targets = (This.Game.CurrentLevel as FrostbyteLevel).allies;
                Sprite target = GetClosestTarget(targets, range);
                if (target != null)
                {
                    attackStartTime = This.gameTime.TotalGameTime;

                    int attackRange = 17;

                    //Create Particle Emitter
                    Effect particleEffect = This.Game.CurrentLevel.GetEffect("ParticleSystem");
                    Texture2D water = This.Game.CurrentLevel.GetTexture("snowflake");
                    ParticleEmitter particleEmitterIce = new ParticleEmitter(1000, particleEffect, water);
                    particleEmitterIce.effectTechnique = "FadeAtXPercent";
                    particleEmitterIce.fadeStartPercent = .98f;
                    particleEmitterIce.blendState = BlendState.Additive;
                    (particleEmitterIce.collisionObjects.First() as Collision_BoundingCircle).Radius = attackRange;
                    (particleEmitterIce.collisionObjects.First() as Collision_BoundingCircle).createDrawPoints();

                    mAttacks.Add(Attacks.T1Projectile(target,
                                              this,
                                              20,
                                              18,
                                              new TimeSpan(0, 0, 0, 1, 750),
                                              attackRange,
                                              3f,
                                              true,
                                              delegate(OurSprite attacker, Vector2 direction, float projectileSpeed, ParticleEmitter particleEmitter)
                                              {
                                                  Random rand = new Random();
                                                  Vector2 tangent = new Vector2(-direction.Y, direction.X);
                                                  for (int i = -5; i < -4; i++)
                                                  {
                                                      float velocitySpeed = rand.Next(30, 55);
                                                      float accelSpeed = rand.Next(-30, -10);
                                                      particleEmitter.createParticles(-direction * velocitySpeed * 2,
                                                                          direction * accelSpeed * 15,
                                                                          particleEmitter.GroundPos,
                                                                          rand.Next(20, 50),
                                                                          rand.Next(500, 1000));
                                                  }
                                              },
                                              particleEmitterIce,
                                              new Vector2(0, 0)).GetEnumerator());
                }
            }
        }
    }
}
