using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Microsoft.Xna.Framework;

namespace Frostbyte.Enemies
{
    internal partial class CrystalMan : Frostbyte.Enemy
    {
        #region Variables
        static List<Animation> Animations = new List<Animation>(){
            This.Game.CurrentLevel.GetAnimation("crystalman-idle-down.anim"),
            This.Game.CurrentLevel.GetAnimation("crystalman-idle-diagdown.anim"),
            This.Game.CurrentLevel.GetAnimation("crystalman-idle-right.anim"),
            This.Game.CurrentLevel.GetAnimation("crystalman-idle-diagup.anim"),
            This.Game.CurrentLevel.GetAnimation("crystalman-idle-up.anim"),
            This.Game.CurrentLevel.GetAnimation("crystalman-teleport-in.anim"),
            This.Game.CurrentLevel.GetAnimation("crystalman-teleport-out.anim"),
            This.Game.CurrentLevel.GetAnimation("crystalman-idle-broken-down.anim"),
            This.Game.CurrentLevel.GetAnimation("crystalman-idle-broken-diagdown.anim"),
            This.Game.CurrentLevel.GetAnimation("crystalman-idle-broken-right.anim"),
            This.Game.CurrentLevel.GetAnimation("crystalman-idle-broken-diagup.anim"),
            This.Game.CurrentLevel.GetAnimation("crystalman-idle-broken-up.anim"),
            This.Game.CurrentLevel.GetAnimation("crystalman-teleport-in-broken.anim"),
            This.Game.CurrentLevel.GetAnimation("crystalman-teleport-out-broken.anim"),
            This.Game.CurrentLevel.GetAnimation("crystalman-shatter-down.anim"),
            This.Game.CurrentLevel.GetAnimation("crystalman-shatter-diagdown.anim"),
            This.Game.CurrentLevel.GetAnimation("crystalman-shatter-right.anim"),
            This.Game.CurrentLevel.GetAnimation("crystalman-shatter-diagup.anim"),
            This.Game.CurrentLevel.GetAnimation("crystalman-shatter-up.anim"),
        };

        #endregion Variables

        public CrystalMan(string name, Vector2 initialPosition)
            : base(name, new Actor(Animations), 20, 1000)
        {
            SpawnPoint = initialPosition;
            movementStartTime = new TimeSpan(0, 0, 1);
            ElementType = Element.Lightning;
        }

        protected override void updateMovement()
        {
           // throw new NotImplementedException();
        }

        protected override void updateAttack()
        {
           // throw new NotImplementedException();
        }
    }
}
