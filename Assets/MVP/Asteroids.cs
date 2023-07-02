using System;
using System.Numerics;

namespace AsteroidGame
{
    internal class Asteroids : IDisposable
    {
        internal Asteroid[] asteroids = null;
        private DateTime _updateTime;

        private void RandomizeSpawn()
        {
            asteroids = new Asteroid[100];
            for (var i = 0; i < asteroids.Length; i++)
            {
                asteroids[i] = new Asteroid()
                {
                    AsteroidSize = AsteroidSize.Medium,
                    BodyModel = new BodyModel()
                    {
                        Angle = 0.0f,
                        AngleVelocity = 0.0f,
                        Position = new Vector2(0.0f, 0.0f),
                        PositionVelocity = new Vector2(0.0f, 0.0f)
                    }
                };
            }
        }

        public void Init()
        {
            RandomizeSpawn();
        }

        public void Update()
        {
            
        }

        public void Dispose()
        {

        }
    }
}