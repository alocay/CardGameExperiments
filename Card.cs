using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolitaireStat
{
    enum Suit
    {
        Clubs = 0,
        Spades,
        Hearts,
        Diamonds
    }

    class Card : ICard
    {
        private string ImageName;

        public int Value
        {
            get; set;
        }

        public Suit Suit
        {
            get; set;
        }

        public Card(int value, Suit suit)
        {
            this.Value = value;
            this.Suit = suit;
            this.ImageName = this.Value.ToString() + "_" + SuitToString(this.Suit);
        }

        public string ImagePath()
        {
            string path = Environment.CurrentDirectory;
            path = Path.Combine(path, "assets");
            path = Path.Combine(path, this.ImageName);

            return path;
        }

        private string SuitToString(Suit suit)
        {
            string s = string.Empty;

            switch (suit)
            {
                case Suit.Clubs:
                    s = "clubs";
                    break;
                case Suit.Spades:
                    s = "spades";
                    break;
                case Suit.Hearts:
                    s = "hearts";
                    break;
                case Suit.Diamonds:
                    s = "diamonds";
                    break;
            }

            return s;
        }
    }
}
