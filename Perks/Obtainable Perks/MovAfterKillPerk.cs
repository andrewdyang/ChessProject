using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessProject.Perks
{
    class MovAfterKillPerk : Perk
    {
        public MovAfterKillPerk()
        {
            PerkType = PerkType.OnCapture;
            Name = "Killing Spree";
            Description = "Whenever it captures, it may move another time";

            ObtainablePieces.Add(PieceType.Knight);
        }
        public override void Effect(Piece p)
        {
            p.MovCountLeft++;
        }
    }
}
