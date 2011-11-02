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
            l.Theme = Element.Earth;

            l.HUD.LoadCommon();

            Characters.Mage mage = new Characters.Mage("Player 1", new Actor(l.GetAnimation("shield_opaque.anim")));
            mage.Pos = new Microsoft.Xna.Framework.Vector2(108 * 64, 119 * 64);
            mage.Speed = 1;
            l.HUD.AddPlayer(mage);

            Characters.Mage mage2 = new Characters.Mage("Player 2", new Actor(l.GetAnimation("shield_opaque.anim")));
            mage2.Pos = new Microsoft.Xna.Framework.Vector2(108 * 64, 117 * 64);
            mage2.Speed = 1;
            l.HUD.AddPlayer(mage2);
            mage2.controller = new KeyboardController();

            This.Game.AudioManager.AddBackgroundMusic("Music/GenericBoss");
            This.Game.AudioManager.AddBackgroundMusic("Music/EarthBG");

            Trigger t = new Trigger("trap", 64, 64);
            t.Orientation = Orientations.Up;
            t.CenterOn(mage);
            t.Pos.Y -= 128;

            #region SingleTargetTrigger
            /*t.TriggerCondition = delegate()
            {
                Sprite target = t.GetClosestTarget(l.allies, 10);
                if (target as Player!= null)
                {
                    return new TriggerSingleTargetEventArgs(target);
                }
                return null;
            };

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
                t.Enabled = false;
            };*/
            #endregion

            #region MultiTrigger
            /*Dictionary<Sprite, bool> triggered = new Dictionary<Sprite,bool>();
            foreach (Sprite s in l.allies)
            {
                triggered.Add(s, false);
            }

            t.TriggerUpdate = delegate()
            {
                foreach (Sprite target in t.GetTargetsInRange(l.allies,
                    Math.Max(t.GetAnimation().Height, t.GetAnimation().Width) / 2))
                {
                    // http://stackoverflow.com/questions/563198/how-do-you-detect-where-two-line-segments-intersect/565282#565282
                    Vector3 triggerDir = new Vector3(t.Direction, 0);
                    Vector3 normal3D = Vector3.Cross(triggerDir, new Vector3(0, 0, -1));
                    Vector3 targetDir = new Vector3(target.Pos - target.PreviousPos, 0);
                    targetDir.Normalize();
                    Vector3 v = new Vector3(target.PreviousPos + target.Center, 0) - new Vector3(t.Pos + t.Center, 0);

                    float crossed = (Vector3.Cross(v, normal3D) / Vector3.Cross(normal3D, targetDir)).Z;
                    float dot;
                    Vector3.Dot(ref triggerDir, ref targetDir, out dot);
                    // Has crossed
                    if (crossed > 0 && dot < 0)
                    {
                        if (triggered.ContainsKey(target))
                        {
                            triggered[target] = true;
                            Console.WriteLine("true");
                        }
                    }
                    // Has uncrossed
                    else if (crossed < 0 && dot > 0)
                    {
                        if (triggered.ContainsKey(target))
                        {
                            triggered[target] = false;
                            Console.WriteLine("false");
                        }
                    }
                }
                
            };

            t.TriggerCondition = delegate()
            {
                if (triggered.Values.All(on => on))
                {
                    return new TriggerMultipleEventArgs(triggered.Keys.ToList());
                }
                return null;
            };

            t.TriggerEffect += delegate(object ths, TriggerEventArgs args)
            {
                for (int x = -1; x <= 1; x += 2)
                {
                    Obstacle rockX = new Obstacles.Rock("rock");
                    rockX.CenterOn(t);
                    rockX.Pos.X += x * t.GetAnimation().Width;
                }

                t.Enabled = false;
            };*/
            #endregion

            This.Game.AudioManager.PlayBackgroundMusic("Music/EarthBG");

            #region loadeffects etc
            l.GetEffect("ParticleSystem");
            #endregion loadeffects etc

            #region load textures
            l.GetTexture("boulder");
            #endregion load textures
        }
    }
}
