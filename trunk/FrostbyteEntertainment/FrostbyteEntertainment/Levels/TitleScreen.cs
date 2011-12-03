using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Frostbyte;
using Frostbyte.Obstacles;
using Microsoft.Xna.Framework.Graphics;

namespace Frostbyte.Levels
{
    internal static class TitleScreen
    {
        static readonly TimeSpan RequiredWaitTime = new TimeSpan(0, 0, 0, 0, 0);
        static TimeSpan LevelInitTime = TimeSpan.MinValue;

        internal static void Load(Level context)
        {
            FrostbyteLevel l = context as FrostbyteLevel;
            l.Theme = Element.None;
            LevelInitTime = TimeSpan.MinValue;
            
            Viewport v = This.Game.GraphicsDevice.Viewport;

            /** load music */
            This.Game.AudioManager.AddBackgroundMusic("Music/TitleScreenBG");
            This.Game.AudioManager.PlayBackgroundMusic("Music/TitleScreenBG", 0.1f);

            Text title = new Text("titletext", "Fonts/Title", "Welcome. Please Press Enter/Start.");
            title.CenterOn(new Vector2(v.Width / 2, v.Height / 2));
            title.Static = true;
            title.DisplayColor = Color.Chartreuse;

            context.GetTexture("regen");
            RestorePlayerHealthTrigger t = new RestorePlayerHealthTrigger("trigger", v.Width);
            t.SpawnPoint = new Vector2(v.Width / 2, v.Height);

            Characters.Mage mage = new Characters.Mage("Player 1", PlayerIndex.One, new Color(255, 0, 0));
            mage.Visible = false;
            mage.SpawnPoint = new Vector2(v.Width / 2, v.Height);
            mage.Speed = 0;
            Characters.Mage mage2 = new Characters.Mage("Player 2", PlayerIndex.Two, new Color(114, 255, 255));
            mage2.Visible = false;
            mage2.Speed = 0;
            mage2.SpawnPoint = new Vector2(v.Width / 2, v.Height);
        }

        internal static void Update()
        {
            GameTime gameTime = This.gameTime;
            if (LevelInitTime == TimeSpan.MinValue)
            {
                LevelInitTime = gameTime.TotalGameTime;
                
            }

            bool PlayerPressedStart = false;
            foreach (Sprite sp in (This.Game.CurrentLevel as FrostbyteLevel).allies)
                if ((sp as Frostbyte.Characters.Mage).controller.Start == ReleasableButtonState.Clicked)
                    PlayerPressedStart = true;

            if ((This.Game as FrostbyteGame).GlobalController.Start == ReleasableButtonState.Clicked
                || PlayerPressedStart)
            {
                (This.Game.CurrentLevel as FrostbyteLevel).LevelCompleted = true;
            }
            else if ((This.Game as FrostbyteGame).GlobalController.GetKeypress(Keys.B) == ReleasableButtonState.Clicked)
            {
                This.Game.AudioManager.Pause();
            }
        }
    }
}
