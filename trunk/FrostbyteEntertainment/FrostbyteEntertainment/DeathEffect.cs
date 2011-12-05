using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Frostbyte
{
    internal class DeathEffect : Sprite
    {
        #region Variables
        private Sprite deadSprite;
        private ParticleEmitter particleEmitter;
        private Color[] image;
        private Rectangle imageRegion;
        SpriteFrame frame;
        private float scale;
        private Vector2 topLeftPosition;
        #endregion Variables

        internal DeathEffect(Sprite _deadSprite, ParticleEmitter _particleEmitter, int MilliSecondsToLive)
            : base("DeathEffect", new Actor(new DummyAnimation()))
        {
            deadSprite = _deadSprite;
            particleEmitter = _particleEmitter;

            //Scale of Sprite
            scale = deadSprite.Scale;

            //Sprite Frame
            frame = deadSprite.GetAnimation();

            //Region of Texture that the current frame is in
            imageRegion = new Rectangle((int)frame.StartPos.X, (int)frame.StartPos.Y, frame.Width, frame.Height);

            //Sprite Color Data as Array from Texture2D
            image = new Color[frame.Width * frame.Height];
            frame.Image.GetData<Color>(0, imageRegion, image, 0, frame.Height * frame.Width);

            //Top left position of image (copied from Sprite Draw Function)
            topLeftPosition = deadSprite.Pos - deadSprite.GetAnimation().AnimationPeg +
                            deadSprite.Center - deadSprite.Center /*scale*/ + //this places scaling in the correct spot (i think)
                            (deadSprite.Hflip ? frame.MirrorOffset * 2 : -frame.MirrorOffset);

            createParticles(.02f, .02f);
        }

        private void createParticles(float sampleWidthPercent, float sampleHeightPercent)
        {
            for (float y = 0; y < frame.Height; y += sampleHeightPercent * (float)(frame.Height))
            {
                for (float x = 0; x < frame.Width; x += sampleWidthPercent * (float)(frame.Width))
                {
                    int floorX = (int)Math.Floor(x);
                    int floorY = (int)Math.Floor(y);

                    bool isGray = (Math.Abs((float)(image[floorX + floorY * frame.Height].B - image[floorX + floorY * frame.Height].R)) < 5 &&
                                   Math.Abs((float)(image[floorX + floorY * frame.Height].B - image[floorX + floorY * frame.Height].G)) < 5f);

                    if (image[floorX + floorY * frame.Height].A > 245 || (image[floorX + floorY * frame.Height].A <= 245f && !isGray))
                    {
                        particleEmitter.createParticles(Vector2.Zero,
                                                        Vector2.Zero,
                                                        topLeftPosition + new Vector2((deadSprite.Hflip ? frame.Width - floorX : floorX) * scale, floorY * scale),
                                                        3,
                                                        100000);
                    }
                }
            }
        }

        internal override void Update()
        {
            if (particleEmitter.ActiveParticleCount <= 0)
            {
                This.Game.CurrentLevel.RemoveSprite(this);
            }
        }
    }
}