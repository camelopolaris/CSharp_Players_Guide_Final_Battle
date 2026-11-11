namespace FinalBattle 
{
    public static class ConsoleOutput 
    {
        #region TURN RELATED
        public static void OutputWhoseTurn(Character character) 
        {
            Console.WriteLine($"It is {character.Name}'s turn...\n");
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

        #endregion

        #region ATTACK TURNS
        public static void OutputAttack(Character attacker, IGameAction attack, Character recipient) 
        {
            Console.WriteLine($"{attacker.Name} used {attack.Name} on {recipient.Name}.");
        }

        //TODO: Consider adding params keyword 

        //TODO: 11/10 StandardAttack could serve as a base class for specialized attacks, weapons, etc. This also ensures that the specified attack is actual an attack action type.
        //The damage number parameter exists so that the actual damage value (e.g. after resistances) is accurately displayed.
        public static void OutputAttackDamageOutcome(StandardAttack attack, Character target, int damage) 
        {
            Console.WriteLine($"{attack.Name} dealt {damage} damage to {target.Name}.");
            Console.WriteLine($"{target.Name} is now at {target.HP}/{target.MaxHP}.");
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

        


    }
    #endregion

}