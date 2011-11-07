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
    internal partial class ElectricBat : Frostbyte.Enemy
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

        internal ElectricBat(string name, Vector2 initialPos)
            : base(name, new Actor(Animations), 20, 100)
        {
            movementStartTime = new TimeSpan(0, 0, 1);
            ElementType = Element.Lightning;
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
            Texture2D lightning = This.Game.CurrentLevel.GetTexture("sparkball");
            particleEmitter = new ParticleEmitter(1000, particleEffect, lightning);
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
                float range = 400.0f;
                List<Sprite> targets = This.Game.CurrentLevel.GetSpritesByType(typeof(Player));
                Sprite target = GetClosestTarget(targets, range);
                if (target != null)
                {
                    isAttacking = true;

                    //particle emitter is created in constructor

                    //set orientation
                    this.Direction = target.GroundPos - this.GroundPos;

                    mAttack = Attacks.T1Projectile(target,
                                              this,
                                              5,
                                              10,
                                              new TimeSpan(0, 0, 0, 1, 750),
                                              new TimeSpan(0, 0, 0, 1, 250),
                                              20,
                                              6f,
                                              true,
                                              delegate(OurSprite attacker, Vector2 direction, float projectileSpeed)
                                              {
                                                  //attacker.particleEmitter.createParticles(direction * projectileSpeed, Vector2.Zero, attacker.particleEmitter.GroundPos, 10, 10);
                                                  Vector2 tangent = new Vector2(-direction.Y, direction.X);
                                                  for (int i = -5; i < 6; i++)
                                                  {
                                                      attacker.particleEmitter.createParticles(-direction * projectileSpeed * 5,
                                                                                                  tangent * -i * 40,
                                                                                                  attacker.particleEmitter.GroundPos + tangent * i * 1.7f - direction*(Math.Abs(i) - 30),
                                                                                                  4,
                                                                                                  300);
                                                  }
                                              }).GetEnumerator();
                }
            }
        }
    }
}
