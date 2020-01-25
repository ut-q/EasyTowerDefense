using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class Projectile : Entity, IDestructible
    {
        [SerializeField] public int Damage; // this is take from the Turret but serializable for debugging purposes
        [SerializeField] private Transform Target;
        [SerializeField] private float Speed;
        [SerializeField] private Collider TurretRef;
        private bool shooting = false;

        // Use this for initialization
        void Start()
        {
            if (TurretRef != null)
            {
                Physics.IgnoreCollision(GetComponent<Collider>(), TurretRef);
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (shooting)
            {
                var newPosition = (Target.position - transform.position) * Time.deltaTime * Speed;

                transform.position = transform.position + newPosition;
                transform.LookAt(Target.position);
            }
        }

        public void ShootProjectile(Transform tr, int damage)
        {
            Target = tr;
            Damage = damage;

            shooting = true;
        }

        public void Destroy()
        {
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
    }
}