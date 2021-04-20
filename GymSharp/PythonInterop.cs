using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace GymSharp
{

    public static class ArrayExtensions
    {
        public static T[,] To2D<T>(this List<T[]> source)
            => source.ToArray().To2D();

        public static T[,] To2D<T>(this T[][] source)
        {
            try
            {
                var FirstDim = source.Length;
                var SecondDim =
                    source.GroupBy(row => row.Length).Single()
                        .Key; // throws InvalidOperationException if source is not rectangular

                var result = new T[FirstDim, SecondDim];
                for (var i = 0; i < FirstDim; ++i)
                    for (var j = 0; j < SecondDim; ++j)
                        result[i, j] = source[i][j];

                return result;
            }
            catch (InvalidOperationException)
            {
                throw new InvalidOperationException("The given jagged array is not rectangular.");
            }
        }


        public static object[] ToArray(this ITuple tuple)
        {
            var len = tuple.Length;
            var result = new object[len];
            for (var i = 0; i < len; i++)
                result[i] = tuple[i];

            return result;
        }

        public static T[] Flatten<T>(this T[,] array)
        {
            return array.ToJagged().SelectMany(x => x).ToArray();
        }

        public static T[,] To2DFast<T>(this List<T[]> source) where T : unmanaged
        {
            return source.ToArray().To2DFast();
        }

        public static T[][] ToJagged<T>(this T[,] array)
        {
            var rank = array.Rank;
            var len = array.GetLength(0);
    
            var numRows = array.GetLength(0);
            var numColumns = array.GetLength(1);
            var result = new T[numRows][];
            for(var i=0; i< numRows; i++)
            {
                var row = new T[numColumns];
                for (var column = 0; column < numColumns; column++)
                {
                    row[column] = array[i, column];
                }
                result[i] = row;
            }
            return result;
        }

        public static (T, T)[] ToTupleArray<T>(this T[,] array)
            => array.ToJagged().Select(x => (x[0], x[1])).ToArray();


        public static T[,] To2DFast<T>(this T[][] source) where T : unmanaged
        {
            var dataOut = new T[source.Length, source[0].Length];
            var assertLength = source[0].Length;

            unsafe
            {
                for (var i = 0; i < source.Length; i++)
                {
                    if (source[i].Length != assertLength)
                    {
                        throw new InvalidOperationException("The given jagged array is not rectangular.");
                    }

                    fixed (T* pDataIn = source[i])
                    {
                        fixed (T* pDataOut = &dataOut[i, 0])
                        {
                            CopyBlockHelper.SmartCopy<T>(pDataOut, pDataIn, assertLength);
                        }
                    }
                }
            }

            return dataOut;
        }
        // Inspired by:
        // http://xoofx.com/blog/2010/10/23/high-performance-memcpy-gotchas-in-c/
        public class CopyBlockHelper
        {

            private const int BlockSize = 16384;

            private static readonly CopyBlockDelegate CpBlock = GenerateCopyBlock();

            private unsafe delegate void CopyBlockDelegate(void* des, void* src, uint bytes);

            private static unsafe void CopyBlock(void* dest, void* src, uint count)
            {
                var local = CpBlock;
                local(dest, src, count);
            }

            static CopyBlockDelegate GenerateCopyBlock()
            {
                // Don't ask...
                var method = new DynamicMethod("CopyBlockIL", typeof(void),
                    new[] { typeof(void*), typeof(void*), typeof(uint) }, typeof(CopyBlockHelper));
                var emitter = method.GetILGenerator();
                // emit IL
                emitter.Emit(OpCodes.Ldarg_0);
                emitter.Emit(OpCodes.Ldarg_1);
                emitter.Emit(OpCodes.Ldarg_2);
                emitter.Emit(OpCodes.Cpblk);
                emitter.Emit(OpCodes.Ret);

                // compile to delegate
                return (CopyBlockDelegate)method.CreateDelegate(typeof(CopyBlockDelegate));
            }

            public static unsafe void SmartCopy<T>(T* pointerDataOutCurrent, T* pointerDataIn, int length) where T : unmanaged
            {
                var sizeOfType = sizeof(T);

                var numberOfBytesInBlock = Convert.ToUInt32(sizeOfType * length);

                var numOfIterations = numberOfBytesInBlock / BlockSize;
                var overheadOfLastIteration = numberOfBytesInBlock % BlockSize;

                uint offset;
                for (var idx = 0u; idx < numOfIterations; idx++)
                {
                    offset = idx * BlockSize;
                    CopyBlock(pointerDataOutCurrent + offset / sizeOfType, pointerDataIn + offset / sizeOfType, BlockSize);
                }

                offset = numOfIterations * BlockSize;
                CopyBlock(pointerDataOutCurrent + offset / sizeOfType, pointerDataIn + offset / sizeOfType, overheadOfLastIteration);
            }
        }
    }
    public class PythonInterop
    {




        /// <summary>
        /// Copies data from a NumPy array to the destination .NET array
        /// </summary>
        /// <param name="pSource">Pointer to a NumPy array to copy from</param>
        /// <param name="dest">.NET array to be copied into</param>
        /// <remarks>This routine handles Boolean arrays in a special way because NumPy arrays have each element occupying 1 byte while .NET has them occupying 4 bytes</remarks>
		public static void CopyFromPointer(IntPtr pSource, Array dest)
        {
            Type elementType = dest.GetType().GetElementType();
            int sizeOfElement = Marshal.SizeOf(elementType);
            if (elementType == typeof(Boolean))
                sizeOfElement = 1;

            int byteCount = sizeOfElement;
            for (int i = 0; i < dest.Rank; i++)
            {
                byteCount *= dest.GetLength(i);
            }

            var gch = GCHandle.Alloc(dest);
            var tPtr = Marshal.UnsafeAddrOfPinnedArrayElement(dest, 0);
            MemCopy(pSource, tPtr, byteCount);
            gch.Free();
        }

        /// <summary>
        /// Copies data from a .NET array to a NumPy array
        /// </summary>
        /// <param name="source">.NET array to be copied from</param>
        /// <param name="pDest">Pointer to a NumPy array to copy into</param>
		public static void CopyToPointer(Array source, IntPtr pDest)
        {
            Type elementType = source.GetType().GetElementType();
            int sizeOfElement = Marshal.SizeOf(elementType);
            if (elementType == typeof(Boolean))
                sizeOfElement = 1;

            int byteCount = sizeOfElement;
            for (int i = 0; i < source.Rank; i++)
                byteCount *= source.GetLength(i);

            var gch = GCHandle.Alloc(source);
            var tPtr = Marshal.UnsafeAddrOfPinnedArrayElement(source, 0);
            MemCopy(tPtr, pDest, byteCount);
            gch.Free();
        }

        private static readonly int SIZEOFINT = Marshal.SizeOf(typeof(int));
        private static unsafe void MemCopy(IntPtr pSource, IntPtr pDest, int byteCount)
        {
            int count = byteCount / SIZEOFINT;
            int rest = byteCount % count;
            unchecked
            {
                int* ps = (int*)pSource.ToPointer(), pd = (int*)pDest.ToPointer();
                // Loop over the cnt in blocks of 4 bytes, 
                // copying an integer (4 bytes) at a time:
                for (int n = 0; n < count; n++)
                {
                    *pd = *ps;
                    pd++;
                    ps++;
                }
                // Complete the copy by moving any bytes that weren't moved in blocks of 4:
                if (rest > 0)
                {
                    byte* ps1 = (byte*)ps;
                    byte* pd1 = (byte*)pd;
                    for (int n = 0; n < rest; n++)
                    {
                        *pd1 = *ps1;
                        pd1++;
                        ps1++;
                    }
                }
            }
        }
    }
}
