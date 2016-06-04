//////////////////////////////////////////////////////////////////////////////////////
// I MAKE HEAVY USE OF THE FOLLOWING TOOLS, THEY DESERVE FAR MORE CREDIT THAN ME
//TargaImage: http://www.codeproject.com/Articles/31702/NET-Targa-Image-Reader thank you david
//BLP2PNG: http://www.wowinterface.com/downloads/info6127-BLP2PNG.html
//////////////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using Paloma;

namespace TGA2BLP
{
    class Program
    {
        static readonly string Path = Directory.GetParent(System.Reflection.Assembly.GetEntryAssembly().Location).ToString();
        public static int Count { get; private set; } = 0;

        static void Main(string[] args)
        {
            Console.WriteLine("Enter the directory for your addons folder");
            string rootDir = Console.ReadLine();
            Console.WriteLine(Path);
            if (rootDir != null && Directory.Exists(rootDir))
            {
                DirectoryInfo root = new DirectoryInfo(rootDir);
                WalkDirectory(root);
                Console.WriteLine();
                Console.WriteLine("done: " + Count);
                Console.ReadKey();
            }
        }

        static void WalkDirectory(DirectoryInfo root)
        {
            var files = root.GetFiles("*.*");
            if (files != null)
            {
                foreach (FileInfo file in files)
                {
                    if (file.Extension.Contains("tga"))
                    {
                        List<string> fileString = file.FullName.Split('\\').ToList();
                        string filename = fileString.Last().Replace(".tga", "");
                        TargaImage workImage = new TargaImage(file.FullName);
                        string pngFile = Directory.GetParent(file.FullName) + "\\" + filename + ".png";
                        if (!File.Exists(pngFile))
                        {
                            workImage.Image.Save(pngFile);
                            workImage.Dispose();
                            ProcessStartInfo blp2Png = new ProcessStartInfo();
                            blp2Png.FileName = Path + "\\BLP2PNG.exe";
                            blp2Png.Arguments = "\"" + pngFile + "\"";
                            blp2Png.CreateNoWindow = false;
                            blp2Png.WindowStyle = ProcessWindowStyle.Hidden;
                            Process proc = Process.Start(blp2Png);
                            proc.WaitForExit();
                            File.Delete(pngFile);
                            File.Delete(file.FullName);
                            Count++;
                            Console.Write(".");
                        }   
                    }
                }
                var subDirs = root.GetDirectories();

                foreach (DirectoryInfo dir in subDirs)
                {
                    WalkDirectory(dir);
                }
            }
        }
    }
}
