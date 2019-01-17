using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Asteroids {

    public abstract class EnemyFormation {

        public abstract int TotalTime { get; }

        public abstract string Key { get; set;}

        public abstract IList<Vector2> GetSpawnList(double from, double to);

    }

}