using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessProject.Perks
{
    class AffectRangeUpPerk : Perk
    {
        public AffectRangeUpPerk()
        {
            PerkType = PerkType.OnSkill;
            Name = "Wide Effect";
            Description = "Increase skill affect range";

            ObtainablePieces.Add(PieceType.Queen);
            ObtainablePieces.Add(PieceType.King);
        }
        public override void Effect(Piece p)
        {
            Skill skillToModify = (Skill)App.Current.Properties["SelectedSkill"];

            skillToModify.AffectRangeModifier++;
        }
    }
}
