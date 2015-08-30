using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolitaireStat
{
    class Program
    {
        static private SolitaireGame game = null;
        static private SolitaireBot robot = null;

        static void Main(string[] args)
        {
            string input = string.Empty;
            UnoGame ugame = null;

            while (string.IsNullOrEmpty(input) || input.First() != 'q')
            {
                System.Console.Write("Provide an option: ");
                
                input = Console.ReadLine().ToLower();

                System.Console.WriteLine("");

                switch (input.First())
                {
                    case 'c':
                        if (game == null || robot == null)
                        {
                            game = new SolitaireGame();
                            robot = new SolitaireBot(game);
                        }

                        robot.Step();
                        break;
                    case 'g':
                        game = new SolitaireGame();
                        robot = new SolitaireBot(game);

                        System.Console.WriteLine("Starting setup: ");
                        game.DisplayState();

                        System.Console.WriteLine("Robot going to work...\n\n");

                        RunSolitaireRobot(1);

                        System.Console.WriteLine("Ending setup: ");
                        game.DisplayState();
                        break;
                    case 's':
                        int totalRounds = GetIntInput("Number of rounds: ", 100);
                        RunSolitaireRobot(totalRounds);
                        break;
                    case 'u':
                        {
                            int numOfPlayers = GetIntInput("Number of players: ", 4);
                            UnoBehavior[] behaviors = GetPlayerBehaviors(numOfPlayers);
                            int numOfGames = GetIntInput("Number of games: ", 100);
                            RunUnoBots(numOfGames, behaviors);
                            break;
                        }
                    case 'o':
                        if (ugame == null)
                        {
                            int numOfPlayers = GetIntInput("Number of players: ", 4);
                            UnoBehavior[] behaviors = GetPlayerBehaviors(numOfPlayers);
                            ugame = new UnoGame(behaviors);
                            ugame.SetupPlay();

                            System.Console.WriteLine("Game Setup: ");
                            ugame.DisplayState();
                            System.Console.WriteLine("Game Starting... \n");
                        }

                        ugame.Step();
                        ugame.DisplayState();
                        break;
                }
            }
        }

        private static int GetIntInput(string msg, int defaultValue)
        {
            int value = 0;
            System.Console.Write(msg);
            string input = Console.ReadLine();

            bool success = int.TryParse(input, out value);
            value = success ? value : defaultValue;

            return value;
        }

        private static UnoBehavior[] GetPlayerBehaviors(int numOfPlayers)
        {
            UnoBehavior[] behaviors = new UnoBehavior[numOfPlayers];

            System.Console.WriteLine("Specify each player behavior (A = Aggressive, P = Passive, R = Random)\n");

            for (int i = 0; i < behaviors.Length; i++)
            {
                System.Console.Write("Player " + i + " behavior: ");
                string input = Console.ReadLine().ToLower();

                System.Console.WriteLine("");

                while (input != "a" && input != "p" && input != "r")
                {
                    System.Console.WriteLine("Invalid option given!\n");
                    System.Console.Write("Player " + i + " behavior: ");
                    input = Console.ReadLine().ToLower();
                }

                switch(input)
                {
                    case "a":
                        behaviors[i] = UnoBehavior.Aggressive;
                        break;
                    case "p":
                        behaviors[i] = UnoBehavior.Passive;
                        break;
                    case "r":
                        behaviors[i] = UnoBehavior.Random;
                        break;
                }

                System.Console.WriteLine("\n");
            }

            return behaviors;
        }

        private static void RunSolitaireRobot(int numOfRounds)
        {
            int numOfWins = 0;
            int totalMoves = 0;
            long totalOfAllRunsMS = 0;
            Stopwatch totalRunSw = new Stopwatch();
            Stopwatch eachRunSw = new Stopwatch();

            totalRunSw.Start();
            for (int i = 0; i < numOfRounds; i++)
            {
                game = new SolitaireGame();
                robot = new SolitaireBot(game);

                eachRunSw.Restart();
                bool isWin = robot.Go();
                eachRunSw.Stop();

                if (isWin)
                {
                    numOfWins++;
                }

                totalOfAllRunsMS += eachRunSw.ElapsedMilliseconds;
                totalMoves += robot.TotalMoves;
            }
            totalRunSw.Stop();

            double winRate = (double)numOfWins / (double)numOfRounds;
            double avgMovesPerRound = (double)totalMoves / (double)numOfRounds;
            double avgTimePerRound = (double)totalOfAllRunsMS / (double)numOfRounds;

            System.Console.WriteLine("Total rounds: " + numOfRounds);
            System.Console.WriteLine("Total Wins: " + numOfWins);
            System.Console.WriteLine("Total Losses: " + (numOfRounds - numOfWins));
            System.Console.WriteLine("Avg moves per round: " + avgMovesPerRound);
            System.Console.WriteLine("Total time: " + totalRunSw.ElapsedMilliseconds + " ms");
            System.Console.WriteLine("Avg time per round: " + avgTimePerRound + " ms");
            System.Console.WriteLine("Win rate: " + (winRate * 100.0) + "%\n");
        }

        private static void RunUnoBots(int numOfRounds, UnoBehavior[] behaviors)
        {
            int numOfPlayers = behaviors.Length;
            int[] wins = new int[numOfPlayers];
            int totalMoves = 0;
            long totalOfAllRunsMS = 0;
            Stopwatch allGamesSw = new Stopwatch();
            Stopwatch eachGameSw = new Stopwatch();

            allGamesSw.Start();
            for (int i = 0; i < numOfRounds; i++)
            {
                UnoGame game = new UnoGame(behaviors);

                eachGameSw.Restart();
                int winner = game.Play();
                eachGameSw.Stop();

                wins[winner]++;

                totalOfAllRunsMS += eachGameSw.ElapsedMilliseconds;
                totalMoves += game.NumOfTurns;
            }
            allGamesSw.Stop();

            //double winRate = (double)numOfWins / (double)numOfRounds;
            double avgMovesPerRound = (double)totalMoves / (double)numOfRounds;
            double avgTimePerRound = (double)totalOfAllRunsMS / (double)numOfRounds;

            System.Console.WriteLine("Total rounds: " + numOfRounds);
            //System.Console.WriteLine("Total Wins: " + numOfWins);
            //System.Console.WriteLine("Total Losses: " + (numOfRounds - numOfWins));
            System.Console.WriteLine("Avg moves per round: " + avgMovesPerRound);
            System.Console.WriteLine("Total time: " + allGamesSw.ElapsedMilliseconds + " ms");
            System.Console.WriteLine("Avg time per round: " + avgTimePerRound + " ms");
            //System.Console.WriteLine("Win rate: " + (winRate * 100.0) + "%\n");
        }
    }
}
