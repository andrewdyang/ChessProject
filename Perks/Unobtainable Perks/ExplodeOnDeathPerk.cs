using ChessProject.Skills;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessProject.Perks
{
    class ExplodeOnDeathPerk : AutoSkillPerk
    {
        KillSkill skill;
        public ExplodeOnDeathPerk()
        {
            PerkType = PerkType.OnDeath;
            Name = "Dying Wish";
            Description = "Kill nearby pieces when this piece dies";

            ObtainablePieces.Add(PieceType.Pawn);

            skill = new KillSkill();
            skill.Name = "Explode";
            skill.CastTypes.Add(MoveType.Self);
            skill.CastFriendly = true;
            skill.CastEnemy = true;
            skill.CastEmpty = true;
            skill.AffectTypes.Add(MoveType.Rectangle);
            skill.AffectRange = 1;
            skill.AffectEnemy = true;
            skill.AffectFriendly = true;
        }
        public override void Effect(Piece p)
        {
            AddToQueue(p, skill);
        }
    }
}
