using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Frostbyte.Enemies
{
    internal partial class FinalBoss : Frostbyte.Enemy
    {
        #region Variables
        static List<Animation> Animations = new List<Animation>(){
            This.Game.CurrentLevel.GetAnimation("antibody.anim")
        };

        #endregion Variables

        public FinalBoss(string name, float speed, int health)
            : base(name, new Actor(Animations), speed, health)
        {
            movementStartTime = new TimeSpan(0, 0, 1);
            ElementType = Element.Fire;
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
