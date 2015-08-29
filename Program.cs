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
            const int defaultTotalRounds = 200;
            string input = string.Empty;
            UnoGame ugame = null;

            while (string.IsNullOrEmpty(input) || input.First() != 'q')
            {
                System.Console.Write("Provide an option: ");
                
                input = Console.ReadLine();

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

                        RunRobot(1);

                        System.Console.WriteLine("Ending setup: ");
                        game.DisplayState();
                        break;
                    case 's':
                        int totalRounds = 0;
                        System.Console.Write("Number of rounds: ");
                        input = Console.ReadLine();

                        bool success = int.TryParse(input, out totalRounds);
                        totalRounds = success ? totalRounds : defaultTotalRounds;
                        RunRobot(totalRounds);

                        break;
                    case 'u':
                        UnoGame ugame2 = new UnoGame(4);
                        ugame2.Play();
                        break;
                    case 'o':
                        if (ugame == null)
                        {
                            ugame = new UnoGame(4);
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

        private static void RunRobot(int numOfRounds)
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
    }
}
