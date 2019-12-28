using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessProject
{
    abstract class AutoSkillPerk : Perk
    {
        private Skill AutoSkill;
        private Square CasterSquare;

        protected void AddToQueue(Piece p, Skill s)
        {
            List<Object> list = (List<Object>)App.Current.Properties["AutoSkillPerkQueue"];

            //Caster = p;
            CasterSquare = p.FindSquareWithPieceID(p.PieceID);
            AutoSkill = s;
            AutoSkill.IsAutoSkill = true;
            list.Insert(0, this);
        }
        public void MarkCast()
        {
            AutoSkill.MarkCastable(CasterSquare);
        }

        public void MarkAffect()
        {
            AutoSkill.AutoSelectFromCastable();
        }

        public void CastSkill()
        {
            AutoSkill.OnAffect();
        }

        public bool IsSelfCast()
        {
            if (AutoSkill.CastTypes.Count == 1 && AutoSkill.CastTypes.First() == MoveType.Self)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
