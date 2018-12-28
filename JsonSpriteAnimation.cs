using System;
using System.IO;

namespace Asteroids.Common {

    public partial class JsonSpriteAnimation : SpriteAnimation {

        /// <summary>
        /// The key of the SpriteSheet the animation uses
        /// </summary>
        public string spriteSheetKey;


        /// <summary>
        /// Should the animation reverse at the end ("bounce") or restart at
        /// the beginning
        /// </summary>
        public bool bounce;


        /// <summary>
        /// The frames to display, and the time to display them, in seconds
        /// </summary>
        public (int, double)[] frames;

        public JsonSpriteAnimation() { }


        /// <summary>
        /// Stream the SpriteAnimation to file
        /// </summary>
        /// <param name="uri">The file the SpriteAnimation should be streamed to</param>
        /// <returns>'true' on success</returns>
        public bool ToStream(Stream outStream) {
            using (var writer = new BinaryWriter(outStream)) {
                writer.Write(Key);
                writer.Write(spriteSheetKey);
                writer.Write(bounce);
                writer.Write(frames.Length);
                foreach (var frame in frames) {
                    writer.Write(frame.Item1);
                    writer.Write(frame.Item2);
                }
            }
            return true;
        }


        /// <summary>
        /// Parse an Animation from file
        /// </summary>
        /// <param name="uri">The file to parse</param>
        /// <returns>The parsed Animation</returns>
        public static SpriteAnimation FromStream(Stream inStream) {
            using (var reader = new BinaryReader(inStream)) {
                var res = new JsonSpriteAnimation();
                res.Key = reader.ReadString();
                res.spriteSheetKey = reader.ReadString();
                res.bounce = reader.ReadBoolean();
                int numFrames = reader.ReadInt32();
                res.frames = new (int, double)[numFrames];
                for (int i = 0; i < numFrames; i++) {
                    int frame = reader.ReadInt32();
                    double frameLength = reader.ReadDouble();
                    res.frames[i] = (frame, frameLength);
                }
                return res;
            }
        }
    }
}