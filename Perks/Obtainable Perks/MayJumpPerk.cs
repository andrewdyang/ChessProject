using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessProject.Perks
{
    class MayJumpPerk : Perk
    {
        public MayJumpPerk()
        {
            PerkType = PerkType.OnObtain;
            Name = "Jump";
            Description = "Your movement is not blocked by other pieces";

            ObtainablePieces.Add(PieceType.Pawn);
            ObtainablePieces.Add(PieceType.Rook);
            ObtainablePieces.Add(PieceType.Bishop);
            ObtainablePieces.Add(PieceType.Queen);
        }
        public override void Effect(Piece p)
        {
            p.MayJump = true;
            Available = false;
        }
    }
}
