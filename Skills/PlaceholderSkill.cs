using ChessProject.Pieces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessProject.Skills
{
    class PlaceholderSkill : Skill
    {
        public PlaceholderSkill()
        {
            Name = "Placeholder";

            CastTypes.Add(MoveType.Rectangle);
            AffectTypes.Add(MoveType.Rectangle);
            CastRange = 10;
            CastEmpty = true;
            CastEnemy = true;
            CastFriendly = true;
            AffectRange = 10;
            AffectEnemy = true;
        }
        public override void Effect(Square s)
        {
            s.Kill();
        }
    }
}
