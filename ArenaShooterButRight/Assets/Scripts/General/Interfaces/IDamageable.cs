namespace General.Interfaces
{
    public interface IDamageable<in T>
    {
        void TakeDamage(T damageTaken);
        void HealDamage(T healingReceived);
    }
}
