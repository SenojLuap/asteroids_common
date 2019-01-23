using Microsoft.Xna.Framework;

namespace Asteroids.Common {

    public class Transform {
        public Vector2 Position { get; set; } = new Vector2();
        public float Rotation { get; set; } = 0.0f;

        private float scale = 1.0f;
        public float Scale {
            get => scale;
            set {
                scale = value;
                scaleVector = new Vector2(scale);
            }
        }

        private Vector2 scaleVector;
        public Vector2 ScaleVector { get => scaleVector; }
    }
}