using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolitaireStat
{
    enum Color
    {
        Red,
        Blue,
        Green,
        Yellow,
        None
    }

    enum Value
    {
        Zero,
        One,
        Two,
        Three,
        Four,
        Five,
        Six,
        Seven,
        Eight,
        Nine,
        Skip,
        DrawTwo,
        DrawFour,
        Reverse,
        Wild
    }

    class UnoCard : ICard
    {
        public Color Color { get; set; }

        public Value Value { get; set; }

        public UnoCard(Color color, Value value)
        {
            this.Color = color;
            this.Value = value;
        }
    }
}
