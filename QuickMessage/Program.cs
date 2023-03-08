using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace quickMessage
{
    class Program
    {
        static void Main(string[] args)
        {

            Console.WriteLine("Hello world! :)\n");
            Console.WriteLine(QuickMessage.genQuickMessage("Are you going to the meeting today at 6pm?"));
            Console.ReadLine();
        }
    }
}