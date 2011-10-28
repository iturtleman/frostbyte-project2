using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Xml.Linq;
using Microsoft.Xna.Framework.Input;
using Frostbyte.Enemies;

namespace Frostbyte.Levels
{
    internal static class Test
    {
        internal static void Load()
        {
            Collision.Lists.Add(new KeyValuePair<int, int>(0, 1));

            FrostbyteLevel l = (This.Game.CurrentLevel != This.Game.NextLevel && This.Game.NextLevel != null ? This.Game.NextLevel : This.Game.CurrentLevel) as FrostbyteLevel;

            //load the level
            XDocument doc = XDocument.Load(@"Content/Level1.xml");
            l.Load(doc);

            //#region Load us some enemies

            //Frostbyte.Enemies.Golem golem = new Frostbyte.Enemies.Golem("Golem", new Vector2(100, 150));
            //Frostbyte.Enemies.Wasp wasp = new Frostbyte.Enemies.Wasp("Wasp", new Vector2(1000, 500));
            //Frostbyte.Enemies.Beetle beetle = new Frostbyte.Enemies.Beetle("Beetle", new Vector2(1000, 400));
            ////beetle.Speed = 1;
            
            //#endregion Load us some enemies


            //LevelFunctions.Spawn(delegate()
            //{
            //    return new FerociousEnemy("e1", new Actor(l.GetAnimation("antibody.anim")), 1.0f, 10);
            //}, 1, new Microsoft.Xna.Framework.Vector2(50, 50));

            /*LevelFunctions.Spawn(delegate()
            {
                return new TestObstacle("e1", new Actor(new DummyAnimation("obstacle", 10, 10)));
            }, 3, new Microsoft.Xna.Framework.Vector2(50, 50));*/

            //Sprite ally = new TestAlly("a1", new Actor(l.GetAnimation("antibody.anim")));
            //ally.Pos = new Vector2(250, 260);

            l.HUD.LoadCommon();

            Characters.Mage mage = new Characters.Mage("mage", new Actor(l.GetAnimation("shield_opaque.anim")));
            mage.Pos = new Microsoft.Xna.Framework.Vector2(269*64, 250*64);
            l.HUD.AddPlayer(mage);
            /*Sprite a = new Sprite("box1", new Actor(new Animation("boxen.anim")),2);

            This.Game.CurrentLevel.AddAnimation(new Animation("boxen.anim"));
            Actor act = new Actor(l.GetAnimation("boxen.anim"));
            Sprite a = new Sprite("box1", act,0);
            a.Pos = new Vector2(15,0);
            This.Game.CurrentLevel.AddAnimation(new Animation("boxen2.anim"));
            act = new Actor(l.GetAnimation("boxen2.anim"));
            Sprite b = new Sprite("box2", act,1);
            b.UpdateBehavior = delegate()
            {
                var key = Keyboard.GetState();
                if(key.IsKeyDown(Keys.A))
                b.Pos.X -= 1;
                if(key.IsKeyDown(Keys.D))
                b.Pos.X += 1;
                if(key.IsKeyDown(Keys.W))
                b.Pos.Y -= 1;
                if(key.IsKeyDown(Keys.S))
                b.Pos.Y += 1;
            };
            Sprite b = new Sprite("box2", new Actor(new Animation("boxen.anim")),3);*/

            #region particles
            //virus = This.Game.Content.Load<Texture2D>("virus2");
            //fire = This.Game.Content.Load<Texture2D>("fire");
            //blueFire = This.Game.Content.Load<Texture2D>("blue fire");
            /*Effect particleEffect = l.GetEffect("ParticleSystem");
            Texture2D fire = l.GetTexture("fire");
            Texture2D blueFire = l.GetTexture("blue fire");
            ParticleEmitter emitter1 = new ParticleEmitter(1000, particleEffect, l.GetTexture("virus2"));
            ParticleEmitter emitter2 = new ParticleEmitter(2000, particleEffect, fire);
            ParticleEmitter emitter3 = new ParticleEmitter(1000, particleEffect, blueFire, fire);
            emitter3.effectTechnique = "ChangePicAndFadeAtPercent";
            emitter3.fadeStartPercent = .8f;
            emitter3.changePicPercent = .5f;
            ParticleEmitter emitter4 = new ParticleEmitter(4000, particleEffect, blueFire, fire);
            emitter4.effectTechnique = "ChangePicAndFadeAtPercent";
            emitter4.fadeStartPercent = .8f;
            emitter4.changePicPercent = .6f;
            emitter1.Static = true;
            emitter2.Static = true;
            emitter3.Static = true;
            emitter4.Static = true;*/
            #endregion particles

        }

        internal static void Update()
        {

        }
    }

    internal class Target : Polygon
    {
        internal Target(string name, Color c)
            : this(name, c, 15)
        {
        }

        internal Target(string name, Color color, int size)
            : base(name, new Actor(new DummyAnimation(name, size, size)), color,
                new Vector3(0, 0, 0),
                new Vector3(size, 0, 0),
                new Vector3(size, size, 0),
                new Vector3(0, size, 0),
                new Vector3(0, 0, 0))
        {
        }
    }

    internal class TestAlly : Player
    {
        internal TestAlly(string name, Actor actor)
            : base(name, actor)
        {
            float height = This.Game.GraphicsDevice.Viewport.Height;
            float width = This.Game.GraphicsDevice.Viewport.Width;
            /*basicEffect.View = Matrix.CreateLookAt(new Vector3(This.Game.GraphicsDevice.Viewport.X + width / 2, This.Game.GraphicsDevice.Viewport.Y + height / 2, -10),
                                                   new Vector3(This.Game.GraphicsDevice.Viewport.X + width / 2, This.Game.GraphicsDevice.Viewport.Y + height / 2, 0), new Vector3(0, -1, 0));
            basicEffect.Projection = Matrix.CreateOrthographic(This.Game.GraphicsDevice.Viewport.Width, This.Game.GraphicsDevice.Viewport.Height, 1, 20);
            basicEffect.VertexColorEnabled = true;*/
        }

        /*BasicEffect basicEffect = new BasicEffect(This.Game.GraphicsDevice);
        VertexPositionColor[] points;

        internal override void Draw(GameTime gameTime)
        {
            foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
            {
                int size = 10;
                points = new VertexPositionColor[5]{
                new VertexPositionColor(new Vector3(Pos.X, Pos.Y, 0), Color.Green),
                new VertexPositionColor(new Vector3(Pos.X + size, Pos.Y, 0), Color.Green),
                new VertexPositionColor(new Vector3(Pos.X + size, Pos.Y + size, 0), Color.Green),
                new VertexPositionColor(new Vector3(Pos.X, Pos.Y + size, 0), Color.Green),
                new VertexPositionColor(new Vector3(Pos.X, Pos.Y, 0), Color.Green)};
                pass.Apply();
                basicEffect.World = Matrix.CreateTranslation(new Vector3(Pos, 0)) * This.Game.CurrentLevel.Camera.GetTransformation(This.Game.GraphicsDevice);
                This.Game.GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineStrip, points, 0, points.Length - 1);
            }
        }*/
    }

    internal class TestObstacle : Obstacle
    {
        internal TestObstacle(string name, Actor actor)
            : base(name, actor)
        {
            float height = This.Game.GraphicsDevice.Viewport.Height;
            float width = This.Game.GraphicsDevice.Viewport.Width;
            basicEffect.View = Matrix.CreateLookAt(new Vector3(This.Game.GraphicsDevice.Viewport.X + width / 2, This.Game.GraphicsDevice.Viewport.Y + height / 2, -10),
                                                   new Vector3(This.Game.GraphicsDevice.Viewport.X + width / 2, This.Game.GraphicsDevice.Viewport.Y + height / 2, 0), new Vector3(0, -1, 0));
            basicEffect.Projection = Matrix.CreateOrthographic(This.Game.GraphicsDevice.Viewport.Width, This.Game.GraphicsDevice.Viewport.Height, 1, 20);
            basicEffect.VertexColorEnabled = true;
        }

        BasicEffect basicEffect = new BasicEffect(This.Game.GraphicsDevice);
        VertexPositionColor[] points;

        internal override void Draw(GameTime gameTime)
        {
            foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
            {
                int size = 10;
                points = new VertexPositionColor[5]{
                new VertexPositionColor(new Vector3(Pos.X, Pos.Y, 0), Color.LightCyan),
                new VertexPositionColor(new Vector3(Pos.X + size, Pos.Y, 0), Color.LightCyan),
                new VertexPositionColor(new Vector3(Pos.X + size, Pos.Y + size, 0), Color.LightCyan),
                new VertexPositionColor(new Vector3(Pos.X, Pos.Y + size, 0), Color.LightCyan),
                new VertexPositionColor(new Vector3(Pos.X, Pos.Y, 0), Color.LightCyan)};
                pass.Apply();
                basicEffect.World = Matrix.Identity;
                This.Game.GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineStrip, points, 0, points.Length - 1);
            }
        }
    }

    internal class FerociousEnemy : Frostbyte.Enemy
    {
        #region Variables

        bool changeState = false;
        TimeSpan idleTime = new TimeSpan(0, 0, 2);


        #endregion Variables

        internal FerociousEnemy(string name, Actor actor, float speed, int _health)
            : base(name, actor, speed, _health)
        {
            movementStartTime = new TimeSpan(0, 0, 1);
        }

        protected override void updateMovement()
        {
            if (changeState)
            {
                movementStartTime = TimeSpan.MaxValue;
            }
            Personality.Update();
        }

        protected override void updateAttack()
        {
            float range = 10f;
            List<Sprite> targets = This.Game.CurrentLevel.GetSpritesByType(typeof(Player));
            Sprite target = GetClosestTarget(targets, range);
            if (target != null)
            {
                // Attack!
            }
        }

        internal override XElement ToXML()
        {
            XElement e = new XElement("Enemy");
            e.SetAttributeValue("Type", this.GetType().ToString());
            e.SetAttributeValue("Name", Name);
            e.SetAttributeValue("Speed", Speed);
            e.SetAttributeValue("Health", Health);
            //add other data about this type of enemy here
            return e;
        }
    }
}
