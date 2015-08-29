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
        private Random rand;
        private int Index;

        public UnoBot(UnoGame game, int index)
        {
            this.Hand = new List<UnoCard>(7);
            this.game = game;
            this.rand = new Random();
            this.Index = index;
        }

        public void PlayTurn()
        {
            UnoCard topCard = this.game.LookAtTopCard();
            List<int> validIndicies = this.GetValidCards(topCard);
            int indexToPlay = -1;
            Color colorToPlay = Color.None;

            if (validIndicies.Count > 0)
            {
                List<int> numberCards = this.GetNumberCards(validIndicies);

                if (numberCards.Count > 0)
                {
                    indexToPlay = numberCards.ElementAt(this.rand.Next(0, numberCards.Count));
                }
                else
                {
                    List<int> actionCards = this.GetActionCards(validIndicies);
                    Debug.Assert(actionCards.Count > 0, "Action cards count should not be 0");
                    indexToPlay = actionCards.ElementAt(this.rand.Next(0, actionCards.Count));
                }
            }
            else
            {
                indexToPlay = DrawCardsUntilMatch(topCard);
            }

            Debug.Assert(indexToPlay != -1, "Index of card to play should not be -1");

            UnoCard cardToPlay = this.Hand.ElementAt(indexToPlay);
            if (cardToPlay.Value == Value.Wild || cardToPlay.Value == Value.DrawFour)
            {
                colorToPlay = this.GetDeclaredColor();
            }

            this.PlayCard(indexToPlay, colorToPlay);
        }

        public void AddCardToHand(UnoCard card)
        {
            this.Hand.Add(card);
        }

        public Color GetDeclaredColor()
        {
            Dictionary<Color, int> colorCounts = new Dictionary<Color, int>();
            Color majorColor = Color.None;

            foreach (UnoCard card in this.Hand)
            {
                try
                {
                    colorCounts.Add(card.Color, 1);
                }
                catch (ArgumentException)
                {
                    colorCounts[card.Color] = colorCounts[card.Color] + 1;
                }
            }

            List<KeyValuePair<Color, int>> colorCountsList = colorCounts.ToList<KeyValuePair<Color, int>>();
            colorCountsList.Sort((firstPair, nextPair) => firstPair.Value.CompareTo(nextPair.Value));
            colorCountsList.Reverse();

            for (int i = 0; i < colorCountsList.Count; i++)
            {
                majorColor = colorCountsList.ElementAt(i).Key;
                if (majorColor != Color.None)
                {
                    break;
                }
            }

            // If the majority color is None then most of the cards in hand are Wilds/Draw Fours
            // In this case, pick any other color
            if (majorColor == Color.None)
            {
                while (majorColor == Color.None)
                {
                    Array values = Enum.GetValues(typeof(Color));
                    majorColor = (Color)values.GetValue(this.rand.Next(values.Length));
                }
            }

            return majorColor;
        }

        private void PlayCard(int index, Color color = Color.None)
        {
            UnoCard cardToPlay = this.Hand.ElementAt(index);
            this.Hand.RemoveAt(index);
            this.game.PlayCard(cardToPlay, color);
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

        private List<int> GetNumberCards(List<int> indicies)
        {
            List<int> numberCardIndicies = new List<int>();

            foreach (int index in indicies)
            {
                UnoCard card = this.Hand.ElementAt(index);
                bool isNumberCard = card.Value != Value.DrawFour &&
                    card.Value != Value.DrawTwo &&
                    card.Value != Value.Reverse &&
                    card.Value != Value.Skip &&
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
