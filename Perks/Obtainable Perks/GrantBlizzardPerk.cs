using ChessProject.Skills;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessProject.Perks
{
    class GrantBlizzardPerk : GrantSkillPerk
    {
        public GrantBlizzardPerk()
        {
            PerkType = PerkType.OnObtain;
            Name = "Skill: Blizzard";
            Description = "Freeze all pieces in radius";

            ObtainablePieces.Add(PieceType.King);


            FreezeSkill fs = new FreezeSkill();
            fs.Name = "Blizzard";
            fs.Cooldown = 5;
            fs.CastTypes.Add(MoveType.Walk);
            fs.CastRange = 3;
            fs.CastEmpty = true;
            fs.CastEnemy = true;
            fs.CastFriendly = true;
            fs.CastMayJump = true;
            fs.AffectTypes.Add(MoveType.Rectangle);
            fs.AffectRange = 2;
            fs.AffectEmpty = true;
            fs.AffectEnemy = true;
            fs.AffectFriendly = true;
            fs.AffectMayJump = true;
            fs.Duration = 2;

            SkillToGrant = fs;
        }

        public override void Effect(Piece p)
        {
            GrantSkill(p);
        }
    }
}
