using ChessProject.Perks;
using ChessProject.Skills;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessProject.Pieces
{
    class Placeholder : Piece
    {
        public Placeholder() {
            PieceType = PieceType.Placeholder;
            ImagePath = "WhiteKing.png";
            MoveTypes.Add(MoveType.Rectangle);
            Range = 2;
            MovCount = 1;
            CastCount = 1;
            Skills.Add(new PlaceholderSkill());
            ObtainedPerks.Add(new RangeUpPerk());
        }
    }
}
