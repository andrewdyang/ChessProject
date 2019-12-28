using ChessProject.Pieces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessProject.Skills
{
    class PierceSkill : Skill
    {
        int DestinationCol;
        int DestinationRow;

        public PierceSkill()
        {
            Name = "Pierce";
        }

        public override void OnMarkCast()
        {
            base.OnMarkCast();

            Chessboard board = (Chessboard)App.Current.Properties["Board"];
            Dictionary<Tuple<int, int>, Square> squares = board.Squares;

            CastTypes.Clear();
            foreach (MoveType m in CasterSquare.Piece.MoveTypes)
            {
                CastTypes.Add(m);
            }
            CastRange = CasterSquare.Piece.Range;
        }

        public override void Effect(Square s)
        {
            if (s.IsEmpty())
            {
                if (DestinationCol == s.Col && DestinationRow == s.Row)
                {
                    Piece caster = CasterSquare.Piece;
                    CasterSquare.Piece = new None();
                    s.Piece = caster;
                }
            }
            else {
                s.Kill();
            }
        }

        public override void MarkCustom(int currentCol, int currentRow)
        {
            DestinationCol = currentCol;
            DestinationRow = currentRow;
            int x = CasterSquare.Col;
            int y = CasterSquare.Row;
            int xDiff;
            int yDiff;

            xDiff = currentCol - x;
            yDiff = currentRow - y;

            while (Math.Abs(xDiff) > 0 || Math.Abs(yDiff) > 0)
            {
                if (Math.Abs(xDiff) == Math.Abs(yDiff))
                {
                    x = ReduceDiff(x, xDiff);
                    y = ReduceDiff(y, yDiff);
                }
                else if (Math.Abs(xDiff) > Math.Abs(yDiff))
                {
                    x = ReduceDiff(x, xDiff);
                }
                else {
                    y = ReduceDiff(y, yDiff);
                }

                CheckAndMark(x, y);

                xDiff = currentCol - x;
                yDiff = currentRow - y;
            }
        }

        private int ReduceDiff(int i, int diff)
        {
            if (diff < 0)
            {
                return --i;
            }
            else if (diff > 0)
            {
                return ++i;
            }
            return i;
        }
    }
}
