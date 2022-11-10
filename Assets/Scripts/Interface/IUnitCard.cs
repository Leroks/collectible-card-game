namespace StormWarfare.Interface
{
    public interface IUnitCard
    {
        public abstract void AttackToCard(int cardUniqueId);
        public abstract void AttackToCommander();
        public abstract void UseAbility();
        public abstract void PlaceToBattleField(int positionalIndex);
    }
}