using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.Xna.Framework;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;

namespace Asteroids.Common {
    public class JsonFormation : EnemyFormation {

        public IList<Tuple<int, Vector2>> schedule;

        public string key;

        public JsonFormation() : base() {
            schedule = new List<Tuple<int, Vector2>>();
        }

        public override int TotalTime {
            get {
                if (schedule == null) return 0;
                Tuple<int, Vector2> last = schedule[schedule.Count - 1];
                return last.Item1;
            }
        }

        public override string Key {
            get => key;
        }

        public override IList<Vector2> GetSpawnList(double from, double to) {
            List<Vector2> res = schedule.Where(item => ((double)item.Item1 >= from && (double)item.Item1 <= to))
                                    .Select(item => item.Item2)
                                    .ToList();
            return res;
        }


        /// <summary>
        /// Stream the formation to stream.
        /// </summary>
        /// <param name="outStream">The stream to serialize to</param>
        /// <returns>'true' on success</returns>
        public bool ToStream(Stream outStream) {
            using (var writer = new BinaryWriter(outStream)) {
                writer.Write(Key);
                writer.Write(schedule.Count);
                foreach (var schedule in schedule) {
                    writer.Write(schedule.Item1);
                    writer.Write(schedule.Item2.X);
                    writer.Write(schedule.Item2.Y);
                }
            }
            return true;
        }

        public static JsonFormation FromStream(string fileUri) {
            var res = new JsonFormation();
            try {
                using (var reader = new BinaryReader(File.OpenRead(fileUri))) {
                    res.key = reader.ReadString();
                    var numEntries = reader.ReadInt32();
                    res.schedule = new List<Tuple<int, Vector2>>(numEntries);
                    for (int i = 0; i < numEntries; i++) {
                        int time = reader.ReadInt32();
                        float x = reader.ReadSingle();
                        float y = reader.ReadSingle();
                        var newElem = new Tuple<int, Vector2>(time, new Vector2(x, y));
                        res.schedule.Add(newElem);
                    }
                }
            } catch (Exception ex) {
                // TODO: Handle this with application error handling
                return null;
            }
            return res;
        }
    }
}