using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolitaireStat
{
    class Hand
    {
        private List<UnoCard> cards;

        public Hand()
        {
            this.cards = new List<UnoCard>(7);
        }

        public void AddCard(UnoCard card)
        {
            this.cards.Add(card);
        }

        public UnoCard DiscardCard(int i)
        {
            if (this.cards.Count > 0)
            {
                UnoCard card = this.cards.ElementAt(i);
                this.cards.RemoveAt(i);
                return card;
            }

            return null;
        }
    }
}
