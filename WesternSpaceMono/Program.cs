using System;

namespace WesternSpace
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            //using (Game1 game = new Game1())
            //{
            //    game.Run();
            //}

            //using (TilingEngineTest test = new TilingEngineTest())
            //{
            //    test.Run();
            //}

            using (ScreenManager.Instance)
            {
                ScreenManager.Instance.Run();
            }
        }
    }
}

