using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading.Channels;
using muxanov_laba_2;

namespace Mukhanov_laba_2
{
    class Program
    {
        const string PATH = "passwordHashes.txt";
        static public bool foundFlag = false;

        static void startMenu()
        {
            bool flag = true;
            while (flag)
            {
                Console.WriteLine("Муханов Матвей БББО-05-20\n\nДобро пожаловать в Меню, выберите пункт программы:\n");
                Console.WriteLine("[1] задание");
                Console.WriteLine("[2] выход");
                Console.Write("\n---> ");
                int choice = int.Parse(Console.ReadLine());
                switch (choice)
                {
                    case 1:
                        string[] txtRead = File.ReadAllLines(PATH);
                        Console.WriteLine("\n\nпо какому хэшу искать: \n");
                        Console.WriteLine($"[1] {txtRead[0]}\n[2] {txtRead[1]}\n[3] {txtRead[2]}");
                        Console.Write("\n---> ");
                        int line = int.Parse(Console.ReadLine());
                        string passwordHash = txtRead[line - 1].ToUpper();
                        Console.Write("\n\nсколько потоков использовать");
                        Console.Write("\n\n---> ");
                        int countStream = int.Parse(Console.ReadLine());
                        Console.Clear();
                        Console.WriteLine("подбираю...");

                        //создаю общий канал данных
                        Channel<string> channel = Channel.CreateBounded<string>(countStream);
                        Stopwatch time = new();
                        time.Reset();
                        time.Start();
                        //создается производитель
                        var prod = Task.Run(() => { new Producer(channel.Writer); });
                        Task[] streams = new Task[countStream + 1];
                        streams[0] = prod;
                        //создаются потребители 
                        for (int i = 1; i < countStream + 1; i++)
                        {
                            streams[i] = Task.Run(() => { new Consumer(channel.Reader, passwordHash); });
                        }
                        //Ожидает завершения выполнения всех указанных объектов Task 
                        Task.WaitAny(streams);
                        time.Stop();
                        Console.WriteLine($"\nвремя: {time.Elapsed}");
                        Console.WriteLine("\nНажмите Enter для выхода в меню...\n");
                        Console.ReadLine();
                        Console.Clear();
                        foundFlag = false;
                        break;
                    case 2:
                        flag = false;
                        break;
                    default:
                        Console.WriteLine("\nОшибка! Повторите ввод!\n");
                        Thread.Sleep(1500);
                        Console.Clear();
                        break;
                }
            }
        }

        static public void Main()
        {
            startMenu();
        }
    }
}