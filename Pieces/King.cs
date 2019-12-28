using ChessProject.Perks;
using ChessProject.Skills;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessProject.Pieces
{
    class King : Piece
    {
        public King()
        {
            Name = "King";
            PieceType = PieceType.King;
            ImagePath = "WhiteKing.png";
            MoveTypes.Add(MoveType.Rectangle);
            Range = 1;
            MovCount = 1;
            CastCount = 1;

            ObtainedPerks.Add(new GrantSummonSinglePawnPerk());
        }
    }
}
