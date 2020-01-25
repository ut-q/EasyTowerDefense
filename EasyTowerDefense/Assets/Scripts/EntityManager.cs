using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Permissions;
using UnityEngine;

namespace Assets.Scripts
{
    public class EntityManager : MonoBehaviour
    {
        [SerializeField] private Turret Turret;
        [SerializeField] private Base Base;
        [SerializeField] private EnemySpawner Spawner;
        [SerializeField] private CameraEntity CameraEntity;
        [SerializeField] private GameObject ProjectilePrefab;
        [SerializeField] private float SpawnIncreaseTimer;
        [SerializeField] private float SpeedIncreaseCoefficient;


        private List<Entity> Entities { get; set; }
        private List<Enemy> Enemies { get; set; }
        private List<Projectile> Projectiles { get; set; }
        private List<Enemy> TurretHitList { get; set; }

        private float _timer = 0.0f;
        private int _speedIncreaseCount = 0;

        // Use this for initialization
        void Start()
        {
            Entities = new List<Entity> { Turret, Base, Spawner, CameraEntity };

            Enemies = new List<Enemy>();
            Projectiles = new List<Projectile>();
            TurretHitList = new List<Enemy>();

            Turret.TurretShooting += TurretShotReceived;
            Spawner.SpawnRequested += SpawnRequestReceived;

            Base.BaseRequestsDestroy += BaseRequestsDestroyReceived;

        }

        // Update is called once per frame
        void Update()
        {
            _timer += Time.deltaTime;

            if (_timer >= SpawnIncreaseTimer)
            {
                _timer = 0.0f;
                _speedIncreaseCount++;
                if (Spawner != null)
                {
                    Spawner.IncreaseSpawn();
                }
            }
        }

        public void ToggleCamera()
        {

        }

        private void CreateProjectile(Enemy target, int damage, Transform spawnPosition)
        {
            var obj = Instantiate(ProjectilePrefab, spawnPosition);

            Projectile pr = obj.GetComponent<Projectile>();
            if (pr != null)
            {
                Entities.Add(pr);
                Projectiles.Add(pr);

                pr.ShootProjectile(target.transform, damage);
            }
        }

        private void CreateEnemy(EnemySpawnData data)
        {
            var obj = Instantiate(data.EnemyPrefab, data.SpawnPosition);

            Enemy enemy = obj.GetComponent<Enemy>();
            if (enemy != null)
            {
                Entities.Add(enemy);
                Enemies.Add(enemy);
                enemy.SetSpeed(_speedIncreaseCount == 0 ? 1 : _speedIncreaseCount * SpeedIncreaseCoefficient);
                enemy.SetGoalPosition(data.GoalPosition.position);
                enemy.EnemyHit += EnemyHitReceived;
                enemy.EnemyRequestsDestroy += EnemyRequestsDestroyReceived;
                enemy.ProjectileRequestsDestroy += ProjectileRequestsDestroyReceived;
                enemy.ProjectileHit += ProjectileHitReceived;
            }
        }

        private void TurretShotReceived(object sender, TurretShootingEventArgs e)
        {
            CreateProjectile(e.Enemy, e.Damage, e.StartPosition);
        }

        private void ProjectileHitReceived(object sender, ProjectileHitEventArgs e)
        {
            Enemy enemy = null;
            try
            {
                enemy = (Enemy)sender;
            }
            catch (InvalidCastException ex)
            {
                Debug.Log(ex);
            }

            if (enemy == e.Enemy && enemy != null)
            {
                enemy.DecreaseHealth(e.Damage);
            }
        }

        private void ProjectileRequestsDestroyReceived(object sender, ProjectileDestroyEventArgs e)
        {
            Projectile pr = e.Projectile;

            Entities.Remove(pr);
            Projectiles.Remove(pr);

            Destroy(pr.gameObject);
        }

        private void SpawnRequestReceived(object sender, EnemySpawnEventArgs e)
        {
            // randomizing enemy types and spawn locations will be done here
            foreach (var data in e.SpawnData)
            {
                CreateEnemy(data);
            }
        }

        private void BaseRequestsDestroyReceived(object sender, EventArgs e)
        {
            Debug.Log("Game Over");
            Debug.Break();
        }

        private void EnemyRequestsDestroyReceived(object sender, EventArgs e)
        {
            Enemy pr = null;
            try
            {
                pr = (Enemy)sender;
            }
            catch (InvalidCastException ex)
            {
                Debug.Log(ex);
            }

            Entities.Remove(pr);
            Enemies.Remove(pr);

            if (pr != null)
            {
                Destroy(pr.gameObject);
            }
        }

        private void EnemyHitReceived(object sender, EnemyHitEventArgs e)
        {
            // time constraints force me to make this a bit non generic
            if (Base != null)
            {
                Base.DecreaseHealth(e.Damage);
            }
        }
    }
}