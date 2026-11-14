namespace FinalBattle 
{
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
            /*
            int numberAlive =
                 (
                 from m in Members
                 where m.IsAlive
                 select m
                 ).Count();
            */

            return Members.Count != 0;
        }

        public override string ToString()
        {
            string result = "";
            /*
            IEnumerator<Character> iterator = Members.GetEnumerator(); 
            string result = "";

            while (iterator.MoveNext()) 
            {
                Character member = iterator.Current;

                if (Members.IndexOf(member) == Members.Count - 1) result += member.Name;
                else result += $"{member.Name}, ";
               
            }*/

            for (int i = 0; i < Members.Count; i++) 
            {

                if (i == Members.Count - 1) result += Members[i].Name;
                else result += $"{Members[i].Name}, ";
            }

            return result;

            
        }
    }

}

public enum PartySide
{
    Heroes,
    Monsters
}