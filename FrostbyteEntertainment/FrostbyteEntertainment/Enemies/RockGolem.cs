using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Frostbyte.Enemies
{
    internal partial class RockGolem : Golem
    {
        #region Variables
        static List<Animation> Animations = new List<Animation>(){
                This.Game.CurrentLevel.GetAnimation("golem-idle-down.anim"),
            This.Game.CurrentLevel.GetAnimation("golem-idle-diagdown.anim"),
            This.Game.CurrentLevel.GetAnimation("golem-idle-right.anim"),
            This.Game.CurrentLevel.GetAnimation("golem-idle-diagup.anim"),
            This.Game.CurrentLevel.GetAnimation("golem-idle-up.anim"),
            This.Game.CurrentLevel.GetAnimation("golem-walk-down.anim"),
            This.Game.CurrentLevel.GetAnimation("golem-walk-diagdown.anim"),
            This.Game.CurrentLevel.GetAnimation("golem-walk-right.anim"),
            This.Game.CurrentLevel.GetAnimation("golem-walk-diagup.anim"),
            This.Game.CurrentLevel.GetAnimation("golem-walk-up.anim"),
            This.Game.CurrentLevel.GetAnimation("golem-attack-down.anim"),
            This.Game.CurrentLevel.GetAnimation("golem-attack-diagdown.anim"),
            This.Game.CurrentLevel.GetAnimation("golem-attack-right.anim"),
            This.Game.CurrentLevel.GetAnimation("golem-attack-diagup.anim"),
            This.Game.CurrentLevel.GetAnimation("golem-attack-up.anim"),
        };
        #endregion Variables
        public RockGolem(string name, Vector2 initialPos)
            : base(name, initialPos, Animations)
        {
            ElementType = Element.Earth;
        }
    }
}
