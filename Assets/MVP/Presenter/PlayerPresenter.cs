using System;
using System.Linq;
using System.Text;
using UnityEngine;
using AsteroidsGameLogic;
using System.Threading.Tasks;
using System.Collections.Generic;

public class PlayerPresenter : MonoBehaviour, IDisposable
{
    public Action<object> ModelChanged { get; set; }

    [SerializeField] private SpaceshipView spaceshipView;
    [SerializeField] private PlayerData playerData;

    private void Init()
    {
        spaceshipView.OnCollision += SpaceshipView_OnCollision;
    }

    public void Dispose()
    {
        spaceshipView.OnCollision -= SpaceshipView_OnCollision;
    }

    public void Update()
    {
        spaceshipView.gameObject.transform.position = playerData.PlayerPosition;
    }

    private void SpaceshipView_OnCollision(int obj)
    {
        throw new NotImplementedException();
    }
}
