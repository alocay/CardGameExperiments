using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolitaireStat
{
    class UnoGame
    {
        private UnoDeck deck;
        private UnoBot[] players;
        private const int NumberOfStartingCards = 7;
        private int currentTurn = 0;
        private bool reversed = false;
        private int numberOfTurns = 0;
        private bool initialSkip = false;
        private bool setupComplete = false;

        public UnoGame(int numOfplayers)
        {
            Random r = new Random();
            this.deck = new UnoDeck();
            this.players = new UnoBot[numOfplayers];
            this.currentTurn = r.Next(0, numOfplayers);
            Array values = Enum.GetValues(typeof(UnoBehavior));

            for (int i = 0; i < numOfplayers; i++)
            {
                UnoBehavior behavior = (UnoBehavior)values.GetValue(r.Next(values.Length));
                this.players[i] = new UnoBot(this, behavior, i);
            }

            this.DealHands();
        }

        public void SetupPlay()
        {
            if (!this.setupComplete)
            {
                UnoCard firstCard = this.PlayFirstCard();
                if (firstCard == null)
                {
                    do
                    {
                        this.deck.ReAddCardAndReShuffle(firstCard);
                        firstCard = this.PlayFirstCard();
                    } while (firstCard == null);
                }

                this.setupComplete = true;
            }
        }

        public int Play()
        {
            int winner = -1;
            this.SetupPlay();

            do
            {
                Step();
                winner = HaveWinner();
            } while (winner < 0);

            Debug.Assert(winner >= 0, "Winner should be >= 0");
            return winner;
        }

        public void Step()
        {
            numberOfTurns++;
            this.players[this.currentTurn].PlayTurn();

            if (this.initialSkip)
            {
                this.currentTurn = GetNextPlayer();
                this.initialSkip = false;
            }

            this.currentTurn = GetNextPlayer();
        }

        public UnoCard LookAtTopCard()
        {
            return ((UnoCard)this.deck.GetCurrentCard());
        }

        public UnoCard DrawCard()
        {
            return this.deck.DrawCard();
        }

        public void PlayCard(UnoCard card, Color selectedColor = Color.None)
        {
            switch (card.Value)
            {
                case Value.Reverse:
                    if (this.players.Length == 2)
                    {
                        this.currentTurn = GetNextPlayer();
                    }
                    else
                    {
                        this.reversed = !this.reversed;
                    }

                    break;
                case Value.Skip:
                    if (!this.initialSkip)
                    {
                        this.currentTurn = GetNextPlayer();
                    }
                    break;
                case Value.DrawTwo:
                    // Make next player draw two cards and skip their turn
                    this.DrawCardsAndSkip(2);
                    break;
                case Value.DrawFour:
                    // Make next player draw four cards and skip their turn
                    this.DrawCardsAndSkip(4);
                    break;
            }

            // If the card is a Wild/DrawFour, give it the chosen color
            if (selectedColor != Color.None)
            {
                card.Color = selectedColor;
            }

            this.deck.PlayCard(card);
        }

        public void DisplayState()
        {
            for (int i = 0; i < this.players.Length; i++)
            {
                UnoBot player = this.players[i];

                if (this.currentTurn == i)
                {
                    System.Console.Write("> " + player.GetBehaviorString() + " Player " + i + ": ");
                }
                else
                {
                    System.Console.Write("  " + player.GetBehaviorString() + " Player " + i + ": ");
                }                

                foreach (UnoCard card in player.Hand)
                {
                    System.Console.Write(GetCardString(card) + "  ");
                }

                System.Console.Write("\n");
            }

            System.Console.Write("\n\n");

            UnoCard topCard = (UnoCard)this.deck.GetCurrentCard();

            System.Console.Write(GetCardString(topCard) + "   D: " + this.deck.GetDeckCount() + "  G: " + this.deck.GetGraveyardCount());

            System.Console.Write("\n\n");

            int winner = HaveWinner();
            if (winner > -1)
            {
                System.Console.WriteLine("Player " + winner + " is the winner!\n");
            }
        }

        private string GetCardString(UnoCard card)
        {
            string cardString = string.Empty;

            switch (card.Color)
            {
                case Color.Red:
                    cardString += "R";
                    break;
                case Color.Blue:
                    cardString += "B";
                    break;
                case Color.Green:
                    cardString += "G";
                    break;
                case Color.Yellow:
                    cardString += "Y";
                    break;
                case Color.None:
                    cardString += "N";
                    break;
            }

            switch (card.Value)
            {
                case Value.Zero:
                    cardString += "00";
                    break;
                case Value.One:
                    cardString += "01";
                    break;
                case Value.Two:
                    cardString += "02";
                    break;
                case Value.Three:
                    cardString += "03";
                    break;
                case Value.Four:
                    cardString += "04";
                    break;
                case Value.Five:
                    cardString += "05";
                    break;
                case Value.Six:
                    cardString += "06";
                    break;
                case Value.Seven:
                    cardString += "07";
                    break;
                case Value.Eight:
                    cardString += "08";
                    break;
                case Value.Nine:
                    cardString += "09";
                    break;
                case Value.Reverse:
                    cardString += "Re";
                    break;
                case Value.Skip:
                    cardString += "Sk";
                    break;
                case Value.DrawTwo:
                    cardString += "D2";
                    break;
                case Value.DrawFour:
                    cardString += "D4";
                    break;
                case Value.Wild:
                    cardString += "Wi";
                    break;
            }

            return cardString;
        }

        private UnoCard PlayFirstCard()
        {
            UnoCard card = (UnoCard)this.deck.GetNextCard();
            Color chosenColor = Color.None;

            switch (card.Value)
            {
                case Value.Skip:
                    this.initialSkip = true;
                    break;
                case Value.Wild:
                    // Next player choose color and then continue with regular play
                    chosenColor = this.players[this.currentTurn + 1].GetDeclaredColor();
                    break;
                case Value.DrawFour:
                    // Reshuffle deck and start again
                    card = null;
                    break;
            }

            if (card != null)
            {
                this.PlayCard(card, chosenColor);
            }

            return card;
        }

        private int GetNextPlayer()
        {
            if (reversed)
            {
                return (this.currentTurn - 1 % this.players.Length + this.players.Length) % this.players.Length;
            }
            else
            {
                return (this.currentTurn + 1) % this.players.Length;
            }
        }

        private void DrawCardsAndSkip(int numOfCardsToDraw)
        {
            int nextPlayer = GetNextPlayer();
            for (int i = 0; i < numOfCardsToDraw; i++)
            {
                this.players[nextPlayer].AddCardToHand(this.deck.DrawCard());
            }

            this.currentTurn = nextPlayer;
        }

        private void DealHands()
        {
            for (int i = 0; i < NumberOfStartingCards; i++)
            {
                foreach (UnoBot player in this.players)
                {
                    UnoCard card = this.deck.DrawCard();
                    player.Hand.Add(card);
                }
            }
        }

        private int HaveWinner()
        {
            for (int i = 0; i < this.players.Length; i++)
            {
                if (this.players[i].Hand.Count == 0)
                {
                    return i;
                }
            }

            return -1;
        }
    }
}
