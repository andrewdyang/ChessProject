using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessProject.BoardActions
{
    class IncreaseMaxPerkCount : BoardAction
    {
        int price;
        public IncreaseMaxPerkCount()
        {
            UpdateName();
        }

        private void UpdateName() {
            Chessboard b = (Chessboard)App.Current.Properties["Board"];

            price = b.ShopMaxPerkCount * 5;

            Name = "Number of perks per piece limit (" + b.ShopMaxPerkCount + " => " + (b.ShopMaxPerkCount + 1) + ") for ";
            if (price > 1)
            {
                Name += price + " points";
            }
            else
            {
                Name += price + " point";
            }
        }
        
        public override bool DoAction()
        {
            Chessboard b = (Chessboard)App.Current.Properties["Board"];
            if (b.Currency >= price)
            {
                b.Currency -= price;
                b.ShopMaxPerkCount++;
                price = b.ShopMaxPerkCount * 5;
                UpdateName();
                return true;
            }
            return false;
        }
    }
}
