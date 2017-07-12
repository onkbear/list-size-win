using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ListSize
{
    class Result
    {
        public Result()
        {
            Size = 0;
            IsError = false;
        }
        public string Type { get; set; }
        public string Name { get; set; }
        public long Size { get; set; }
        public bool IsError { get; set; }
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

            // List directories
            foreach (DirectoryInfo subDirInfo in dirInfo.GetDirectories())
            {                
                Result result = GetDirectorySize(subDirInfo);
                result.Type = "D";
                result.Name = subDirInfo.Name;
                list.Add(result);

                // Add to total
                totalSize += result.Size;
            }

            // List files
            foreach (FileInfo fileInfo in dirInfo.GetFiles())
            {
                Result result = new Result();
                result.Type = "F";
                result.Name = fileInfo.Name;
                result.Size = fileInfo.Length;
                list.Add(result);

                // Add to total
                totalSize += fileInfo.Length;
            }

            // Sort
            if (isSort)
            {
                list.Sort((x, y) => y.Size.CompareTo(x.Size));
            }

            Console.WriteLine("Type              Size Occupancy File");
            foreach (Result result in list)
            {
                string message = "";
                if (result.IsError)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    message = "[Failed]";
                }
                double percentage = (double)result.Size / totalSize * 100;
                Console.WriteLine("{0} {1,20}{2,9:0.00}% {3} {4}",
                    result.Type,
                    ConvertSizeFormat(result.Size),
                    Math.Round(percentage, 2, MidpointRounding.AwayFromZero),
                    result.Name,
                    message);
                Console.ResetColor();
            }

            Console.WriteLine("All {0,18}   100.00%", ConvertSizeFormat(totalSize));
        }

        // Cals directory size
        private static Result GetDirectorySize(DirectoryInfo dirInfo)
        {
            long size = 0;
            Result result = new Result();

            try
            {
                // Calc files in current directory
                foreach (FileInfo fileInfo in dirInfo.GetFiles())
                {
                    size += fileInfo.Length;
                }
            }
            catch (Exception)
            {
                result.IsError = true;
            }

            try
            {
                // Calc sub directories
                Parallel.ForEach(
                    dirInfo.GetDirectories(),   // source collection
                    () => 0L,                   // thread local initializer
                    (subDirInfo, loopState, localSum) =>
                    {
                        Result resultTemp = GetDirectorySize(subDirInfo);
                        localSum += resultTemp.Size;
                        if (resultTemp.IsError)
                        {
                            result.IsError = resultTemp.IsError;
                        }
                        return localSum;
                    },
                    (localSum) =>               // thread local aggregator
                    {
                        lock (dirInfo)
                        {
                            size += localSum;
                        }
                    }
                );
            }
            catch (Exception)
            {
                result.IsError = true;
            }

            result.Size = size;
            return result;
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
