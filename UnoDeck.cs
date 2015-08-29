using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolitaireStat
{
    class UnoDeck : Deck
    {
        public UnoDeck()
        {
            this.cards.Clear();

            foreach (Color color in Enum.GetValues(typeof(Color)).Cast<Color>())
            {
                if (color == Color.None)
                {
                    continue;
                }

                foreach (Value value in Enum.GetValues(typeof(Value)).Cast<Value>())
                {
                    bool isWild = value == Value.Wild || value == Value.DrawFour;
                    this.cards.Add(new UnoCard((isWild ? Color.None : color), value));

                    if (value != Value.Zero && value != Value.DrawFour && value != Value.Wild)
                    {
                        this.cards.Add(new UnoCard(color, value));
                    }
                }
            }

            this.Shuffle();
        }

        public void ReAddCardAndReShuffle(UnoCard card)
        {
            this.cards.Add(card);
            this.Shuffle();
        }

        public void PlayCard(UnoCard card)
        {
            if (this.currentCard != null)
            {
                if (((UnoCard)this.currentCard).Value == Value.DrawFour || 
                    ((UnoCard)this.currentCard).Value == Value.Wild)
                {
                    ((UnoCard)this.currentCard).Color = Color.None;
                }

                this.graveyard.Add(this.currentCard);
            }

            this.currentCard = card;
        }

        public UnoCard DrawCard()
        {
            if (this.cards.Count == 0)
            {
                this.MakeGraveyardActive();
                this.Shuffle();
            }

            return ((UnoCard)this.GetNextCard());
        }

        public void DiscardCard(UnoCard card)
        {
            this.graveyard.Add(card);
        }
    }
}
