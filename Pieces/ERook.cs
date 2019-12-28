using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessProject.Pieces
{
    class ERook:Piece
    {
        public ERook() {
            PieceType = PieceType.ERook;
            ImagePath = "BlackRook.png";
            IsEnemy = true;
            IsChasing = true;
            IsAuto = true;
            TargetID = -1;

            MoveTypes.Add(MoveType.Straight);
            Range = 3;
            MovCount = 1;
            DiffPoint = 6;
        }
    }
}
