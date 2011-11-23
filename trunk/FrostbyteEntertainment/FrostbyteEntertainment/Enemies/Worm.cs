using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Frostbyte.Enemies
{

    internal partial class Worm : Frostbyte.Boss
    {
        #region Variables
        static List<Animation> Animations = new List<Animation>(){
            This.Game.CurrentLevel.GetAnimation("worm-idle-down.anim"),
            This.Game.CurrentLevel.GetAnimation("worm-idle-diagdown.anim"),
            This.Game.CurrentLevel.GetAnimation("worm-idle-right.anim"),
            This.Game.CurrentLevel.GetAnimation("worm-idle-diagup.anim"),
            This.Game.CurrentLevel.GetAnimation("worm-idle-up.anim"),  // 4
            This.Game.CurrentLevel.GetAnimation("worm-underground.anim"),
            This.Game.CurrentLevel.GetAnimation("worm-underground.anim"),
            This.Game.CurrentLevel.GetAnimation("worm-underground.anim"),
            This.Game.CurrentLevel.GetAnimation("worm-underground.anim"),
            This.Game.CurrentLevel.GetAnimation("worm-underground.anim"),  // 9
            This.Game.CurrentLevel.GetAnimation("worm-spew-down.anim"),
            This.Game.CurrentLevel.GetAnimation("worm-spew-diagdown.anim"),
            This.Game.CurrentLevel.GetAnimation("worm-spew-right.anim"),
            This.Game.CurrentLevel.GetAnimation("worm-spew-diagup.anim"),
            This.Game.CurrentLevel.GetAnimation("worm-spew-up.anim"),  // 14
            This.Game.CurrentLevel.GetAnimation("worm-underground.anim"),
            This.Game.CurrentLevel.GetAnimation("worm-submerge.anim"),  // Go-underground
            This.Game.CurrentLevel.GetAnimation("worm-surface-down.anim"),  // Go-above-ground
            This.Game.CurrentLevel.GetAnimation("worm-vomit.anim"),
            This.Game.CurrentLevel.GetAnimation("worm-die.anim"),
        };
        private bool changeState = false;
        internal bool HasVomited = false;
        internal bool IsSubmerged;
        #endregion Variables

        public Worm(string name, Vector2 initialPos)
            : base(name, new Actor(Animations), 1, 1000)
        {
            SpawnPoint = initialPos;
            movementStartTime = new TimeSpan(0, 0, 1);
            ElementType = Element.Earth;
            Personality = new UndergroundAttackPersonality(this);
            SetAnimation(15);
            IsSubmerged = true;
        }
        
        protected override void Die()
        {
            (This.Game.CurrentLevel as FrostbyteLevel).SpawnExitPortal();
            base.Die();
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
            if (isAttackAnimDone && !IsSubmerged)
            {
                if (HasVomited || This.Game.rand.Next(4) != 0)
                {
                    #region Spew
                    float range = 450.0f;
                    List<Sprite> targets = (This.Game.CurrentLevel as FrostbyteLevel).allies;
                    Sprite target = GetClosestTarget(targets, range);
                    if (target != null)
                    {
                        int attackRange = 11;

                        //Create Particle Emitter
                        Effect particleEffect = This.Game.CurrentLevel.GetEffect("ParticleSystem");
                        Texture2D boulder = This.Game.CurrentLevel.GetTexture("boulder");
                        ParticleEmitter particleEmitterEarth = new ParticleEmitter(1000, particleEffect, boulder);
                        particleEmitterEarth.effectTechnique = "NoSpecialEffect";
                        particleEmitterEarth.blendState = BlendState.AlphaBlend;
                        (particleEmitterEarth.collisionObjects.First() as Collision_BoundingCircle).Radius = attackRange;
                        (particleEmitterEarth.collisionObjects.First() as Collision_BoundingCircle).createDrawPoints();
                        particleEmitters.Add(particleEmitterEarth);

                        Vector2 oldDirection = Direction;
                        double angle = Math.Atan2(Direction.Y, Direction.X);
                        for (int x = 0; x < 3; x++)
                        {

                            mAttacks.Add(Attacks.T1Projectile(target,
                                                      this,
                                                      5,
                                                      41,
                                                      new TimeSpan(0, 0, 0, 1, 750),
                                                      attackRange,
                                                      6f,
                                                      false,
                                                      delegate(OurSprite attacker, Vector2 direction, float projectileSpeed, ParticleEmitter particleEmitter)
                                                      {
                                                          Random randPosition = new Random();
                                                          particleEmitter.createParticles(direction * projectileSpeed, Vector2.Zero, particleEmitter.GroundPos, 10, 10);
                                                          Vector2 tangent = new Vector2(-direction.Y, direction.X);
                                                          for (int i = -5; i < 6; i++)
                                                          {
                                                              particleEmitter.createParticles(-direction * projectileSpeed * 5,
                                                                                                       tangent * -i * 40,
                                                                                                       particleEmitter.GroundPos + tangent * i * ParticleEmitter.EllipsePerspectiveModifier + (float)randPosition.NextDouble() * direction * 8f,
                                                                                                       1.5f,
                                                                                                       300);
                                                          }
                                                      },
                                                      particleEmitterEarth,
                                                      Vector2.Zero).GetEnumerator());
                        }
                    }
                    #endregion
                }
                else
                {
                    mAttacks.Add(Vomit().GetEnumerator());
                }
            }
        }

        private IEnumerable<bool> Vomit()
        {
            Rewind();
            isAttackAnimDone = false;
            while (Frame != FrameCount() - 1)
            {
                SetAnimation(18);
                yield return false;
            }
            isAttackAnimDone = true;
            HasVomited = true;
            yield return true;
        }
    }
}
