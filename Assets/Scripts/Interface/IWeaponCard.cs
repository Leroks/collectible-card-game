namespace StormWarfare.Interface
{
    public interface IWeaponCard
    {
        public abstract void AttackToCard(int cardUniqueId);
        public abstract void AttackToCommander();
        public abstract void UseAbility();
    }
}