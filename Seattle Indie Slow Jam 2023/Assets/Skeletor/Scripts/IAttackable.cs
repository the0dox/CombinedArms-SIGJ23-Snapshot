// created by Skeletor
// add this interface to an object that can be hit by projectiles
using UnityEngine;

public interface IAttackable
{
    // called when a raycast hits this object
    public abstract void TakeDamage(GameObject hit, float value);
}
