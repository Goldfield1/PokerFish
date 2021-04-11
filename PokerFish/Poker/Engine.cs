using System.Collections.Generic;
using System.Linq;

namespace PokerFish.Poker
{
    public class Engine
    {
        private Table table;
        
        private int currentPlayerN;
        private int bettingRoundN;
        
        
        public Engine(Table table)
        {
            this.table = table;

            currentPlayerN = 2;
            bettingRoundN = 0;
        }

        public void BettingRound()
        {
                        
        }

        /*
         * This is for the AI, to run through all rounds from the current one
         */
        public void AllDealingAndBettingRounds()
        {
            BettingRound();
            BettingRound();
            BettingRound();
            BettingRound();
        }
        
        /*
         * This is for the scraber, engine has to update as it goes along, with updates from screen
         */
        public void IterateDealingAndBettingRounds()
        {
            BettingRound();
            BettingRound();
            BettingRound();
            BettingRound();
        }
        
        /*
         * Maybe a better approach is getting converting the input from the Scraber into a State, which can then be fed into the engine
         */
        public void StateFromScreen()
        {
            
        }

        public List<Player> PlayersInOrderOfBetting(bool firstRound)        
        {
            if (firstRound)
            {
                var players1 = new List<Player>(table.Players.GetRange(2,4));
                var players2 = new List<Player>(table.Players.GetRange(0,2));
                players1.AddRange(players2);
                return players1;
            }
            return table.Players;
        }

        public void MoveBlinds()
        {
            var first = table.Players[0];
            table.Players.Add(first);
            table.Players.RemoveAt(0);
        }
    }
}