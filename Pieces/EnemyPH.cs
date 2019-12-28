using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessProject.Pieces
{
    class EnemyPH : Piece
    {
        public EnemyPH() {
            PieceType = PieceType.EnemyPH;
            ImagePath = "BlackPawn.png";
            IsEnemy = true;
            IsChasing = true;
            TargetID = -1;

            MoveTypes.Add(MoveType.Rectangle);
            Range = 1;
            MovCount = 1;
            DiffPoint = 2;
        }
    }
}
