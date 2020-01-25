using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class Base : Entity, IDamageable, IDestructible
    {
        [SerializeField] private int Health;
        [SerializeField] public GameObject[] Positions;

        // List of positions for the Nav mesh Agents to aim for
        // they pick one at random so they don't clutter at a single point
        public GameObject[] GoalPositions
        {
            get { return Positions; }
        }

        public EventHandler BaseRequestsDestroy;

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
        }

        public virtual void OnDestroyRequested(EventArgs e)
        {
            if (BaseRequestsDestroy != null)
            {
                BaseRequestsDestroy.Invoke(this, e);
            }
        }
    }
}