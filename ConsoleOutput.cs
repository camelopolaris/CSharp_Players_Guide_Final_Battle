namespace FinalBattle 
{
    public static class ConsoleOutput 
    {
        #region TURN / GAME LOGIC RELATED

        public static void OutputCurrentSide(BattleRound battle) 
        {
            ConsoleColor color = battle._currentParty.Side == PartySide.Heroes ? ConsoleColor.Cyan : ConsoleColor.DarkMagenta;
            WriteColorLine(color, $"Current Side: {battle._currentParty.Side}");
            
        }
        public static void OutputWhoseTurn(Character character) 
        {

            WriteColorLine(ConsoleColor.Yellow, $"It is {character.Name}'s turn...\n");
           
        }


        //Executed when a character chooses and follows through with a certain action
        public static void OutputTurnAction(Character character, IGameAction action) 
        {
            Console.WriteLine($"{character.Name} did {action.Name}.");
        }


        //TODO: TEMPORARY Unless better solution is not necessary
        //TODO: Consider merging this method and OutputMembers into a generic method since functionality is identical.
        public static void OutputAvailableActions(Character character) 
        {
            int placeValue = 1;
            foreach (IGameAction action in character.Actions) 
            {
                Console.WriteLine($"{placeValue} - {action.Name}");

                placeValue++;
            }
        }

        public static void OutputGameOutcome(Party heroes, Party monsters) 
        {
            if (heroes.Side != PartySide.Heroes) throw new ArgumentException($"Parameter \"heroes\" Side property was not of expected type {PartySide.Heroes}");
            if (monsters.Side != PartySide.Monsters) throw new ArgumentException($"Parameter \"monsters\" Side property was not of expected type {PartySide.Monsters}");

            if (heroes.CheckIsAlive() && !monsters.CheckIsAlive()) WriteColorLine(ConsoleColor.Cyan, "The heroes have won!");
            else if (monsters.CheckIsAlive() && !heroes.CheckIsAlive()) WriteColorLine(ConsoleColor.DarkMagenta, "The Uncoded One's forces have prevailed");
        }
        #endregion

        #region ATTACK TURNS
        public static void OutputAttack(Character attacker, IGameAction attack, Character recipient) 
        {
            WriteColorLine(ConsoleColor.Yellow, $"{attacker.Name} used {attack.Name} on {recipient.Name}.");
           
        }

        //TODO: Consider adding params keyword 

        //TODO: 11/10 StandardAttack could serve as a base class for specialized attacks, weapons, etc. This also ensures that the specified attack is actual an attack action type.
        //The damage number parameter exists so that the actual damage value (e.g. after resistances) is accurately displayed.
        public static void OutputAttackDamageOutcome(StandardAttack attack, Character target, int damage) 
        {

            WriteColorLine(ConsoleColor.Blue, $"{attack.Name} dealt {damage} damage to {target.Name}.");
          

            //TODO: TEMPORARY CW    
            Console.WriteLine(target.HP / (double)target.MaxHP);

            //Returned color varies on target's health
            ConsoleColor color = (target.HP / (double)target.MaxHP) switch
            {
                >= 0.75 and <= 1 => ConsoleColor.Green,
                >= 0.25          => ConsoleColor.DarkYellow,
                > 0              => ConsoleColor.Red,
                  0              => ConsoleColor.DarkRed,
            };


            WriteColorLine(color, $"{target.Name} is now at {target.HP}/{target.MaxHP}.");
           
        }

        public static void OutputCharacterDeath(Character character) 
        {

            WriteColorLine(ConsoleColor.Blue, $"{character.Name} has been defeated!");

        }

       
        #endregion

        #region PARTY / CHARACTER INFORMATION
        public static void OutputMembers(Party party) 
        {
            int placeValue = 1;
            foreach (Character character in party.Members) 
            {
                Console.Write(placeValue - 1 != party.Members.Count - 1 ? $"{placeValue} - {character.Name}, " :  $"{placeValue} - {character.Name}");

                placeValue++;
            }

            Console.WriteLine();
        }
        #endregion

        #region HUMAN PROMPTS    
        public static string PromptUser(string prompt, bool returnInt = false)
        {
            Console.Write(prompt);

            return Console.ReadLine() ?? "";
        }

        public static int PromptUserInt(string prompt) 
        {

            Console.Write(prompt);  

            string? userInput = Console.ReadLine();

            if (int.TryParse(userInput, out int value)) return value;

            else
            {
                Console.WriteLine("Please enter a valid integer");
                return PromptUserInt(prompt);
            }
              
          
        }


        public static void WriteColorLine(ConsoleColor color, string output)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(output);
            Console.ResetColor();
        }

    }
    #endregion

   

}