using Microsoft.Xna.Framework;
using MonoGame.Utils.Extensions;

namespace MonoGame.ECS.Components.Physics
{
    /// <summary>
    /// Represents something in motion.
    /// </summary>
    public class Dynamic
    {

        public Vector2 Velocity { get; set; }

        public Vector2 Gravity { get; set; }

        public Dynamic(Vector2 velocity, Vector2 gravity)
        {
            Velocity = velocity.Copy();
            Gravity = gravity.Copy();
        }

        public Dynamic(float velocityX = 0, float velocityY = 0, float gravityX = 0, float gravityY = 0)
        {
            Velocity = new Vector2(velocityX, velocityY);
            Gravity = new Vector2(gravityX, gravityY);
        }

    }
}
