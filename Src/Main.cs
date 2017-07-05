using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ListSize
{
    struct Result
    {
        public string Type { get; set; }
        public string Name { get; set; }
        public long Size { get; set; }
    }

    class Program
    {
        // The static method, Main, is the application's entry point.
        public static int Main(string[] args)
        {
            // Display help
            if (args.Length == 1 && IsHelpRequired(args[0]))
            {
                Console.WriteLine("Usage: lss [OPTION] [FILE]");
                Console.WriteLine("List directory contents size");
                Console.WriteLine("");
                Console.WriteLine("{0,4} {1,-20}Display this help and exit.", "-h,", "--help");
                Console.WriteLine("{0,4} {1,-20}Sort by file size.", "-s,", "--sort");
            }
            // Execute
            else
            {
                if (1 <= args.Length && !args[args.Length - 1].StartsWith("-"))
                {
                    List(args[args.Length - 1]);
                }
                else
                {
                    List(".");
                }
            }

            return 0;
        }

        private static void List(string path)
        {
            bool isSort = false;

            // args[0] is this executable file name
            string[] args = Environment.GetCommandLineArgs();
            if (2 <= args.Length)
            {
                isSort = IsSortRequired(args[1]);
            }

            long totalSize = 0;
            List<Result> list = new List<Result>();

            System.IO.DirectoryInfo dirInfo = new System.IO.DirectoryInfo(path);

            foreach (DirectoryInfo subDirInfo in dirInfo.GetDirectories())
            {
                long dirsize = GetDirectorySize(subDirInfo);
                totalSize += dirsize;

                Result result = new Result();
                result.Type = "D";
                result.Name = subDirInfo.Name;
                result.Size = dirsize;
                list.Add(result);
            }

            foreach (FileInfo fileInfo in dirInfo.GetFiles())
            {
                totalSize += fileInfo.Length;

                Result result = new Result();
                result.Type = "F";
                result.Name = fileInfo.Name;
                result.Size = fileInfo.Length;
                list.Add(result);
            }

            // Sort
            if (isSort)
            {
                list.Sort((x, y) => y.Size.CompareTo(x.Size));
            }

            foreach (Result result in list)
            {
                double percentage = (double)result.Size / totalSize * 100;
                Console.WriteLine("{0} {1,20}{2,7:0.00}% {3}",
                    result.Type,
                    ConvertSizeFormat(result.Size),
                    Math.Round(percentage, 2, MidpointRounding.AwayFromZero),
                    result.Name);
            }

            Console.WriteLine("All {0,18} 100.00%", ConvertSizeFormat(totalSize));
        }

        // Cals directory size
        private static long GetDirectorySize(DirectoryInfo dirInfo)
        {
            long size = 0;

            // Calc files in current directory
            try
            {
                Parallel.ForEach(dirInfo.GetFiles(), fileInfo => {
                    size += fileInfo.Length;
                });
            }
            catch (Exception exception)
            {
                // TODO
                if (exception is UnauthorizedAccessException)
                {
                }
                else if (exception is PathTooLongException)
                {
                }
            }

            // Calc sub directories
            try
            {
                Parallel.ForEach(dirInfo.GetDirectories(), subDirInfo => {
                    size += GetDirectorySize(subDirInfo);
                });
            }
            catch (Exception exception)
            {
                // TODO
                if (exception is UnauthorizedAccessException)
                {
                }
                else if (exception is PathTooLongException)
                {
                }
            }

            return size;
        }

        private static string ConvertSizeFormat(long size)
        {
            return String.Format("{0:#,0}", size);
        }

        private static bool IsHelpRequired(string param)
        {
            return (param == "-h" || param == "--help");
        }

        private static bool IsSortRequired(string param)
        {
            return (param == "-s" || param == "--sort");
        }
    }
}
