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
            This.Game.CurrentLevel.GetAnimation("worm-idle-up.anim"),
            This.Game.CurrentLevel.GetAnimation("worm-underground.anim"),
            This.Game.CurrentLevel.GetAnimation("worm-underground.anim"),
            This.Game.CurrentLevel.GetAnimation("worm-underground.anim"),
            This.Game.CurrentLevel.GetAnimation("worm-underground.anim"),
            This.Game.CurrentLevel.GetAnimation("worm-underground.anim"),
            This.Game.CurrentLevel.GetAnimation("worm-spew-down.anim"),
            This.Game.CurrentLevel.GetAnimation("worm-spew-diagdown.anim"),
            This.Game.CurrentLevel.GetAnimation("worm-spew-right.anim"),
            This.Game.CurrentLevel.GetAnimation("worm-spew-diagup.anim"),
            This.Game.CurrentLevel.GetAnimation("worm-spew-up.anim"),
            This.Game.CurrentLevel.GetAnimation("worm-underground.anim"),
            This.Game.CurrentLevel.GetAnimation("worm-submerge.anim"),
            This.Game.CurrentLevel.GetAnimation("worm-surface-down.anim"),
            This.Game.CurrentLevel.GetAnimation("worm-vomit.anim"),
            This.Game.CurrentLevel.GetAnimation("worm-die.anim"),
        };
        #endregion Variables

        public Worm(string name, Vector2 initialPos)
            : base(name, new Actor(Animations), 1, 1000)
        {
            SpawnPoint = initialPos;
            movementStartTime = new TimeSpan(0, 0, 1);
            ElementType = Element.Earth;
            Personality = new UndergroundAttackPersonality(this);
        }

        private bool changeState = false;
        private bool isSubmerged = false;

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
            if (isAttackAnimDone && !isSubmerged)
            {
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
                                              particleEmitterEarth).GetEnumerator());
                }
            }
        }
    }
}
