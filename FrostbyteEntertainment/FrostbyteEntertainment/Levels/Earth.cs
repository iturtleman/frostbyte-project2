using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Microsoft.Xna.Framework;

namespace Frostbyte.Levels
{
    internal static class Earth
    {
        internal static void Load(Level context)
        {
            FrostbyteLevel l = context as FrostbyteLevel;
            This.Game.AudioManager.AddBackgroundMusic("Music/EarthBoss");
            XDocument doc = XDocument.Load(@"Content/EarthLevel.xml");
            l.Load(doc);

            l.HUD.LoadCommon();

            Characters.Mage mage = new Characters.Mage("mage", new Actor(l.GetAnimation("shield_opaque.anim")));
            mage.Pos = new Microsoft.Xna.Framework.Vector2(108 * 64, 119 * 64);
            mage.Speed = 1;
            l.HUD.AddPlayer(mage);

            Trigger t = new Trigger("trap", 64, 64);
            t.CenterOn(mage);
            t.Pos.Y -= 128;

            t.TriggerCondition = delegate()
            {
                Sprite target = t.GetClosestTarget(l.allies, 10);
                if (target as Player!= null)
                {
                    return new TriggerSingleTargetEventArgs(target);
                }
                return null;
            };

            This.Game.AudioManager.AddBackgroundMusic("Music/GenericBoss");
            t.TriggerEffect += delegate(object ths, TriggerEventArgs args)
            {
                for (int x = -1; x <= 1; x += 2)
                {
                    Obstacle rockX = new Obstacles.Rock("rock");
                    rockX.CenterOn(t);
                    rockX.Pos.X += x * t.GetAnimation().Width;
                }
                ((args as TriggerSingleTargetEventArgs).Target as Player).Health -= 10;

                This.Game.AudioManager.PlayBackgroundMusic("Music/GenericBoss");
            };

            This.Game.AudioManager.PlayBackgroundMusic("Music/EarthBoss");

            #region loadeffects etc
            l.GetEffect("ParticleSystem");
            #endregion loadeffects etc

            #region load textures
            l.GetTexture("boulder");
            #endregion load textures
        }
    }
}
