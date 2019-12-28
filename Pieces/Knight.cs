using ChessProject.Perks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessProject.Pieces
{
    class Knight : Piece
    {
        public Knight() {
            Name = "Knight";
            PieceType = PieceType.Knight;
            ImagePath = "WhiteKnight.png";
            MoveTypes.Add(MoveType.Knight);
            Range = 1;
            MovCount = 1;
            CastCount = 1;

            ObtainedPerks.Add(new StealthPerk());
        }
    }
}
