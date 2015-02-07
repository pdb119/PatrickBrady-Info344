using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using System.Web;
using System.Net;

namespace server1
{
    public class WebRole : RoleEntryPoint
    {
        public override bool OnStart()
        {
            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.
            //TrieBuild.buidTrie();            
            //string s = GlobalVars.tester;   
            //HttpContext.Current.Application = new HttpApplicationState();
            //HttpApplicationState appState = HttpContext.Current.Application;
            //HttpRequest req = new HttpRequest("index.html");
            //HttpContext.Current = new HttpContext();
            //appState["test"] = "tesete";
            try
            {
                WebRequest req = WebRequest.Create("http://bradypat.cloudapp.net/serv.asmx/buildTrie");
                //WebRequest req = WebRequest.Create("http://localhost:50196/serv.asmx/buildTrie");
                //WebRequest req = WebRequest.Create(new Uri("index.html", UriKind.Relative));
                WebResponse res = req.GetResponse();
                res.Close();
            }
            catch (WebException e)
            {
                
            }
            //TrieBuild.buildTrie();
            return base.OnStart();
        }
    }
    /*
    public static class GlobalVars
    {
        public static TrieNode root;        public static string tester;
    }
      */
}
