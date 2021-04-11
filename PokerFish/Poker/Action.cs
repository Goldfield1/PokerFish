namespace PokerFish.Poker
{
    public enum ActionType 
    {
        Fold,
        Call,
        Raise,
        Check
    }
    public class Action
    {
        public ActionType Type;
        public int Amount;
        
        public Action(ActionType type, int amount = 0)
        {
            Type = type;
            Amount = amount;
        }
    }
}