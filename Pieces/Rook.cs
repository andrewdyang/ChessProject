using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessProject.Pieces
{
    class Rook:Piece
    {
        public Rook() {
            Name = "Rook";
            PieceType = PieceType.Rook;
            ImagePath = "WhiteRook.png";
            MoveTypes.Add(MoveType.Straight);
            Range = 2;
            MovCount = 1;
            CastCount = 1;
        }
    }
}
