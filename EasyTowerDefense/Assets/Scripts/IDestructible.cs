using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    // interface used for Entities which can be destroyed
    public interface IDestructible
    {
        void Destroy();
    }
}