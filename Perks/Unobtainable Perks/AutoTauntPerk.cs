using ChessProject.Skills;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessProject.Perks
{
    class AutoTauntPerk : AutoSkillPerk
    {
        TargetSkill skill;
        public AutoTauntPerk()
        {
            PerkType = PerkType.EndOfTurn;
            Name = "Front Line";
            Description = "At the end of turn, get targeted by nearby enemies";

            ObtainablePieces.Add(PieceType.Pawn);

            skill = new TargetSkill();
            skill.Name = "Auto Taunt";
            skill.CastTypes.Add(MoveType.Self);
            skill.CastFriendly = true;
            skill.AffectTypes.Add(MoveType.Rectangle);
            skill.AffectRange = 2;
            skill.AffectEnemy = true;
        }
        public override void Effect(Piece p)
        {
            skill.Target = p;
            AddToQueue(p, skill);
        }
    }
}
