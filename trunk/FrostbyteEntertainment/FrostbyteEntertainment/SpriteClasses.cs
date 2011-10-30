using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Frostbyte
{
    public delegate void ManaChangedHandler(object obj, int value);

    internal abstract class Player : OurSprite
    {
        public event ManaChangedHandler ManaChanged = delegate { };

        internal Player(string name, Actor actor)
            : base(name, actor)
        {
            (This.Game.CurrentLevel as FrostbyteLevel).allies.Add(this);
            Mana = MaxMana;

            MaxHealth = 100;
            Health = MaxHealth;

            ItemBag = new List<Item>();
        }

        internal int MaxMana { get { return 100; } }

        /// <summary>
        /// Player's Mana value
        /// </summary>
        private int mMana;
        internal int Mana
        {
            get
            {
                return mMana;
            }
            set
            {
                mMana = value < 0 ? 0 :
                    (value > MaxMana ? MaxMana :
                        value);
                ManaChanged(this, mMana);
            }
        }

        internal int ItemBagCapacity { get { return 10; } }
        internal List<Item> ItemBag;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <returns>Returns true if the item was picked up, false if not.</returns>
        protected bool PickUpItem(Item i)
        {
            if (ItemBag.Count < ItemBagCapacity)
            {
                This.Game.CurrentLevel.RemoveSprite(i);
                ItemBag.Add(i);
                return true;
            }
            return false;
        }
    }

    internal abstract class Obstacle : OurSprite
    {
        internal Obstacle(string name, Actor actor)
            : base(name, actor)
        {
            (This.Game.CurrentLevel as FrostbyteLevel).obstacles.Add(this);
        }
    }

    internal abstract partial class OurSprite : Sprite
    {
        #region Attacking Variables
        public bool isAttackingAllowed = true;
        public bool isMovingAllowed = true;
        public int AttackRange = 0;
        #endregion Attacking Variables


        internal OurSprite(string name, Actor actor)
            : base(name, actor)
        {
            Health = MaxHealth;
        }

        internal OurSprite(string name, Actor actor, int collisionlist)
            : base(name, actor, collisionlist)
        {
            Health = MaxHealth;
        }

        #region Collision
        internal float groundCollisionRadius = 18f;
        protected Vector2 previousFootPos = Vector2.Zero;

        /// <summary>
        /// Check for collision with background and move enemy out of collision with background until no collisions exist
        /// </summary>
        internal void checkBackgroundCollisions()
        {
            if (Vector2.DistanceSquared(previousFootPos, GroundPos) <= 1)
            {
                GroundPos = previousFootPos;
                return;
            }

            Vector2 positiveInfinity = new Vector2(float.PositiveInfinity, float.PositiveInfinity);
            Tuple<Vector2, Vector2> closestObject = new Tuple<Vector2, Vector2>(positiveInfinity, positiveInfinity);
            Vector2 closestIntersection = positiveInfinity;
            Vector2 footPos = this.GroundPos;
            Vector2 originalFootPos = previousFootPos;
            bool isMoved = false;

            while (Vector2.DistanceSquared(footPos,previousFootPos) > 1f)
            {
                detectBackgroundCollisions(footPos, previousFootPos, out closestObject, out closestIntersection);

                if (closestIntersection == positiveInfinity)
                    break;

                if (closestObject.Item2 == positiveInfinity && closestObject.Item1 != positiveInfinity) //this is for a circle
                {
                    Vector2 A1 = closestObject.Item1 - closestIntersection;
                    Vector2 B1 = footPos - closestIntersection;
                    A1.Normalize();
                    Vector2 tangentToA1 = new Vector2(-A1.Y, A1.X);
                    float magnitudeOfTangentToA1 = A1.Length();
                    float magnitudeOfb = B1.Length();
                    float distFromFootToProjIntersection = Vector2.Dot(tangentToA1, B1) / magnitudeOfTangentToA1;
                    Vector2 newFootPos = closestIntersection + distFromFootToProjIntersection * tangentToA1;
                    if (Vector2.DistanceSquared(previousFootPos, closestIntersection) <= Vector2.DistanceSquared(previousFootPos, footPos) && closestIntersection != positiveInfinity)
                    {
                        Vector2 normal = new Vector2(-A1.X, -A1.Y);
                        normal.Normalize();
                        previousFootPos = closestIntersection + 0.2f * normal;
                        footPos = newFootPos + 0.2f * normal;

                        isMoved = true;
                    }
                    else
                    {
                        previousFootPos = footPos;
                    }
                }
                else //this is for a line segment
                {
                    Vector2 A1 = new Vector2();
                    if (closestObject.Item2 == closestIntersection && closestIntersection != positiveInfinity)
                        A1 = closestObject.Item1 - closestIntersection;
                    else
                        A1 = closestObject.Item2 - closestIntersection;
                    Vector2 B1 = footPos - closestIntersection;
                    A1.Normalize();
                    float magnitudeOfa = A1.Length();
                    float magnitudeOfb = B1.Length();
                    float distFromFootToProjIntersection = Vector2.Dot(A1, B1) / magnitudeOfa;
                    Vector2 newFootPos = closestIntersection + distFromFootToProjIntersection * A1;
                    if (Vector2.DistanceSquared(previousFootPos, closestIntersection) <= Vector2.DistanceSquared(previousFootPos, footPos) && closestIntersection != positiveInfinity)
                    {
                        Vector2 tangent = closestObject.Item1 - closestObject.Item2;
                        Vector2 normal = new Vector2(-tangent.Y, tangent.X);
                        normal.Normalize();
                        previousFootPos = closestIntersection + .2f * normal;
                        footPos = newFootPos + .2f * normal;
                        isMoved = true;
                    }
                    else
                    {
                        previousFootPos = footPos;
                    }
                }
            }


            //This takes care of the sprite moving too slow and updates position
            if (isMoved && Vector2.DistanceSquared(footPos, originalFootPos) >= 1.8f)
                this.GroundPos = footPos;
            else if (isMoved)
            {
                this.GroundPos = originalFootPos;
            }
        }

        internal void detectBackgroundCollisions(Vector2 currentPosition, Vector2 previousPosition, out Tuple<Vector2, Vector2> closestObjectOut, out Vector2 closestIntersectionOut)
        {
            float collisionRadius = 18f;    //change this later to correct value***************************************************************************************
            List<Tuple<Vector2, Vector2>> boundaryLineSegments = new List<Tuple<Vector2, Vector2>>();   //line segments to check collision with sprite
            List<Tuple<Vector2, Vector2>> boundaryCircles = new List<Tuple<Vector2, Vector2>>();        //circles to check collision with sprite

            //If previous position is same as current position then no collision is possible
            if (previousPosition == currentPosition)
            {
                closestIntersectionOut = new Vector2(float.PositiveInfinity);
                closestObjectOut = new Tuple<Vector2, Vector2>(new Vector2(float.PositiveInfinity), new Vector2(float.PositiveInfinity));
                return;
            }

            //Add line segments and circles from each tile inside bounding box formed by topLeftMostTile and bottomRightMostTile
            TileList tileMap = (This.Game.CurrentLevel as FrostbyteLevel).TileMap;
            Tuple<int, int> topLeftMostTile = new Tuple<int, int>((int)Math.Floor(((Math.Min(previousPosition.X, currentPosition.X) - collisionRadius) / This.CellSize)),     //top left most tile that could possible hit sprite
                                                                (int)Math.Floor(((Math.Min(previousPosition.Y, currentPosition.Y) - collisionRadius)) / This.CellSize));
            Tuple<int, int> bottomRightMostTile = new Tuple<int, int>((int)Math.Floor((Math.Max(previousPosition.X, currentPosition.X) + collisionRadius) / This.CellSize), //bottom right most tile that could possible hit sprite
                                                                    (int)Math.Floor((Math.Max(previousPosition.Y, currentPosition.Y) + collisionRadius) / This.CellSize));

            for (int x = topLeftMostTile.Item1; x <= bottomRightMostTile.Item1; x++)
                for (int y = topLeftMostTile.Item2; y <= bottomRightMostTile.Item2; y++)
                {
                    Tile tile;
                    tileMap.TryGetValue(x, y, out tile);

                    float tileStartPosX = 0;
                    float tileStartPosY = 0;
                    if (tile.GridCell != null)  //protect collision from tileMap code
                    {
                        tileStartPosX = tile.GridCell.Pos.X;
                        tileStartPosY = tile.GridCell.Pos.Y;
                    }

                    #region Add Tile Boundary Line Segments and Circles to Appropriate Lists
                    switch (tile.Type)
                    {
                        case TileTypes.Wall: //top wall
                            boundaryLineSegments.Add(new Tuple<Vector2, Vector2>(new Vector2(tileStartPosX + This.CellSize, tileStartPosY + This.CellSize + collisionRadius), //add bottom side of tile
                                                                                 new Vector2(tileStartPosX, tileStartPosY + This.CellSize + collisionRadius)));
                            break;
                        case TileTypes.Bottom: //bottom wall
                            boundaryLineSegments.Add(new Tuple<Vector2, Vector2>(new Vector2(tileStartPosX, tileStartPosY + This.CellSize / 2 - collisionRadius), //add top side of tile
                                                                                 new Vector2(tileStartPosX + This.CellSize, tileStartPosY + This.CellSize / 2 - collisionRadius)));
                            break;
                        case TileTypes.SideWall: //side wall
                            if (tile.Orientation == Orientations.Up_Left) //right side wall
                                boundaryLineSegments.Add(new Tuple<Vector2, Vector2>(new Vector2(tileStartPosX - collisionRadius, tileStartPosY + This.CellSize), //add left side of tile
                                                                                     new Vector2(tileStartPosX - collisionRadius, tileStartPosY)));
                            else //left side wall
                                boundaryLineSegments.Add(new Tuple<Vector2, Vector2>(new Vector2(tileStartPosX + This.CellSize + collisionRadius, tileStartPosY), //add left side of tile
                                                                                     new Vector2(tileStartPosX + This.CellSize + collisionRadius, tileStartPosY + This.CellSize)));
                            break;
                        case TileTypes.BottomConvexCorner: //bottom convex corner wall
                            boundaryLineSegments.Add(new Tuple<Vector2, Vector2>(new Vector2(tileStartPosX, tileStartPosY + This.CellSize / 2 - collisionRadius), //add top side of tile
                                                                                 new Vector2(tileStartPosX + This.CellSize, tileStartPosY + This.CellSize / 2 - collisionRadius)));
                            if (tile.Orientation == Orientations.Right) //right bottom convex corner
                            {
                                boundaryLineSegments.Add(new Tuple<Vector2, Vector2>(new Vector2(tileStartPosX - collisionRadius, tileStartPosY + This.CellSize), //add left side of tile
                                                                                     new Vector2(tileStartPosX - collisionRadius, tileStartPosY + This.CellSize / 2)));
                                boundaryCircles.Add(new Tuple<Vector2, Vector2>(new Vector2(tileStartPosX, tileStartPosY + This.CellSize / 2), new Vector2(float.PositiveInfinity, float.PositiveInfinity))); //add top left point of tile
                            }
                            else //left bottom convex corner
                            {
                                boundaryLineSegments.Add(new Tuple<Vector2, Vector2>(new Vector2(tileStartPosX + This.CellSize + collisionRadius, tileStartPosY + This.CellSize / 2), //add right side of tile
                                                                                     new Vector2(tileStartPosX + This.CellSize + collisionRadius, tileStartPosY + This.CellSize)));
                                boundaryCircles.Add(new Tuple<Vector2, Vector2>(new Vector2(tileStartPosX + This.CellSize, tileStartPosY + This.CellSize / 2), new Vector2(float.PositiveInfinity, float.PositiveInfinity))); //add top right point of tile
                            }
                            break;
                        case TileTypes.ConvexCorner: //top convex corner wall
                            boundaryLineSegments.Add(new Tuple<Vector2, Vector2>(new Vector2(tileStartPosX + This.CellSize, tileStartPosY + This.CellSize + collisionRadius), //add bottom side of tile
                                                                                 new Vector2(tileStartPosX, tileStartPosY + This.CellSize + collisionRadius)));
                            if (tile.Orientation == Orientations.Right) //right top convex corner |_
                            {
                                boundaryLineSegments.Add(new Tuple<Vector2, Vector2>(new Vector2(tileStartPosX - collisionRadius, tileStartPosY + This.CellSize), //add left side of tile
                                                                                     new Vector2(tileStartPosX - collisionRadius, tileStartPosY)));
                                boundaryCircles.Add(new Tuple<Vector2, Vector2>(new Vector2(tileStartPosX, tileStartPosY + This.CellSize), new Vector2(float.PositiveInfinity, float.PositiveInfinity))); //add bottom left point of tile
                            }
                            else //left top convex corner _|
                            {
                                boundaryLineSegments.Add(new Tuple<Vector2, Vector2>(new Vector2(tileStartPosX + This.CellSize + collisionRadius, tileStartPosY), //add right side of tile
                                                                                     new Vector2(tileStartPosX + This.CellSize + collisionRadius, tileStartPosY + This.CellSize)));
                                boundaryCircles.Add(new Tuple<Vector2, Vector2>(new Vector2(tileStartPosX + This.CellSize, tileStartPosY + This.CellSize), new Vector2(float.PositiveInfinity, float.PositiveInfinity))); //add bottom right point of tile
                            }
                            break;
                        case TileTypes.BottomCorner: //bottom concave corner wall
                            if (tile.Orientation == Orientations.Right) //right bottom concave corner _|
                            {
                                boundaryLineSegments.Add(new Tuple<Vector2, Vector2>(new Vector2(tileStartPosX - collisionRadius, tileStartPosY + This.CellSize / 2), //add left side of tile
                                                                                     new Vector2(tileStartPosX - collisionRadius, tileStartPosY)));
                            }
                            else //left bottom concave corner |_
                            {
                                boundaryLineSegments.Add(new Tuple<Vector2, Vector2>(new Vector2(tileStartPosX + This.CellSize + collisionRadius, tileStartPosY), //add right side of tile
                                                                                     new Vector2(tileStartPosX + This.CellSize + collisionRadius, tileStartPosY + This.CellSize / 2)));
                            }
                            break;
                        case TileTypes.Corner: //top convex corner wall
                            //add nothing because it is not possible to hit
                            break;
                        default:
                            break;
                    }
                    #endregion Add Tile Boundary Line Segments and Circles to Appropriate Lists
                }

            //If there are no line segments or circle then there are no possible collisions
            if (boundaryLineSegments.Count == 0 && boundaryCircles.Count == 0)
            {
                closestIntersectionOut = new Vector2(float.PositiveInfinity);
                closestObjectOut = new Tuple<Vector2, Vector2>(new Vector2(float.PositiveInfinity), new Vector2(float.PositiveInfinity));
                return;
            }

            #region Calculate closest intersection
            Vector2 positiveInfinity = new Vector2(float.PositiveInfinity, float.PositiveInfinity);
            Tuple<Vector2, Vector2> closestObject = new Tuple<Vector2, Vector2>(positiveInfinity, positiveInfinity);
            float closestDistanceSquared = float.PositiveInfinity;
            Vector2 closestIntersection = positiveInfinity;

            foreach (Tuple<Vector2, Vector2> lineSegment in boundaryLineSegments)   //calculate closest line segment
            {
                float tValue = ((lineSegment.Item2.X - lineSegment.Item1.X) * (previousPosition.Y - lineSegment.Item1.Y) - (lineSegment.Item2.Y - lineSegment.Item1.Y) * (previousPosition.X - lineSegment.Item1.X)) /
                               ((lineSegment.Item2.Y - lineSegment.Item1.Y) * (currentPosition.X - previousPosition.X) - (lineSegment.Item2.X - lineSegment.Item1.X) * (currentPosition.Y - previousPosition.Y));

                Vector2 intersection = new Vector2(previousPosition.X + tValue * (currentPosition.X - previousPosition.X), previousPosition.Y + tValue * (currentPosition.Y - previousPosition.Y));

                float distanceSquared = Vector2.DistanceSquared(previousPosition, intersection);

                intersection.X = (float)Math.Round(intersection.X, 4);  //protect against floating point errors
                intersection.Y = (float)Math.Round(intersection.Y, 4);  //protect against floating point errors

                if (distanceSquared >= 0 && distanceSquared <= closestDistanceSquared && (tValue >= 0)
                    && intersection.X <= Math.Max(lineSegment.Item1.X, lineSegment.Item2.X) && intersection.Y <= Math.Max(lineSegment.Item1.Y, lineSegment.Item2.Y)
                    && intersection.X >= Math.Min(lineSegment.Item1.X, lineSegment.Item2.X) && intersection.Y >= Math.Min(lineSegment.Item1.Y, lineSegment.Item2.Y))
                {
                    closestDistanceSquared = distanceSquared;
                    closestObject = lineSegment;
                    closestIntersection = intersection;
                }
            }

            foreach (Tuple<Vector2, Vector2> circle in boundaryCircles) //calculate closest circle
            {
                Vector2 D = currentPosition - previousFootPos;
                //D.Normalize();
                float A = Vector2.Dot(D, D);
                float B = 2f * Vector2.Dot((previousFootPos - circle.Item1), D);
                float C = Vector2.Dot((previousFootPos - circle.Item1), (previousFootPos - circle.Item1)) - collisionRadius * collisionRadius;

                float discriminant = B * B - 4 * A * C;

                if (discriminant < 0)
                {
                    //there are no intersections
                }
                else if (discriminant == 0)
                {
                    float tValue = (-B) / (2 * A);

                    Vector2 intersection = new Vector2(previousPosition.X + tValue * (currentPosition.X - previousPosition.X), previousPosition.Y + tValue * (currentPosition.Y - previousPosition.Y));

                    float distanceSquared = Vector2.DistanceSquared(previousPosition, intersection);

                    if (distanceSquared >= 0 && distanceSquared <= closestDistanceSquared && tValue >= 0)
                    {
                        closestDistanceSquared = distanceSquared;
                        closestObject = circle;
                        closestIntersection = intersection;
                    }
                }
                else //discriminant > 0
                {
                    float tValue1 = (-B + (float)Math.Sqrt(discriminant)) / (2 * A);
                    float tValue2 = (-B - (float)Math.Sqrt(discriminant)) / (2 * A);

                    float closesttValue = 0;
                    if (tValue1 < tValue2 && tValue1 >= 0)
                    {
                        closesttValue = tValue1;
                    }
                    else if (tValue2 >= 0)
                    {
                        closesttValue = tValue2;
                    }
                    else
                        closesttValue = -1f;


                    Vector2 intersection = new Vector2(previousPosition.X + closesttValue * (currentPosition.X - previousPosition.X), previousPosition.Y + closesttValue * (currentPosition.Y - previousPosition.Y));

                    float distanceSquared = Vector2.DistanceSquared(previousPosition, intersection);

                    if (distanceSquared >= 0 && distanceSquared <= closestDistanceSquared && (closesttValue >= 0))
                    {
                        closestDistanceSquared = distanceSquared;
                        closestObject = circle;
                        closestIntersection = intersection;
                    }
                }
            }
            #endregion Calculate closest intersection


            closestObjectOut = closestObject;
            closestIntersectionOut = closestIntersection;
        }
        #endregion Collision
    }
}
