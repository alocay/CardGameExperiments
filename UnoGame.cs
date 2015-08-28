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
        private int movementOfTurns = 1;
        private int numberOfTurns = 0;

        public UnoGame(int numOfplayers)
        {
            Random r = new Random();
            this.deck = new UnoDeck();
            this.players = new UnoBot[numOfplayers];
            this.currentTurn = r.Next(0, numOfplayers);

            for (int i = 0; i < numOfplayers; i++)
            {
                this.players[i] = new UnoBot(this, i);
            }

            this.DealHands();
        }

        public int Play()
        {
            int winner = -1;

            UnoCard firstCard = this.PlayFirstCard();
            if (firstCard == null)
            {
                do
                {
                    this.deck.ReAddCardAndReShuffle(firstCard);
                    firstCard = this.PlayFirstCard();
                } while (firstCard == null);
            }

            do
            {
                numberOfTurns++;
                this.players[this.currentTurn].PlayTurn();
                this.currentTurn = GetNextPlayer();
                winner = HaveWinner();
            } while (winner < 0);

            Debug.Assert(winner >= 0, "Winner should be >= 0");
            return winner;
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
                    this.movementOfTurns *= -1;
                    break;
                case Value.Skip:
                    this.currentTurn = GetNextPlayer();
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

        private UnoCard PlayFirstCard()
        {
            UnoCard card = (UnoCard)this.deck.GetNextCard();
            Color chosenColor = Color.None;

            switch (card.Value)
            {
                case Value.Wild:
                    // Next player choose color and then continue with regular play
                    chosenColor = this.players[this.currentTurn + this.movementOfTurns].GetDeclaredColor();
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
            return (this.currentTurn + this.movementOfTurns) % this.players.Length;
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
