#region PLAYERS
using FinalBattle;

public abstract class Player
{

    public abstract IGameAction? ChooseAction(Character character);

    public abstract InventoryItem ChooseItem(Party party, Character character);


    public abstract Character GetTarget(BattleRound battle);

}


public class HumanPlayer : Player
{
    public override IGameAction? ChooseAction(Character character)
    {
        int actionNumber = ConsoleOutput.PromptUserInt("What will you do? ");

        int actionIndex = actionNumber - 1;



        return IGameAction.GetAction(character, actionIndex);

    }

    public override InventoryItem ChooseItem(Party party, Character character)
    {
        ConsoleOutput.OutputItems(party);
        int itemIndex = ConsoleOutput.PromptUserInt("Choose an item: ") - 1;

        if (Helpers.IsValidCollectionMember(party.Inventory, itemIndex)) return party.Inventory[itemIndex];
        else
        {
            ConsoleOutput.WriteColorLine(ConsoleColor.Red, "Please choose a valid inventory item");
            return ChooseItem(party, character);
        }
    }


    public override Character GetTarget(BattleRound battle)
    {
        Party opposingSide = battle.GetOpposingSide();

        Console.WriteLine("\nTargets:");
        ConsoleOutput.OutputMembers(opposingSide);
        int targetIndex = ConsoleOutput.PromptUserInt("Who would you like to target? ") - 1;

        if (Helpers.IsValidCollectionMember(opposingSide.Members, targetIndex))
        {
            return opposingSide.Members[targetIndex];
        }
        else
        {
            Console.WriteLine("Please pick a valid character \n");

            return GetTarget(battle);
        }
    }


}


public class ComputerPlayer : Player
{

    public override IGameAction? ChooseAction(Character character)
    {
        Random random = new Random();
        int actionIndex = random.Next(character.Actions.Count);

        //TODO: 11/14 Prevent ComputerPlayer from picking UseItem if there are not items in the inventory. The same should be done with HumanPlayer.
        return IGameAction.GetAction(character, actionIndex);



    }


    public override Character GetTarget(BattleRound battle)
    {
        Random random = new Random();
        Party opposingSide = battle.GetOpposingSide();

        int targetIndex = random.Next(opposingSide.Members.Count);

        if (Helpers.IsValidCollectionMember(opposingSide.Members, targetIndex))
        {
            return opposingSide.Members[targetIndex];
        }
        else
        {

            throw new ArgumentOutOfRangeException($"ComputerPlayer failed to find a valid Character instance at index {targetIndex} for party side {opposingSide.Side}");
        }
    }

    public override InventoryItem ChooseItem(Party party, Character character)
    {
        Random random = new Random();
        int itemIndex;

        //The ComputerPlayer only considers potions when one exists and the current turn character's HP is below half. There is a 25% chance to use potions when potions are legal to use for the ComputerPlayer.
        IEnumerable<InventoryItem> availablePotions =
            from item in party.Inventory
            where item is HealthPotion
            select item;

        if (availablePotions.Count() > 0 && (character.HP / (double)character.MaxHP) < 0.5 && random.NextDouble() <= 0.25)
        {
            HealthPotion? result = null;
            int max = 0;
            //Choose potion with greatest heal amount
            foreach (HealthPotion potion in availablePotions)
            {
                if (potion.HealAmount > max) max = potion.HealAmount;
                result = potion;
            }

            return result;

        }
        else 
        {
            //TODO: 11/15 Figure out how to exit out of UseItem action for ComputerPlayer if there are no valid items when excluding potions (potion use case requirements are not met)
            //Difficult scenario regarding handling since this method must return an InventoryItem
            IEnumerable<InventoryItem> nonPotionItems = 
                from item in party.Inventory
                where item is not HealthPotion
                select item;

            return nonPotionItems.ElementAt(random.Next(nonPotionItems.Count()));
        };
        

       

        if (Helpers.IsValidCollectionMember(party.Inventory, itemIndex)) return party.Inventory[itemIndex];
        else throw new ArgumentOutOfRangeException($"ComputerPlayer failed to find a valid InventoryItem instance at index {itemIndex} for party side {party.Side}");
    }
}


public enum PlayerTypes
{
    Human,
    Computer
}

#endregion PLAYERS