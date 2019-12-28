using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessProject
{
    class SellPiece : BoardAction
    {
        int price = 3;
        Piece piece;
        public SellPiece(int price, Piece piece)
        {
            this.price = price;
            this.piece = piece;

            Name = "Sell " + piece.Name + " for ";
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
            b.Currency += price;
            b.Army.Remove(piece);
            b.ArmyAvailable.Remove(piece);
            return true;
        }
    }
}
