#region CHARACTERS
using FinalBattle;

public class Character
{
    //TODO: Initialize HP for each Character-based class
    //The protected accessibility modifier lets descendants of Character modify their own HP values directly.
    public int HP { get; protected set; }

    public int MaxHP { get; protected set; }

    //Differentiates characters of the same name (primarily used for determining which character has the current turn when outputting the battle status)
    //public Guid Identifier { get; }

    //TODO: PerformAction will be loaded with the character's current action depending on which one was chosen by the player (either human or computer)


    [Obsolete]
    public Action<BattleRound>? PerformAction { get; set; }

    //TODO: The Actions property will hold a list of possible methods to insert into PerformAction. This selection will occur when a given player type selects an action.
    public List<IGameAction> Actions { get; } = new List<IGameAction>();
    public string? Name { get; protected set; }

    public bool IsAlive
    {
        get { return HP > 0; }
    }


    public Character(int health)
    {
        MaxHP = health;
        HP = MaxHP;

        //Identifier = Guid.NewGuid();

        Actions.Add(new DoNothing());
    }

    [Obsolete]
    public void DoNothing()
    {
        // ConsoleOutput.OutputTurnAction(this, "NOTHING");
    }

    //Method is public so that StandardAttacks or similar methods of other Characters / object types can inflict damage

    //TODO: Consider making abstract OR virtual so that derived Characters can define things such as their own damage resistances (or chance of 0 damage). Alternatively, account for a Resistance property
    public void TakeDamage(int amount)
    {
        int result = HP - amount;

        HP = Math.Clamp(result, 0, MaxHP);

        //TODO: Implement event invocation that occurs when HP is 0 and indicates death of character. May have to either use IsAlive instead.
        //When the event is triggered, break the BattleRound loop and exit.
    }

    /*
    public override bool Equals(object? obj)
    {
        Character? comparedCharacter = obj as Character;

        return comparedCharacter?.Identifier == Identifier;
    }
    */



}

public class Skeleton : Character
{
    //The base Chararcter constructor automatically gives every Character a DoNothing action.
    public Skeleton(int health) : base(health)
    {
        Name = "SKELETON";


        Actions.Add(new StandardAttack(
            "BONE CRUNCH",
            (battle, target) =>
            {
                Random random = new Random();

                //Randomly deals 0-1 damage
                int damageValue = random.Next(2);
                target.TakeDamage(damageValue);

                return new StandardAttack.AttackData(damageValue);



            }

        ));
    }

    [Obsolete("StandardAttack instances are initialized with a lambda expression where the lambda is of type Func<BattleRound, Character, int>")]

    public void BoneCrunch(BattleRound battle, IGameAction actionType, Character target)
    {

        ConsoleOutput.OutputAttack(this, actionType, target);
    }

}

//Represents the actual player
public class TrueProgrammer : Character
{
    public TrueProgrammer(int health) : base(health)
    {

        Name = ConsoleOutput.PromptUser("Enter a name for the True Programmer: ");

        Actions.Add(
            new StandardAttack(
            "DEBUG / TEST",
            (battle, target) =>
            {
                int damageValue = 9999999;
                target.TakeDamage(damageValue);

                return new StandardAttack.AttackData(damageValue);
            }


            ));

    }



}

public class UncodedOne : Character
{

    public UncodedOne(int health) : base(health)
    {
        Name = "UNCODED ONE";


        Actions.Add(

            new StandardAttack(

                "UNRAVEL",
                (battle, target) =>
                {
                    Random random = new Random();
                    int damageValue = random.Next(3);

                    target.TakeDamage(damageValue);

                    return new StandardAttack.AttackData(damageValue);
                }
                )

            );

    }
}

#endregion