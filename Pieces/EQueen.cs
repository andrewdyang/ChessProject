using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessProject.Pieces
{
    class EQueen : Piece
    {
        public EQueen()
        {
            PieceType = PieceType.EQueen;
            ImagePath = "BlackQueen.png";
            IsEnemy = true;
            IsChasing = true;
            IsAuto = true;
            TargetID = -1;

            MoveTypes.Add(MoveType.Straight);
            MoveTypes.Add(MoveType.Diagonal);
            Range = 3;
            MovCount = 1;
            DiffPoint = 8;
        }
    }
}
