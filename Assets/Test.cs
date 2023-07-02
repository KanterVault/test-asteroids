using TMPro;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class Test : MonoBehaviour
{
    [Header("CheatMenu")]
    [SerializeField] private bool spawnRockTest = false;
    [SerializeField] private bool godMod = false;
    [SerializeField] private bool unlimitHP = false;
    [SerializeField] private bool minigun = false;

    [Space(20)]
    private InputActions _inputActions = null;
    [SerializeField] private SpaceshipView spaceship;
    [SerializeField] private SpaceshipView spaceshipPrefab;
    [SerializeField] private float rotationSpeed = 250.0f;
    [SerializeField] private float accelerate = 0.2f;
    [SerializeField] private float decelerate = 0.075f;
    [SerializeField] private float maxVelocity = 0.3f;
    [SerializeField] private Camera cam;

    [SerializeField] private float dieTimeout = 1.0f;



    [SerializeField] private float rockKillZoneRadius = 1.8f;

    [SerializeField] private int largeScore = 10;
    [SerializeField] private int mediumScore = 40;
    [SerializeField] private int smallScore = 100;

    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float bulletSpeed = 8.0f;
    [SerializeField] private GameObject[] rockPrefabs;
    [SerializeField] private TMP_Text textInfo;
    [SerializeField] private TMP_Text currentScoreText;
    [SerializeField] private GameObject gameOverText;
    [SerializeField] private GameObject[] healthIcons;
    [SerializeField] private int health = 5;
    [SerializeField] private int totalScore = 0;
    [SerializeField] private int currentScore = 0;

    [SerializeField] private float bigRockSpeedRange;
    [SerializeField] private float mediumRockSpeedRange;
    [SerializeField] private float smallRockSpeedRange;

    [SerializeField] private GameObject explosionEffect;
    [SerializeField] private AudioSource gameOverAudio;
    [SerializeField] private AudioSource clickAudio;

    private ContactFilter2D _contactFilter2D;
    private void OnEnable()
    {
        if (PlayerPrefs.HasKey("score"))
        {
            totalScore = PlayerPrefs.GetInt("score", 0);
            PlayerPrefs.Save();
        }

        if (gameOverText.activeInHierarchy) gameOverText.SetActive(false);
        UpdateScoreText();
        UpdateHealthIcons();

        _contactFilter2D = new ContactFilter2D().NoFilter();
        _contactFilter2D.SetLayerMask(LayerMask.GetMask("Rocks"));
        Application.targetFrameRate = 24;
        QualitySettings.vSyncCount = 0;

        _inputActions = new InputActions();
        _inputActions.Player.Move.started += Click;
        _inputActions.Player.Move.performed += Move;
        _inputActions.Player.Move.canceled += Move;
        _inputActions.Player.Fire.performed += Fire;
        _inputActions.Player.Enable();

        StartCoroutine(SpawnRocks());
        StartCoroutine(TextCoroutine());
    }

    private void Click(InputAction.CallbackContext obj)
    {
        clickAudio.Play();
    }

    private IEnumerator TextCoroutine()
    {
        var waiter = new WaitForSeconds(0.1f);
        while (true)
        {
            if (spaceship != null)
            {
                _sb.Clear();
                _sb.AppendLine($"Position: {spaceship.transform.position}");
                _sb.AppendLine($"Angle: {spaceship.transform.eulerAngles.z.ToString("0.00")} degrees");
                _sb.AppendLine($"Speed: {(_currentVelocity.magnitude * 10.0f).ToString("0.00")}/{(maxVelocity * 10.0f).ToString("0.00")}");
                textInfo.text = _sb.ToString();
            }
            else
            {
                textInfo.text = "Game over!";
            }
            yield return waiter;
        }
    }

    private void UpdateScoreText()
    {
        currentScoreText.text = $"SCORE: {currentScore} TOTAL: {totalScore}";
    }

    private void UpdateHealthIcons()
    {
        for (var i = 0; i < healthIcons.Length; i++)
        {
            if (i < health)
            {
                if (!healthIcons[i].activeInHierarchy) healthIcons[i].SetActive(true);
            }
            else
            {
                if (healthIcons[i].activeInHierarchy) healthIcons[i].SetActive(false);
            }
        }
    }

    public enum RockType
    {
        Large,
        Medium,
        Small
    }
    private struct Rock
    {
        public RockType RockType;
        public GameObject Instance;
        public Vector3 Velocity;
    }

    private List<Rock> _rocks = new List<Rock>();
    private IEnumerator SpawnRocks()
    {
        while (_viewportSizeInWorld.magnitude < 0.1f) yield return null;
        for (var i = 0; i < Mathf.Clamp(4 + (int)(currentScore / 2000), 4, 16); i++)
        {
            var pos = Vector3.zero;
            switch (Random.Range(0, 4))
            {
                case 0:
                    {
                        pos = new Vector3(
                            Random.Range(-_viewportSizeInWorld.x, _viewportSizeInWorld.x),
                            _viewportSizeInWorld.y,
                            0.0f);
                        break;
                    }
                case 1:
                    {
                        pos = new Vector3(
                            Random.Range(-_viewportSizeInWorld.x, _viewportSizeInWorld.x),
                            -_viewportSizeInWorld.y,
                            0.0f);
                        break;
                    }
                case 2:
                    {
                        pos = new Vector3(
                            _viewportSizeInWorld.x,
                            Random.Range(-_viewportSizeInWorld.y, _viewportSizeInWorld.y),
                            0.0f);
                        break;
                    }
                case 3:
                    {
                        pos = new Vector3(
                            -_viewportSizeInWorld.x,
                            Random.Range(-_viewportSizeInWorld.y, _viewportSizeInWorld.y),
                            0.0f);
                        break;
                    }
            }

            SpawnRock(RockType.Large, pos);
        }

        while (true)
        {
            if (_rocks.Count == 0)
            {
                StartCoroutine(SpawnRocks());
                yield break;
            }
            _rocks.ForEach(rock =>
            {
                rock.Instance.transform.position += rock.Velocity * Time.deltaTime;
                TeleportFromEdges(rock.Instance.transform, _viewportSizeInWorld, 1.0f);
            });
            yield return null;
        }
    }

    private Vector2 _move = Vector2.zero;
    private void Move(InputAction.CallbackContext obj) => _move = obj.ReadValue<Vector2>();
    private void Fire(InputAction.CallbackContext obj)
    {
        clickAudio.Play();
        if (minigun)
        {
            StartCoroutine(Bullet());
            StartCoroutine(Bullet());
            StartCoroutine(Bullet());
            StartCoroutine(Bullet());
        }
        StartCoroutine(Bullet());
    }

    private void SpawnRock(RockType rockType, Vector3 pos)
    {
        var range = 0;
        var velocityRange = 0.0f;
        switch (rockType)
        {
            case RockType.Large: range = Random.Range(0, 3); velocityRange = bigRockSpeedRange; break;
            case RockType.Medium: range = Random.Range(3, 6); velocityRange = mediumRockSpeedRange; break;
            case RockType.Small: range = Random.Range(6, 9); velocityRange = smallRockSpeedRange; break;
        }
        var rock = new Rock()
        {
            Instance = Instantiate(rockPrefabs[range], pos, Quaternion.AngleAxis(Random.Range(0.0f, 360.0f), Vector3.back)),
            Velocity = new Vector3(
                Random.Range(-velocityRange, velocityRange),
                Random.Range(-velocityRange, velocityRange),
                0.0f),
            RockType = rockType
        };
        _rocks.Add(rock);
    }

    private IEnumerator Bullet()
    {
        if (spaceship == null) yield break;
        spaceship.FireSound.Play();
        var collisions = new Collider2D[1];
        var bullet = Instantiate(bulletPrefab, spaceship.transform.position, spaceship.transform.rotation).GetComponent<PolygonCollider2D>();
        var time = Time.time + 1.0f;
        while (time > Time.time)
        {
            bullet.transform.position += bullet.transform.up * bulletSpeed * Time.deltaTime;
            TeleportFromEdges(bullet.transform, _viewportSizeInWorld, 0.5f);
            collisions[0] = null;
            bullet.OverlapCollider(_contactFilter2D, collisions);
            if (collisions[0] != null)
            {
                var rock = _rocks.FirstOrDefault(f => f.Instance.Equals(collisions[0].gameObject));
                var pos = rock.Instance.transform.position;
                
                if (rock.RockType == RockType.Large)
                {
                    SpawnRock(RockType.Medium, pos);
                    SpawnRock(RockType.Medium, pos);
                    currentScore += largeScore;
                }
                else if (rock.RockType == RockType.Medium)
                {
                    SpawnRock(RockType.Small, pos);
                    SpawnRock(RockType.Small, pos);
                    currentScore += mediumScore;
                }
                else if (rock.RockType == RockType.Small)
                {
                    currentScore += smallScore;
                }
                if (currentScore > totalScore) totalScore = currentScore;
                UpdateScoreText();
                _rocks.Remove(rock);
                Destroy(rock.Instance);
                StartCoroutine(Explosion(pos));
                break;
            }
            yield return null;
        }
        Destroy(bullet.gameObject);
        yield break;
    } 

    private IEnumerator GameOver()
    {
        health = Mathf.Clamp(health - 1, 0, 5);
        UpdateHealthIcons();
        Destroy(spaceship.gameObject);
        _currentAngle = 0.0f;
        _currentVelocity = Vector3.zero;
        if (!gameOverText.activeInHierarchy) gameOverText.SetActive(true);
        if (health == 0) gameOverAudio.Play();
        yield return new WaitForSeconds(dieTimeout);
        if (health == 0) yield break;
        if (gameOverText.activeInHierarchy) gameOverText.SetActive(false);
        spaceship = Instantiate(spaceshipPrefab.gameObject, GetSpawnPointWithNotOverlap(), Quaternion.identity).GetComponent<SpaceshipView>();
    }

    private IEnumerator Explosion(Vector3 pos)
    {
        var expl = Instantiate(explosionEffect, pos, Quaternion.identity);
        yield return new WaitForSeconds(1.0f);
        Destroy(expl);
    }

    private Vector3 _currentVelocity;
    private float _currentAngle;
    private Vector3 _viewportSizeInWorld;
    private StringBuilder _sb = new StringBuilder();
    private Collider2D[] _collisions = new Collider2D[1];
    private void Update()
    {
        if (unlimitHP) health = 5;

        if (spawnRockTest)
        {
            spawnRockTest = false;
            SpawnRock(
                RockType.Large,
                new Vector3(
                    Random.Range(-_viewportSizeInWorld.x, _viewportSizeInWorld.x),
                    Random.Range(-_viewportSizeInWorld.y, _viewportSizeInWorld.y),
                    0.0f));
        }

        if (spaceship == null) return;

        _collisions[0] = null;
        spaceship.PolygonCollider2D.OverlapCollider(_contactFilter2D, _collisions);
        if (_collisions[0] != null && godMod == false)
        {
            StartCoroutine(Explosion(spaceship.transform.position));
            StartCoroutine(GameOver());
            return;
        }

        _viewportSizeInWorld = cam.ViewportToWorldPoint(Vector3.one);
        _currentAngle += _move.x * rotationSpeed * Time.deltaTime;
        spaceship.transform.rotation = Quaternion.AngleAxis(_currentAngle, Vector3.back);

        if (_move.y > 0.1f)
        {
            if (!spaceship.EngineFireEffect.activeInHierarchy) spaceship.EngineFireEffect.SetActive(true);
            if (!spaceship.EngineFireAudio.isPlaying) spaceship.EngineFireAudio.Play();
            _currentVelocity += spaceship.transform.up.normalized * Mathf.Clamp(_move.y, 0.0f, 1.0f) * accelerate * Time.deltaTime;
        }
        else
        {
            if (spaceship.EngineFireEffect.activeInHierarchy) spaceship.EngineFireEffect.SetActive(false);
            if (spaceship.EngineFireAudio.isPlaying) spaceship.EngineFireAudio.Stop();
            if (_currentVelocity.magnitude > 0.01f)
            {
                _currentVelocity -= _currentVelocity.normalized * decelerate * Time.deltaTime;
            }
            else
            {
                _currentVelocity = Vector3.zero;
            }
        }
        _currentVelocity = Vector3.ClampMagnitude(_currentVelocity, maxVelocity);
        spaceship.transform.position += _currentVelocity;

        TeleportFromEdges(spaceship.transform, _viewportSizeInWorld, 0.0f);
    }

    private void TeleportFromEdges(Transform self, Vector3 viewportSizeInWorld, float offset)
    {
        if (self.position.x < -_viewportSizeInWorld.x - offset)
        {
            self.position = new Vector3(
                _viewportSizeInWorld.x + offset,
                self.position.y,
                self.position.z);
        }

        if (self.position.x > _viewportSizeInWorld.x + offset)
        {
            self.position = new Vector3(
                -_viewportSizeInWorld.x - offset,
                self.position.y,
                self.position.z);
        }

        if (self.position.y < -_viewportSizeInWorld.y - offset)
        {
            self.position = new Vector3(
                self.position.x,
                _viewportSizeInWorld.y + offset,
                self.position.z);
        }

        if (self.position.y > _viewportSizeInWorld.y + offset)
        {
            self.position = new Vector3(
                self.position.x,
                -_viewportSizeInWorld.y - offset,
                self.position.z);
        }
    }

    private Vector3 GetSpawnPointWithNotOverlap()
    {
        var currentPos = Vector3.zero;
        for (var i = 0; i < _rocks.Count; i++)
        {
            var rock = _rocks[i];
            if (Vector3.Distance(currentPos, rock.Instance.transform.position) < rockKillZoneRadius)
            {
                currentPos = new Vector3(
                    Random.Range(-_viewportSizeInWorld.x, _viewportSizeInWorld.x),
                    Random.Range(-_viewportSizeInWorld.y, _viewportSizeInWorld.y),
                    0.0f);
                i = 0;
            }
        }
        return currentPos;
    }
    
    private void OnDisable()
    {
        if (currentScore >= totalScore)
        {
            PlayerPrefs.SetInt("score", currentScore);
            PlayerPrefs.Save();
        }
        StopCoroutine(TextCoroutine());
        StopCoroutine(SpawnRocks());
        _inputActions.Player.Disable();
        _inputActions.Player.Move.started -= Click;
        _inputActions.Player.Move.performed -= Move;
        _inputActions.Player.Move.canceled -= Move;
        _inputActions.Player.Fire.performed -= Fire;
        _inputActions.Dispose();
        _inputActions = null;
    }
}
