using ChessProject.Pieces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessProject.Skills
{
    class KillSkill : Skill
    {
        public KillSkill()
        {
            Name = "Kill";
        }
        public override void Effect(Square s)
        {
            s.Kill();
        }
    }
}
