using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolitaireStat
{
    enum RobotStates
    {
        Ready,
        PlaySetsChecked,
        DeckChecked,
        PilesChecked,
        NoSolution
    }

    class SolitaireBot : IRobot
    {
        private SolitaireGame game;
        private RobotStates state;
        private bool moveMade = false;

        public int TotalMoves
        {
            get;
            set;
        }

        public SolitaireBot(SolitaireGame g)
        {
            this.state = RobotStates.Ready;
            this.game = g;
            this.TotalMoves = 0;
        }

        public bool Go()
        {
            while (!this.game.HasWon() && !this.game.NoSolutionDetected())
            {
                this.Step();
            }

            return this.game.HasWon() && !this.game.NoSolutionDetected();
        }

        public void Step()
        {
            if (this.game.HasWon())
            {
                return;
            }

            switch(this.state)
            {
                case RobotStates.Ready:
                    if (!this.game.MoveAnyPlaySet())
                    {
                        state = RobotStates.PlaySetsChecked;
                    }
                    else
                    {
                        this.moveMade = true;
                        this.TotalMoves++;
                    }

                    break;
                case RobotStates.PlaySetsChecked:
                    if (this.game.MoveCurentDeckCard())
                    {
                        state = RobotStates.Ready;
                        this.moveMade = true;
                        this.TotalMoves++;
                    }
                    else
                    {
                        state = RobotStates.DeckChecked;
                    }

                    break;
                case RobotStates.DeckChecked:
                    if (game.HaveGoneThroughEntireDeck() && !this.moveMade)
                    {
                        if(this.game.PileAnyPlaySetCards())
                        {
                            state = RobotStates.Ready;
                            this.moveMade = true;
                            this.TotalMoves++;
                        }
                        else if (this.game.PileAnyDeckCard())
                        {
                            state = RobotStates.Ready;
                            this.moveMade = true;
                            this.TotalMoves++;
                        }
                        else
                        {
                            state = RobotStates.PilesChecked;
                        }
                    }
                    else if (game.HaveGoneThroughEntireDeck() && this.moveMade)
                    {
                        this.moveMade = false;
                        game.CycleDeck();
                        state = RobotStates.Ready;
                        this.TotalMoves++;
                    }
                    else
                    {
                        game.CycleDeck();
                        state = RobotStates.Ready;
                        this.TotalMoves++;
                    }
                    
                    break;
                case RobotStates.PilesChecked:
                    if (!this.game.HasWon())
                    {
                        this.game.NoSolution();
                    }
                    break;
            }
        }
    }
}
