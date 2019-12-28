using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessProject.Pieces
{
    class EPawn:Piece
    {
        public EPawn() {
            PieceType = PieceType.EPawn;
            ImagePath = "BlackPawn.png";
            IsEnemy = true;
            IsChasing = true;
            IsAuto = true;
            TargetID = -1;

            MoveTypes.Add(MoveType.Walk);
            Range = 1;
            MovCount = 1;
            DiffPoint = 1;
        }
    }
}
