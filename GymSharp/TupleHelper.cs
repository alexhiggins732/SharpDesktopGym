using System.Runtime.CompilerServices;

namespace GymSharp
{
    public class TupleHelper
    {
        public static int GetLength(ITuple tuple) => tuple.Length;
        public static dynamic Item(ITuple tuple, int index) => tuple[index];
    }
}
