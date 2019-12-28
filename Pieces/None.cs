using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ChessProject.Pieces
{
    class None : Piece
    {
        public None() {
            PieceImage = new Image();
            PieceImage.IsHitTestVisible = false;
            PieceType = PieceType.None;
        }
    }
}
