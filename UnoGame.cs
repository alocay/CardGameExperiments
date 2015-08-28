using System;
using System.Collections.Generic;
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

        public UnoGame(int numOfplayers)
        {
            Random r = new Random();
            this.deck = new UnoDeck();
            this.players = new UnoBot[numOfplayers];
            this.currentTurn = r.Next(0, numOfplayers);

            for (int i = 0; i < numOfplayers; i++)
            {
                this.players[i] = new UnoBot(this);
            }

            this.DealHands();
        }

        public int Play()
        {
            int winner = HaveWinner();

            while (winner < 0)
            {
                this.players[this.currentTurn].PlayTurn();

                this.currentTurn += ((currentTurn + movementOfTurns) % this.players.Length);
            }

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
            this.deck.PlayCard(card);
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
