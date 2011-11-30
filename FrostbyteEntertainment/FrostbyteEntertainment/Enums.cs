using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
/// \file Enums.cs This is Shared with the Level Editor
namespace Frostbyte
{
    // REST OF CLASS LOCATED IN Shared/Enums.cs

    internal static class EnumExtensions
    {
        internal static Vector2 Scale(this Orientations o)
        {
            /*if (o == Orientations.Up_Left)
            {
                return new ScaleTransform(-1, -1);
            }
            else if (o == Orientations.Up)
            {
                return new ScaleTransform(1, -1);
            }
            else if (o == Orientations.Right)
            {
                return new ScaleTransform(-1, 1);
            }
            return new ScaleTransform(1, 1);*/
            if (o == Orientations.Up_Left)
            {
                return new Vector2(-1, -1);
            }
            else if (o == Orientations.Up)
            {
                return new Vector2(1, -1);
            }
            else if (o == Orientations.Right)
            {
                return new Vector2(-1, 1);
            }
            return new Vector2(1, 1);
        }
    }

    internal static class Extensions
    {
        private static Random random = new Random();

        public static T GetRandomElement<T>(this IEnumerable<T> list)
        {
            // If there are no elements in the collection, return the default value of T
            if (list.Count() == 0)
                return default(T);

            return list.ElementAt(random.Next(list.Count()));
        }
    }
}
