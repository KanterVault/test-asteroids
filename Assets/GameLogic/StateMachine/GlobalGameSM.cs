using System;

namespace AsteroidsGameLogic
{
    internal class GlobalGameSM : IDisposable
    {
        private GlobalGameState _currentGlobalGameState { get; set; }

        internal GlobalGameSM()
        {
            _currentGlobalGameState = GlobalGameState.None;
        }

        internal void ChangeGameState(GlobalGameState gameState)
        {
            switch (gameState)
            {
                case GlobalGameState.None:

                    break;
                case GlobalGameState.MainMenu:

                    break;
                case GlobalGameState.GameWorld:

                    break;
            }
        }

        public void Dispose()
        {
            
        }
    }
}