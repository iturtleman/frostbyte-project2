namespace Frostbyte
{
    internal abstract class Obstacle : OurSprite
    {
        internal Obstacle(string name, Actor actor)
            : base(name, actor)
        {
            (This.Game.LoadingLevel as FrostbyteLevel).obstacles.Add(this);
        }
    }

    internal abstract class TargetableObstacle : Obstacle
    {
        internal TargetableObstacle(string name, Actor actor)
            : base(name, actor)
        {

        }
    }
}
