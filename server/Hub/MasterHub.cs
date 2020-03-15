using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using System;
using Microsoft.EntityFrameworkCore;
using server.Model;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;


namespace server
{
    public class MasterHub : Hub
    {
        private bdContext db;
        private Boolean allInfo=false;
        public MasterHub(bdContext bdContext)
        {
            db = bdContext;
        }
        public async Task SendMessage(List<string> list)
        {
            var context = this.Context.GetHttpContext();
            List<Message> clientMessages = new List<Message>();

            foreach (string body in list)
            {
                Message clientMessage = new Message();
                clientMessage.user = body.Substring(0, body.IndexOf(':') - 1);
                clientMessage.ip = context.Connection.RemoteIpAddress.ToString();
                clientMessage.message = body.Substring(body.IndexOf(':') + 1);
                if(clientMessage.message=="print") {this.allInfo=true;
                continue;}
                clientMessage.time = DateTime.Now;
                clientMessages.Add(clientMessage);
            }
            try
            {
                await db.AddRangeAsync(clientMessages);
                db.SaveChanges();
                await Clients.Caller.SendAsync("ReceiveMessage", "Ваши сообщения", " успешно добавлены");
                allInformation();
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("ReceiveMessage", "ваши сообщения не добавлены", ex.Message.ToString());
            }

        }
        public async void allInformation(){
            if(this.allInfo==true){
               List<Message> l= await db.Messages.ToListAsync();
               List<string> ls=new List<string>();
               foreach(Message mes in l){
                   ls.Add("Идентификатор: "+mes.id+" Пользователь: "+mes.user+" Время сообщения: "+mes.time.ToString()+" Сообщение: "+mes.message.ToString());
               }
               await Clients.Caller.SendAsync("Receive",ls);
                this.allInfo=false;
            }
        }
    }
}
