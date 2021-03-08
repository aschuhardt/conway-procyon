using ProcyonSharp;

namespace Conway
{
    public class Program
    {
        public static void Main(string[] args)
        {
            using var game = Game.Create<GameState, MenuState>();
            game.Start(800, 600, "Conway's Game of Life");
        }
    }
}