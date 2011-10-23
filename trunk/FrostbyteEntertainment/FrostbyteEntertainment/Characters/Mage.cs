﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

namespace Frostbyte.Characters
{
    class Mage : Player
    {
        enum TargetAlignment
        {
            Ally,
            Enemy,
            None
        }

        #region Constructors
        public Mage(string name, Actor actor)
            : this(name, actor, PlayerIndex.One)
        {

        }

        internal Mage(string name, Actor actor, PlayerIndex input)
            : base(name, actor)
        {
            //controller = new GamePadController(input);
            controller = new KeyboardController();
            currentTargetAlignment = TargetAlignment.None;
            target = new Levels.Target("target", Color.Red);
            target.Visible = false;
            sortType = new DistanceSort(this);

            UpdateBehavior = mUpdate;
        }
        #endregion

        #region Variables
        private Sprite currentTarget = null;
        private TargetAlignment currentTargetAlignment;
        private IController controller;
        private Sprite target;
        BasicEffect basicEffect = new BasicEffect(This.Game.GraphicsDevice);
        private IComparer<Sprite> sortType;
        private int spellManaCost = 10;
        #endregion

        #region Methods
        /// <summary>
        /// Finds the closest enemy sprite to the player that's further than the current target
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private Sprite findMinimum(List<Sprite> list)
        {
            if (list.Contains(this))
            {
                list.Remove(this);
            }
            list.Sort(sortType);

            int next = list.IndexOf(currentTarget);
            for (int x = 0; x < list.Count; x++)
            {
                Sprite target = list[(next + 1 + x) % list.Count];
                if (target.Visible)
                {
                    return target;
                }
            }

            cancelTarget(); // Every sprite in list is invisible, there's nothing to target
            return null;
        }

        private void cancelTarget()
        {
            target.Visible = false;
            currentTarget = null;
            currentTargetAlignment = TargetAlignment.None;
        }

        /// <summary>
        /// Chooses and executes the attack.
        /// Ensures that only one attack is performed per update (eg. no sword *and* magic)
        /// </summary>
        private void attack()
        {
            if (Mana >= spellManaCost)
            {
                if (controller.Earth == ReleasableButtonState.Clicked)
                {
                    Mana -= spellManaCost;
                    return;
                }
                else if (controller.Fire == ReleasableButtonState.Clicked)
                {
                    Mana -= spellManaCost;
                    return;
                }
                else if (controller.Lightning == ReleasableButtonState.Clicked)
                {
                    Mana -= spellManaCost;
                    return;
                }
                else if (controller.Water == ReleasableButtonState.Clicked)
                {
                    (This.Game.CurrentLevel as FrostbyteLevel).HUD.ScrollText(
                        "There was a general clapping of hands at this: it was the first really clever " +
                            "thing the King had said that day. `That proves his guilt,' said the Queen. " +
                            "`It proves nothing of the sort!' said Alice. `Why, you don't even know what " +
                            "they're about!' `Read them,' said the King. The White Rabbit put on his spectacles. "+
                            "`Where shall I begin, please your Majesty?' he asked. `Begin at the beginning,' "+
                            "the King said gravely, `and go on till you come to the end: then stop.'");
                    //Mana -= spellManaCost;
                    return;
                }
            }
            if (controller.Sword > 0)
            {

                return;
            }
        }

        public void mUpdate()
        {
            controller.Update();

            if (Health == 100)
            {
                Health = 0;
            }
            Health++;
            
            if (currentTarget != null && !currentTarget.Visible)
            {
                cancelTarget();
            }

            if (controller.IsConnected)
            {
                #region Targeting
                if (controller.TargetEnemies)
                {
                    if (currentTargetAlignment != TargetAlignment.Enemy)
                    {
                        currentTarget = null;
                    }

                    currentTarget = findMinimum((This.Game.CurrentLevel as FrostbyteLevel).enemies);

                    if (currentTarget != null)
                    {
                        currentTargetAlignment = TargetAlignment.Enemy;
                    }
                }
                else if (controller.TargetAllies)
                {
                    if (currentTargetAlignment != TargetAlignment.Ally)
                    {
                        currentTarget = null;
                    }

                    currentTarget = findMinimum((This.Game.CurrentLevel as FrostbyteLevel).allies.Concat(
                        (This.Game.CurrentLevel as FrostbyteLevel).obstacles).ToList());
                    if (currentTarget != null)
                    {
                        currentTargetAlignment = TargetAlignment.Ally;
                    }
                }

                if (controller.CancelTargeting == ReleasableButtonState.Clicked)
                {
                    cancelTarget();
                }

                if (currentTarget != null)
                {
                    target.Visible = true;
                    target.CenterOn(currentTarget);
                }
                #endregion Targeting

                #region Movement

                Pos.X += controller.Movement.X * 3;
                Pos.Y -= controller.Movement.Y * 3;

                #endregion Movement

                attack();
            }
        }
        #endregion
    }
}
