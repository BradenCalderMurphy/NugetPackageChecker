using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NugetPackageChecker {
    class Program {
        static void Main(string[] args) {
            if (args == null || args.Length < 1) {
                Console.WriteLine("Invalid argument. Please provide a folder name.");
            }
            else {
                PackageHandle handle = new PackageHandle(args[0]);
                List<string> lst = handle.GetDiscrepancies();
                lst.ForEach(x => Console.WriteLine(x));
            }
            Console.ReadLine();
        }
    }
}
