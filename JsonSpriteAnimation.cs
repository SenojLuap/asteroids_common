using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Asteroids.Common {

    public class JsonSpriteAnimation : SpriteAnimation {

        /// <summary>
        /// The key of the SpriteSheet the animation uses
        /// </summary>
        public string spriteSheetKey;


        /// <summary>
        /// The frames to display, and the time to display them, in seconds.
        /// </summary>
        public Frame Frames { get; set; }


        /// <summary>
        /// The SpriteSheet the animation uses
        /// </summary>
        public SpriteSheet SpriteSheet {
            get; private set;
        }


        /// <summary>
        /// Has the animation been intitialized
        /// </summary>
        public bool Initialized { get => SpriteSheet != null; }


        public JsonSpriteAnimation() { }


        public void Initialize(ISpriteSheetProvider provider) {
            SpriteSheet = provider.GetSpriteSheetByKey(spriteSheetKey);
        }

        override public ISpriteAnimationContext CreateContext() {
            if (!Initialized) throw new InvalidOperationException("Animation has not been initialized");
            return new JsonSpriteAnimationContext(this);
        }


        /// <summary>
        /// Does the animation bounce at the end.
        /// </summary>
        public bool Bounces() {
            var visited = new List<Frame>();
            Frame lastFrame = Frames;
            visited.Add(lastFrame);
            while (!visited.Contains(lastFrame.Next)) {
                visited.Add(lastFrame.Next);
                lastFrame = lastFrame.Next;
            }
            return lastFrame.Next != Frames;
        }


        /// <summary>
        /// The number of frames in the animation
        /// </summary>
        public int FrameCount() {
            var visited = new HashSet<Frame>();
            var frame = Frames;
            while (!visited.Contains(frame)) {
                visited.Add(frame);
                frame = frame.Next;
            }
            return visited.Count;
        }


        public class JsonSpriteAnimationContext : ISpriteAnimationContext {


            /// <summary>
            /// The time scale applied to the animation.
            /// </summary>
            public double TimeScale { get; set; } = 1.0;


            /// <summary>
            /// The current frame of the animation.
            /// </summary>
            public Frame CurrentFrame { get; set; }


            /// <summary>
            /// The current width of the animation, in pixels.
            /// </summary>
            public int Width {
                get => SpriteSheetContext.Width;
                set {
                    throw new NotSupportedException();
                }
            }


            /// <summary>
            /// The current height of the animation, in pixels.
            /// </summary>
            public int Height {
                get => SpriteSheetContext.Height;
                set {
                    throw new NotSupportedException();
                }
            }


            /// <summary>
            /// The transform applied to the animation.
            /// </summary>
            public Transform Transform {
                get => SpriteSheetContext.Transform;
                set
                {
                    SpriteSheetContext.Transform = value;
                }
            }


            #region Private Fields

            // The sprite sheet context
            private SpriteSheet.SpriteSheetContext SpriteSheetContext;

            private double time;

            #endregion


            internal JsonSpriteAnimationContext(JsonSpriteAnimation animation) {
                SpriteSheetContext = animation.SpriteSheet.CreateContext();
                CurrentFrame = animation.Frames;
            }

            public void Draw(SpriteBatch spriteBatch) {
                SpriteSheetContext.Draw(spriteBatch);
            }

            public void Update(GameTime delta) {
                time += delta.ElapsedGameTime.TotalSeconds * TimeScale;
                if (time >= CurrentFrame.Duration) {
                    time -= CurrentFrame.Duration;
                    CurrentFrame = CurrentFrame.Next;
                    SpriteSheetContext.Frame = CurrentFrame.SpriteSheetFrame;
                }
            }
        }


        /// <summary>
        /// Frame class, used to optimize resolving animation frames
        /// </summary>
        public class Frame {

            /// <summary>
            /// The current sprite frame to display
            /// </summary>
            public int SpriteSheetFrame { get; set; }


            /// <summary>
            /// The duration of the frame
            /// </summary>
            public double Duration { get; set; }


            /// <summary>
            /// The next frame in the sequence
            /// </summary>
            public Frame Next { get; set; }


            public Frame(int frame, double duration) {
                this.SpriteSheetFrame = frame;
                this.Duration = duration;
            }
        }

        #region Streaming

        /// <summary>
        /// Stream the SpriteAnimation to file
        /// </summary>
        /// <param name="uri">The file the SpriteAnimation should be streamed to</param>
        /// <returns>'true' on success</returns>
        public bool ToStream(Stream outStream) {
            using (var writer = new BinaryWriter(outStream)) {
                writer.Write(Key);
                writer.Write(spriteSheetKey);
                writer.Write(Bounces());
                int frameCount = FrameCount();
                writer.Write(frameCount);
                var frame = Frames;
                for (int i = 0; i < frameCount; i++) {
                    writer.Write(frame.SpriteSheetFrame);
                    writer.Write(frame.Duration);
                    frame = frame.Next;
                }
            }
            return true;
        }


        /// <summary>
        /// Parse an Animation from file
        /// </summary>
        /// <param name="uri">The file to parse</param>
        /// <returns>The parsed Animation</returns>
        public static JsonSpriteAnimation FromStream(Stream inStream) {
            using (var reader = new BinaryReader(inStream)) {
                var res = new JsonSpriteAnimation();
                res.Key = reader.ReadString();
                res.spriteSheetKey = reader.ReadString();

                var bounce = reader.ReadBoolean();
                int numFrames = reader.ReadInt32();

                var frames = new List<(int, double)>();
                for (int i = 0; i < numFrames; i++) {
                    int frame = reader.ReadInt32();
                    double frameLength = reader.ReadDouble();
                    frames.Add((frame, frameLength));
                }
                res.Frames = CreateFrames(bounce, frames.ToArray());

                return res;
            }
        }


        public static Frame CreateFrames(bool bounce, (int, double)[] frames) {
            Frame firstFrame = null;
            Frame lastFrame = null;
            Frame lastBounceFrame = null;

            foreach (var frameInfo in frames) {
                int frame = frameInfo.Item1;
                double frameLength = frameInfo.Item2;
                var newFrame = new Frame(frame, frameLength);

                if (lastFrame == null) {
                    firstFrame = newFrame;
                    lastBounceFrame = newFrame;
                }
                else {
                    lastFrame.Next = newFrame;
                    if (bounce) {
                        var newBounceFrame = new Frame(frame, frameLength);
                        newBounceFrame.Next = lastBounceFrame;
                        lastBounceFrame = newBounceFrame;
                    }
                }
                lastFrame = newFrame;
            }

            if (bounce) {
                lastFrame.Next = lastBounceFrame.Next;
            }
            else {
                lastFrame.Next = firstFrame;
            }
            return firstFrame;
        }

        #endregion

    }
}