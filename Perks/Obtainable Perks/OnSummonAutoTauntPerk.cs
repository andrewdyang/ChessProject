using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessProject.Perks
{
    class OnSummonAutoTauntPerk : Perk
    {
        public OnSummonAutoTauntPerk()
        {
            PerkType = PerkType.OnSummon;
            Name = "Fearless Army";
            Description = "Its summoned pieces gets targeted by nearby pieces at the end of its turn";

            ObtainablePieces.Add(PieceType.King);
        }
        public override void Effect(Piece p)
        {
            Piece summonedPiece = (Piece)App.Current.Properties["SummonedPiece"];

            summonedPiece.ObtainedPerks.Add(new AutoTauntPerk());
        }
    }
}
