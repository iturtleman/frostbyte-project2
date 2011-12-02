using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Microsoft.Xna.Framework;

namespace Frostbyte.Enemies
{
    internal partial class CrystalMan : Frostbyte.Boss
    {
        internal List<Crystal> Crystals;

        float radius = 64 * 7;
        static int numOuterCrystals { get { return 5; } }
        static int crystalHealth { get { return 500; } }
        internal TimeSpan attackWait = TimeSpan.MaxValue;

        public CrystalMan(string name, Vector2 initialPosition)
            : base(name, new Actor(new DummyAnimation()), 20, crystalHealth * (numOuterCrystals + 1))
        {
            SpawnPoint = initialPosition;
            movementStartTime = new TimeSpan(0, 0, 1);
            ElementType = Element.Lightning;

            Personality = new ShiningPersonality(this);
        }

        protected override void Die()
        {
            FrostbyteLevel l = (This.Game.CurrentLevel as FrostbyteLevel);
            l.SpawnExitPortal();
            l.HUD.ScrollText("You feel all the hairs on your body stand on end.\n\n\n\n\n\n\nHold down the Left trigger and press Y (gamepad) W (keyboard) ane then release the trigger to cast a Lightning spell!!!!");
            base.Die();
        }

        protected void updateHealth(object cryst, int health)
        {
            // Update health to sum of each of its crystals' health
            Health = Crystals.Sum(x => x.Health);
        }

        private void init()
        {
            Crystals = new List<Crystal>();

            Crystal inner = new Crystal("crystal_center", SpawnPoint, crystalHealth, this);
            SpriteFrame animation = GetAnimation();
            animation.Height = inner.GetAnimation().Height;
            animation.Width = inner.GetAnimation().Width;
            Crystals.Add(inner);
            inner.HealthChanged += new HealthChangedHandler(updateHealth);

            for (int x = 0; x < numOuterCrystals; x++)
            {
                double angle = 2 * Math.PI * x / numOuterCrystals - Math.PI / 2;
                Vector2 offset = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * radius;
                Crystal outer = new Crystal("crystal_" + x, Pos + offset, crystalHealth, this);
                Crystals.Add(outer);
                outer.HealthChanged += new HealthChangedHandler(updateHealth);
                outer.Direction = -offset;
            }
        }

        protected override void updateMovement()
        {
            if (Crystals == null)
            {
                init();
            }
            Personality.Update();
        }

        protected override void updateAttack()
        {
            if (isAttackAnimDone && attackWait < This.gameTime.TotalGameTime)
            {
                attackWait = TimeSpan.MaxValue;
                foreach (Crystal c in Crystals)
                {
                    mAttacks.Add(Attacks.LightningRod(c, this, 1, 0).GetEnumerator());
                }
                //mAttacks.Add(Attacks.LightningRod(Crystals.GetRandomElement(), this, 5, 0).GetEnumerator());
            }
        }
    }

    internal class Crystal : Enemy
    {
        #region Variables
        static List<String> Animations = new List<String>(){
           "crystalman-idle-down.anim",
           "crystalman-idle-diagdown.anim",
           "crystalman-idle-right.anim",
           "crystalman-idle-diagup.anim",
           "crystalman-idle-up.anim",  // 4
           "crystalman-teleport-in.anim",
           "crystalman-teleport-out.anim",
           "crystalman-idle-broken-down.anim",
           "crystalman-idle-broken-diagdown.anim",
           "crystalman-idle-broken-right.anim",
           "crystalman-idle-broken-diagup.anim",
           "crystalman-idle-broken-up.anim",  // 11
           "crystalman-teleport-in-broken.anim",
           "crystalman-teleport-out-broken.anim",
           "crystalman-shatter-down.anim",
           "crystalman-shatter-diagdown.anim",
           "crystalman-shatter-right.anim",
           "crystalman-shatter-diagup.anim",
           "crystalman-shatter-up.anim",  // 18
           "crystalman-empty.anim",
           "crystalman-empty-broken.anim",
        };
        #endregion Variables

        CrystalMan master;

        public Crystal(string name, Vector2 initialPosition, int health, CrystalMan master)
            : base(name, new Actor(Animations), 1, health)
        {
            this.master = master;
            SpawnPoint = CenteredOn(initialPosition);
            Pos = SpawnPoint;
            Scale = 1.25f;

            HealthChanged += new HealthChangedHandler(delegate(object o, int value)
            {
                if (Health == 0)
                {
                    State = SpriteState.Dead;
                }
                else if (Health <= MaxHealth * 0.5)
                {
                    for (int x = 0; x < 7; x++)
                    {
                        mActor.Animations[x] = mActor.Animations[x + 7];
                    }
                    // REPLACE TO BROKEN ANIMATIONS HERE
                    mActor.Animations[19] = mActor.Animations[20];
                }
            });
        }

        protected override void updateAttack()
        {
        }

        protected override void updateMovement()
        {
        }
    }
}
