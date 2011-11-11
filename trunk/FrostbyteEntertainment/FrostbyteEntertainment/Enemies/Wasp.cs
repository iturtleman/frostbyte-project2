using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace Frostbyte.Enemies
{
    internal partial class Wasp : Frostbyte.Enemy
    {
        #region Variables

        bool changeState = false;
        TimeSpan idleTime = new TimeSpan(0, 0, 2);

        static List<Animation> Animations = new List<Animation>(){
            This.Game.CurrentLevel.GetAnimation("wasp-idle-down.anim"),
            This.Game.CurrentLevel.GetAnimation("wasp-idle-diagdown.anim"),
            This.Game.CurrentLevel.GetAnimation("wasp-idle-right.anim"),
            This.Game.CurrentLevel.GetAnimation("wasp-idle-diagup.anim"),
            This.Game.CurrentLevel.GetAnimation("wasp-idle-up.anim"),
            This.Game.CurrentLevel.GetAnimation("wasp-idle-down.anim"),
            This.Game.CurrentLevel.GetAnimation("wasp-idle-diagdown.anim"),
            This.Game.CurrentLevel.GetAnimation("wasp-idle-right.anim"),
            This.Game.CurrentLevel.GetAnimation("wasp-idle-diagup.anim"),
            This.Game.CurrentLevel.GetAnimation("wasp-idle-up.anim"),
            This.Game.CurrentLevel.GetAnimation("wasp-attack-down.anim"),
            This.Game.CurrentLevel.GetAnimation("wasp-attack-diagdown.anim"),
            This.Game.CurrentLevel.GetAnimation("wasp-attack-right.anim"),
            This.Game.CurrentLevel.GetAnimation("wasp-attack-diagup.anim"),
            This.Game.CurrentLevel.GetAnimation("wasp-attack-up.anim"),
        };

        #endregion Variables

        public Wasp(string name, Vector2 initialPos)
            : base(name, new Actor(Animations), 1, 100)
        {
            GroundPos = initialPos;
            movementStartTime = new TimeSpan(0, 0, 1);
            Personality = new DartPersonality(this);
            ElementType = Element.Normal;


            //Create Particle Emmiter
            Effect particleEffect = This.Game.CurrentLevel.GetEffect("ParticleSystem");
            Texture2D boulder = This.Game.CurrentLevel.GetTexture("boulder");
            particleEmitter = new ParticleEmitter(1000, particleEffect, boulder);
            particleEmitter.effectTechnique = "NoSpecialEffect";
            particleEmitter.blendState = BlendState.AlphaBlend;

            This.Game.AudioManager.AddSoundEffect("Effects/Wasp_Attack");
            This.Game.AudioManager.AddSoundEffect("Effects/Wasp_Move");
            if (MovementAudioName == null)
            {
                MovementAudioName = "Effects/Wasp_Move";
                This.Game.AudioManager.InitializeLoopingSoundEffect(MovementAudioName);
            }
        }

        protected override void updateMovement()
        {
            if (changeState)
            {
                movementStartTime = TimeSpan.MaxValue;
            }
            List<Sprite> targets = This.Game.CurrentLevel.GetSpritesByType(typeof(Player));
            Personality.Update();
        }

        protected override void updateAttack()
        {
            if (isAttacking)
            {
                mAttack.MoveNext();
                isAttacking = !mAttack.Current;
            }
            else
            {
                float range = 450.0f;
                List<Sprite> targets = This.Game.CurrentLevel.GetSpritesByType(typeof(Player));
                Sprite target = GetClosestTarget(targets, range);
                if (target != null)
                {
                    isAttacking = true;

                    //particle emitter is created in constructor

                    int attackRange = 11;

                    (particleEmitter.collisionObjects.First() as Collision_BoundingCircle).Radius = attackRange;
                    (particleEmitter.collisionObjects.First() as Collision_BoundingCircle).createDrawPoints();


                    mAttack = Attacks.T1Projectile(target,
                                              this,
                                              5,
                                              30,
                                              new TimeSpan(0, 0, 0, 1, 750),
                                              new TimeSpan(0, 0, 0, 0, 750),
                                              attackRange,
                                              6f,
                                              false,
                                              delegate(OurSprite attacker, Vector2 direction, float projectileSpeed)
                                              {
                                                  Random randPosition = new Random();
                                                  attacker.particleEmitter.createParticles(direction * projectileSpeed, Vector2.Zero, attacker.particleEmitter.GroundPos, 10, 10);
                                                  Vector2 tangent = new Vector2(-direction.Y, direction.X);
                                                  for (int i = -5; i < 6; i++)
                                                  {
                                                      attacker.particleEmitter.createParticles(-direction * projectileSpeed * 5,
                                                                                               tangent * -i * 40,
                                                                                               attacker.particleEmitter.GroundPos + tangent * i * 1.7f + (float)randPosition.NextDouble() * direction * 8f,
                                                                                               1.5f,
                                                                                               300);
                                                  }
                                              }
                                              ).GetEnumerator();
                    This.Game.AudioManager.PlaySoundEffect("Effects/Wasp_Attack");
                }
            }
        }

    }
}
