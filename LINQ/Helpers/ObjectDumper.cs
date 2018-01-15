using System;
using System.Collections.Generic;
using LINQ.Classes;

namespace LINQ
{
    internal class ObjectDumper
    {
        internal static void Write(IEnumerable<Result> results)
        {
            foreach (var r in results)
            {
                Console.WriteLine("City:{0}\tCount:{1}", r.City, r.Count);
            }

            Console.ReadLine();
        }
    }
}