using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Frostbyte
{
    internal delegate TriggerEventArgs TriggerCondition();
    internal delegate void TriggerHandler(object sender, TriggerEventArgs args);

    internal class TriggerEventArgs : EventArgs
    {
    }

    internal class TriggerSingleTargetEventArgs : TriggerEventArgs
    {
        internal TriggerSingleTargetEventArgs(Sprite target){
            Target = target;
        }
        internal Sprite Target;
    }

    internal class Trigger : OurSprite
    {
        internal Trigger(string name, int width, int height)
            : base(name, new Actor(new DummyAnimation(name, width, height)))
        {
            UpdateBehavior += Update;
        }

        internal TriggerCondition TriggerCondition = () => { return null; };
        internal event TriggerHandler TriggerEffect = (Object, TriggerSingleTargetEventArgs) => { };
        internal bool Enabled = true;
        

        private new void Update(){
            if (Enabled)
            {
                TriggerEventArgs args = TriggerCondition();
                if (args != null)
                {
                    TriggerEffect(this, args);
                }
            }
        }
    }
}
