using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolitaireStat
{
    class SolitaireDeck : Deck
    {
        private bool hasGoneThroughEntireDeck = false;

        public SolitaireDeck()
        {
            for (int i = 0; i < 13; i++)
            {
                int index = i * 4;
                this.cards.Add(new Card((i + 1), Suit.Clubs));
                this.cards.Add(new Card((i + 1), Suit.Spades));
                this.cards.Add(new Card((i + 1), Suit.Hearts));
                this.cards.Add(new Card((i + 1), Suit.Diamonds));
            }

            this.Shuffle();
        }

        public bool HasGoneThroughEntireDeck()
        {
            return this.hasGoneThroughEntireDeck;
        }

        public Card Cycle()
        {
            ICard oldCard = this.currentCard;
            this.currentCard = this.GetNextCard();

            if (oldCard != null)
            {
                this.graveyard.Add(oldCard);
            }

            if (this.currentCard == null)
            {
                this.hasGoneThroughEntireDeck = true;
                this.MakeGraveyardActive();
            }
            else
            {
                this.hasGoneThroughEntireDeck = false;
            }

            return ((Card)this.currentCard);
        }

        public Card PlayCurrentCard()
        {
            ICard card = this.currentCard;
            this.currentCard = null;
            return (Card)card;
        }

        public void PrintDeckString()
        {
            System.Console.Write("Cards left in deck: ");

            foreach (Card card in this.cards)
            {
                System.Console.Write(this.GetVisibleCardString(card) + " ");
            }

            System.Console.Write("\n\n");
        }

        private string GetVisibleCardString(Card card)
        {
            string suit = string.Empty;

            switch (card.Suit)
            {
                case Suit.Clubs:
                    suit = "C";
                    break;
                case Suit.Spades:
                    suit = "S";
                    break;
                case Suit.Hearts:
                    suit = "H";
                    break;
                case Suit.Diamonds:
                    suit = "D";
                    break;
            }

            return suit + card.Value.ToString("D2");
        }
    }
}
