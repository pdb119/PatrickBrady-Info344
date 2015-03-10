using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace dashboard
{
    public class TrieSearch
    {
        private TrieNode root;
        public TrieSearch(TrieNode root)
        {
            this.root = root;
        }
        public List<string> getResults(string pref)
        {
            HttpApplicationState appState = HttpContext.Current.Application;
            if (root != null)
            {
                List<string> results = new List<string>();
                recurse(pref, root, results);
                return results;
            }
            else
            {
                List<String> emptyList = new List<string>();
                emptyList.Add("empty");
                return emptyList;
            }
        }

        private bool recurse(string remainingPrefix, TrieNode curr, List<String> results)
        {
            if (remainingPrefix == "")
            {
                if (curr.word != null && curr.word != "")
                {
                    results.Add(curr.word);
                    if (results.Count > 9)
                    {
                        return true;
                    }
                }
                foreach (char c in curr.branches.Keys)
                {
                    if (recurse("", curr.branches[c], results))
                    {
                        return true;
                    }
                }
                return false;
            }
            char[] nextPrefix = remainingPrefix.Substring(0, 1).ToCharArray();
            if (curr.branches.Keys.Contains(nextPrefix[0]))
            {
                recurse(remainingPrefix.Substring(1), curr.branches[nextPrefix[0]], results);
                return false;
            }
            return false;
        }
    }
}