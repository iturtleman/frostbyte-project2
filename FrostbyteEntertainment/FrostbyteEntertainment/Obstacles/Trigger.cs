using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Frostbyte
{
    internal delegate TriggerEventArgs TriggerCondition();
    internal delegate void TriggerHandler(object sender, TriggerEventArgs args);

    internal class TriggerEventArgs : EventArgs
    {
    }

    internal class TriggerSingleTargetEventArgs : TriggerEventArgs
    {
        internal TriggerSingleTargetEventArgs(Sprite target){
            Target = target;
        }
        internal Sprite Target;
    }

    internal class TriggerMultipleTargetEventArgs : TriggerEventArgs
    {
        internal TriggerMultipleTargetEventArgs(List<Sprite> targets)
        {
            Targets = targets;
        }
        internal List<Sprite> Targets;
    }

    internal class Trigger : OurSprite
    {
        internal Trigger(string name, int width, int height)
            : base(name, new Actor(new DummyAnimation(name, width, height)))
        {
            GroundPos = CenterPos;
            UpdateBehavior += Update;
        }

        internal Behavior TriggerUpdate = () => { };
        internal TriggerCondition TriggerCondition = () => { return null; };
        internal event TriggerHandler TriggerEffect = (Object, TriggerSingleTargetEventArgs) => { };
        internal bool Enabled = true;
        

        private new void Update(){
            if (Enabled)
            {
                TriggerUpdate();
                TriggerEventArgs args = TriggerCondition();
                if (args != null)
                {
                    TriggerEffect(this, args);
                }
            }
        }
    }

    internal class PartyCrossTrigger : Trigger
    {
        internal PartyCrossTrigger(string name, int width, int height, List<Sprite> party)
            : base(name, width, height)
        {
            this.party = party;
            base.TriggerUpdate += TriggerUpdate;
            base.TriggerCondition += TriggerCondition;
            base.TriggerEffect += TriggerEffect;

            triggered = new Dictionary<Sprite, bool>();
            foreach (Sprite s in party)
            {
                triggered.Add(s, false);
            }

            triggerRect = new Rectangle(
                0, 
                0,
                (int)(GetAnimation().Width * triggerRectScale),
                (int)(GetAnimation().Height * triggerRectScale));
        }

        /// <summary>
        /// Constructor for the Level Editor
        /// </summary>
        /// <param name="name">Sprite name of Trigger</param>
        /// <param name="initialPos">Position of Trigger</param>
        /// <param name="orientation">Trigger's orientation/direction</param>
        public PartyCrossTrigger(string name, Vector2 initialPosition, Orientations orientation=Orientations.Up)
            : this(name, Tile.TileSize, Tile.TileSize, (This.Game.CurrentLevel as FrostbyteLevel).allies)
        {
            Orientation = orientation;
            Pos = initialPosition;
        }

        private List<Sprite> party;
        private Dictionary<Sprite, bool> triggered;
        private float triggerRectScale = 0.6f;
        private Rectangle triggerRect;

        #region Methods
        private float cross(Vector3 v, Vector3 w)
        {
            return v.X * w.Y - v.Y * w.X;
        }

        private new void TriggerUpdate()
        {
            triggerRect.X = (int)(Pos.X + GetAnimation().Width * (1 - triggerRectScale));
            triggerRect.Y = (int)(Pos.Y + GetAnimation().Height * (1 - triggerRectScale));
            foreach (Sprite target in this.GetTargetsInRectangle(party, triggerRect))
            {
                // http://stackoverflow.com/questions/563198/how-do-you-detect-where-two-line-segments-intersect/565282#565282
                Vector3 normalDir = Vector3.Cross(new Vector3(this.Direction, 0), new Vector3(0, 0, -1));
                Vector3 targetDir = new Vector3(target.Pos - target.PreviousPos, 0);
                targetDir.Normalize();
                Vector3 pmq = new Vector3(target.PreviousPos + target.Center, 0) - new Vector3(this.Pos + this.Center, 0);

                float ta = cross(pmq, targetDir);
                float tb = cross(normalDir, targetDir);
                float t = ta / tb;
                float ua = cross(pmq, normalDir);
                float ub = cross(normalDir, targetDir);
                float u = (ua / ub);

                
                if (u > 0)
                {
                    float cos = this.Direction.X * targetDir.X + this.Direction.Y * targetDir.Y;

                    // Has crossed
                    if (cos > 0)
                    {
                        if (triggered.ContainsKey(target))
                        {
                            triggered[target] = true;
                        }
                    }
                    // Has uncrossed
                    else
                    {
                        if (triggered.ContainsKey(target))
                        {
                            triggered[target] = false;
                        }
                    }
                }
            }
        }

        private new TriggerMultipleTargetEventArgs TriggerCondition()
        {
            if (triggered.Values.All(on => on))
                {
                    return new TriggerMultipleTargetEventArgs(triggered.Keys.ToList());
                }
                return null;
        }

        private new void TriggerEffect(object ths, TriggerEventArgs args)
        {
            Obstacle rock = new Obstacles.Rock("rock");
            rock.CenterOn(this);
            rock.Pos.Y += Tile.TileSize * Math.Sign(-Direction.Y);

            this.Enabled = false;
        }
        #endregion
    }

    internal class RestorePlayerHealthTrigger : Trigger
    {
        internal RestorePlayerHealthTrigger(string name, int width, int height)
            : base(name, width, height)
        {
            base.TriggerUpdate += TriggerUpdate;
            base.TriggerCondition += TriggerCondition;
            base.TriggerEffect += TriggerEffect;

            #region Particles
            Effect particleEffect = This.Game.CurrentLevel.GetEffect("ParticleSystem");
            Texture2D lightning = This.Game.CurrentLevel.GetTexture("sparkball");
            ParticleEmitter particleEmitterTrigger = new ParticleEmitter(1000, particleEffect, lightning);
            particleEmitterTrigger.effectTechnique = "NoSpecialEffect";
            particleEmitterTrigger.blendState = BlendState.Additive;
            particleEmitters.Add(particleEmitterTrigger);
            #endregion

            mAttacks.Add(Attacks.T1Projectile(null, this, 0, 0,
                TimeSpan.MaxValue, new TimeSpan(0, 0, 0, 1, 250),
                3, 3f, false,
                delegate(OurSprite attacker, Vector2 direction, float projectileSpeed, ParticleEmitter particleEmitter)
                {
                    Random rand = new Random();
                    Vector2 tangent = new Vector2(-direction.Y, direction.X);
                    for (int i = -5; i < 6; i++)
                    {
                        float velocitySpeed = rand.Next(50, 85);
                        float accelSpeed = rand.Next(-70, -40);
                        particleEmitter.createParticles(-direction * velocitySpeed + tangent * rand.Next(-100, 100),
                                        Vector2.Zero,
                                        particleEmitter.GroundPos,
                                        10,
                                        200);
                    }
                },
                particleEmitterTrigger).GetEnumerator());
        }

        private List<Sprite> playersInRange;

        private new void TriggerUpdate()
        {
            playersInRange = GetTargetsInRange(This.Game.CurrentLevel.GetSpritesByType(typeof(Player)), GetAnimation().Height);
        }

        private new TriggerMultipleTargetEventArgs TriggerCondition()
        {
            if (playersInRange.Count > 0)
            {
                return new TriggerMultipleTargetEventArgs(playersInRange);
            }
            return null;
        }

        private new void TriggerEffect(object ths, TriggerEventArgs args)
        {
            //This.Game.AudioManager.PlaySoundEffect("regen");
            foreach (Player p in playersInRange)
            {
                p.Regen();
            }
        }
    }
}
