using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Watcher
{
    class Program
    {
        static void Main(string[] args)
        {
            var loc = @"C:\Users\nickjosevski.ACMELBDC\AppData\Local\Temp";
            var ext = @"*.cs";

            var watcher = new FileSystemWatcher(loc, ext);

            watcher.EnableRaisingEvents = true;

            watcher.Created += Created;

            Console.WriteLine("waiting...");
            Console.ReadLine();
        }

        static void Created(object sender, FileSystemEventArgs e)
        {
            var x = 0;
            while (x < 1000)
            {
                try
                {
                    x++;
                    File.Copy(e.FullPath, @"C:\dev\mailzor\Watcher\bin\Debug\" + e.Name);
                }
                catch (Exception ex)
                {
                    
                }
            }

            Console.WriteLine("found: {0}", e.FullPath);
        }
    }
}
