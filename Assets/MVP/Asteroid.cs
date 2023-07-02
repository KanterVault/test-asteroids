using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsteroidGame
{
    internal enum AsteroidSize
    {
        Large,
        Medium,
        Small
    };

    internal struct Asteroid
    {
        public BodyModel BodyModel;
        public AsteroidSize AsteroidSize;
    }
}
