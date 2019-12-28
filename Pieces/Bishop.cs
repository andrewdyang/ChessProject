using ChessProject.Perks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessProject.Pieces
{
    class Bishop:Piece
    {
        public Bishop() {
            Name = "Bishop";
            PieceType = PieceType.Bishop;
            ImagePath = "WhiteBishop.png";
            MoveTypes.Add(MoveType.Diagonal);
            Range = 2;
            MovCount = 1;
            CastCount = 1;

            ObtainedPerks.Add(new GrantSnipePerk());
        }
    }
}
