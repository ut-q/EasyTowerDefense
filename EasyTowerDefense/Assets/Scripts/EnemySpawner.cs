using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

namespace Assets.Scripts
{
    public class EnemySpawnEventArgs : EventArgs
    {
        public EnemySpawnData[] SpawnData { get; set; }

        public EnemySpawnEventArgs(EnemySpawnData[] spData)
        {
            //deep copy here to be safe
            SpawnData = new EnemySpawnData[spData.Length];
            for (int i = 0; i < SpawnData.Length; i++)
            {
                SpawnData[i] = spData[i].Copy();
            }
        }
    }

    public class EnemySpawnData
    {
        public Transform SpawnPosition { get; set; }
        public Transform GoalPosition { get; set; }
        public GameObject EnemyPrefab { get; set; }

        public EnemySpawnData(Transform start, Transform goal, GameObject prefab)
        {
            SpawnPosition = start;
            EnemyPrefab = prefab;
            GoalPosition = goal;
        }

        public EnemySpawnData Copy()
        {
            return new EnemySpawnData(SpawnPosition, GoalPosition, EnemyPrefab);
        }
    }

    public class EnemySpawner : Entity
    {
        [SerializeField] private float SpawnRate;
        [SerializeField] private int SpawnCount;
        [SerializeField] private GameObject[] EnemyTypes;
        [SerializeField] private GameObject[] SpawnPositions;
        [SerializeField] private Base Base;

        private float _timer = 0.0f;

        private Random _rand;

        public event EventHandler<EnemySpawnEventArgs> SpawnRequested;

        // Use this for initialization
        void Start()
        {
            _rand = new Random();

            // quick hack to make sure we spawn right away
            _timer = SpawnRate;
        }

        // Update is called once per frame
        void Update()
        {
            _timer += Time.deltaTime;

            if (_timer >= SpawnRate)
            {
                Spawn();
                _timer = 0.0f;
            }
        }

        public void IncreaseSpawn()
        {
            SpawnCount++;
        }

        private void Spawn()
        {
            var spData = new EnemySpawnData[SpawnCount];

            for (int i = 0; i < spData.Length; ++i)
            {
                var prefab = EnemyTypes[_rand.Next(EnemyTypes.Length)];
                var start = SpawnPositions[_rand.Next(SpawnPositions.Length)].transform;
                var goal = Base.GoalPositions[_rand.Next(Base.GoalPositions.Length)].transform;

                spData[i] = new EnemySpawnData(start, goal, prefab);
            }

            OnSpawnRequested(new EnemySpawnEventArgs(spData));
        }

        public void OnSpawnRequested(EnemySpawnEventArgs e)
        {
            if (SpawnRequested != null)
            {
                SpawnRequested.Invoke(this, e);
            }
        }

    }
}
