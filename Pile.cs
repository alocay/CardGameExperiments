using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolitaireStat
{
    class Pile
    {
        private Stack<Card> pile;

        public Pile()
        {
            this.pile = new Stack<Card>();
        }

        public bool CanAddCard(Card card)
        {
            if (pile.Count == 0 && card.Value == 1)
            {
                return true;
            }
            else if (pile.Count > 0 && card.Suit == pile.Peek().Suit && card.Value == (pile.Peek().Value + 1))
            {
                {
                    return true;
                }
            }

            return false;
        }

        public void AddCardToPile(Card card)
        {
            this.pile.Push(card);
        }

        public Card GetTopCard()
        {
            if (this.pile.Count > 0)
            {
                return this.pile.Peek();
            }

            return null;
        }

        public Card RemoveTopCard()
        {
            if (this.pile.Count > 0)
            {
                return this.pile.Pop();
            }

            return null;
        }
    }
}
