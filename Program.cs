using System;
using System.Reflection;
using System.Collections.Generic;
using System.Collections;

namespace Challenges
{
    public class Solution
    {
        public static void Main(string[] args)
        {

            var cloningServiceTest = new CloningServiceTest();
            var allTests = cloningServiceTest.AllTests;
            while (true)
            {
                var line = Console.ReadLine();
                if (string.IsNullOrEmpty(line))
                    break;
                var test = allTests[int.Parse(line)];
                try
                {
                    test.Invoke();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed on {test.GetMethodInfo().Name}." + ex.Message + ex.StackTrace);
                }
            }
            Console.WriteLine("Done.");


            /*HashSet<object> set = new HashSet<object>();
            set.Add(1);
            LinkedList<int> list = new LinkedList<int>();
            list.AddFirst(10);
            list.AddFirst(9);
            set.Add(2);
            set.Add(1);
            set.Add(list);
            LinkedList<int> list2 = new LinkedList<int>();
            list.AddFirst(21);
            list.AddFirst(20);
            set.Add(list2);
            
            foreach (object t in set)
            {
                if (t.GetType().IsPrimitive)
                {
                    Console.WriteLine(t);
                }
                else
                {
                    if (t is IEnumerable)
                    {
                        var e = t as IEnumerable;
                        foreach (var x in e)
                        {
                            Console.WriteLine(x);
                        }
                    }
                }
            }*/
        }
    }
}
