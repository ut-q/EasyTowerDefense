using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    // interface to be used by Entities which can take damage
    public interface IDamageable
    {
        void DecreaseHealth(int health);

        void IncreaseHealth(int health);

    }
}