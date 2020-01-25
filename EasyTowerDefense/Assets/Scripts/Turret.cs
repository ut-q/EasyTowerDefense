using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class TurretShootingEventArgs : EventArgs
    {
        public Enemy Enemy { get; set; }
        public int Damage { get; set; }
        public Transform StartPosition { get; set; }

        public TurretShootingEventArgs(Enemy enemy, int damage, Transform start)
        {
            Enemy = enemy;
            Damage = damage;
            StartPosition = start;
        }
    }

    public class Turret : Entity
    {
        [SerializeField] private float AreaDistance;
        [SerializeField] private float FireRate;
        [SerializeField] private int Damage;

        [SerializeField] private Base BaseReference;
        [SerializeField] private Transform CannonLocation;

        private float _timer = 0.0f;

        private List<Enemy> Enemies; // this is a potential bug, this list should be taken from Entitymanager at all times but I didn't have enough time to architect this better unfortunately

        public event EventHandler<TurretShootingEventArgs> TurretShooting;

        // Use this for initialization
        void Start()
        {
            Enemies = new List<Enemy>();

            SphereCollider col = GetComponent<SphereCollider>();
            if (col != null)
            {
                col.radius = AreaDistance;
            }

            _timer = FireRate;
        }

        // Update is called once per frame
        void Update()
        {
            if (Enemies.Count > 0)
            {
                _timer += Time.deltaTime;
                if (_timer >= FireRate)
                {
                    Shoot();
                    _timer = 0.0f;
                }
            }
        }

        private void Shoot()
        {
            Enemy enemy = FindEnemyToShoot();
            if (enemy != null)
            {
                OnTurretShooting(new TurretShootingEventArgs(enemy, Damage, CannonLocation));
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            Enemy enemy = other.gameObject.GetComponent<Enemy>();
            if (enemy != null)
            {
                if (!Enemies.Contains(enemy))
                {
                    Enemies.Add(enemy);
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            Enemy enemy = other.gameObject.GetComponent<Enemy>();
            if (enemy != null)
            {
                if (Enemies.Contains(enemy))
                {
                    Enemies.Remove(enemy);
                }
            }
        }

        private Enemy FindEnemyToShoot()
        {
            Enemy victim = null;
            var distanceSquare = Double.MaxValue;
            // we want to get enemies closest to the base first
            var position = BaseReference.transform.position;
            foreach (var e in Enemies)
            {
                if (e == null)
                {
                    continue; // potential bug, will come back to this if I have time
                }
                double sqrt = (e.transform.position - position).sqrMagnitude;
                if (sqrt < distanceSquare)
                {
                    distanceSquare = sqrt;
                    victim = e;
                }
            }

            return victim;
        }

        public virtual void OnTurretShooting(TurretShootingEventArgs e)
        {
            if (TurretShooting != null)
            {
                TurretShooting.Invoke(this, e);
            }
        }
    }
}
