using System.Collections.Generic;
using System.Drawing;

namespace PokerFish.Scraber
{
    public class TrieNode
    {
        public TrieNode[] Children;
        public List<Bitmap> Residents;
        public List<string> ResidentKeys;

        public TrieNode()
        {
            Children = new TrieNode[10];
            Residents = new List<Bitmap>();
            ResidentKeys = new List<string>();
        }
    }
}