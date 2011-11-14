using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Microsoft.Xna.Framework;

namespace Frostbyte.Enemies
{
    internal partial class ManInCrystalBoss : Frostbyte.Enemy
    {
        #region Variables
        static List<Animation> Animations = new List<Animation>(){
            This.Game.CurrentLevel.GetAnimation("antibody.anim")
        };

        #endregion Variables

        public ManInCrystalBoss(string name, Vector2 initialPosition)
            : base(name, new Actor(Animations), 20, 1000)
        {
            SpawnPoint = initialPosition;
            movementStartTime = new TimeSpan(0, 0, 1);
            ElementType = Element.Lightning;
        }

        protected override void updateMovement()
        {
            throw new NotImplementedException();
        }

        protected override void updateAttack()
        {
            throw new NotImplementedException();
        }
    }
}
