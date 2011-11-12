using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Frostbyte
{
    internal abstract class Player : OurSprite
    {
        public event ManaChangedHandler ManaChanged = delegate { };
        internal enum TargetAlignment
        {
            Ally,
            Enemy,
            None
        }

        internal Player(string name, Actor actor)
            : base(name, actor)
        {
            (This.Game.LoadingLevel as FrostbyteLevel).allies.Add(this);
            Mana = MaxMana;

            MaxHealth = 100;
            Health = MaxHealth;

            ItemBag = new List<Item>();
        }

        #region Targeting
        public Sprite currentTarget { get; protected set; }
        public TargetAlignment currentTargetAlignment { get; protected set; }
        #endregion

        #region Mana
        internal int MaxMana { get { return 100; } }
        internal TimeSpan ManaRegenRate = new TimeSpan(0, 0, 2);
        internal float ManaRegenScale = 0.1f;
        private TimeSpan ElapsedManaRegenTime;

        /// <summary>
        /// Player's Mana value
        /// </summary>
        private int mMana;
        internal int Mana
        {
            get
            {
                return mMana;
            }
            set
            {
                mMana = value < 0 ? 0 :
                    (value > MaxMana ? MaxMana :
                        value);
                ManaChanged(this, mMana);
            }
        }
        #endregion

        #region Items
        internal int ItemBagCapacity { get { return 10; } }
        internal List<Item> ItemBag;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <returns>Returns true if the item was picked up, false if not.</returns>
        protected bool PickUpItem(Item i)
        {
            if (ItemBag.Count < ItemBagCapacity)
            {
                This.Game.CurrentLevel.RemoveSprite(i);
                ItemBag.Add(i);
                return true;
            }
            return false;
        }
        #endregion

        internal override void Regen()
        {
            base.Regen();
            Mana = MaxMana;
        }

        internal override void Update()
        {
            base.Update();
            ElapsedManaRegenTime += This.gameTime.ElapsedGameTime;
            if (ElapsedManaRegenTime > ManaRegenRate)
            {
                Mana += (int)(ManaRegenScale * MaxMana);
                ElapsedManaRegenTime = new TimeSpan();
            }
        }
    }
}
