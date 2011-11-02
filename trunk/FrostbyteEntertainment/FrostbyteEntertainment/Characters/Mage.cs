using System;
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
            CollidesWithBackground = true;
        }

        internal Mage(string name, Actor actor, PlayerIndex input)
            : base(name, actor)
        {
            if (GamePad.GetState(input).IsConnected)
            {
                controller = new GamePadController(input);
            }
            else
            {
                controller = new KeyboardController();
            }
            currentTargetAlignment = TargetAlignment.None;
            This.Game.LoadingLevel.AddAnimation(new Animation("target.anim"));
            target = new Sprite("target", new Actor(new Animation("target.anim")));//new Target("target", Color.Red);
            target.Visible = false;
            sortType = new DistanceSort(this);

            UpdateBehavior = mUpdate;
            CollidesWithBackground = true;

            This.Game.AudioManager.AddSoundEffect("Effects/Sword_Attack");
        }
        #endregion

        #region Variables
        private Sprite currentTarget = null;
        private TargetAlignment currentTargetAlignment;
        internal IController controller;
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
            if (isAttacking)
            {
                mAttack.MoveNext();
                isAttacking = !mAttack.Current;
            }
            else if (isAttackingAllowed)
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
                        Mana -= spellManaCost;
                        return;
                    }
                }
                if (controller.Sword > 0)
                {
                    #region Start Melee Attack
                    float range = 150.0f;
                    List<Sprite> targets = This.Game.CurrentLevel.GetSpritesByType(typeof(Enemy));
                    Sprite target = GetClosestTarget(targets, range);
                    if (target != null)
                    {
                        isAttacking = true;
                        isMovingAllowed = false;
                        mAttack = Attacks.Melee(target, this, 25, 0, 50,TimeSpan.Zero).GetEnumerator();
                        This.Game.AudioManager.PlaySoundEffect("Effects/Sword_Attack");
                    }
                    #endregion Start Melee Attack
                    return;
                }
            }
            if (controller.Interact == ReleasableButtonState.Clicked)
            {
                Item i = new Key("key");
                PickUpItem(i);
                return;
            }
        }

        public void mUpdate()
        {
            controller.Update();

            if (currentTarget != null && !currentTarget.Visible)
            {
                cancelTarget();
            }

            if (controller.IsConnected)
            {
                //necessary for collision
                if (this.CollidesWithBackground)
                    previousFootPos = this.GroundPos;

                #region Targeting
                if (controller.TargetEnemies)
                {
                    if (currentTargetAlignment != TargetAlignment.Enemy)
                    {
                        currentTarget = null;
                    }

                    currentTarget = findMinimum(GetTargetsInRange(
                        (This.Game.CurrentLevel as FrostbyteLevel).enemies,
                        This.Game.GraphicsDevice.Viewport.Width));

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

                    currentTarget = findMinimum(GetTargetsInRange(
                        (This.Game.CurrentLevel as FrostbyteLevel).allies.Concat(
                        (This.Game.CurrentLevel as FrostbyteLevel).obstacles).ToList(),
                        This.Game.GraphicsDevice.Viewport.Width));
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

                if (!(This.Game.CurrentLevel as FrostbyteLevel).enemies.Contains(currentTarget))
                {
                    cancelTarget();
                }
                #endregion Targeting

                #region Movement
                if (isMovingAllowed)
                {
                    PreviousPos = Pos;
                    
                    Pos.X += controller.Movement.X * 3 * Speed;
                    Pos.Y -= controller.Movement.Y * 3 * Speed;

                    State = PreviousPos == Pos ? SpriteState.Idle : SpriteState.Moving;
                }
                #endregion Movement

                //perform collision detection with background
                if (this.CollidesWithBackground)
                    checkBackgroundCollisions();

                attack();
            }
        }
        #endregion
    }
}
