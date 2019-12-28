using ChessProject.Skills;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessProject.Perks
{
    class GrantSnipePerk : GrantSkillPerk
    {
        public GrantSnipePerk()
        {
            PerkType = PerkType.OnObtain;
            Name = "Skill: Snipe";
            Description = "Kill a piece on the target square";

            ObtainablePieces.Add(PieceType.Bishop);

            SkillToGrant = new KillSkill();
            SkillToGrant.Name = "Snipe";
            SkillToGrant.Cooldown = 2;
            SkillToGrant.CastTypes.Add(MoveType.Diagonal);
            SkillToGrant.CastRange = 5;
            SkillToGrant.CastEmpty = true;
            SkillToGrant.CastEnemy = true;
            SkillToGrant.CastFriendly = true;
            SkillToGrant.AffectTypes.Add(MoveType.Self);
            SkillToGrant.AffectEnemy = true;
            SkillToGrant.AffectFriendly = true;
        }

        public override void Effect(Piece p)
        {
            GrantSkill(p);
        }
    }
}
