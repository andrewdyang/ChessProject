using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessProject.Perks
{
    class ExtraMovPerk : Perk
    {
        public ExtraMovPerk()
        {
            PerkType = PerkType.OnObtain;
            Name = "Extra Move";
            Description = "May move another time";

            ObtainablePieces.Add(PieceType.Pawn);
            ObtainablePieces.Add(PieceType.Knight);
            ObtainablePieces.Add(PieceType.Bishop);
            ObtainablePieces.Add(PieceType.Rook);
            ObtainablePieces.Add(PieceType.Queen);
            ObtainablePieces.Add(PieceType.King);
        }
        public override void Effect(Piece p)
        {
            p.MovCount++;
            Available = false;
        }
    }
}
