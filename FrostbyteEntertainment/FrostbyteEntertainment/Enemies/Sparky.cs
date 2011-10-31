using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Microsoft.Xna.Framework;

namespace Frostbyte.Enemies
{
    /// <summary>
    /// Spark ball
    /// </summary>
    internal partial class Sparky : Frostbyte.Enemy
    {
        #region Variables
        bool changeState = false;
        static List<Animation> Animations = new List<Animation>(){
            This.Game.CurrentLevel.GetAnimation("antibody.anim")
        };

        #endregion Variables

        internal Sparky(string name, float speed, int health, Vector2 initialPos)
            : base(name, new Actor(Animations), speed, health)
        {
            Pos = initialPos;
            movementStartTime = new TimeSpan(0, 0, 1);
            ElementType = Element.Lightning;
            Personality = new WanderingMinstrelPersonality(this);
        }

        protected override void updateMovement()
        {
            if (changeState)
            {
                movementStartTime = TimeSpan.MaxValue;
            }
            List<Sprite> targets = This.Game.CurrentLevel.GetSpritesByType("Mage");
            Personality.Update();
        }

        protected override void updateAttack()
        {
            //throw new NotImplementedException();
        }

    }
}
