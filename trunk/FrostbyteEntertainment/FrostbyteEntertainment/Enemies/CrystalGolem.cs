using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Frostbyte.Enemies
{
    internal partial class CrystalGolem : Golem
    {
        #region Variables
        static List<Animation> Animations = new List<Animation>(){
                This.Game.CurrentLevel.GetAnimation("crystalgolem-idle-down.anim"),
            This.Game.CurrentLevel.GetAnimation("crystalgolem-idle-diagdown.anim"),
            This.Game.CurrentLevel.GetAnimation("crystalgolem-idle-right.anim"),
            This.Game.CurrentLevel.GetAnimation("crystalgolem-idle-diagup.anim"),
            This.Game.CurrentLevel.GetAnimation("crystalgolem-idle-up.anim"),
            This.Game.CurrentLevel.GetAnimation("crystalgolem-walk-down.anim"),
            This.Game.CurrentLevel.GetAnimation("crystalgolem-walk-diagdown.anim"),
            This.Game.CurrentLevel.GetAnimation("crystalgolem-walk-right.anim"),
            This.Game.CurrentLevel.GetAnimation("crystalgolem-walk-diagup.anim"),
            This.Game.CurrentLevel.GetAnimation("crystalgolem-walk-up.anim"),
            This.Game.CurrentLevel.GetAnimation("crystalgolem-attack-down.anim"),
            This.Game.CurrentLevel.GetAnimation("crystalgolem-attack-diagdown.anim"),
            This.Game.CurrentLevel.GetAnimation("crystalgolem-attack-right.anim"),
            This.Game.CurrentLevel.GetAnimation("crystalgolem-attack-diagup.anim"),
            This.Game.CurrentLevel.GetAnimation("crystalgolem-attack-up.anim"),
        };
        #endregion Variables

        public CrystalGolem(string name, Vector2 initialPos)
            : base(name, initialPos, Animations)
        {
            ElementType = Element.Lightning;
        }
    }
}
