using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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

        internal Wasp(string name, Vector2 initialPos)
            : base(name, new Actor(Animations), 1, 100)
        {
            GroundPos = initialPos;
            movementStartTime = new TimeSpan(0, 0, 1);
            Personality = new DartPersonality(this);
            ElementType = Element.Normal;
            
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
                particleEmitter.Update();
            }
            else
            {
                float range = 400.0f;
                List<Sprite> targets = This.Game.CurrentLevel.GetSpritesByType(typeof(Player));
                Sprite target = GetClosestTarget(targets, range);
                if (target != null)
                {
                    isAttacking = true;

                    //Create Particle Emmiter
                    Effect particleEffect = This.Game.CurrentLevel.GetEffect("ParticleSystem");
                    Texture2D boulder = This.Game.Content.Load<Texture2D>("Textures/boulder");
                    particleEmitter = new ParticleEmitter(1, particleEffect, boulder);
                    particleEmitter.effectTechnique = "NoSpecialEffect";
                    particleEmitter.blendState = BlendState.AlphaBlend;

                    //set orientation
                    this.Direction = target.GroundPos - this.GroundPos;

                    mAttack = Attacks.EarthT1(target, this, 5, 30, Element.Lightning).GetEnumerator();
                }
            }
        }

    }
}
