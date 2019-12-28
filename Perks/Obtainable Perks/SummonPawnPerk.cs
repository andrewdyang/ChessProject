using ChessProject.Pieces;
using ChessProject.Skills;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessProject.Perks
{
    class SummonPawnPerk : AutoSkillPerk
    {

        SummonSkill skill = new SummonSkill();
        public SummonPawnPerk()
        {
            PerkType = PerkType.EndOfTurn;
            Name = "Reinforcement";
            Description = "At the end of its turn, summon a pawn around it";

            ObtainablePieces.Add(PieceType.King);

            skill.Name = "Summon";
            skill.CastTypes.Add(MoveType.Rectangle);
            skill.CastRange = 1;
            skill.CastEmpty = true;
            skill.AffectTypes.Add(MoveType.Self);
            skill.AffectEmpty = true;
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
            AddToQueue(p, skill);
        }
    }
}
