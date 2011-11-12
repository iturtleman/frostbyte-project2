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
            GroundPos = initialPos;
            Personality = new PseudoWanderPersonality(this);
            Scale = .5f;

            This.Game.AudioManager.AddSoundEffect("Effects/Bat_Move");
            if (MovementAudioName == null)
            {
                MovementAudioName = "Effects/Bat_Move";
                This.Game.AudioManager.InitializeLoopingSoundEffect(MovementAudioName);
            }


            //Create Particle Emmiter
            Effect particleEffect = This.Game.CurrentLevel.GetEffect("ParticleSystem");
            Texture2D fire = This.Game.CurrentLevel.GetTexture("fire");
            particleEmitter = new ParticleEmitter(1000, particleEffect, fire);
            particleEmitter.effectTechnique = "NoSpecialEffect";
            particleEmitter.blendState = BlendState.Additive;
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

                    int attackRange = 3;

                    (particleEmitter.collisionObjects.First() as Collision_BoundingCircle).Radius = attackRange;
                    (particleEmitter.collisionObjects.First() as Collision_BoundingCircle).createDrawPoints();

                    mAttack = Attacks.T1Projectile(target,
                                              this,
                                              5,
                                              10,
                                              new TimeSpan(0, 0, 0, 1, 750),
                                              new TimeSpan(0, 0, 0, 1, 250),
                                              attackRange,
                                              6f,
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
                                              }).GetEnumerator();
                }
            }
        }
    }
}
