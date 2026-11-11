
using FinalBattle;
using System.Runtime.CompilerServices;


List<Character> heroes = new List<Character>();
heroes.Add(new Skeleton());
heroes.Add(new TrueProgrammer());

List<Character> monsters = new List<Character>();
monsters.Add(new Skeleton());

Game testGame = new Game(heroes, monsters, new HumanPlayer(), new ComputerPlayer());

testGame.Run();

namespace FinalBattle 
{


    #region CHARACTERS
    public  class Character
    {
        //TODO: Initialize HP for each Character-based class
        //The protected accessibility modifier lets descendants of Character modify their own HP values directly.
        public int HP { get;  protected set; }

        public int MaxHP { get; protected set; }

        //TODO: PerformAction will be loaded with the character's current action depending on which one was chosen by the player (either human or computer)
        //TODO: An Actions property (List<class that derives from an interface>) may be necessary for selecting a given action and displaying all actions that the current character has.
        //Actions that derive from the IGameAction interface can be instantiated inside their given character classes, then placed in the character's Actions property.

        [Obsolete]
        public Action<BattleRound>? PerformAction { get; set; }

        //TODO: The Actions property will hold a list of possible methods to insert into PerformAction. This selection will occur when a given player type selects an action.
        public List<IGameAction> Actions { get; } = new List<IGameAction>();
        public string? Name { get; protected set; }

        public bool IsAlive
        {
            get { return HP > 0; }
        }


        public Character() 
        {

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

}

    public class Skeleton : Character
    {
        //The base Chararcter constructor automatically gives every Character a DoNothing action.
        public Skeleton() : base()
        {
            Name = "SKELETON";

            MaxHP = 5;
            HP = MaxHP;

            Actions.Add(new StandardAttack(
                "BONE CRUNCH", 
                (battle,  target) => 
            {
                Random random = new Random();

                //Randomly deals 0-1 damage
                int damageValue = random.Next(2);
                target.TakeDamage(damageValue);

                return damageValue;

               
                
            }

            ));
        }

        [Obsolete("StandardAttack instances are initialized with a lambda expression where the lambda is of type Action<BattleRound, Character>")]
        
        public void BoneCrunch(BattleRound battle, IGameAction actionType, Character target) 
        {

            ConsoleOutput.OutputAttack(this, actionType, target);
        }
    
    }

    //Represents the actual player
    public class TrueProgrammer : Character 
    {
        public TrueProgrammer() 
        {
            MaxHP = 25;
            HP = MaxHP;
            
            Name = ConsoleOutput.PromptUser("Enter a name for the True Programmer: ");

        }


        
    }
    #endregion 

    //TODO: Consider removing a party member from the Members collection when they die. THis makes it so that their IsAlive property doesn't have to be checked long after they are no longer in battle.
    public class Party 
    {
    
        //Represents the type of Player that will dictate the party's actions.
        public Player Player { get; }
        public PartySide Side { get; init; }
        public List<Character> Members { get; }


        //TODO: Consider consequences of passing collection since it is a reference type by nature
        public Party(List<Character> members, PartySide side, Player player)

        {
            Members = members;
            Side = side;
            Player = player;
        }

        //A Party is still considered alive if at least one member is still alive
        public bool CheckIsAlive() 
        {
            int numberAlive =
                 (
                 from m in Members
                 where m.IsAlive
                 select m
                 ).Count();

            return numberAlive > 0;
        }
    }

    public enum PartySide 
    {
        Heroes,
        Monsters
    }

    public class Game
    {
        //TODO: Consider error handling for the PartySide of each side property. There must be one Heroes and one Monsters side, where no other combinations are possible.
        public Party HeroesSide { get; init; }

        public Party MonstersSide { get; init; }

        //TODO: Consider altering Party instantiation depending on initialization logic upon starting a new game or battle round. The current implementation will likely work, though it could be necessary to have the constructor parameters be Party classes instead.
        //TODO: Ask for the type of player to instantiate each party with.
        public Game(List<Character> heroes, List<Character> monsters, Player player1, Player player2 ) 
        {
            HeroesSide = new Party(heroes, PartySide.Heroes, player1);
            MonstersSide = new Party(monsters, PartySide.Monsters, player2);
        }
        public void Run()
        {

            BattleRound firstBattle = new BattleRound(this);
            firstBattle.BeginRound();
            
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

            //TODO: Possibly utilize an Event for death state here.
            while (_game.HeroesSide.CheckIsAlive() && _game.MonstersSide.CheckIsAlive())
            {
                //TODO: TEMPORARY CW
                Console.WriteLine($"Current Side: {_currentParty.Side}");

                //TODO: Consider moving depending on round logic. The same goes with the character actions seen below.
                foreach (Character character in _currentParty.Members)
                {
                    ConsoleOutput.OutputWhoseTurn(character);

                    //TODO: Only output if current player is a HumanPlayer
                    if(_currentParty.Player is HumanPlayer) ConsoleOutput.OutputAvailableActions(character);

                    //TODO: Could just call Execute() instead polymorphically directly
                    //TODO: The PerformAction property could be used for something such as cancelling using an item (e.g. change action before actually executing it).

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

                    //TODO: REMOVE SLEEP
                    Thread.Sleep(2000);


                    if (!_game.HeroesSide.CheckIsAlive() || !_game.MonstersSide.CheckIsAlive()) break;
             
                }

                


                ToggleSideTurn(CurrentPartyTurn);


            }

            //TODO: TEMPORARY CW
            Console.WriteLine($"{(!_currentParty.CheckIsAlive() ? _currentParty.Side : GetOpposingSide().Side)}'s party has been defeated");
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

        private readonly Func<BattleRound, Character, int> _attack;
        public string Name { get; init; }

        
        public StandardAttack(string name, Func<BattleRound, Character, int> attackBehavior)
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
             * TODO: If necessary, go back to using the HP difference (where calculation is priorHP - HP) to get true damage value. Caveat is that attacks that have large damage values will only show
             * the amount of damage leading up to zero HP.
             */
         

            int damageTaken = _attack.Invoke(battle, target);


            ConsoleOutput.OutputAttack(character, this, target);
            ConsoleOutput.OutputAttackDamageOutcome(attack: this, target: target, damage: damageTaken);
        }

        [type: Obsolete("_attack in the StandardAttack class represents a suppliable Action<BattleRound, Character> type.")]
        private delegate void AttackDelegate(BattleRound battle, Character target);


        [Obsolete]
        //TODO: TEMPORARY UNLESS USEFUL ENOUGH
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