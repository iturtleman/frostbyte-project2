using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Frostbyte
{
    internal class ElementalBuff : StatusEffect
    {
        internal ElementalBuff(Element e)
            :base(new TimeSpan(0,0,42), LevelFunctions.DoNothing)
        {
            Element = e;
            #region Particles
            Effect particleEffect = This.Game.CurrentLevel.GetEffect("ParticleSystem");
            Texture2D element = This.Game.CurrentLevel.GetTexture("regen");
            switch(e)
            {
                case Frostbyte.Element.Earth:
                    element = This.Game.CurrentLevel.GetTexture("boulder");
                    break;
                case Frostbyte.Element.Lightning:
                    element = This.Game.CurrentLevel.GetTexture("sparkball");
                    break;
                case Frostbyte.Element.Water:
                    element = This.Game.CurrentLevel.GetTexture("water stream");
                    break;
                case Frostbyte.Element.Fire:
                    element = This.Game.CurrentLevel.GetTexture("fire");
                    break;
            }
            particleEmitter = new ParticleEmitter(1000, particleEffect, element);
            particleEmitter.effectTechnique = "NoSpecialEffect";
            particleEmitter.blendState = BlendState.AlphaBlend;
            #endregion            
        }

        Element Element = Element.Normal;

        internal override void Draw(Sprite target)
        {
            base.Draw(target);

            Random rand = new Random();
            int numLayers = 5;
            int size = target.GetAnimation().Height;
            int scale = size;
            switch (Element)
            {
                case Frostbyte.Element.Earth:
                    scale /= 2;
                    break;
                case Frostbyte.Element.Lightning:
                    scale *= 1;
                    break;
                case Frostbyte.Element.Water:
                    scale *= 1;
                    break;
                case Frostbyte.Element.Fire:
                    scale *= 1;
                    break;
            }
            for (int i = 0; i < numLayers; i++)
            {
                double radius = (i + 1) * size / numLayers;
                double theta = rand.NextDouble() * 3 * Element.GetHashCode() * Math.PI - Math.PI;
                if (rand.Next() % 2 == 0)
                    theta = -theta;
                Vector2 origin = new Vector2((float)(radius * Math.Cos(theta)), (float)(radius * Math.Sin(theta)));
                Vector2 velocity = new Vector2(-origin.Y, origin.X);

                velocity.Normalize();
                particleEmitter.createParticles(new Vector2(0, -10 * (i + 1)),
                                (velocity * velocity) / new Vector2((float)radius, (float)radius) * 100,
                                target.Pos + target.Center + origin,
                                scale / numLayers - 1,
                                100);
            }
        }
    }
}
