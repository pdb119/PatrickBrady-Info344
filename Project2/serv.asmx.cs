using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Web.Services;
using System.IO;
using Microsoft.WindowsAzure.ServiceRuntime;
using System.Diagnostics;
using System.Web.Script.Serialization;

namespace server1
{
    /// <summary>
    /// Summary description for serv
    /// </summary>
    [WebService(Namespace = "http://bradypat.cloudapp.net/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class serv : System.Web.Services.WebService
    {
        private TrieSearch searcher;

        [WebMethod]
        public string buildTrie()
        {            
            return TrieBuild.buildTrie();
        }

        [WebMethod]
        public string getResults(string pref)
        {
            pref = pref.ToLower();
            if (searcher == null)
            {
                HttpApplicationState appState = HttpContext.Current.Application;
                searcher = new TrieSearch((TrieNode)appState["trieRoot"]);                
            }
            return "{\"results\":" + new JavaScriptSerializer().Serialize(searcher.getResults(pref)) + "}";
        }
    }
}
