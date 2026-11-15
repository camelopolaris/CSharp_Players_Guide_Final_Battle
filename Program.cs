
using FinalBattle;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;

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