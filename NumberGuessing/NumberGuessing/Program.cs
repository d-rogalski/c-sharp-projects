namespace NumberGuessing
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var game = new NumberGuessingGame();
            
            bool start = DoInRobust(game.SetDifficulty, game.Header);
            if (start)
            {
                bool state = true;
                while (state)
                {
                    state = DoInRobust(game.MakeGuess, $"{game.Header}\n" +
                        $"Difficulty: {game.Difficulty}\n" +
                        $"Score: {(game.Count > 0 ? (float)game.Score/game.Count*100 : 0):F1}% ({game.Score}/{game.Count})");
                }
            }
        }
        static bool DoInRobust(Action action, string header)
        {
            do
            {
                try
                {
                    Console.Clear();
                    Console.WriteLine(header);
                    action.Invoke();
                    return true;
                }
                catch (FormatException)
                {
                    Console.WriteLine("Thank you for your game, come back again!");
                    return false;
                }
                catch (Exception)
                {
                    Console.WriteLine("Something was wrong with your input. Press enter and try again...");
                    Console.ReadLine();
                    continue;
                }
            } while (true);
        }
    }

}