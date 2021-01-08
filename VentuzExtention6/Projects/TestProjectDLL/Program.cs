using System;
using Ventuz.Extention.Conf;
using PartyGovArmy;
using System.IO;
using System.Text.RegularExpressions;

namespace TestProjectDLL
{
    class Program
    {

      

        static void Main(string[] args)
        {
            string x = "hello world \"but this is a string\" so it is.";
            string[] parts = Regex.Split(x, "\"([^\"]*?)\"|(\\S+)");

            Console.WriteLine("hello");
            foreach(string v in parts)
            {
                string y = v.Trim();
                if(!string.IsNullOrEmpty(y))
                {
                    Console.WriteLine(y);
                }
            }

            Console.WriteLine("----");
        }
    }
}
