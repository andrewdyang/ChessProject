using ChessProject.Skills;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessProject.Perks
{
    class GrantExplosionPerk : GrantSkillPerk
    {
        public GrantExplosionPerk() {
            PerkType = PerkType.OnObtain;
            Name = "Skill: Explosion";
            Description = "Kill all pieces in radius";

            ObtainablePieces.Add(PieceType.Queen);

            SkillToGrant = new KillSkill();
            SkillToGrant.Name = "Explosion";
            SkillToGrant.Cooldown = 5;
            SkillToGrant.CastTypes.Add(MoveType.Rectangle);
            SkillToGrant.CastRange = 2;
            SkillToGrant.CastEmpty = true;
            SkillToGrant.CastEnemy = true;
            SkillToGrant.CastFriendly = true;
            SkillToGrant.AffectTypes.Add(MoveType.Walk);
            SkillToGrant.AffectRange = 2;
            SkillToGrant.AffectEnemy = true;
            SkillToGrant.AffectFriendly = true;
        }

        public override void Effect(Piece p)
        {
            GrantSkill(p);
        }
    }
}
