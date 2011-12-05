using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Frostbyte.Enemies
{
    internal partial class WaterBlobBoss : Frostbyte.Boss
    {
        #region Variables
        static List<String> Animations = new List<String>(){
           "antibody.anim",
        };

        internal TimeSpan attackWait = TimeSpan.MaxValue;

        #endregion Variables

        public WaterBlobBoss(string name, Vector2 initialPosition)
            : base(name, new Actor(Animations), 20, 1000)
        {
            SpawnPoint = initialPosition;
            movementStartTime = new TimeSpan(0, 0, 1);
            ElementType = Element.Water;
            Personality = new LiquidPersonality(this);
        }

        protected override void updateMovement()
        {
            Personality.Update();
        }

        protected override void updateAttack()
        {
            if (isAttackAnimDone)
            {
                FrostbyteLevel l = (This.Game.CurrentLevel as FrostbyteLevel);
                if (attackWait < This.gameTime.TotalGameTime)
                {
                    int randAttack = This.Game.rand.Next(10);
                    Sprite currentTarget = GetClosestTarget(l.allies);
                    int attackFrame = 0;

                    if (randAttack < 8)
                    {
                        #region Water Tier 1

                        int attackRange = 11;

                        //Create Earth Tier 1 Particle Emmiter
                        Effect particleEffect = l.GetEffect("ParticleSystem");
                        Texture2D snowflake = l.GetTexture("waterParticle");
                        ParticleEmitter particleWaterTier1 = new ParticleEmitter(500, particleEffect, snowflake);
                        particleWaterTier1.effectTechnique = "FadeAtXPercent";
                        particleWaterTier1.fadeStartPercent = .98f;
                        particleWaterTier1.blendState = BlendState.Additive;
                        (particleWaterTier1.collisionObjects.First() as Collision_BoundingCircle).Radius = attackRange;
                        (particleWaterTier1.collisionObjects.First() as Collision_BoundingCircle).createDrawPoints();
                        particleEmitters.Add(particleWaterTier1);

                        mAttacks.Add(Attacks.T1Projectile(currentTarget,
                            this,
                            20,
                            attackFrame,
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
                                                                            10.0f,
                                                                            This.Game.rand.Next(10, 300));
                                }
                            },
                            particleWaterTier1,
                            new Vector2(0, -38),
                            Element.Water
                            ).GetEnumerator());

                        This.Game.AudioManager.PlaySoundEffect("Effects/Water_T1");
                        #endregion Water Tier 1
                    }
                    else if (randAttack < 10)
                    {
                        #region Water Tier 2
                        mAttacks.Add(Attacks.WaterPush(this, attackFrame).GetEnumerator());
                        This.Game.AudioManager.PlaySoundEffect("Effects/Water_T2", .1f);
                        #endregion Water Tier 2
                    }
                    else if (randAttack < 11)
                    {
                        #region Water Tier 3
                        mAttacks.Add(Attacks.Freeze(currentTarget, this, attackFrame).GetEnumerator());
                        This.Game.AudioManager.PlaySoundEffect("Effects/Water_T3");
                        #endregion Water Tier 3
                    }
                    attackWait = This.gameTime.TotalGameTime + new TimeSpan(0, 0, This.Game.rand.Next(3, 5));
                }
            }
        }

    }
}
