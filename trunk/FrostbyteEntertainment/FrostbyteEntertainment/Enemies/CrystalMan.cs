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
        #region Variables
        static List<String> Animations = new List<String>(){
           "crystalman-idle-down.anim",
           "crystalman-idle-diagdown.anim",
           "crystalman-idle-right.anim",
           "crystalman-idle-diagup.anim",
           "crystalman-idle-up.anim",
           "crystalman-teleport-in.anim",
           "crystalman-teleport-out.anim",
           "crystalman-idle-broken-down.anim",
           "crystalman-idle-broken-diagdown.anim",
           "crystalman-idle-broken-right.anim",
           "crystalman-idle-broken-diagup.anim",
           "crystalman-idle-broken-up.anim",
           "crystalman-teleport-in-broken.anim",
           "crystalman-teleport-out-broken.anim",
           "crystalman-shatter-down.anim",
           "crystalman-shatter-diagdown.anim",
           "crystalman-shatter-right.anim",
           "crystalman-shatter-diagup.anim",
           "crystalman-shatter-up.anim",
        };
        private bool changeState = false;
        #endregion Variables

        public CrystalMan(string name, Vector2 initialPosition)
            : base(name, new Actor(Animations), 20, 1000)
        {
            SpawnPoint = initialPosition;
            movementStartTime = new TimeSpan(0, 0, 1);
            ElementType = Element.Lightning;
            Personality = new ShiningPersonality(this);
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
            
        }
    }
}
