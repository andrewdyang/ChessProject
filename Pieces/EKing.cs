using ChessProject.Perks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessProject.Pieces
{
    class EKing : Piece
    {
        public EKing() {
            PieceType = PieceType.EKing;
            ImagePath = "BlackKing.png";
            IsEnemy = true;
            IsChasing = false;
            IsAuto = true;
            TargetID = -1;

            MoveTypes.Add(MoveType.Rectangle);
            Range = 1;
            MovCount = 1;
            DiffPoint = 8;

            ObtainedPerks.Add(new SummonPawnPerk());
        }
    }
}
