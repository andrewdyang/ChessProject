using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessProject.Perks
{
    class RangeUpPerk : Perk
    {
        public RangeUpPerk()
        {
            PerkType = PerkType.OnObtain;
            Name = "Far Reach";
            Description = "Increase movement range";

            ObtainablePieces.Add(PieceType.Pawn);
            ObtainablePieces.Add(PieceType.Bishop);
            ObtainablePieces.Add(PieceType.Rook);
            ObtainablePieces.Add(PieceType.Queen);
            ObtainablePieces.Add(PieceType.King);
        }
        public override void Effect(Piece p)
        {
            if (p.PieceType == PieceType.Bishop || p.PieceType == PieceType.Rook || p.PieceType == PieceType.EBishop || p.PieceType == PieceType.ERook)
            {
                p.Range += 2;
            }
            else
            {
                p.Range++;
            }
            Available = false;
        }
    }
}
