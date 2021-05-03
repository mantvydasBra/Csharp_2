using System;
using ClassLibrary_5_uzd;
using System.Management;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime;

namespace TEST
{
    class Program
    {
        static void Main(string[] args)
        {

            Machine test = new Machine();
            try
            {
                var mainObj = test.SystemInfo();

                object value;
                foreach (var n in mainObj.Keys)
                {
                    mainObj.TryGetValue(n, out value);
                    if (value.GetType().IsGenericType && value is IList<Object>)
                    {
                        Console.WriteLine("------------" + n + "--------------");
                        List<Object> b = (List<object>)value;
                        foreach(var c in b)
                        {
                            Console.WriteLine(c);
                        }
                    }
                    else if (value.GetType().IsGenericType && value is IDictionary<string, List<Object>>)
                    {
                        Console.WriteLine("------------" + n + "--------------");
                        Dictionary<string, List<Object>> temp = (Dictionary<string, List<object>>)value;
                        foreach (var key in temp)
                        {
                            Console.WriteLine(key.Key);
                            foreach (var tempList in temp[key.Key])
                            {
                                Console.WriteLine(tempList);
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("------------" + n + "--------------");
                        Console.WriteLine(value);
                    }
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.ToString());
            }
            Console.WriteLine("------------EVENTS--------------");
            IEnumerable<dynamic> omg = test.GetEvents("ESENT");
            foreach(var n in omg)
            {
                Console.WriteLine(n);
            }
        }
    }
}
