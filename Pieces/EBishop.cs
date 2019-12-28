using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessProject.Pieces
{
    class EBishop:Piece
    {
        public EBishop() {
            PieceType = PieceType.EBishop;
            ImagePath = "BlackBishop.png";
            IsEnemy = true;
            IsChasing = true;
            IsAuto = true;
            TargetID = -1;

            MoveTypes.Add(MoveType.Diagonal);
            Range = 3;
            MovCount = 1;
            DiffPoint = 3;
        }
    }
}
