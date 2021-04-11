using System;
using System.Collections.Generic;
using System.Diagnostics;
using PokerFish.WindowHandling;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Text;
using PokerFish.Scraber;
using PokerFish.Poker;

namespace PokerFish
{    
    internal class Program
    {
        public static void Main(string[] args)
        {
            //var obs = new Observer.Observer();
            //obs.Start();

            var playes = new List<Player>()
                {new Player("A"), new Player("B"), new Player("C"), new Player("D"), new Player("E"), new Player("F")};
            var table = new Table(playes, new Pot());
            var engine = new Engine(table);
            foreach (var p in engine.PlayersInOrderOfBetting(true))
            {
                Console.WriteLine(p.Name);    
            }
            engine.MoveBlinds();
            foreach (var p in engine.PlayersInOrderOfBetting(false))
            {
                Console.WriteLine(p.Name);    
            }
            
        }
    }
}