using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessProject.Perks
{
    class OnSummonExplodeOnDeathPerk : Perk
    {
        public OnSummonExplodeOnDeathPerk()
        {
            PerkType = PerkType.OnSummon;
            Name = "Ruthless Army";
            Description = "Its summoned pieces kill nearby pieces when they die";

            ObtainablePieces.Add(PieceType.King);
        }
        public override void Effect(Piece p)
        {
            Piece summonedPiece = (Piece)App.Current.Properties["SummonedPiece"];

            summonedPiece.ObtainedPerks.Add(new ExplodeOnDeathPerk());
        }
    }
}
