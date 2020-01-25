using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Scripts
{
    public class EnemyHitEventArgs : EventArgs
    {
        public Entity Entity { get; set; }
        public int Damage { get; set; }

        public EnemyHitEventArgs(Entity e, int damage)
        {
            Entity = e;
            Damage = damage;
        }
    }

    public class ProjectileHitEventArgs : EventArgs
    {
        public Enemy Enemy { get; set; }
        public int Damage { get; set; }

        public ProjectileHitEventArgs(Enemy enemy, int damage)
        {
            Enemy = enemy;
            Damage = damage;
        }
    }

    public class ProjectileDestroyEventArgs : EventArgs
    {
        public Projectile Projectile { get; set; }

        public ProjectileDestroyEventArgs(Projectile pr)
        {
            Projectile = pr;
        }
    }

    public class Enemy : Entity, IDamageable, IDestructible
    {
        [SerializeField] private int Health;
        [SerializeField] private float Speed;
        [SerializeField] private float HitRate;
        [SerializeField] private int Damage;

        [SerializeField]
        private Vector3 GoalPosition { get; set; }

        public EventHandler EnemyRequestsDestroy;
        public EventHandler<EnemyHitEventArgs> EnemyHit;
        public EventHandler<ProjectileDestroyEventArgs> ProjectileRequestsDestroy;
        public EventHandler<ProjectileHitEventArgs> ProjectileHit;

        private float _hitTimer = 0.0f;

        // Use this for initialization
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
        }

        public void DecreaseHealth(int health)
        {
            Health -= health;

            if (Health <= 0)
            {
                Destroy();
            }
        }

        public void IncreaseHealth(int health)
        {
            Health += health;
        }

        public void Destroy()
        {
            OnDestroyRequested(EventArgs.Empty);

            var coll = GetComponent<Collider>();
            if (coll != null)
            {
                coll.enabled = false;
            }

            var rend = GetComponent<Renderer>();
            if (rend != null)
            {
                rend.enabled = false;
            }
        }

        // used to increase speed when needed
        public void SetSpeed(float coefficient)
        {
            Speed *= coefficient;
        }

        public void SetGoalPosition(Vector3 position)
        {
            GoalPosition = position;

            NavMeshAgent agent = GetComponent<NavMeshAgent>();
            if (agent != null)
            {
                agent.enabled = true;
                agent.destination = GoalPosition;
                agent.speed = Speed;
            }
        }

        public void OnDestroyRequested(EventArgs e)
        {
            if (EnemyRequestsDestroy != null)
            {
                EnemyRequestsDestroy.Invoke(this, e);
            }
        }

        public void OnEnemyHit(EnemyHitEventArgs e)
        {
            if (EnemyHit != null)
            {
                EnemyHit.Invoke(this, e);
            }
        }

        public void OnProjectileDestroy(ProjectileDestroyEventArgs e)
        {
            if (ProjectileRequestsDestroy != null)
            {
                ProjectileRequestsDestroy.Invoke(this, e);
            }
        }

        public virtual void OnProjectileHit(ProjectileHitEventArgs e)
        {
            if (ProjectileHit != null)
            {
                ProjectileHit.Invoke(this, e);
            }
        }

        private void OnTriggerStay(Collider other)
        {
            // only capable of hitting the base for now
            if (other.GetComponent<Base>() != null)
            {
                _hitTimer += Time.deltaTime;

                if (_hitTimer >= HitRate)
                {
                    _hitTimer = 0;
                    OnEnemyHit(new EnemyHitEventArgs(this, Damage));

                    // An enemy can hit the base once...
                    OnDestroyRequested(EventArgs.Empty);
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            Projectile pr = other.gameObject.GetComponent<Projectile>();
            if (pr != null)
            {
                OnProjectileHit(new ProjectileHitEventArgs(this, pr.Damage));
                OnProjectileDestroy(new ProjectileDestroyEventArgs(pr));
            }
        }
    }
}
