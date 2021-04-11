namespace PokerFish.Poker
{
    public class State
    {
        public State PreviousState { get; private set; }
        public Table Table { get; private set; }
        public Player Player { get; private set; }
        //public  IsActive { get; private set; }
        public bool IsTerminal { get; private set; }

        public State(State previousState, Table table, Player player, bool isTerminal)
        {
            PreviousState = previousState;
            Table = table;
            Player = player;
            IsTerminal = isTerminal;
        }
        
        /* Maybe make utlity function where winner gets 1 and others -1 */
    }
}