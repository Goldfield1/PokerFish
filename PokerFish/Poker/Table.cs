using System.Collections.Generic;

namespace PokerFish.Poker
{
    public class Table
    {
        public List<Player> Players { get; private set; }
        public List<Card> CommunityCards { get; private set; }
        public Pot Pot { get; private set; }

        public Table(List<Player> players, Pot pot)
        {
            Players = players;
            CommunityCards = new List<Card>();
            Pot = pot;
        }

        public void AddCommunityCard(Card card)
        {
            CommunityCards.Add(card);
        }
    }
}