using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using System;
using Microsoft.EntityFrameworkCore;
using server.Model;
using Microsoft.Extensions.DependencyInjection;

namespace server{
    public class MasterHub:Hub{
        private bdContext db;
        public MasterHub(bdContext bdContext){
            db=bdContext;
        }
        public async Task SendMessage(string user,string message){
            var context=this.Context.GetHttpContext();
            Message clientMessage=new Message();
            clientMessage.user=user;
            clientMessage.ip=context.Connection.RemoteIpAddress.ToString();
            clientMessage.message=message;
            clientMessage.time=DateTime.Now;
            db.Add(clientMessage);
            db.SaveChanges();
            await Clients.All.SendAsync("ReceiveMessage",user,message);
            Console.WriteLine(clientMessage.user+":"+clientMessage.message+" ip "+clientMessage.ip
            +" date "+clientMessage.time.ToString("dd.MM.yyyy H:mm:ss"));
        }
    }
}
