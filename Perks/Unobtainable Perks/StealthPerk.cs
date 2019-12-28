using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessProject.Perks
{
    class StealthPerk : Perk
    {
        public StealthPerk()
        {
            PerkType = PerkType.OnObtain;
            Name = "Maneuver";
            Description = "Less likely to be targeted by enemy";

        }
        public override void Effect(Piece p)
        {
            p.IsLessTargetable = true;
            Available = false;
        }
    }
}
