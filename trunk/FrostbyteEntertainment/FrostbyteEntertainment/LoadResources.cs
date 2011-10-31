using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Frostbyte
{
    internal partial class Game : Microsoft.Xna.Framework.Game
    {


        void LoadResources(){
            mLevels.Add(new FrostbyteLevel("TitleScreen", Levels.TitleScreen.Load, Levels.TitleScreen.Update, Levels.TitleScreen.Unload, Levels.TitleScreen.CompletionCondition));
            mLevels.Add(new FrostbyteLevel("Intro", Levels.Intro.Load, LevelFunctions.DoNothing, Levels.Intro.Unload, Levels.Intro.CompletionCondition));
            mLevels.Add(new FrostbyteLevel("Earth", Levels.Earth.Load, LevelFunctions.DoNothing, LevelFunctions.DoNothing, delegate() { return false; }));
            //mLevels.Add(new FrostbyteLevel("TitleScreen", Levels.Nemec.Load, Levels.Nemec.Update, delegate() { }, delegate() { return false; }));
        }
    }
}
