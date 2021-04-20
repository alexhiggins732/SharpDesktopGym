using GymSharp.Tests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSharpTests
{
    class Program
    {
        static void Main(string[] args)
        {
            var test = new TicTacToeSharpEnvironmentTests();
            test.test_randomint();
        }
    }
}
