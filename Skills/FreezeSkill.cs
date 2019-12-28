using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessProject.Skills
{
    class FreezeSkill : Skill
    {
        public int Duration;
        public FreezeSkill()
        {
            Name = "Freeze";
        }
        public override void Effect(Square s)
        {
            if (s.FreezeDuration < Duration) {
                s.FreezeDuration = Duration;
            }
        }
    }
}
