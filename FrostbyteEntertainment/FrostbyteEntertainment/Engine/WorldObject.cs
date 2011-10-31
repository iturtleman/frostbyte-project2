﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Frostbyte
{
    abstract internal class WorldObject : IComparable<WorldObject>
    {
        internal WorldObject(int z = 0)
        {
            ZOrder = z;
            Visible = true;
            mTransparency = 1;
            mAngle = 0;
            Static = false;
        }

        #region Methods
        /// <summary>
        /// Draws the obejct
        /// </summary>
        /// <param name="gameTime">The gametime for the drawing frame.</param>
        internal abstract void Draw(Microsoft.Xna.Framework.GameTime gameTime);

        /// <summary>
        /// 
        /// </summary>
        internal void DoCollisions()
        {
            CollisionBehavior();
        }

        internal abstract void Update();

        /// <summary>
        /// Aligns the current World Object ('this') with the provided target sprite
        /// </summary>
        /// <param name="target">The World Object to align on</param>
        internal void CenterOn(WorldObject target)
        {
            Pos = CenteredOn(target);
        }

        /// <summary>
        /// Retrieves the correct upper-left position to place the 
        /// current world object if you wanted to center it
        /// directly on the target.
        /// </summary>
        /// <param name="target">The World Object to align on</param>
        /// <returns></returns>
        internal Vector2 CenteredOn(WorldObject target)
        {
            return target.Pos + target.Center - this.Center;
        }

        #endregion Methods

        internal Behavior CollisionBehavior = () => { };

        #region Properties
        internal bool CollidesWithBackground = false;

        /// <summary>
        ///     gets the sprite's name
        /// </summary>
        internal string Name { get { return mName; } }

        /// <summary>
        /// Sets the Sprite's transparency.
        /// </summary>
        /// <param name="f">The sprite's transparancy [0,1] other values will be force set </param>
        internal float Transparency
        {
            get { return mTransparency; }
            set { mTransparency = value > 1 ? 1 : value < 0 ? 0 : value; }
        }
        /// <summary>
        /// Angle in degrees.
        /// </summary>
        /// <returns>Angle in degrees</returns>
        internal float Angle
        {
            get { return mAngle; }
            set
            {
                float a = value;
                mAngle = a;
                while (a > 360)
                    a -= 360;
            }
        }

        /// <summary>
        /// The current (x,y) position
        /// </summary>
        internal Vector2 Pos = new Vector2(0, 0);

        /// <summary>
        /// Stacking order. Determines what draws on top.
        /// </summary>
        internal int ZOrder;

        /// <summary>
        /// Sprite's scale for drawing
        /// </summary>
        internal float Scale = 1;

        /// <summary>
        /// Whether or not we should do horizontal flip
        /// </summary>
        internal bool Hflip { get; set; }

        /// <summary>
        /// Whether or not we should do a vertical flip
        /// </summary>
        internal bool Vflip { get; set; }

        /// <summary>
        /// Determines whether or not the WorldObject is transformed by the camera or not
        /// </summary>
        internal bool Static { get; set; }

        /// <summary>
        /// The centerpoint of the sprite relative to Pos
        /// </summary>
        internal Vector2 Center { get; set; }

        internal Orientations Orientation { get; set; }

        /// <summary>
        /// Returns the actual world position of the centerpoint
        /// </summary>
        internal Vector2 CenterPos
        {
            get
            {
                return Pos + Center;
            }
            set
            {
                Pos = value - Center;
            }
        }

        /// <summary>
        /// This will be the base of the world object relative to Pos (for placing in cells) This is defined as Centerpoint + Centerpoint.Y
        /// </summary>
        internal Vector2 GroundPos
        {
            get
            {
                if (GetCollision().Count > 0)
                {
                    return (this.GetCollision()[0] as Collision_BoundingCircle).Center + Pos;
                }
                else
                {
                    return new Vector2(Center.X, Center.Y * 2) + Pos;
                }
            }
            set
            {
                if (GetCollision().Count > 0)
                {
                    Pos = value - (this.GetCollision()[0] as Collision_BoundingCircle).Center;
                }
                else
                {
                    Pos = value - new Vector2(Center.X, Center.Y * 2);
                }
            }
        }
        #endregion Properties

        #region Variables
        /// <summary>
        /// Sprite's name
        /// </summary>
        protected string mName;

        /// <summary>
        /// Determine if Object should be visible
        /// </summary>
        internal bool Visible;
        /// <summary>
        /// Transparency!
        /// </summary>
        protected float mTransparency;

        /// <summary>
        /// Angle of rotation
        /// </summary>
        protected float mAngle;

        /// <summary>
        /// \todo get rid of this
        /// this is to make it work
        /// </summary>
        internal CollisionObject Col { get; set; }
        #endregion Variables

        /// <summary>
        /// Obtains collision data
        /// </summary>
        /// <returns>this object's collision data</returns>
        internal abstract List<CollisionObject> GetCollision();

        /// <summary>
        /// Obtains collision data
        /// </summary>
        /// <returns>this object's collision data</returns>
        internal abstract List<Vector2> GetHotSpots();

        /// <summary>
        /// Allows sorting
        /// </summary>
        /// <param name="other">value with which to compare</param>
        /// <returns></returns>
        public int CompareTo(WorldObject other)
        {
            int result = GroundPos.Y.CompareTo(other.GroundPos.Y);
            return result == 0 ? ZOrder.CompareTo(other.ZOrder) : result;
        }

        /// <summary>
        /// Collision list for World objects Objects in the same list are not checked against eachother
        /// Background's collision defaults to list 0
        /// All other objects default to 1
        /// </summary>
        internal int CollisionList = 1;


    }
}