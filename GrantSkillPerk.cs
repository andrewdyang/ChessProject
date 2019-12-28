using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessProject
{
    abstract class GrantSkillPerk : Perk
    {
        protected Skill SkillToGrant;

        protected void GrantSkill(Piece pieceToGrant)
        {
            pieceToGrant.Skills.Add(SkillToGrant);

            Available = false;
        }
    }
}
