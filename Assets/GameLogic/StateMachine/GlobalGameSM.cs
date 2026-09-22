using System;

namespace AsteroidsGameLogic
{
    public class GlobalGameSM : IDisposable
    {
        private GlobalGameState _currentGlobalGameState { get; set; }

        public GlobalGameSM()
        {
            _currentGlobalGameState = GlobalGameState.None;
        }

        public void ChangeGameState(GlobalGameState gameState)
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