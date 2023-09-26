namespace NumberGuessing
{
    internal class NumberGuessingGame
    {
        public readonly string Header = "-----Number Guessing Game-----";
        public int Seed { get; }
        public int Maximum { get; set; }
        public int Score { get => score; }
        public int Count { get => count; }
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


        private readonly Random rng;
        private int count, score;
        private int currentNumber;
        private int difficulty;

        public NumberGuessingGame()
        {
            Seed = Environment.TickCount;
            count = 0;
            score = 0;
            difficulty = 2;
            rng = new Random(Seed);
        }
        public NumberGuessingGame(int seed)
        {
            Seed = seed;
            count = 0;
            score = 0;
            difficulty = 2;
            rng = new Random(Seed);
        }

        private void GenerateNumber()
        {
            currentNumber = rng.Next(1, Maximum + 1);
            Console.WriteLine("A new number has been generated.");
        }

        public void MakeGuess()
        {
            GenerateNumber();
            Console.WriteLine("Make your guess or press enter to exit the game: ");
            int guess = Convert.ToInt32(Console.ReadLine());

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
        public void SetDifficulty()
        {
            Console.WriteLine("Choose difficulty level:");
            Console.WriteLine("1 - Very Easy (Numbers from 1 to 2");
            Console.WriteLine("2 - Easy (Numbers from 1 to 4");
            Console.WriteLine("3 - Medium (Numbers from 1 to 6");
            Console.WriteLine("4 - Hard (Numbers from 1 to 10");
            Console.WriteLine("5 - Very Hard (Numbers from 1 to 15");
            Console.WriteLine("Or press enter to exit...");

            difficulty = Convert.ToInt32(Console.ReadLine());
            switch (difficulty)
            {
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
