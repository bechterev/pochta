using System;
using Microsoft.AspNetCore.SignalR.Client;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using System.Threading;

namespace client
{
    class Program
    {
        private static int _count=0;
        public static HubConnection connection;
        static void Main(string[] args)
        {
            connection = new HubConnectionBuilder().WithUrl("http://localhost:5000/master").Build();
            connection.ServerTimeout = TimeSpan.FromSeconds(10);

         
            Restart();
            ConsoleKeyInfo b;
            string name = "";
            if (connection.State == HubConnectionState.Connected)
            {
                do
                {
                    if (name == "")
                    {
                        Console.WriteLine("Введите имя пользователя");
                        name = Console.ReadLine();
                    }
                    Console.WriteLine("Введите сообщение");
                    string mess = Console.ReadLine();
                    connection.InvokeCoreAsync("SendMessage", args: new[] { name, mess });
                    connection.On("ReceiveMessage", (string name, string message) =>
                    {
                        Console.WriteLine(name + ":" + message);
                    });
                    if(connection.State == HubConnectionState.Disconnected){
                        Console.WriteLine("Соединение разорвано");
                        Restart();
                    }
                    b = Console.ReadKey();
                }
                while (b.Key != ConsoleKey.Escape);
            }
            else
            {
                Console.WriteLine("Соединение не установлено");
                Restart();
            }


        }
        public  static void Restart(){
            if(_count==0){
                   try { connection.StartAsync().Wait(); }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            }
                  Timer timer=new Timer(new TimerCallback(Recon),null,0,2000);
                if(_count>=5){timer.Dispose();}
                
            
        }
        public static void Recon(object obj){
               try{connection.StartAsync().Wait();}
                            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
                if(connection.State!=HubConnectionState.Connected)
            {
                Console.WriteLine("Превышено кол-во попыток подключения");
            }
            
        }
    }
}
