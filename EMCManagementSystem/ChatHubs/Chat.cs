using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace EMCManagementSystem.ChatHubs
{
    public class Chat : Hub
    {
        public void Send(string name, string message)
        {
            //Clients.All.send(name, message);
            //Clients.Caller.send("yao2", message);
            Clients.All.addNewMessageToPage(name, message);
        }
    }
}