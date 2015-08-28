using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolitaireStat
{
    class Deck
    {
        protected List<ICard> cards = new List<ICard>();
        protected List<ICard> graveyard = new List<ICard>();
        protected ICard currentCard = null;

        public int GetGraveyardCount()
        {
            return this.graveyard.Count;
        }

        public int GetDeckCount()
        {
            return this.cards.Count;
        }

        public ICard GetNextCard()
        {
            ICard card = this.cards.FirstOrDefault();

            if (card != null)
            {
                this.cards.RemoveAt(0);
            }

            return card;
        }

        public ICard GetCurrentCard()
        {
            return this.currentCard;
        }

        protected void MakeGraveyardActive()
        {
            this.cards.Clear();
            this.cards.AddRange(this.graveyard);
            this.graveyard.Clear();
        }

        protected void Shuffle()
        {
            Random r = new Random();
            //	Based on Java code from wikipedia:
            //	http://en.wikipedia.org/wiki/Fisher-Yates_shuffle

            for (int n = this.cards.Count - 1; n > 0; --n)
            {
                int k = r.Next(n + 1);
                ICard temp = this.cards[n];
                this.cards[n] = cards[k];
                this.cards[k] = temp;
            }
        }
    }
}
