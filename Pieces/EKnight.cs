using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessProject.Pieces
{
    class EKnight : Piece
    {
        public EKnight()
        {
            PieceType = PieceType.EKnight;
            ImagePath = "BlackKnight.png";
            IsEnemy = true;
            IsChasing = true;
            IsAuto = true;
            TargetID = -1;

            MoveTypes.Add(MoveType.Knight);
            Range = 1;
            MovCount = 1;
            DiffPoint = 3;
        }
    }
}
