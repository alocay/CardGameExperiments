using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolitaireStat
{
    class SolitaireGame
    {
        private SolitaireDeck deck;
        private PlaySet[] playSets = new PlaySet[7];
        private Pile[] piles = new Pile[4];
        private bool noSolution = false;

        public SolitaireGame()
        {
            deck = new SolitaireDeck();
            SetupCards();
        }

        public bool MoveAnyPlaySet()
        {
            for (int i = 0; i < this.playSets.Length; i++)
            {
                for (int j = 0; j < this.playSets.Length; j++)
                {
                    if (j == i)
                    {
                        continue;
                    }

                    if (this.CanMove(i, j) && !this.IsKingWithNoHiddenCards(i))
                    {
                        this.MoveCards(i, j);
                        return true;
                    }
                }
            }

            return false;
        }

        public bool MoveCurentDeckCard()
        {
            Card card = (Card)this.deck.GetCurrentCard();

            if (card != null)
            {
                for (int i = 0; i < this.playSets.Length; i++)
                {
                    if (this.CanMoveCardTo(this.playSets[i], card))
                    {
                        this.MoveDeckCard(i);
                        return true;
                    }
                }
            }

            return false;
        }

        public bool PileAnyPlaySetCards()
        {
            for (int i = 0; i < this.playSets.Length; i++)
            {
                if (this.MoveSetTopCardToPile(i))
                {
                    return true;
                }
            }

            return false;
        }

        public bool PileAnyDeckCard()
        {
            while(this.deck.GetCurrentCard() != null)
            {
                bool moved = MoveCardToPile((Card)this.deck.GetCurrentCard());

                if (moved)
                {
                    this.deck.PlayCurrentCard();
                    return moved;
                }
                else
                {
                    this.deck.Cycle();
                }
            }
           
            return false;
        }

        public bool HasWon()
        {
            foreach(PlaySet playSet in this.playSets)
            {
                if (playSet.GetNumberOfHiddenCards() > 0)
                {
                    return false;
                }
            }

            return true;
        }

        public bool NoSolutionDetected()
        {
            return this.noSolution;
        }

        public void NoSolution()
        {
            this.noSolution = true;
        }

        public void CycleDeck()
        {
            this.deck.Cycle();
        }

        public bool HaveGoneThroughEntireDeck()
        {
            return this.deck.HasGoneThroughEntireDeck();
        }

        public void DisplayState()
        {
            int mostVisibleCards = 0;

            Utility.PrintPiles(this.piles);

            System.Console.Write("\n\n");

            Utility.PrintPlaySetIndices(this.playSets);

            System.Console.Write("\n");

            foreach (PlaySet set in this.playSets)
            {
                System.Console.Write("Hi" + set.GetNumberOfHiddenCards() + "  ");

                if (set.GetNumberOfVisibleCards() > mostVisibleCards)
                {
                    mostVisibleCards = set.GetNumberOfVisibleCards();
                }
            }

            Card deckCard = (Card)this.deck.GetCurrentCard();
            System.Console.Write("    D: " + (deckCard != null ? Utility.GetVisibleCardString(deckCard) : "X") + " (" + this.deck.GetDeckCount() + ")    G: " + deck.GetGraveyardCount());

            System.Console.Write("\n");

            for (int i = 0; i < mostVisibleCards; i++)
            {
                foreach (PlaySet set in this.playSets)
                {
                    List<Card> visibleCards = set.GetVisibleCards();

                    Card card = visibleCards.ElementAtOrDefault(i);

                    if (card != null)
                    {
                        System.Console.Write(Utility.GetVisibleCardString(card) + "  ");
                    }
                    else
                    {
                        System.Console.Write("     ");
                    }
                }

                System.Console.Write("\n");
            }

            System.Console.Write("\n");

            if (this.deck.GetDeckCount() > 0)
            {
                this.deck.PrintDeckString();
            }

            if (this.HasWon())
            {
                System.Console.Write("You Win!\n");
            }
            else if (this.noSolution)
            {
                System.Console.Write("No Solution!\n");
            }

            System.Console.Write("\n\n");
        }

        private void SetupCards()
        {
            for (int i = 0; i < this.playSets.Length; i++)
            {
                this.playSets[i] = new PlaySet();

                for (int j = 0; j < i; j++)
                {
                    this.playSets[i].AddHiddenCard((Card)this.deck.GetNextCard());
                }

                this.playSets[i].AddVisibleCard((Card)this.deck.GetNextCard());
            }

            for (int i = 0; i < this.piles.Length; i++)
            {
                this.piles[i] = new Pile();
            }
        }

        private bool MoveCardToPile(Card card)
        {
            foreach (Pile pile in this.piles)
            {
                if (pile.CanAddCard(card))
                {
                    pile.AddCardToPile(card);
                    return true;
                }
            }

            return false;
        }

        private bool MoveSetTopCardToPile(int slot)
        {
            Card card = this.playSets[slot].GetTopCard();

            if (card != null)
            {
                bool moved = MoveCardToPile(card);

                if (moved)
                {
                    this.playSets[slot].RemoveTopCard();
                }

                return moved;
            }

            return false;
        }

        private bool IsKingWithNoHiddenCards(int set)
        {
            Card card = this.playSets[set].GetBottomCard();
            return (card.Value == 13 && this.playSets[set].GetNumberOfHiddenCards() == 0);
        }

        private bool CanMove(int set1, int set2)
        {
            Card card = this.playSets[set1].GetBottomCard();
            PlaySet destSet = this.playSets[set2];

            return (card != null ? CanMoveCardTo(destSet, card) : false);
        }

        private bool CanMoveCardTo(PlaySet destSet, Card card)
        {
            Card destCard = destSet.GetTopCard();

            if (destCard == null)
            {
                return card.Value == 13;
            }

            if (AreAlternateSuits(destCard, card))
            {
                return destCard.Value == (card.Value + 1);
            }

            return false;
        }

        private bool AreAlternateSuits(Card c1, Card c2)
        {
            if (c1.Suit == Suit.Clubs || c1.Suit == Suit.Spades)
            {
                return c2.Suit == Suit.Hearts || c2.Suit == Suit.Diamonds;
            }
            else
            {
                return c2.Suit == Suit.Clubs || c2.Suit == Suit.Spades;
            }
        }

        private void MoveCards(int set1, int set2)
        {
            List<Card> cards = this.playSets[set1].RemoveVisibleCards();
            this.playSets[set2].AddSetToVisibleCards(cards);
        }

        private void MoveDeckCard(int set)
        {
            Card card = this.deck.PlayCurrentCard();
            this.playSets[set].AddVisibleCard(card);
        }
    }
}
