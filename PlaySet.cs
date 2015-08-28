using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolitaireStat
{
    class PlaySet
    {
        private List<Card> hiddenCards;
        private List<Card> visibleCards;

        public PlaySet()
        {
            hiddenCards = new List<Card>();
            visibleCards = new List<Card>();
        }

        public void AddHiddenCard(Card card)
        {
            this.hiddenCards.Add(card);
        }

        public void AddVisibleCard(Card card)
        {
            this.visibleCards.Add(card);
        }

        public List<Card> GetVisibleCards()
        {
            return this.visibleCards;
        }

        public List<Card> RemoveVisibleCards()
        {
            List<Card> cards = new List<Card>();
            this.AddToSet(cards, this.visibleCards);
            this.visibleCards.Clear();
            this.ShowTopHiddenCard();

            return cards;
        }

        public void AddSetToVisibleCards(List<Card> set)
        {
            this.AddToSet(this.visibleCards, set);
        }

        public int GetNumberOfHiddenCards()
        {
            return this.hiddenCards.Count;
        }

        public int GetNumberOfVisibleCards()
        {
            return this.visibleCards.Count;
        }

        public Card GetTopCard()
        {
            return this.visibleCards.LastOrDefault();
        }

        public Card GetBottomCard()
        {
            return this.visibleCards.FirstOrDefault();
        }

        public void RemoveTopCard()
        {
            this.visibleCards.RemoveAt(this.visibleCards.Count - 1);
            if (this.visibleCards.Count == 0)
            {
                this.ShowTopHiddenCard();
            }
        }

        private void AddToSet(List<Card> destSet, List<Card> setToAdd)
        {
            foreach(Card card in setToAdd)
            {
                destSet.Add(card);
            }
        }

        private void ShowTopHiddenCard()
        {
            if (this.hiddenCards.Count > 0)
            {
                Card card = this.hiddenCards.First();
                this.visibleCards.Add(card);
                this.hiddenCards.RemoveAt(0);
            }
        }
    }
}
