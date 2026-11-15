#region ACTIONS

//TODO: CharacterTurn could be a base class for actions such as attacks.
using FinalBattle;

public class DoNothing : IGameAction
{
    public string Name { get; init; } = "NOTHING";



    //Does nothing... who woulda thunk!
    public void Execute(BattleRound battle, Character character)
    {
        ConsoleOutput.OutputTurnAction(character, this);
    }
}


/*
 * TODO: Begin 11/10 Consider refactoring how action types are defined. For now, the current plan is that either a new StandardAttack will be instantiated with information regarding the
 * caller of the attack and the current battle round, or the _battle and _callingCharacter fields will be loaded with such information when this action is chosen.
 * The reason why the second approach may be better is because it assumes that StandardAttack instances already exist in a Character's Actions property since each Character will usually 
 * call List<T>.Add() in their constructor. Speaking of, this may also need to be redefined in general such that certain actions are abstract and MUST be implemented.
 */

//TODO: Extension methods could represent adding weapons to a character's inventory. This means that StandardAttack (for  now) strictly represents a character's native attack type.
public class StandardAttack : IGameAction
{

    private readonly Func<BattleRound, Character, AttackData> _attack;
    public string Name { get; init; }


    public StandardAttack(string name, Func<BattleRound, Character, AttackData> attackBehavior)
    {
        Name = name;

        //Supplied by the Character that has its Actions property instantiated with a StandardAttack
        _attack = attackBehavior;
    }




    public void Execute(BattleRound battle, Character character)
    {
        int priorTargetHealth;
        int healthDifference;

        Character target = battle._currentParty.Player.GetTarget(battle);

        //HP difference is recorded here instead of just reporting the attack's actual damage value just in case things such as damage resistances / damage debuffs are added.
        /*
         * TODO: 11/11 If possible, create an AttackData record that contains metadata about the given attack. Main issue currently is getting references to such data such as damage values
         * since that data lives in the delegate passed in the constructor. 
         * 
         * Current solution is to alter _attack to be a Func<BattleRound, Character, int> instead of an Action<BattleRound, Character>. 
         * This still allows the possibility of getting the true damage value since it is possible to obtain damage values after resistances while invoking _attack.
         * For example, a Character class may override TakeDamage so that it takes 50% damage rounded up. Since TakeDamage is accessible within an _attack, that value can be reported here.
         * 
         * 11/11: Another possible solution is to return an AttackData instance with all data. Potential issues with this approach is that future AttackData instances may have historic data (e.g. hit frequencies).
         * 
         * TODO: If necessary, go back to using the HP difference (where calculation is priorHP - HP) to get true damage value. Caveat is that attacks that have large damage values will only show
         * the amount of damage leading up to zero HP.
         */


        AttackData attackData = _attack.Invoke(battle, target);

        //TODO: Potentially move TakeDamage invocation from the attackBehavior delegate type to here. It may not matter either way since an instance of the target Character is accessible both here and in the delegate in StandardAttack.




        ConsoleOutput.OutputAttack(character, this, target);
        ConsoleOutput.OutputAttackDamageOutcome(attack: this, target: target, damage: attackData.damageValue);


        //It is legal to remove the opposing side's party member when they die since the party executing the attack is the only one being iterated through at this point.
        //Caveat is that any other scenario would require a different approach (e.g. if a Character has an attack that hurts themselves too)

        if (!target.IsAlive)
        {
            battle.GetOpposingSide().Members.Remove(target);
            ConsoleOutput.OutputCharacterDeath(target);
        }

    }

    [type: Obsolete("_attack in the StandardAttack class represents a suppliable Action<BattleRound, Character> type.")]
    private delegate void AttackDelegate(BattleRound battle, Character target);



    public record AttackData(int damageValue);


}

public class UseItem : IGameAction
{
    //private readonly Action<BattleRound, Character> _useItem;
    public string Name { get; init; }
    public UseItem()
    {
        Name = "USE ITEM";
    }

    public void Execute(BattleRound battle, Character character)
    {
        InventoryItem selectedItem;

        if (battle._currentParty.Inventory.Count > 0)
        {
            selectedItem = battle._currentParty.Player.ChooseItem(battle._currentParty, character);


            selectedItem.Use(character);

            //TODO: If equipable items are used, assign them to the invoking character's EquippedItem property once added
            battle._currentParty.Inventory.Remove(selectedItem);
        }
        else
        {

            

            IGameAction? action;

            do
            {
                ConsoleOutput.WriteColorLine(ConsoleColor.Red, "Party's inventory is empty");

                action = battle._currentParty.Player.ChooseAction(character);
            }
            while (action is UseItem);

            //Recursively execute action until the Player chooses an action that is not a UseItem action (given that their party's inventory is empty), execute action normally otherwise.
            action.Execute(battle, character);
            

        }
        

    }
}


//TODO: Consider how to incorporate since delegates may be used instead for character actions
public interface IGameAction
{
    string Name { get; init; }
    void Execute(BattleRound battle, Character character);

    

    //Returns null if the given game action does not exist or if the supplied index is invalid.
    public static IGameAction? GetAction(Character character, int actionIndex)
    {
        if (Helpers.IsValidCollectionMember(character.Actions, actionIndex))
        {

            return character.Actions[actionIndex];

        }

        return null;

    }
}

#endregion