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



        public interface ISpriteAnimationContext {

            /// <summary>
            /// The transformation applied to the animation.
            /// </summary>
            Transform Transform { get; set; }


            /// <summary>
            /// The time scale applied to the animation.
            /// </summary>
            double TimeScale { get; set; }


            /// <summary>
            /// Update the animation context.
            /// </summary>
            void Update(GameTime delta);


            /// <summary>
            /// Render the animation.
            /// </summary>
            void Draw(SpriteBatch spriteBatch);
        }
    }
}