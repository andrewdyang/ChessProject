using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessProject.Perks
{
    class OnSummonRangeUpPerk : Perk
    {
        public OnSummonRangeUpPerk()
        {
            PerkType = PerkType.OnSummon;
            Name = "Elite Army";
            Description = "Increase movement range of its summoned pieces";

            ObtainablePieces.Add(PieceType.King);
        }
        public override void Effect(Piece p)
        {
            Piece summonedPiece = (Piece)App.Current.Properties["SummonedPiece"];

            summonedPiece.ObtainedPerks.Add(new RangeUpPerk());
            summonedPiece.OnObtain();
        }
    }
}

