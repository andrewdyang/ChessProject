using ChessProject.Pieces;
using ChessProject.Skills;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessProject.Perks
{
    class GrantSummonSinglePawnPerk : GrantSkillPerk
    {
        SummonSkill skill = new SummonSkill();
        public GrantSummonSinglePawnPerk()
        {
            PerkType = PerkType.OnObtain;
            Name = "Skill: Summon a Pawn";
            Description = "Summon a pawn on the target square";

            ObtainablePieces.Add(PieceType.King);

            skill.Name = "Summon a Pawn";
            skill.Cooldown = 2;
            skill.CastTypes.Add(MoveType.Rectangle);
            skill.CastRange = 2;
            skill.CastEmpty = true;
            skill.CastEnemy = false;
            skill.CastFriendly = false;
            skill.CastMayJump = true;
            skill.AffectTypes.Add(MoveType.Self);
            skill.AffectRange = 1;
            skill.AffectEmpty = true;
            skill.AffectMayJump = false;

            SkillToGrant = skill;
        }

        public override void Effect(Piece p)
        {
            if (p.IsEnemy)
            {
                skill.PieceToSummon = new EPawn();
                skill.Caster = p;
            }
            else
            {
                skill.PieceToSummon = new Pawn();
                skill.Caster = p;
                skill.PieceToSummon.ObtainedPerks.Add(new AutoMovePerk());
            }
            GrantSkill(p);
        }
    }
}
