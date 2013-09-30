using System;
//@author: Microsoft
//Instructor: Professor Cascioli
//Date: 5/7/11 - 5/8/11
//
//Program.cs
//
//Program.cs runs the program
namespace GraphHomework
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (Game1 game = new Game1())
            {
                game.Run();
            }
        }
    }
#endif
}

