using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [SerializeField]
    Transform      _enemyContainer;

    [SerializeField]
    float           TIME_BTW_WAVES = 15.0f;

    [SerializeField]
    float           WAVE_DURATION = 45.0f;

    [SerializeField]
    [Range(0.1f, 1.0f)]
    float           ENEMIES_MULTIPLIER = 0.2f;

    [SerializeField]
    int             ENEMIES_TO_SPAWN = 10;

    [SerializeField]
    Slider          _healthBar;
    [SerializeField]
    Slider          _repairBar;


    
    Transform[]     _spawnPoints;
    GameObject      _enemyPrefab;
    Player          _player;

    float           _time;
    float           _gameTime;
    int             _round;
    int             _enemiesToSpawn;
    int             _numberOfEnemies;

    bool            _spawnWave;


    void Awake()
    {
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        _healthBar.value = _player.HPFraction;
        _repairBar.value = _player.RepairFraction;

        GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("Respawn");
        _spawnPoints = new Transform[spawnPoints.Length];

        for (int i = 0; i < spawnPoints.Length; i++)
        {
            _spawnPoints[i] = spawnPoints[i].transform;
        }

        _time = 0;
        _gameTime = 0;
        _round = 0;

        _numberOfEnemies = ENEMIES_TO_SPAWN;
        _enemiesToSpawn = _numberOfEnemies;

        _enemyPrefab = Resources.Load<GameObject>("Enemy");

        AudioManager.Instance().PlayMusic(AudioManager.Music.AMBIANCE);
    }



    void Update()
    {
        _time += Time.deltaTime;
        _gameTime += Time.deltaTime;

        UpdateWaves();
        UpdateUI();
    }


    private void UpdateWaves()
    {
        if (_spawnWave)
        {
            if (_time >= WAVE_DURATION)
            {
                AudioManager.Instance().PlayMusic(AudioManager.Music.AMBIANCE);
                AudioManager.Instance().PlaySoundEffect(AudioManager.SoundEffect.WAVE_END);

                _spawnWave = false;
                _round++;
                _time = 0.0f;
            }
        }
        else
        {
            if (_time >= TIME_BTW_WAVES)
            {
                StartWave();
            }
        }
    }

    private void StartWave()
    {
        _spawnWave = true;
        _time = 0.0f;
        AudioManager.Instance().PlaySoundEffect(AudioManager.SoundEffect.WAVE_START);
        AudioManager.Instance().PlayMusic(AudioManager.Music.WAVE);

        StartCoroutine(SpawnEnemies());
    }

    private IEnumerator SpawnEnemies()
    {
        _numberOfEnemies += (int) (ENEMIES_TO_SPAWN * ENEMIES_MULTIPLIER);
        _enemiesToSpawn = _numberOfEnemies;

        for (int i = 0; i < _enemiesToSpawn; i++)
        {
            Transform spawnPoint = _spawnPoints[Random.Range(0, _spawnPoints.Length)];
            GameObject enemy = Instantiate(_enemyPrefab, spawnPoint.position, Quaternion.identity);
            enemy.transform.parent = _enemyContainer;

            yield return new WaitForSeconds(WAVE_DURATION / _numberOfEnemies);
        }
    }


    void UpdateUI()
    {
        _healthBar.value = _player.HPFraction;
        _repairBar.value = _player.RepairFraction;
    }
}
