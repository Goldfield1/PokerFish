using System.Collections.Generic;
using System.Linq;

namespace PokerFish.Poker
{
    public class Player
    {
        public string Name { get; private set; }
        
        public int Chips { get; private set; }
        public int BetChips
        {
            get { return Pot[this]; } 
        }
        
        //public bool Cards { get; private set; }
        public Pot Pot { get; private set; }
        public bool IsActive { get; set; }

        public bool IsAllIn
        {
            get { return IsActive && Chips == 0; }
        }


        public Player(string name, int initialChips=0, Pot pot=null)
        {
            
            Name = name;
            Chips = initialChips;
            Pot = pot;
            IsActive = true; 
        }

        public void AddChips(int chips)
        {
            this.Chips += chips;
        }

        public void Fold()
        {
            this.IsActive = false;
        }

        public void Call(List<Player> players)
        {
            if (IsAllIn)
            {
                
            }
            else
            {
                var biggestBet = players.OrderByDescending(p => p.BetChips).First().BetChips;
                var chipsToCall = biggestBet - BetChips;
            } 
        }

        public int AddToPot(int chips)
        {
            if (chips < 0)
            {
                return -1;
            }
            
            // No bet can be greater than the amount of chips a players has
            if (chips > Chips)
            {
                chips = Chips;
            }
            
            Pot.AddChips(this, chips);
            Chips -= chips;
            return chips;
        }
    }
}