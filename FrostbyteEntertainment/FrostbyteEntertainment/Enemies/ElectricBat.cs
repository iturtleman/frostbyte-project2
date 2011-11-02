using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Microsoft.Xna.Framework;

namespace Frostbyte.Enemies
{
    internal partial class ElectricBat : Frostbyte.Enemy
    {
        bool changeState = false;
        TimeSpan idleTime = new TimeSpan(0, 0, 2);

        #region Variables
        static List<Animation> Animations = new List<Animation>(){
            This.Game.CurrentLevel.GetAnimation("bat-down.anim"),
            This.Game.CurrentLevel.GetAnimation("bat-diagdown.anim"),
            This.Game.CurrentLevel.GetAnimation("bat-right.anim"),
            This.Game.CurrentLevel.GetAnimation("bat-diagup.anim"),
            This.Game.CurrentLevel.GetAnimation("bat-up.anim"),
            This.Game.CurrentLevel.GetAnimation("bat-down.anim"),
            This.Game.CurrentLevel.GetAnimation("bat-diagdown.anim"),
            This.Game.CurrentLevel.GetAnimation("bat-right.anim"),
            This.Game.CurrentLevel.GetAnimation("bat-diagup.anim"),
            This.Game.CurrentLevel.GetAnimation("bat-up.anim"),
            This.Game.CurrentLevel.GetAnimation("bat-down.anim"),
            This.Game.CurrentLevel.GetAnimation("bat-diagdown.anim"),
            This.Game.CurrentLevel.GetAnimation("bat-right.anim"),
            This.Game.CurrentLevel.GetAnimation("bat-diagup.anim"),
            This.Game.CurrentLevel.GetAnimation("bat-up.anim"),
        };

        #endregion Variables

         internal ElectricBat(string name, Vector2 initialPos)
            : base(name, new Actor(Animations), 20, 100)
        {
            movementStartTime = new TimeSpan(0, 0, 1);
            ElementType = Element.Lightning;
            GroundPos = initialPos;
            Personality = new PseudoWanderPersonality(this);

            This.Game.AudioManager.AddSoundEffect("Effects/Bat_Move");
            MovementAudio = This.Game.AudioManager.InitializeLoopingSoundEffect("Effects/Bat_Move");
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
            //throw new NotImplementedException();
        }
    }
}
