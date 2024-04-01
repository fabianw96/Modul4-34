namespace Fabian.KI.EnemyFSM
{
    public interface IDamageable<in T>
    {
        void TakeDamage(T damageTaken);
        void HealDamage(T healingReceived);
    }
}
