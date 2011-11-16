﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
/// \file Enums.cs This is Shared with the Level Editor
namespace Frostbyte
{
    /// <summary>
    /// Orientations for any object on screen
    /// </summary>
    public enum Orientations
    {
        /// <summary>
        /// V
        /// </summary>
        Down = 0,
        /// <summary>
        /// <-
        /// </summary>
        Left,
        /// <summary>
        /// ^
        /// </summary>
        Up,
        /// <summary>
        /// ->
        /// </summary>
        Right,
        /// <summary>
        /// \
        /// </summary>
        Up_Left,
        /// <summary>
        /// /
        /// </summary>
        Up_Right,
        /// <summary>
        /// _/
        /// </summary>
        Down_Left,
        /// <summary>
        /// \_
        /// </summary>
        Down_Right
    }

    /// <summary>
    /// Possible tiles for the level
    /// </summary>
    public enum TileTypes
    {
        /// <summary>
        /// This is going to be used to signify an area we don't want people walking (eg the part above walls etc)
        /// </summary>
        DEFAULT = -1,
        /// <summary>
        /// Foor Top or Side wall tiles (determined by orientation)
        /// </summary>
        Wall = 0,
        /// <summary>
        /// Bottom wall tiles 
        /// </summary>
        Bottom,
        /// <summary>
        /// A corner (may be tl, tr, bl, br) \todo mabe distinguish between types of corners (determined by orientation)
        /// </summary>
        Corner,
        /// A corner (may be tl, tr, bl, br) \todo mabe distinguish between types of corners (determined by orientation)
        /// </summary>
        ConvexCorner,
        /// <summary>
        /// This is for floor tiles
        /// </summary>
        Floor,
        /// <summary>
        /// Lava tile
        /// </summary>
        Lava,
        /// <summary>
        /// Stone tile
        /// </summary>
        Stone,
        /// <summary>
        /// Water tile
        /// </summary>
        Water,
        /// <summary>
        /// Sidewall (faces left so we need to set Orientation accordingly to ensure flips)
        /// </summary>
        SideWall,
        /// <summary>
        /// For Room class (needed for editor)
        /// </summary>
        Room,
        /// <summary>
        /// A cell we want specifically empty
        /// </summary>
        Empty,
        /// <summary>
        /// Bottom convex corner for map
        /// </summary>
        BottomConvexCorner,
        /// <summary>
        /// Area where user will never walk
        /// </summary>
        TopArea,
        /// <summary>
        /// Bottom corner for map
        /// </summary>
        BottomCorner,
    }

    /// <summary>
    /// Whether or not the floor is Themed or not (I know it's repetitive but I couldn't think of a better way atm) \todo See if we can get rid of this
    /// </summary>
    public enum FloorTypes
    {
        DEFAULT = -1,
        Themed = 0,
        Water,
        Lava
    }

    /// <summary>
    /// Elements / Themes
    /// </summary>
    public enum Element
    {
        DEFAULT = -1,
        Earth = 0,
        Lightning,
        Water,
        Fire,
        Normal
    }
}