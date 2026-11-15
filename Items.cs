public abstract class InventoryItem
{
    public abstract string Name { get; init; }
    public abstract void Use(Character target);
}

public class HealthPotion : InventoryItem
{

    public override string Name { get; init; }
    public int HealAmount { get; }
    public HealthPotion(int healAmount)
    {
        Name = $"HEALTH POTION ({healAmount})";
        HealAmount = healAmount;
    }
    public override void Use(Character target)
    {

        target.Heal(HealAmount);
    }


}