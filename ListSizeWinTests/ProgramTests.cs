#if !MSTEST
using NUnit.Framework;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif
using ListSize;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ListSize.Tests
{
    [TestClass()]
    public class ProgramTests
    {
        [TestMethod()]
        public void GetDirectorySizeTest ()
        {
            this.Setup();
            System.IO.DirectoryInfo dirInfo = new System.IO.DirectoryInfo("temp");

            // Run
            Program.Result result = Program.GetDirectorySize(dirInfo);
            Assert.IsTrue(result.Size == 0);

            // Create test file
            using (StreamWriter sw = new StreamWriter("temp/10byte.txt"))
            {
                sw.Write("0000000000");
            }

            // Run
            result = Program.GetDirectorySize(dirInfo);
            Assert.IsTrue(result.Size == 10);
        }

        private void Setup ()
        {
            // Cleanup
            if (System.IO.Directory.Exists("temp"))
            {
                System.IO.Directory.Delete("temp", true);
            }

            // Create test directory
            System.IO.Directory.CreateDirectory("temp");
        }
    }
}