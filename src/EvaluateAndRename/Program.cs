﻿using CommandLine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {

        static void Main(string[] args)
        {
            CommandLineArgs cmdArgs;

            if (args.Length >0)
            {
                cmdArgs = new CommandLineArgs();
                if (!Parser.Default.ParseArguments(args, cmdArgs))
                {
                    Console.ReadKey();
                    return;
                }
            }
            else
            {
                cmdArgs = new CommandLineArgs();                
            }

            if(cmdArgs.SourceDirectory == null)
            {
                cmdArgs.SourceDirectory = Environment.CurrentDirectory;
            }

            if(cmdArgs.TargetDirectory == null)
            {
                cmdArgs.TargetDirectory = Path.Combine(Environment.CurrentDirectory, Guid.NewGuid().ToString());
            }

            if (cmdArgs.Orientation == null)
            {
                cmdArgs.Orientation = 270;
            }

            ReadAndRename rename = new ReadAndRename(cmdArgs.TargetDirectory, cmdArgs.Orientation.Value);

            foreach (var file in Directory.GetFiles(cmdArgs.SourceDirectory, "*.jpg"))
            {
                Console.WriteLine($"Try rename file {file}");

                var result = rename.TryRename(file);
                var col = Console.ForegroundColor;

                switch (result.Item2)
                {
                    case 1:
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Environment.ExitCode = 1;
                        break;
                    case 2:
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        Environment.ExitCode = 2;
                        break;
                }

                Console.WriteLine(result.Item1);
                Console.ForegroundColor = col;
            }

            if (Environment.ExitCode>0)
            {
                Console.WriteLine("At least one image could not be renamed");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
        }
    }
  
}
