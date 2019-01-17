using System.IO;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Asteroids.Common {
    public class SpriteSheet {

        /// <summary>
        /// The asset file name being referenced.
        /// </summary>
        public string TextureName { get; set; }


        /// <summary>
        /// A unique identifier for the sprite sheet
        /// </summary>
        public string Key { get; set; }


        /// <summary>
        /// The width of an individual frame, in pixels
        /// </summary>
        public int FrameWidth { get; set; }


        /// <summary>
        /// The height of an individual frame, in pixels
        /// </summary>
        public int FrameHeight { get; set; }


        /// <summary>
        /// The offset of the sprites within the image, in pixels.
        /// Defaults to (0, 0)
        /// </summary>
        public Point Offset { get; set; } = new Point(0, 0);


        /// <summary>
        /// The texture being referenced.
        /// </summary>
        public Texture2D Texture { get; private set; }


        /// <summary>
        /// Indicates that the texture has been loaded.
        /// </summary>
        private bool initialized = false;


        /// <summary>
        /// Serialize the sprite sheet to stream
        /// </summary>
        /// <param name="outStream">The stream to serialize to</param>
        /// <returns>'true' on success</returns>
        public bool ToStream(Stream outStream) {
            using (var writer = new BinaryWriter(outStream)) {
                writer.Write(TextureName);
                writer.Write(Key);
                writer.Write(FrameWidth);
                writer.Write(FrameHeight);
                writer.Write(Offset.X);
                writer.Write(Offset.Y);
            }
            return true;
        }


        /// <summary>
        /// Deserialize a sprite sheet from a stream
        /// </summary>
        /// <param name="inStream">The stream to deserialize from</param>
        /// <returns>The loaded SpriteSheet</returns>
        public static SpriteSheet FromStream(Stream inStream) {
            using (var reader = new BinaryReader(inStream)) {
                var res = new SpriteSheet();
                res.TextureName = reader.ReadString();
                res.Key = reader.ReadString();
                res.FrameWidth = reader.ReadInt32();
                res.FrameHeight = reader.ReadInt32();
                int offsetX = reader.ReadInt32();
                int offsetY = reader.ReadInt32();
                res.Offset = new Point(offsetX, offsetY);
                return res;
            }
            return null;
        }


        /// <summary>
        /// Deserialize a sprite sheet from a stream
        /// </summary>
        /// <param name="inStream">The stream to deserialize from</param>
        /// <param name="content">The ContentManager to pull the Texture2D from</param>
        /// <returns>The loaded sprite sheet</returns>
        public static SpriteSheet FromStream(Stream inStream, ContentManager content) {
            var res = FromStream(inStream);
            if (res != null) res.LoadTexture(content);
            return res;
        }


        /// <summary>
        /// Load the texture from the ContentManager
        /// </summary>
        /// <param name="content">The ContentManager to pull the Texture2D from</param>
        /// <returns>'true' on success</returns>
        private bool LoadTexture(ContentManager content) {
            var texture = content.Load<Texture2D>(TextureName);
            if (texture != null) {
                initialized = true;
                Texture = texture;
                return true;
            }
            return false;
        }


        /// <summary>
        /// Create a context with which to use the sprite sheet.
        /// </summary>
        public SpriteSheetContext CreateContext() {
            return new SpriteSheetContext(this);
        }


        public class SpriteSheetContext {

            /// <summary>
            /// The sprite sheet that generated the context.
            /// </summary>
            public SpriteSheet SpriteSheet { get; internal set; }


            /// <summary>
            /// The frame to draw.
            /// </summary>
            public int Frame {
                get => frame;
                set {
                    frame = value;
                    FrameChanged();
                }
            }

            #region Private Fields

            /// <summary>
            /// THe frame to draw;
            /// </summary>
            private int frame;


            /// <summary>
            /// The section of the sprite sheet to draw.
            /// </summary>
            private Rectangle sourceRect;

            #endregion

            internal SpriteSheetContext(SpriteSheet parent) {
                SpriteSheet = parent;
                Frame = 0;
            }


            private void FrameChanged() {
                var framesPerRow = SpriteSheet.Texture.Width / SpriteSheet.FrameWidth;
                sourceRect = new Rectangle((frame % framesPerRow) * SpriteSheet.FrameWidth,
                    (frame / framesPerRow) * SpriteSheet.FrameHeight, SpriteSheet.FrameWidth, SpriteSheet.FrameHeight);
            }
        }
    }
}