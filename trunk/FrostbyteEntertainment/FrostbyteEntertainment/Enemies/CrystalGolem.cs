using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace Frostbyte.Enemies
{
    internal partial class CrystalGolem : Golem
    {
        #region Variables
        static List<Animation> Animations = new List<Animation>(){
                This.Game.CurrentLevel.GetAnimation("crystalgolem-idle-down.anim"),
            This.Game.CurrentLevel.GetAnimation("crystalgolem-idle-diagdown.anim"),
            This.Game.CurrentLevel.GetAnimation("crystalgolem-idle-right.anim"),
            This.Game.CurrentLevel.GetAnimation("crystalgolem-idle-diagup.anim"),
            This.Game.CurrentLevel.GetAnimation("crystalgolem-idle-up.anim"),
            This.Game.CurrentLevel.GetAnimation("crystalgolem-walk-down.anim"),
            This.Game.CurrentLevel.GetAnimation("crystalgolem-walk-diagdown.anim"),
            This.Game.CurrentLevel.GetAnimation("crystalgolem-walk-right.anim"),
            This.Game.CurrentLevel.GetAnimation("crystalgolem-walk-diagup.anim"),
            This.Game.CurrentLevel.GetAnimation("crystalgolem-walk-up.anim"),
            This.Game.CurrentLevel.GetAnimation("crystalgolem-attack-down.anim"),
            This.Game.CurrentLevel.GetAnimation("crystalgolem-attack-diagdown.anim"),
            This.Game.CurrentLevel.GetAnimation("crystalgolem-attack-right.anim"),
            This.Game.CurrentLevel.GetAnimation("crystalgolem-attack-diagup.anim"),
            This.Game.CurrentLevel.GetAnimation("crystalgolem-attack-up.anim"),
        };
        #endregion Variables

        public CrystalGolem(string name, Vector2 initialPos)
            : base(name, initialPos, Animations)
        {
            ElementType = Element.Lightning;
            Personality = new StrictSentinelPersonality(this);
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

                    int attackRange = 19;

                    //Create Particle Emmiter
                    Effect particleEffect = This.Game.CurrentLevel.GetEffect("ParticleSystem");
                    Texture2D lightning = This.Game.CurrentLevel.GetTexture("sparkball");
                    ParticleEmitter particleEmitterEarth = new ParticleEmitter(1000, particleEffect, lightning);
                    particleEmitterEarth.effectTechnique = "FadeAtXPercent";
                    particleEmitterEarth.fadeStartPercent = .3f;
                    particleEmitterEarth.blendState = BlendState.Additive;
                    (particleEmitterEarth.collisionObjects.First() as Collision_BoundingCircle).Radius = attackRange;
                    (particleEmitterEarth.collisionObjects.First() as Collision_BoundingCircle).createDrawPoints();
                    particleEmitters.Add(particleEmitterEarth);

                    mAttacks.Add(Attacks.T1Projectile(target,
                                              this,
                                              20,
                                              14,
                                              new TimeSpan(0, 0, 0, 1, 750),
                                              attackRange,
                                              3f,
                                              true,
                                              delegate(OurSprite attacker, Vector2 direction, float projectileSpeed, ParticleEmitter particleEmitter)
                                              {
                                                  double directionAngle = This.Game.rand.NextDouble() * Math.PI * 2;
                                                  Vector2 directionNormal = new Vector2((float)Math.Cos(directionAngle), (float)Math.Sin(directionAngle));
                                                  Vector2 tangent = new Vector2(-directionNormal.Y, directionNormal.X);
                                                  int positionOffset = This.Game.rand.Next(1, 30);
                                                  for (int i = -4; i < 5; i++)
                                                  {
                                                      particleEmitter.createParticles(-direction * projectileSpeed*30,
                                                                                                  Vector2.Zero,
                                                                                                  particleEmitter.GroundPos + tangent * i * ParticleEmitter.EllipsePerspectiveModifier - directionNormal * (Math.Abs(i) * 7) + directionNormal * positionOffset,
                                                                                                  25,
                                                                                                  1000);
                                                  }
                                              },
                                              particleEmitterEarth,
                                              Vector2.Zero).GetEnumerator());
                }
            }
        }
    }
}
