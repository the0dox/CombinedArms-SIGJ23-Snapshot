// created by Skeletor
// add this interface to an object that can be hit by projectiles
public interface IAttackable
{
    // called when a projectile hits this object
    public abstract void TakeDamage(float value);
}
