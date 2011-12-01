using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Microsoft.Xna.Framework;

namespace Frostbyte.Enemies
{
    internal partial class FrostOwl : Frostbyte.Enemy
    {
        #region Variables
        static List<String> Animations = new List<String>(){
           "owl-idle-down.anim",
           "owl-idle-diagdown.anim",
           "owl-idle-right.anim",
           "owl-idle-diagup.anim",
           "owl-idle-up.anim",
           "owl-idle-down.anim",
           "owl-idle-diagdown.anim",
           "owl-idle-right.anim",
           "owl-idle-diagup.anim",
           "owl-idle-up.anim",
           "owl-idle-down.anim",
           "owl-idle-diagdown.anim",
           "owl-idle-right.anim",
           "owl-idle-diagup.anim",
           "owl-idle-up.anim",
        };

        #endregion Variables

        public FrostOwl(string name, Vector2 initialPosition)
            : base(name, new Actor(Animations), 20, 100)
        {
            SpawnPoint = initialPosition;
            movementStartTime = new TimeSpan(0, 0, 1);
            ElementType = Element.Water;
        }

        protected override void updateMovement()
        {

        }

        protected override void updateAttack()
        {

        }
    }
}
