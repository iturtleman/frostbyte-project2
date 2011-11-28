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
        public CrystalMan(string name, Vector2 initialPosition, float radius=64*6)
            : base(name, new Actor(new DummyAnimation()), 20, 1000)
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
            Personality.Update();
        }

        protected override void updateAttack()
        {

        }
    }

    internal partial class Crystal : OurSprite
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
        };
        #endregion Variables

        public Crystal(string name, Vector2 initialPosition)
            : base(name, new Actor(Animations))
        {

        }


    }
}
