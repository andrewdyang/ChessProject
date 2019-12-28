using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessProject.Skills
{
    class SummonSkill : Skill
    {
        public Piece PieceToSummon;
        public Piece Caster;
        public SummonSkill()
        {
            Name = "Summon";
        }
        public override void Effect(Square s)
        {
            if (PieceToSummon == null || Caster == null) {
                return;
            }

            Chessboard board = (Chessboard)App.Current.Properties["Board"];
            Piece summonedPiece = PieceToSummon.Copy();
            Caster.OnSummon(summonedPiece);

            board.PlacePiece(summonedPiece, s.Col, s.Row);
        }
    }
}
