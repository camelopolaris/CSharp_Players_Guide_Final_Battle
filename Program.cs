
using FinalBattle;
using System.Reflection.Metadata.Ecma335;

Player? player1 = null;
Player? player2 = null;


while (player1 is null && player2 is null) {
    int chosenGamemode = ConsoleOutput.PromptUserInt("\nGamemodes:\n1 - Player versus CPU\n2 - CPU versus CPU\n3 - Player versus Player\n\nChoose a gamemode: ");

    switch (chosenGamemode)
    {
        case 1:
            player1 = new HumanPlayer();
            player2 = new ComputerPlayer();
            break;
        case 2:
            player1 = new ComputerPlayer();
            player2 = new ComputerPlayer();
            break;
        case 3:
            player1 = new HumanPlayer();
            player2 = new HumanPlayer();
            break;
        default:
            ConsoleOutput.WriteColorLine(ConsoleColor.Red, "Please enter a valid gamemode number");
            continue;

    }
}





Party heroes = new Party(new List<Character>()
{
    new Skeleton(5),
    new TrueProgrammer(25),

},
    PartySide.Heroes,
    player1

);

Party initialMonsters = new Party(
    new List<Character>()
    { 
    new Skeleton(5),
    },
    PartySide.Monsters,
    player2

);

Game testGame = new Game(heroes, initialMonsters, player1, player2);

testGame.AddToMonsterQueue(
    new Party(
                new List<Character>() 
                { 
                  new Skeleton(5),
                  new Skeleton(5),
                },
                PartySide.Monsters,
                player2
                ));

testGame.AddToMonsterQueue(

    new Party(

        new List<Character>() { new UncodedOne(15)},
        PartySide.Monsters,
        player2
        )
    );

testGame.Run();

namespace FinalBattle 
{

    #region CHARACTER IDEAS

    /*
     * DoubleAgent: They're just in it for the money. Randomly switches which party they belong to every few turns, but its attacks are weaker if they are on the Monsters party.
     * CouncilOfFreach: Standard attack fills an array with random integer values between 0 and 10 non-inclusive; the total number of values randomly ranges from 3-6 inclusive. The final damage value is the average of these values rounded down to the nearest integer.
     * Simula: Standard action allows players to construct a soup consisting of different ingredients, then adds that soup to the party's inventory. Each ingredient adds things such as attack increases, heals, etc. Second action is a LADEL SWING, which flings boiling soup and deals 1 damage, then 1 damage for 2 turns. Low MaxHP.
     * Awat: Standard attack DESYNCHRONIZE does nothing at first, but creates a new Thread which randomly damages the target at the start of ANY character's turn for {x} damage for up to 3 turns (only counted when damage is dealt). However, it lowers Awat's MaxHp each time it is unsuccessful and halves the effectiveness of heals until it is done running. Cannot be used again while a DESYNCHRONIZE instance is already active. Medium-high MaxHP.
     * 
     * */
    #endregion

    #region CHARACTERS
    public class Character
    {
        //TODO: Initialize HP for each Character-based class
        //The protected accessibility modifier lets descendants of Character modify their own HP values directly.
        public int HP { get;  protected set; }

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
        public  void TakeDamage(int amount) 
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
                (battle,  target) => 
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

    

    public class Game
    {

        private List<Party> _monstersQueue = new List<Party>();


        //TODO: Consider error handling for the PartySide of each side property. There must be one Heroes and one Monsters side, where no other combinations are possible.
        public Party HeroesSide { get; init; }

        //Null until constructor runs; represents the initial monster party, then is reassigned with monster parties after the first BattleRound is succeeded by heroes. 
        //Lives in the Game Class since both this and HeroesSide are evaluated to determine the result of an entire game once an endgame state is reached (heroes die once or _monsterQueue has been iterated through)
        public Party? MonstersSide { get; private set; }

        //TODO: Consider altering Party instantiation depending on initialization logic upon starting a new game or battle round. The current implementation will likely work, though it could be necessary to have the constructor parameters be Party classes instead.
        //TODO: Ask for the type of player to instantiate each party with.
        public Game(Party heroes, Party initialMonsters, Player player1, Player player2 ) 
        {
            HeroesSide = heroes;


            _monstersQueue.Add(initialMonsters);
        }
        public void Run()
        {

            foreach (Party party in _monstersQueue) 
            {
                if (!IsGameOver())
                {
                    MonstersSide = party;
                    BattleRound battle = new BattleRound(this);
                    battle.BeginRound();
                }
                else break;

            }

            ConsoleOutput.OutputGameOutcome(HeroesSide, MonstersSide);
        }


        //Game over state is different from BattleRound over state; game is over if EITHER the hero party is defeated ONCE or if ALL consecutive monster parties are defeated
        //TODO: 11/12 Potentially remove second condition since iterating to the end of _monstersQueue implies that all monsters are defeated
        public bool IsGameOver() 
        {
            return !HeroesSide.CheckIsAlive() || _monstersQueue.Count == 0;
        }


        //Potential usage other than obviously adding new monster parties to iterate through before each round is to have a MonsterSide Character call in another party (e.g. add an entire round)
        public void AddToMonsterQueue(Party party) 
        {
            if (party.Side != PartySide.Monsters) throw new ArgumentException("Parties added to _monsterQueue must have a Side of type PartySide.Monsters");
            else if(party.Members.Count == 0) throw new ArgumentException("Parties added to _monsterQueue must have at least one member");

            _monstersQueue.Add(party);
        
        }
        

    }

    //TODO: Consider passing the Game object in BeginRound rather than the constructor. This might not be needed since other information may be needed about the game at some point before a round begins.
    public class BattleRound 
    {
        public Game _game { get; }

        public Party _currentParty;

        public PartySide CurrentPartyTurn { get; private set; }

        public BattleRound(Game game) 
        { 
            _game = game;
        }

        //TODO: Could return party that died, if any
        public void BeginRound() 
        {
            _currentParty = _game.HeroesSide;
            CurrentPartyTurn = PartySide.Heroes;


            ConsoleOutput.WriteColorLine(ConsoleColor.DarkMagenta, $"Enemy party members: {_game.MonstersSide.ToString()}\n");

            //TODO: Possibly utilize an Event for death state here.
            while (!IsRoundOver())
            {
                ConsoleOutput.OutputCurrentSide(this);

                //TODO: Consider moving depending on round logic. The same goes with the character actions seen below.
                foreach (Character character in _currentParty.Members)
                {
                    ConsoleOutput.OutputBattleStatus(this, character);
                    ConsoleOutput.OutputWhoseTurn(character);

                    if(_currentParty.Player is HumanPlayer) ConsoleOutput.OutputAvailableActions(character);

                    //TODO: The PerformAction property could be used for something such as cancelling using an item (e.g. change action before actually invoking Execute).

                    IGameAction? chosenAction;

                    do
                    {
                        chosenAction = _currentParty.Player.ChooseAction(character);

                        if (chosenAction is null) 
                        {
                            Console.WriteLine($"Action \"{chosenAction?.Name ?? "null"}\" does not exist.");
                        }

                    } while (chosenAction is null);


                    chosenAction.Execute(this, character);

                    

                    Console.WriteLine();

                    Thread.Sleep(2000);


                    if (IsRoundOver()) break;
             
                }

                //Prematurely ends round if an endgame state is reached (party death of either side)
                if (IsRoundOver()) break;


                ToggleSideTurn(CurrentPartyTurn);
                

                //TODO: 11/12 Add logic for determining party death state PRIOR to starting another BattleRound instance. Easiest solution could be to only instantiate a BattleRound / call BeginRound  within a Game instance when both parties are alive.
            }

            

        }


        public bool IsRoundOver()
        {
            return !_game.HeroesSide.CheckIsAlive() || !_game.MonstersSide.CheckIsAlive();
        }

        //Switches the party that has its turn. Used at the end of each party turn cycle.
        public void ToggleSideTurn(PartySide currentSide)
        {
           
            if (currentSide == PartySide.Heroes)
            {
                _currentParty = _game.MonstersSide;
                CurrentPartyTurn = PartySide.Monsters;
            }
            else if (currentSide == PartySide.Monsters)
            {
                _currentParty = _game.HeroesSide;
                CurrentPartyTurn = PartySide.Heroes;
            }
            //TODO: Consider throwing exception here just in case PartySide values are inconsistent with the current party side, though this would only happen if the incorrect side is assigned to a given party
        }

        

        public Party GetOpposingSide() 
        {
            if (CurrentPartyTurn == PartySide.Heroes) return _game.MonstersSide;
            else return _game.HeroesSide;
        }
        

    }
    //TODO: CharacterTurn could be a base class for actions such as attacks.
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
    

    //TODO: Consider how to incorporate since delegates may be used instead for character actions
    public interface IGameAction 
    {
        string Name { get; init; }
        void Execute(BattleRound battle, Character character);

        [Obsolete]
        public static bool IsValidAction(Character character, int actionIndex)
        {
            if (actionIndex >= 0 && actionIndex < character.Actions.Count)
            {
                return character.Actions[actionIndex] is not null;
            }

            return false;
        }

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

    #region PLAYERS
    public abstract class Player 
    {

        public abstract IGameAction? ChooseAction(Character character);

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
    }


    public enum PlayerTypes 
    {
        Human,
        Computer
    }

#endregion PLAYERS

    public static class Helpers 
    {
        public static bool IsValidCollectionMember<T>(List<T> collection, int index) 
        {
            if (index >= 0 && index < collection.Count) 
            {
                return collection[index] is not null;
            }

            return false;
        }

    }

}