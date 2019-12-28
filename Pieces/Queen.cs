using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessProject.Pieces
{
    class Queen : Piece
    {
        public Queen() {
            Name = "Queen";
            PieceType = PieceType.Queen;
            ImagePath = "WhiteQueen.png";
            MoveTypes.Add(MoveType.Straight);
            MoveTypes.Add(MoveType.Diagonal);
            Range = 2;
            MovCount = 1;
            CastCount = 1;
        }
    }
}
