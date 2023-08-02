using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class Spawner : MonoBehaviour
{
    [SerializeField] private List<Wave> _waves;
    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private Player _player;

    private Wave _currentWave;
    private int _currentWaveNumber = 0;
    private float _timeAfterLastSpawn;
    private int _spawned;

    public event UnityAction AllEnemySpawned;
    public event UnityAction<int,int> EnemyCountChange;

    private void Start()
    {
        SetWave(_currentWaveNumber);
    }

    private void Update()
    {
        if (_currentWave==null)
            return;

        _timeAfterLastSpawn += Time.deltaTime;

        if (_timeAfterLastSpawn>=_currentWave.Delay)
        {
            InstantiateEnemy();
            _spawned++;
            _timeAfterLastSpawn = 0;
            EnemyCountChange?.Invoke(_spawned,_currentWave.Count);
        }

        if (_currentWave.Count<=_spawned)
        {
            if (_waves.Count > _currentWaveNumber + 1)
                AllEnemySpawned?.Invoke();
            

            _currentWave = null;
        }
    }

    private void InstantiateEnemy()
    {
        int indexRandom = Random.Range(0, _currentWave.Template.Count);
        Enemy enemy= Instantiate(_currentWave.Template[indexRandom],_spawnPoint.position,_spawnPoint.rotation,_spawnPoint).GetComponent<Enemy>();
        enemy.Init(_player);
        enemy.Dying += OnEnemyDying;
    }

    private void SetWave(int index)
    {
        _currentWave = _waves[index];
        EnemyCountChange?.Invoke(0, 1);
    }

    public void NextWave()
    {
        SetWave(++_currentWaveNumber);
        _spawned = 0;
    }
    
    private void OnEnemyDying(Enemy enemy)
    {
        enemy.Dying -= OnEnemyDying;

        _player.AddMoney(enemy.Reward);
    }
}

[System.Serializable]
public class Wave
{
    public List< GameObject> Template;
    public float Delay;
    public int Count;
}
