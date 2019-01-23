using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Asteroids.Common {

    public abstract class SpriteAnimation {
        /// <summary>
        /// A unique identifier for the animation
        /// </summary>
        public virtual string Key { get; set; }


        /// <summary>
        /// Create a context for utilizing an animation
        /// </summary>
        /// <returns>The generated context</returns>
        public abstract ISpriteAnimationContext CreateContext();



    }
}