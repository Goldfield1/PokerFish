using System.Collections.Generic;
using System.Linq;

namespace PokerFish.Poker
{
    public class Pot
    {
        private Dictionary<Player, int> pot;
        
        public int Total
        {
            get { return pot.Sum(x => x.Value); }
        }
        public Pot()
        {
            pot = new Dictionary<Player, int>();
        }

        public int this[Player player]
        {
            get
            {
                if (!pot.ContainsKey(player))
                {
                    pot.Add(player,0);        
                }
                return pot[player];
            }
            set { pot[player] = value; }
        }
        
        public void AddChips(Player player, int chips)
        {
            if (!pot.ContainsKey(player))
            {
                pot.Add(player,0);            
            }
            pot[player] += chips;
        }
    }
}