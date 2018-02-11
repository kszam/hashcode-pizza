using System;

namespace Pizza_Slicing
{
    class StartUp
    {
        static void Main(string[] args)
        {
            Pizza pi = new Pizza();
            if (args.Length > 1)
            {
                pi.readProblemFromFile(args[0]);
                pi.outputFileRoot = args[1]; 
            }
            else
            {
                pi.readProblemFromFile(@"C:\temp\Pizzas\small.in");
                // pi.readProblemFromFile(@"C:\temp\Pizzas\medium.in");
                // pi.readProblemFromFile(@"C:\temp\Pizzas\big.in");
                // pi.readProblemFromFile(@"C:\temp\Pizzas\example.in");
                pi.outputFileRoot = (@"C:\temp\Pizzas\smallSolutionScore");
            }
            pi.findPieces();
            Console.ReadLine();
        }
    }
}
