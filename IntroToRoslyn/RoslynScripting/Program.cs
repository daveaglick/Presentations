using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Scripting;

namespace RoslynScripting
{
    class Program
    {
        static void Main(string[] args)
        {
            // Simple script
            var result = CSharpScript.EvaluateAsync("1 + 3").Result;

            // Script with variables
            //var script = CSharpScript.Create("int x = 1 + 3;");
            //var result = script.RunAsync().Result;

            // Script with external class instance
            //var value = CSharpScript.RunAsync("X + Y", null, new MyVars { X = 5, Y = 7 }).Result;

            // More examples: https://github.com/dotnet/roslyn/wiki/Scripting-API-Samples

        }
    }

    public class MyVars
    {
        public int X { get; set; }
        public int Y { get; set; }
    }

}
