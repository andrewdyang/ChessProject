using ChessProject.Perks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessProject.Pieces
{
    class Pawn:Piece
    {
        public Pawn() {
            Name = "Pawn";
            PieceType = PieceType.Pawn;
            ImagePath = "WhitePawn.png";
            MoveTypes.Add(MoveType.Walk);
            Range = 1;
            MovCount = 1;
            CastCount = 1;
        }
    }
}
