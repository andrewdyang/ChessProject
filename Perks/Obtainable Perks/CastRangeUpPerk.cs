using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessProject.Perks
{
    class CastRangeUpPerk : Perk
    {
        public CastRangeUpPerk()
        {
            PerkType = PerkType.OnSkill;
            Name = "Far Cast";
            Description = "Increase skill cast range";

            ObtainablePieces.Add(PieceType.Queen);
            ObtainablePieces.Add(PieceType.King);
            ObtainablePieces.Add(PieceType.Bishop);
        }
        public override void Effect(Piece p)
        {
            Skill skillToModify = (Skill)App.Current.Properties["SelectedSkill"];

            skillToModify.CastRangeModifier++;
        }
    }
}
