namespace NumberGuessing
{
    /// <summary>
    /// A class providing a full funcionality of the number guessing game.
    /// </summary>
    internal class NumberGuessingGame
    {
        /// <summary>
        /// Header of the game, that can be displayed at the top of the console.
        /// </summary>
        public readonly string Header = "-----Number Guessing Game-----\n";
        /// <summary>
        /// Seed of the RNG used to generate the numbers.
        /// </summary>
        public int Seed { get; }
        /// <summary>
        /// Upper bound of the numbers to generate.
        /// </summary>
        public int Maximum { get; set; }
        /// <summary>
        /// Score of the current game - number of correct guesses.
        /// </summary>
        public int Score { get => score; }
        /// <summary>
        /// Count of the current game - number of generated numbers.
        /// </summary>
        public int Count { get => count; }
        /// <summary>
        /// Name of the game's difficulty level.
        /// </summary>
        public string Difficulty
        {
            get
            {
                switch (difficulty)
                {
                    case 1:
                        return "Very Easy (1-2)";
                    case 2:
                        return "Easy (1-4)";
                    case 3:
                        return "Medium (1-6)";
                    case 4:
                        return "Hard (1-10)";
                    case 5:
                        return "Very Hard (1-15)";
                    default:
                        return null;
                }
            }
        }

        /// <summary>
        /// Random number generator for the game.
        /// </summary>
        private readonly Random rng;
        /// <summary>
        /// Variable storing the number of guesses.
        /// </summary>
        private int count;
        /// <summary>
        /// Variable storing the number of points
        /// </summary>
        private int score;
        /// <summary>
        /// Variable storing the last generated number
        /// </summary>
        private int currentNumber;
        /// <summary>
        /// Variable storing the difficulty level
        /// </summary>
        private int difficulty;

        /// <summary>
        /// Creates a new game object with the default difficulty (Easy) and default seed (tick count) for RNG.
        /// </summary>
        public NumberGuessingGame()
        {
            Seed = Environment.TickCount;
            count = 0;
            score = 0;
            difficulty = 2;
            rng = new Random(Seed);
        }
        /// <summary>
        /// Creates a new game object with the default difficulty (Easy) and the provided seed for RNG.
        /// </summary>
        public NumberGuessingGame(int seed)
        {
            Seed = seed;
            count = 0;
            score = 0;
            difficulty = 2;
            rng = new Random(Seed);
        }
        /// <summary>
        /// Generates a new random number and prints a message about it.
        /// </summary>
        private void GenerateNumber()
        {
            currentNumber = rng.Next(1, Maximum + 1);
            Console.WriteLine("A new number has been generated.");
        }
        /// <summary>
        /// Provides a single round of the game with taking the input from the player and printing the result.
        /// </summary>
        /// <exception cref="ArgumentNullException">User wants to exit the game.</exception>
        public void MakeGuess()
        {
            GenerateNumber();
            Console.WriteLine("Make your guess or press enter to exit the game: ");
            int tmp;
            int guess = int.TryParse(Console.ReadLine(), out tmp) ? tmp : 0;

            count++;
            if (guess == currentNumber)
            {
                score++;
                Console.WriteLine($"Congratulations! You correctly guessed that the number is {guess}!");
            }
            else if (guess == 0)
            {
                throw new ArgumentNullException();
            }
            else
            {
                Console.WriteLine($"Too bad! The number was {currentNumber}, not {guess}. Better luck next time!");
            }
            Console.WriteLine("Press enter to continue...");
            Console.ReadLine();
        }
        /// <summary>
        /// Provides a difficulty selection menu.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Incorrect choice.</exception>
        /// <exception cref="ArgumentNullException">Exit the game.</exception>
        public void SetDifficulty()
        {
            Console.WriteLine("Choose difficulty level:\n" +
                "1 - Very Easy (Numbers from 1 to 2)\n" +
                "2 - Easy (Numbers from 1 to 4)\n" +
                "3 - Medium (Numbers from 1 to 6)\n" +
                "4 - Hard (Numbers from 1 to 10)\n" +
                "5 - Very Hard (Numbers from 1 to 15)\n" +
                "Or press enter to exit...");

            difficulty = int.TryParse(Console.ReadLine(), out difficulty) ? difficulty : 0;
            switch (difficulty)
            {
                case 0:
                    throw new ArgumentNullException();
                case 1:
                    Maximum = 2;
                    break;
                case 2:
                    Maximum = 4;
                    break;
                case 3:
                    Maximum = 6;
                    break;
                case 4:
                    Maximum = 10;
                    break;
                case 5:
                    Maximum = 15;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("difficulty", "The difficulty level should be between 0 and 4 (inclusive).");
            }
        }
    }
}
