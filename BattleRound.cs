namespace FinalBattle 
{
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

}