using System;
using Microsoft.AspNetCore.SignalR.Client;
using System.Threading.Tasks;
using System.IO;
using System.Collections.Generic;

namespace client
{
    class Program
    {
        static string _pathstorage = "./storage";
        private static int _count = 0;//счетчик подключений
        public static HubConnection connection;
        public static string name = "";
        static void Main(string[] args)
        {
            connection = new HubConnectionBuilder().WithUrl("http://localhost:5000/master").Build();
            connection.ServerTimeout = TimeSpan.FromSeconds(60);
            Restart();
            if (connection.State == HubConnectionState.Connected)
            {
                string s = "";
                do
                {

                    if (connection.State == HubConnectionState.Disconnected)
                    {
                        Console.WriteLine("Соединение разорвано, для установки \n соединения и отправки сообщения клавиша 'y', \n для выхода клавиша любую другую клавишу");
                        ConsoleKeyInfo k = Console.ReadKey();
                        if (k.Key == ConsoleKey.Y)
                        {
                            _count = 1;
                            Restart();
                        }
                        else return;
                    }
                    s = Push();
                }
                while (s != "esc");
            }
            else
            {
                Restart();
                Console.WriteLine("Соединение не установлено, попробуйте позже");

            }
        }
        //активация рестрата подключения к серверу
        public static void Restart()
        {
            if (_count == 0)
            {
                try { connection.StartAsync().Wait(); }
                catch (Exception ex)
                {
                    Recon();
                    Console.WriteLine(ex.Message);
                }
                _count += 1;
            }
            connection.Closed += async (error) =>
                {
                    Task r = null;
                    if (_count <= 5 && _count > 0)
                    {
                        r = Task.Delay(new Random().Next(0, 5) * 1000);
                        await r;
                        Console.WriteLine("restart");
                        Recon();
                    }
                    else
                    {
                        r.Dispose();

                    }
                };
        }
        //метод подключения к серверу
        public static void Recon()
        {
            _count += 1;
            Console.WriteLine("Start");
            try { connection.StartAsync().Wait(); }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            if (connection.State != HubConnectionState.Connected && _count >= 5)
            {
                Console.WriteLine("Превышено кол-во попыток подключения");
            }

        }
        //Ввод и отправка данных серверу
        static string Push()
        {
            if (name == "")
            {
                Console.WriteLine("Введите имя пользователя");
                name = Console.ReadLine();
            }
            ConsoleKeyInfo b;
            string ret = "";
            Console.WriteLine("Введите сообщение");
            string mess = Console.ReadLine();
            StreamWriter stream = File.AppendText(_pathstorage);
            stream.WriteLine(name + ":" + mess);
            stream.Close();

            List<string> lines = new List<string>();

            foreach (string line in File.ReadLines(_pathstorage))
            {
                lines.Add(line);
            }
            try
            {
                connection.Remove("ReceiveMessage");
                connection.Remove("Receive");
                connection.InvokeCoreAsync("SendMessage", args: new[] { lines });
                File.Delete(_pathstorage);
                connection.On("ReceiveMessage", (string name, string message) =>
                {
                    Console.WriteLine(name + ":" + message);     
                });
                
                connection.On("Receive",(List<string> l)=>{
                    foreach(string st in l){
                        int a=st.IndexOf("Пользователь");
                        Console.Write(st.Substring(0,a));
                        int b=st.IndexOf("Время сообщения",a);
                        Console.ForegroundColor=ConsoleColor.Green;
                        Console.Write(st.Substring(a,b-a));
                        int c=st.IndexOf("Сообщение");
                        Console.ForegroundColor=ConsoleColor.Cyan;
                        Console.Write(st.Substring(b,c-b));   
                        Console.ForegroundColor=ConsoleColor.Gray;
                        Console.WriteLine(st.Substring(c,st.Length-c));
                        Console.ResetColor();                     
                    }
                    
                });
                
            }
            catch (Exception ex)
            {
                Console.WriteLine("К сожалению ваши сообщения не добавлены на сервер, по пробуйте позже т.к. " + ex.Message);
            }
            Console.WriteLine("Для выхода нажмите клавишу 'esc'");
            b = Console.ReadKey();
            if (b.Key == ConsoleKey.Escape)
            {
                ret = "esc";
            }
            _count = 1;
            return ret;
        }
    }
}
