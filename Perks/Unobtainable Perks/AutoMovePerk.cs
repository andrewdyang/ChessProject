using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessProject.Perks
{
    class AutoMovePerk : Perk
    {
        public AutoMovePerk()
        {
            PerkType = PerkType.OnObtain;
            Name = "Auto Move";
            Description = "Moves automatically at the end of its turn";

        }
        public override void Effect(Piece p)
        {
            p.IsAuto = true;
            p.IsChasing = true;
            p.TargetID = -1;
            Available = false;
        }
    }
}
