using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using System;

namespace server{
    public class MasterHub:Hub{
        public async Task SendMessage(string user,string message){
            var context=this.Context.GetHttpContext();
            await Clients.All.SendAsync("ReceiveMessage",user,message);
            Console.WriteLine(user+":"+message+" ip "+context.Connection.RemoteIpAddress.ToString()
            +" date "+DateTime.Now.ToString("dd.MM.yyyy H:mm:ss"));
        }
    }
}
