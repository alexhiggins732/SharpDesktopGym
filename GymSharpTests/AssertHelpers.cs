using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace GymSharp.Tests
{
    public static class AssertHelpers
    {
        public static void AssertEquals<T> (this T[,] first, T[,] second)
        {
            var a = first.Flatten();
            var b = second.Flatten();
            Assert.IsTrue(a.SequenceEqual(b));
        }
    }

}