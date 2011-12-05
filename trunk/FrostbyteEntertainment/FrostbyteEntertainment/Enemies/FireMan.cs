using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Microsoft.Xna.Framework;

namespace Frostbyte.Enemies
{
    internal partial class FireMan : Frostbyte.Boss
    {
        #region Variables
        static List<String> Animations = new List<String>(){
           "firegolem-idle-down.anim",
           "firegolem-idle-diagdown.anim",
           "firegolem-idle-right.anim",
           "firegolem-idle-diagup.anim",
           "firegolem-idle-up.anim",
           "firegolem-walk-down.anim",
           "firegolem-walk-diagdown.anim",
           "firegolem-walk-right.anim",
           "firegolem-walk-diagup.anim",
           "firegolem-walk-up.anim",
           "firegolem-attack-down.anim",
           "firegolem-attack-diagdown.anim",
           "firegolem-attack-right.anim",
           "firegolem-attack-diagup.anim",
           "firegolem-attack-up.anim",
        };

        internal TimeSpan attackWait = TimeSpan.MaxValue;
        #endregion Variables

        public FireMan(string name, Vector2 initialPosition)
            : base(name, new Actor(Animations), 20, 1000)
        {
            SpawnPoint = initialPosition;
            movementStartTime = new TimeSpan(0, 0, 1);
            ElementType = Element.Fire;

            Personality = new LumberingPersonality(this);
        }

        protected override void updateMovement()
        {
            Personality.Update();
        }

        protected override void updateAttack()
        {
            throw new NotImplementedException();
        }
    }
}
