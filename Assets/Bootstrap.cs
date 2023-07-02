using UnityEngine;
using AsteroidGame;
using System.Collections.Generic;

public class Bootstrap : MonoBehaviour
{
    [SerializeField] private List<GameObject> dontDestroyOnLoad;

    internal Simulation Simulation { get; private set; }

    private void OnEnable()
    {
#if UNITY_STANDALONE
        Application.targetFrameRate = -1;
        QualitySettings.vSyncCount = 1;
#else
        Application.targetFrameRate = (int)Screen.currentResolution.refreshRateRatio.value;
        QualitySettings.vSyncCount = 0;
#endif

        dontDestroyOnLoad.ForEach(obj => DontDestroyOnLoad(obj));
        Simulation = new Simulation();
        Simulation.OnUpdate += UpdateGame;
        Simulation.Init(delayMs: 100);
    }

    private void UpdateGame(object sender, DeltaTimeEventArgs e)
    {
        Debug.Log($"Test: {e.DeltaTime}");
    }

    private void OnDisable()
    {
        Simulation.Dispose();
#if UNITY_EDITOR
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;
#endif
    }
}
