using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Threading_Control
{
    public class FileWatcher
    {
        private string _path;

        public DateTime dateTime = DateTime.MinValue;
        public FileWatcher(string path)
        {
            _path = path;
        }
        public Action <DateTime,string> Change;

      public void Start()
        {
            Thread thread = new Thread(() =>
             {
                 dateTime = File.GetLastWriteTime(_path);
                 while (true)
                 {
                     if (dateTime < File.GetLastWriteTime(_path))
                     {
                         dateTime = File.GetLastWriteTime(_path);
                         Change?.Invoke(dateTime, _path);
                     }
                 }
             });
            thread.Start();
         
        }
   
    }
    class Program
    {
        private static object obj = new object();

        static void Main(string[] args)
        {
            var text = "TextControl.txt";
            if(File.Exists(text))
            File.Create(text).Close();

            var watcher = new FileWatcher(text);
            watcher.Change += Watcher_Change;
            watcher.Start();
                while (true)
                {
                    Console.WriteLine("Do your whant change 0 on 1?");
                    var yes = Console.ReadLine();
                    if (yes == "yes")
                    {

                        File.WriteAllText(text, "1");
                    }
                }
            
        }

        private static void Watcher_Change(DateTime changeTime, string path)
        {
            Console.WriteLine("Change" + changeTime);
            ThreadPool.QueueUserWorkItem((a =>
            {
                lock (obj)
                {
                    string container = File.ReadAllText(path);

                    if (container == "1")
                    {
                        File.WriteAllText(path, "0");
                        Thread.Sleep(100000);

                        Console.WriteLine("Ok");
                    }
                }
            }));
        }
    }
}
