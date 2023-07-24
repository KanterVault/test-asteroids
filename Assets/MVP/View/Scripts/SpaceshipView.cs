using System;
using UnityEngine;

public class SpaceshipView : MonoBehaviour
{
    public event Action<int> OnCollision;

    [SerializeField] private GameObject EngineFireEffect;
    [SerializeField] public AudioSource EngineFireAudio;
    [SerializeField] public AudioSource FireSound;

    private PolygonCollider2D PolygonCollider2D;
    private ContactFilter2D _contactFilter2D;
    private Collider2D[] _collisions = new Collider2D[1];
    
    private void Start()
    {
        _contactFilter2D = new ContactFilter2D().NoFilter();
    }

    private void Update()
    {
        PolygonCollider2D.OverlapCollider(_contactFilter2D, _collisions);
        if (_collisions[0] != null) OnCollision?.Invoke(_collisions[0].GetInstanceID());
        _collisions[0] = null;
    }
}
