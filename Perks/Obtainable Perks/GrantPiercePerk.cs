using ChessProject.Skills;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessProject.Perks
{
    class GrantPiercePerk : GrantSkillPerk
    {
        PierceSkill ps;

        public GrantPiercePerk()
        {
            PerkType = PerkType.OnObtain;
            Name = "Skill: Pierce";
            Description = "Move and kill enemy pieces on the path";


            PrerequisitePerk.Add(typeof(MayJumpPerk));

            ObtainablePieces.Add(PieceType.Queen);
            ObtainablePieces.Add(PieceType.Bishop);
            ObtainablePieces.Add(PieceType.Rook);
            ObtainablePieces.Add(PieceType.Pawn);

            ps = new PierceSkill();
            ps.Name = "Pierce";
            ps.Cooldown = 3;
            ps.CastEmpty = true;
            ps.CastMayJump = true;
            ps.AffectEnemy = true;
            ps.AffectEmpty = true;
            ps.AffectTypes.Add(MoveType.Custom);
        }

        public override void Effect(Piece p)
        {
            SkillToGrant = ps;

            GrantSkill(p);
        }
    }
}
