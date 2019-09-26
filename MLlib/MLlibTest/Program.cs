using System;
using MLlib;

namespace MLlibTest
{
    class Program
    {
        static void Main(string[] args)
        {
            OBJModelLoader modelLoader = new OBJModelLoader();

            bool isRunning = true;
            while(isRunning)
            {
                Console.WriteLine("Type filepath of model: ");
                string filename = Console.ReadLine();

                Model model = modelLoader.LoadModel(filename);

                switch(Console.ReadKey().Key)
                {
                    case ConsoleKey.Backspace:
                        {
                            isRunning = false;
                            break;
                        }
                }
            }
        }
    }
}
