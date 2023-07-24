using UnityEngine;
using AsteroidsGameLogic;
using System.Collections.Generic;

public class Bootstrap : MonoBehaviour
{
    [SerializeField] private List<GameObject> dontDestroyOnLoad;

    private void SetupFrameRate()
    {
#if UNITY_STANDALONE
        Application.targetFrameRate = -1;
        QualitySettings.vSyncCount = 1;
#else
        Application.targetFrameRate = (int)Screen.currentResolution.refreshRateRatio.value;
        QualitySettings.vSyncCount = 0;
#endif
    }

    private GlobalGameSM _globalGameStateMachine = null;

    private void OnEnable()
    {
        SetupFrameRate();
        dontDestroyOnLoad.ForEach(obj => DontDestroyOnLoad(obj));

        _globalGameStateMachine = new GlobalGameSM();
        _globalGameStateMachine.ChangeGameState(GlobalGameState.MainMenu);
    }

    private void OnDisable()
    {
        _globalGameStateMachine.ChangeGameState(GlobalGameState.None);
    }
}
