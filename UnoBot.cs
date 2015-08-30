using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolitaireStat
{
    enum UnoBehavior
    {
        Passive = 0,
        Aggressive = 1,
        Random = 2
    }

    class UnoBot
    {
        public List<UnoCard> Hand { get; set; }
        private UnoGame game;
        private Random rand;
        private int Index;
        private UnoBehavior behavior;

        public UnoBot(UnoGame game, int index) : this(game, UnoBehavior.Passive, index)
        {
        }

        public UnoBot(UnoGame game, UnoBehavior behavior, int index)
        {
            this.Hand = new List<UnoCard>(7);
            this.game = game;
            this.rand = new Random();
            this.Index = index;
            this.behavior = behavior;
        }

        public void PlayTurn()
        {
            UnoCard topCard = this.game.LookAtTopCard();
            List<int> validIndicies = this.GetValidCards(topCard);
            int indexToPlay = -1;
            Color colorToPlay = Color.None;

            if (validIndicies.Count > 0)
            {
                switch(this.behavior)
                {
                    case UnoBehavior.Passive:
                        indexToPlay = this.PickCardPassive(validIndicies);
                        break;
                    case UnoBehavior.Aggressive:
                        indexToPlay = this.PickCardAggressive(validIndicies);
                        break;
                    case UnoBehavior.Random:
                        indexToPlay = this.PickCardRandom(validIndicies);
                        break;
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
            Color majorColor = this.GetMajorityColor();

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

        public UnoBehavior GetBehavior()
        {
            return this.behavior;
        }

        private int PickCardPassive(List<int> validIndicies)
        {
            int indexToPlay;
            List<int> numberCards = this.GetNumberCards(validIndicies);

            if (numberCards.Count > 0)
            {
                indexToPlay = numberCards.ElementAt(0); // default value

                Color majorColor = this.GetMajorityColor();
                for (int i = 0; i < numberCards.Count; i++)
                {
                    int index = numberCards.ElementAt(i);
                    if (this.Hand.ElementAt(index).Color == majorColor)
                    {
                        indexToPlay = index;
                        break;
                    }
                }
            }
            else
            {
                List<int> actionCards = this.GetActionCards(validIndicies);
                Debug.Assert(actionCards.Count > 0, "Action cards count should not be 0");
                indexToPlay = actionCards.ElementAt(this.rand.Next(0, actionCards.Count));
            }

            return indexToPlay;
        }

        private int PickCardAggressive(List<int> validIndicies)
        {
            int indexToPlay;
            List<int> actionCards = this.GetActionCards(validIndicies);

            if (actionCards.Count > 0)
            {
                indexToPlay = actionCards.ElementAt(this.rand.Next(0, actionCards.Count));
            }
            else
            {
                List<int> numberCards = this.GetNumberCards(validIndicies);
                Debug.Assert(numberCards.Count > 0, "Number cards count should not be 0");
                indexToPlay = numberCards.ElementAt(this.rand.Next(0, numberCards.Count));
            }

            return indexToPlay;
        }

        private int PickCardRandom(List<int> validIndicies)
        {
            return validIndicies.ElementAt(this.rand.Next(0, validIndicies.Count));
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

                if (drawnCard == null)
                {
                    int x = 0;
                }

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
                catch (ArgumentException)
                {
                    colorCounts[card.Color] = colorCounts[card.Color] + 1;
                }
            }

            List<KeyValuePair<Color, int>> colorCountsList = colorCounts.ToList<KeyValuePair<Color, int>>();
            colorCountsList.Sort((firstPair, nextPair) => firstPair.Value.CompareTo(nextPair.Value));
            colorCountsList.Reverse();

            if (colorCountsList.Count == 1)
            {
                majorColor = colorCountsList.First().Key;
            }
            else
            {
                for (int i = 0; i < colorCountsList.Count; i++)
                {
                    majorColor = colorCountsList.ElementAt(i).Key;
                    if (majorColor != Color.None)
                    {
                        break;
                    }
                }
            }

            return majorColor;
        }
    }
}
