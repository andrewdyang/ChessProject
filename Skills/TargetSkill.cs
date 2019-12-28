using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessProject.Skills
{
    class TargetSkill : Skill
    {
        public Piece Target { get; set; }
        public TargetSkill()
        {
            Name = "Taunt";
        }
        public override void Effect(Square s)
        {
            if (Target == null) {
                return;
            }

            s.Piece.TargetID = Target.PieceID;
        }
    }
}
