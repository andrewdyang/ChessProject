using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessProject
{
    class BuyPiece : BoardAction
    {
        int price;
        Piece piece;
        public BuyPiece(int price, Piece piece)
        {
            this.price = price;
            this.piece = piece;

            Name = "Buy " + piece.Name + " for ";
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
                Piece p = piece.Copy();
                b.Army.Add(p);
                b.ArmyAvailable.Add(p);
                b.RemovePiece(piece);
                return true;
            }
            return false;
        }
    }
}
