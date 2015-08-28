using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolitaireStat
{
    class UnoBot
    {
        public List<UnoCard> Hand { get; set; }

        private UnoGame game;

        public UnoBot(UnoGame game)
        {
            this.Hand = new List<UnoCard>(7);
            this.game = game;
        }

        public void PlayTurn()
        {
            UnoCard topCard = this.game.LookAtTopCard();
            List<int> validIndicies = this.GetValidCards(topCard);

            if (validIndicies.Count > 0)
            {
                Random r = new Random();
                List<int> numberCards = this.GetNumberCards(validIndicies);

                if (numberCards.Count > 0)
                {
                    int index = r.Next(0, numberCards.Count);
                    this.game.PlayCard(this.Hand.ElementAt(index));
                    this.PlayCard
                }
                else
                {
                    List<int> actionCards = this.GetActionCards(validIndicies);
                    Debug.Assert(actionCards.Count > 0, "Action cards count should not be 0");
                }
            }
            else
            {
                Color selectedColor = Color.None;
                int matchingCardIndex = DrawCardsUntilMatch(topCard);
                UnoCard drawnMatchingCard = this.Hand.ElementAt(matchingCardIndex);

                if (drawnMatchingCard.Value == Value.Wild ||
                    drawnMatchingCard.Value == Value.DrawFour)
                {
                    selectedColor = this.GetMajorityColor();
                    Debug.Assert(selectedColor != Color.None, "Major color should not be None");
                }

                this.Hand.RemoveAt(matchingCardIndex);
                this.game.PlayCard(drawnMatchingCard, selectedColor);
            }
        }

        private void PlayCard(int index, Color color = Color.None)
        {
            this.game.PlayCard(this.Hand.ElementAt(index), color);
            this.Hand.RemoveAt(index);
        }

        private List<int> GetValidCards(UnoCard cardToMatch)
        {
            List<int> validIndicies = new List<int>();
            List<int> drawFourIndicies = new List<int>();

            for (int i = 0; i < this.Hand.Count; i++)
            {
                UnoCard card = this.Hand.ElementAt(i);

                if (card.Value == Value.DrawFour)
                {
                    drawFourIndicies.Add(i);
                }
                else if (DoCardsMatch(card, cardToMatch))
                {
                    validIndicies.Add(i);
                }
            }

            return (validIndicies.Count > 0 ? validIndicies : drawFourIndicies);
        }

        private bool DoCardsMatch(UnoCard cardToCompare, UnoCard cardToMatch)
        {
            return (cardToCompare.Value == Value.Wild ||
                cardToCompare.Value == Value.DrawFour ||
                cardToCompare.Value == cardToMatch.Value ||
                cardToCompare.Color == cardToMatch.Color);
        }

        private int DrawCardsUntilMatch(UnoCard cardToMatch)
        {
            UnoCard drawnCard = null;
            int matchingCardIndex = this.Hand.Count - 1;

            do
            {
                drawnCard = this.game.DrawCard();
                this.Hand.Add(drawnCard);
                matchingCardIndex++;
            } while (!DoCardsMatch(drawnCard, cardToMatch));

            return matchingCardIndex;
        }

        private Color GetMajorityColor()
        {
            Dictionary<Color, int> colorCounts = new Dictionary<Color, int>();
            Color majorColor = Color.None;

            foreach (UnoCard card in this.Hand)
            {
                try
                {
                    colorCounts.Add(card.Color, 1);
                }
                catch(ArgumentException)
                {
                    colorCounts[card.Color] = colorCounts[card.Color] + 1;
                }
            }

            int maxCount = 0;
            foreach (KeyValuePair<Color, int> counts in colorCounts)
            {
                if (counts.Value > maxCount)
                {
                    maxCount = counts.Value;
                    majorColor = counts.Key;
                }
            }

            return majorColor;
        }

        private List<int> GetNumberCards(List<int> indicies)
        {
            List<int> numberCardIndicies = new List<int>();

            foreach (int index in indicies)
            {
                UnoCard card = this.Hand.ElementAt(index);
                bool isNumberCard = card.Value != Value.DrawFour ||
                    card.Value != Value.DrawTwo ||
                    card.Value != Value.Reverse ||
                    card.Value != Value.Skip ||
                    card.Value != Value.Wild;

                if (isNumberCard)
                {
                    numberCardIndicies.Add(index);
                }
            }

            return numberCardIndicies;
        }

        private List<int> GetActionCards(List<int> indicies)
        {
            List<int> actionCardIndicies = new List<int>();

            foreach (int index in indicies)
            {
                UnoCard card = this.Hand.ElementAt(index);
                bool isActionCard = card.Value == Value.DrawFour ||
                    card.Value == Value.DrawTwo ||
                    card.Value == Value.Reverse ||
                    card.Value == Value.Skip ||
                    card.Value == Value.Wild;

                if (isActionCard)
                {
                    actionCardIndicies.Add(index);
                }
            }

            return actionCardIndicies;
        }
    }
}
