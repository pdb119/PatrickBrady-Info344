using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dashboard
{
    public class TrieNode
    {
        public Dictionary<char, TrieNode> branches = new Dictionary<char, TrieNode>();
        public char prefix { get; private set; }
        public string word { get; set; }
        public TrieNode(char prefix)
        {
            this.prefix = prefix;
        }
    }
}