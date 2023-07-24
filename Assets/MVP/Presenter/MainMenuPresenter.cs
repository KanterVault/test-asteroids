using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuPresenter : MonoBehaviour
{
    private class View_Spaceship { }
    private class View_Rocks { }
    private class View_GameScene { }
    private class View_MainMenu { }
    private class Service_MainMovement
    {
        public Action OnMove;
        public Action OnFire;
    }
    private class Service_PlayerGuns
    {
        public Action OnFire;
    }
    private class Service_RockSpawner { }
    private class Service_InputSystem
    {
        public Action OnMove;
        public Action OnFire;
    }
    private class Model_PlayerData { }
    private class Model_GameData { }
    public enum GameState
    {
        None,
        MainMenu,
        GameWorld
    }








    private SM_GameState sm_gameState;

    private View_Spaceship viewSpaceShip;
    private View_Rocks[] viewRocks;
    private View_MainMenu viewMainMenu;
    private View_GameScene viewGameScene;




    private Model_PlayerData model_PlayerData;
    private Model_GameData model_GameData;

    public void Init()
    {
        sm_gameState.OnChange?.Invoke(GameState.MainMenu);
    }

    private class SM_GameState : IDisposable
    {
        private Service_RockSpawner service_RockSpawner;
        private Service_InputSystem service_InputSystem;
        private Service_MainMovement service_PlayerMovement;
        private Service_PlayerGuns service_PlayerGuns;



        public GameState currentGameState { get; private set; }
        public Action<GameState> OnChange;
        private SM_GameState()
        {
            currentGameState = GameState.None;
            OnChange += OnChangeGS;
        }

        private void OnChangeGS(GameState gameState)
        {
            switch (gameState)
            {
                case GameState.None:    
               
                    break;
                case GameState.MainMenu:

                    break;
                case GameState.GameWorld:
                    service_InputSystem.OnMove += service_PlayerMovement.OnMove;
                    service_InputSystem.OnFire += service_PlayerGuns.OnFire;
                    break;
            }
        }

        public void Dispose() => OnChange -= OnChangeGS;
    }

    public void Dispose()
    {

    }
}
