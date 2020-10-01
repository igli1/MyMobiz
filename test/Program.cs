using System;

namespace test
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            string target = "EurKm";
            Class1 item = new Class1();
            System.Reflection.PropertyInfo pi = item.GetType().GetProperty(target);
            pi.SetValue(item, 1.3);
            Console.WriteLine(item.EurKm);
        }
    }
}
