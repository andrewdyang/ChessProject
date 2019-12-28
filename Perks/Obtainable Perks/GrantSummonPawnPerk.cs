using ChessProject.Pieces;
using ChessProject.Skills;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessProject.Perks
{
    class GrantSummonPawnPerk : GrantSkillPerk
    {
        SummonSkill skill = new SummonSkill();
        public GrantSummonPawnPerk()
        {
            PerkType = PerkType.OnObtain;
            Name = "Skill: Summon Pawn Squad";
            Description = "Summon pawns on target area";

            ObtainablePieces.Add(PieceType.King);

            skill.Name = "Summon Pawn Squad";
            skill.Cooldown = 6;
            skill.CastTypes.Add(MoveType.Rectangle);
            skill.CastRange = 3;
            skill.CastEmpty = true;
            skill.CastEnemy = true;
            skill.CastFriendly = true;
            skill.CastMayJump = true;
            skill.AffectTypes.Add(MoveType.Diagonal);
            skill.AffectTypes.Add(MoveType.Self);
            skill.AffectRange = 1;
            skill.AffectEmpty = true;
            skill.AffectMayJump = true;

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
