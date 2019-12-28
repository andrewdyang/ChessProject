using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessProject.BoardActions
{
    class IncreasePieceCount : BoardAction
    {
        int price;
        public IncreasePieceCount()
        {
            UpdateName();
        }

        private void UpdateName()
        {
            Chessboard b = (Chessboard)App.Current.Properties["Board"];

            price = b.ShopPieceCount * 3;

            Name = "Number of offered pieces in shop (" + b.ShopPieceCount + " => " + (b.ShopPieceCount + 1) + ") for ";
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
                b.ShopPieceCount++;
                price = b.ShopPieceCount * 3;
                UpdateName();
                return true;
            }
            return false;
        }
    }
}
