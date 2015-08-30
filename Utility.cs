using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolitaireStat
{
    static class Utility
    {
        public static void PrintPiles(Pile[] piles)
        {
            foreach (Pile pile in piles)
            {
                Card topCard = pile.GetTopCard();
                System.Console.Write((topCard != null ? GetVisibleCardString(topCard) : "X") + "  ");
            }
        }

        public static void PrintPlaySetIndices(PlaySet[] playSets)
        {
            for (int i = 0; i < playSets.Length; i++)
            {
                System.Console.Write(i.ToString("D2") + "   ");
            }
        }

        public static void PrintPlaySets(PlaySet[] playSets)
        {
            int mostVisibleCards = 0;

            foreach (PlaySet set in playSets)
            {
                System.Console.Write("Hi" + set.GetNumberOfHiddenCards() + "  ");

                if (set.GetNumberOfVisibleCards() > mostVisibleCards)
                {
                    mostVisibleCards = set.GetNumberOfVisibleCards();
                }
            }
        }

        public static string GetVisibleCardString(Card card)
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

        public static string GetBehaviorString(UnoBehavior behavior)
        {
            string behaviorString = string.Empty;

            switch (behavior)
            {
                case UnoBehavior.Passive:
                    behaviorString = "P";
                    break;
                case UnoBehavior.Aggressive:
                    behaviorString = "A";
                    break;
                case UnoBehavior.Random:
                    behaviorString = "R";
                    break;
            }

            return behaviorString;
        }

    }
}
