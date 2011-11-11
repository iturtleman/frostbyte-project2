﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

namespace Frostbyte
{
    /// <summary>
    /// Do anything required for Game-specific code here
    /// to avoid cluttering up the Engine
    /// </summary>
    static class GameData
    {
        internal static int Score { get; set; }
        internal static int NumberOfLives { get; set; }
        internal static readonly int DefaultNumberOfLives = 4;
        internal static int livesAwarded = 0;
    }

    /// <summary>
    /// Enables sorting Sprite lists by distance from an origin Sprite
    /// </summary>
    internal class DistanceSort : IComparer<Sprite>
    {
        Sprite origin;

        internal DistanceSort(Sprite origin)
        {
            this.origin = origin;
        }

        int IComparer<Sprite>.Compare(Sprite x, Sprite y)
        {
            double lx = (x.Pos - origin.Pos).LengthSquared();
            double ly = (y.Pos - origin.Pos).LengthSquared();
            if (lx > ly)
            {
                return 1;
            }
            else if (lx < ly)
            {
                return -1;
            }
            else
            {
                return 0;
            }
        }
    }

    /// <summary>
    /// Add Game-specific level code here to avoid cluttering up the Engine
    /// </summary>
    class FrostbyteLevel : Level
    {
        #region Constructors
        internal Vector2[] PlayerSpawnPoint = new Vector2[2]{
            new Vector2(50, 50),
            new Vector2(60, 50)
        };

        internal FrostbyteLevel(string n, Behavior loadBehavior, Behavior updateBehavior,
            Behavior endBehavior, Condition winCondition)
            : base(n, loadBehavior, updateBehavior, endBehavior, winCondition)
        {
        }
        #endregion

        #region Variables
        /// <summary>
        /// Target lists
        /// </summary>
        internal List<Sprite> allies = new List<Sprite>();
        internal List<Sprite> enemies = new List<Sprite>();
        internal List<Sprite> obstacles = new List<Sprite>();

        internal TileList TileMap = new TileList();
        private Vector3 StartDraw;
        private Vector3 EndDraw;
        private Polygon viewportPolygon = null;
        #endregion

        #region Constants
        private readonly float MAX_ZOOM = 1.0f;
        private readonly float MIN_ZOOM = 0.8f;
        private readonly int BORDER_WIDTH = 200;
        private readonly int BORDER_HEIGHT = 200;
        #endregion

        /// <summary>
        /// A list of levels in the order they should be played through
        /// </summary>
        internal static List<string> LevelProgression = new List<string>()
        {
            "Earth",
            "Wind",
            "Lightning",
            "Fire",
            "Heart"
        };

        /// <summary>
        /// Retains progress through our levels
        /// </summary>
        internal static int CurrentStage = 0;

        /// <summary>
        /// Iterates through every player onscreen to gather the minimum and maximum X and Y coordinates
        /// for any of the players. The new zoom/scale factor is calculated and then the viewport is shifted
        /// if need be.
        /// </summary>
        internal void RealignViewport()
        {
            #region Create Viewport
            if (viewportPolygon == null)
            {
                Viewport viewport = This.Game.GraphicsDevice.Viewport;
                viewportPolygon = new Polygon("viewport", Color.DarkRed, new Vector3[5]{
                    new Vector3(200, 200, 0), 
                    new Vector3(viewport.Width - 200, 200, 0), 
                    new Vector3(viewport.Width - 200, viewport.Height - 200, 0), 
                    new Vector3(200, viewport.Height - 200, 0),
                    new Vector3(200, 200, 0)
                });
                viewportPolygon.Static = true;
            }
            #endregion

            List<Sprite> players = GetSpritesByType(typeof(Player));
            if (players != null && players.Count > 0)
            {
                Vector2 min = new Vector2(float.PositiveInfinity, float.PositiveInfinity);
                Vector2 max = new Vector2(float.NegativeInfinity, float.NegativeInfinity);

                #region Find Min and Max
                foreach (Sprite player in players)
                {
                    if (player.Pos.X < min.X)
                    {
                        min.X = player.Pos.X;
                    }
                    if (player.Pos.Y < min.Y)
                    {
                        min.Y = player.Pos.Y;
                    }
                    if (player.Pos.X + player.GetAnimation().Width > max.X)
                    {
                        max.X = player.Pos.X + player.GetAnimation().Width;
                    }
                    if (player.Pos.Y + player.GetAnimation().Height > max.Y)
                    {
                        max.Y = player.Pos.Y + player.GetAnimation().Height;
                    }
                }
                #endregion

                #region Calculate Zoom Factor
                Viewport viewport = This.Game.GraphicsDevice.Viewport;
                float zoom = This.Game.CurrentLevel.Camera.Zoom;
                Vector2 span = max - min;
                zoom = Math.Min((viewport.Width - 2 * BORDER_WIDTH) / span.X,
                                (viewport.Height - 2 * BORDER_HEIGHT) / span.Y);

                // Normalize values if necessary
                zoom = zoom > MAX_ZOOM ? MAX_ZOOM : zoom;
                zoom = zoom < MIN_ZOOM ? MIN_ZOOM : zoom;
                #endregion

                Vector2 cameraPos = This.Game.CurrentLevel.Camera.Pos;
                #region Shift Viewport
                if (zoom <= MAX_ZOOM && zoom > MIN_ZOOM)
                {
                    Vector2 topLeftCorner = min - cameraPos;
                    Vector2 bottomRightCorner = max - cameraPos;

                    if (topLeftCorner.X < viewport.X + BORDER_WIDTH / zoom)
                    {
                        cameraPos.X += topLeftCorner.X - (viewport.X + BORDER_WIDTH) / zoom;
                    }
                    else if (bottomRightCorner.X > viewport.X + (viewport.Width - BORDER_WIDTH) / zoom)
                    {
                        cameraPos.X += bottomRightCorner.X - (viewport.X + (viewport.Width - BORDER_WIDTH) / zoom);
                    }

                    if (topLeftCorner.Y < viewport.Y + BORDER_HEIGHT / zoom)
                    {
                        cameraPos.Y += topLeftCorner.Y - (viewport.Y + BORDER_HEIGHT) / zoom;
                    }
                    else if (bottomRightCorner.Y > viewport.Y + (viewport.Height - BORDER_HEIGHT) / zoom)
                    {
                        cameraPos.Y += bottomRightCorner.Y - (viewport.Y + (viewport.Height - BORDER_HEIGHT) / zoom);
                    }
                }
                #endregion

                This.Game.CurrentLevel.Camera.Pos = cameraPos;
                This.Game.CurrentLevel.Camera.Zoom = zoom;
            }
        }

        internal override void Update()
        {
            base.Update();

            RealignViewport();

            Vector3 cameraPosition = new Vector3(Camera.Pos, 0);
            Viewport viewport = This.Game.GraphicsDevice.Viewport;
            float zoom = This.Game.CurrentLevel.Camera.Zoom;

            StartDraw = (cameraPosition + new Vector3(viewport.X, viewport.Y, 0)) / Tile.TileSize;
            EndDraw = (cameraPosition + new Vector3(viewport.X + viewport.Width / zoom,
                                                viewport.Y + viewport.Height / zoom, 0)) / Tile.TileSize;
        }
        
        internal override void Draw(GameTime gameTime)
        {
            #region Draw Base Tiles
            This.Game.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Camera.GetTransformation(This.Game.GraphicsDevice));

            for (int x = (int)Math.Floor(StartDraw.X); x < (int)Math.Ceiling(EndDraw.X); x++)
            {
                for (int y = (int)Math.Floor(StartDraw.Y); y < (int)Math.Ceiling(EndDraw.Y); y++)
                {
                    Tile toDraw;
                    TileMap.TryGetValue(x, y, out toDraw);

                    toDraw.Draw();
                }
            }
            This.Game.spriteBatch.End();
            #endregion

            base.Draw(gameTime);

            /// \todo draw bottom tiles
        }
    }
}