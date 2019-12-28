using ChessProject.Pieces;
using ChessProject.Skills;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace ChessProject
{
    abstract class Skill
    {
        public string Name { get; set; }
        public int Cooldown
        {
            get; set;
        }

        public string CooldownText { get; set; }
        public int CurrentCooldown
        {
            get
            {
                return currentCooldown;
            }
            set
            {
                currentCooldown = value;
                if (currentCooldown > 0)
                {
                    CooldownText = "Cooldown: " + currentCooldown;
                }
                else
                {
                    CooldownText = "Ready";
                }
            }
        }
        private int currentCooldown;
        public List<MoveType> CastTypes = new List<MoveType>();
        public List<MoveType> AffectTypes = new List<MoveType>();
        Random R = new Random();

        public bool CastMayJump { get; set; }
        public bool AffectMayJump { get; set; }
        public int CastRange { get; set; }
        public int AffectRange { get; set; }
        public bool CastFriendly { get; set; }
        public bool CastEnemy { get; set; }
        public bool CastEmpty { get; set; }
        public bool AffectFriendly { get; set; }
        public bool AffectEnemy { get; set; }
        public bool AffectEmpty { get; set; }
        public int Range { get; set; }
        public bool MayJump { get; set; }
        public MarkType MarkMode { get; set; }
        public Square CasterSquare { get; set; }
        public int CastRangeModifier { get; set; }
        public int AffectRangeModifier { get; set; }
        public bool IsAutoSkill { get; set; }

        public Skill()
        {
            CurrentCooldown = 0;
        }

        public void MarkAffectable(Square squareToAffect)
        {
            MarkMode = MarkType.Affectable;
            Range = AffectRange + AffectRangeModifier;
            MayJump = AffectMayJump;
            MarkMoves(squareToAffect.Col, squareToAffect.Row);
        }
        public void MarkCastable(Square casterSquare)
        {
            CasterSquare = casterSquare;

            MarkMode = MarkType.Castable;
            OnMarkCast();
            Range = CastRange + CastRangeModifier;
            MayJump = CastMayJump;
            MarkMoves(casterSquare.Col, casterSquare.Row);
        }

        public void MarkMoves(int currentCol, int currentRow)
        {
            List<MoveType> moveTypes = new List<MoveType>();

            if (MarkMode == MarkType.Castable)
            {
                moveTypes = CastTypes;
            }
            else if (MarkMode == MarkType.Affectable)
            {
                moveTypes = AffectTypes;
            }

            foreach (MoveType m in moveTypes)
            {
                switch (m)
                {
                    case MoveType.Rectangle:
                        MarkRectangle(currentCol, currentRow);
                        break;
                    case MoveType.Walk:
                        MarkWalk(currentCol, currentRow);
                        break;
                    case MoveType.Diagonal:
                        MarkDiagonal(currentCol, currentRow);
                        break;
                    case MoveType.Straight:
                        MarkStraight(currentCol, currentRow);
                        break;
                    case MoveType.Knight:
                        MarkKnight(currentCol, currentRow);
                        break;
                    case MoveType.Self:
                        MarkSelf(currentCol, currentRow);
                        break;
                    case MoveType.Custom:
                        MarkCustom(currentCol, currentRow);
                        break;
                }
            }
        }

        public void MarkStraight(int currentCol, int currentRow)
        {
            Chessboard board = (Chessboard)App.Current.Properties["Board"];
            Dictionary<Tuple<int, int>, Square> squares = board.Squares;

            for (int i = 0; i < 4; i++)
            {
                int x = 0;
                int y = 0;
                switch (i)
                {
                    case 0:
                        x = -1;
                        y = 0;
                        break;
                    case 1:
                        x = 1;
                        y = 0;
                        break;
                    case 2:
                        x = 0;
                        y = -1;
                        break;
                    case 3:
                        x = 0;
                        y = 1;
                        break;
                }

                for (int j = 1; j <= Range && CheckAndMark(currentCol + j * x, currentRow + j * y); j++) ;
            }
        }
        public void MarkDiagonal(int currentCol, int currentRow)
        {
            Chessboard board = (Chessboard)App.Current.Properties["Board"];
            Dictionary<Tuple<int, int>, Square> squares = board.Squares;


            for (int x = -1; x < 2; x += 2)
            {
                for (int y = -1; y < 2; y += 2)
                {
                    for (int j = 1; j <= Range && CheckAndMark(currentCol + j * x, currentRow + j * y); j++) ;
                }
            }
        }
        public void MarkKnight(int currentCol, int currentRow)
        {
            Chessboard board = (Chessboard)App.Current.Properties["Board"];
            Dictionary<Tuple<int, int>, Square> squares = board.Squares;

            for (int i = 0; i < 8; i++)
            {
                int x = 0;
                int y = 0;
                switch (i)
                {
                    case 0:
                        x = -2;
                        y = 1;
                        break;
                    case 1:
                        x = -2;
                        y = -1;
                        break;
                    case 2:
                        x = 2;
                        y = 1;
                        break;
                    case 3:
                        x = 2;
                        y = -1;
                        break;
                    case 4:
                        x = 1;
                        y = 2;
                        break;
                    case 5:
                        x = 1;
                        y = -2;
                        break;
                    case 6:
                        x = -1;
                        y = 2;
                        break;
                    case 7:
                        x = -1;
                        y = -2;
                        break;
                }

                CheckAndMark(currentCol + x, currentRow + y);
            }
        }
        public void MarkRectangle(int currentCol, int currentRow)
        {
            Chessboard board = (Chessboard)App.Current.Properties["Board"];
            Dictionary<Tuple<int, int>, Square> squares = board.Squares;
            int i, j;
            for (i = -Range; i <= Range; i++)
            {
                for (j = -Range; j <= Range; j++)
                {
                    CheckAndMark(currentCol + i, currentRow + j);
                }
            }
        }
        public void MarkWalk(int currentCol, int currentRow)
        {
            MarkWalkRecursion(1, currentCol - 1, currentRow);
            MarkWalkRecursion(1, currentCol + 1, currentRow);
            MarkWalkRecursion(1, currentCol, currentRow - 1);
            MarkWalkRecursion(1, currentCol, currentRow + 1);
        }
        private void MarkWalkRecursion(int distance, int col, int row)
        {
            if (!CheckAndMark(col, row) || Range <= distance)
            {
                return;
            }

            Chessboard board = (Chessboard)App.Current.Properties["Board"];
            Dictionary<Tuple<int, int>, Square> squares = board.Squares;

            distance++;
            MarkWalkRecursion(distance, col - 1, row);
            MarkWalkRecursion(distance, col + 1, row);
            MarkWalkRecursion(distance, col, row - 1);
            MarkWalkRecursion(distance, col, row + 1);
        }
        private void MarkSelf(int currentCol, int currentRow)
        {
            CheckAndMark(currentCol, currentRow);
        }
        protected bool CheckAndMark(int col, int row)
        {
            Chessboard board = (Chessboard)App.Current.Properties["Board"];
            Dictionary<Tuple<int, int>, Square> squares = board.Squares;

            if (squares.TryGetValue(Tuple.Create(col, row), out Square current))
            {
                if (current.IsFrozen)
                {
                    return false;
                }
                else if (!current.IsEmpty() && !MayJump)
                {
                    Mark(current);
                    return false;
                }
                else
                {
                    Mark(current);
                }
            }
            else if (!MayJump)
            {
                return false;
            }
            return true;
        }
        protected void Mark(Square s)
        {
            if (s.MarkType == MarkMode)
            {
                return;
            }
            if (MarkMode == MarkType.Castable)
            {
                if (s.IsEmpty())
                {
                    if (CastEmpty)
                    {
                        s.MarkType = MarkMode;
                    }
                }
                else if ((CasterSquare.Piece.IsEnemy != s.Piece.IsEnemy && CastEnemy) || (CasterSquare.Piece.IsEnemy == s.Piece.IsEnemy && CastFriendly))
                {
                    s.MarkType = MarkMode;
                }
            }
            else if (MarkMode == MarkType.Affectable)
            {
                if (s.IsEmpty())
                {
                    s.MarkType = MarkMode;
                }
                else if ((CasterSquare.Piece.IsEnemy != s.Piece.IsEnemy && AffectEnemy) || (CasterSquare.Piece.IsEnemy == s.Piece.IsEnemy && AffectFriendly))
                {
                    s.MarkType = MarkMode;
                }
            }
        }
        public virtual void MarkCustom(int currentCol, int currentRow) { }

        public bool IsReady()
        {
            return CurrentCooldown <= 0;
        }
        public void AutoSelectFromCastable()
        {
            Chessboard board = (Chessboard)App.Current.Properties["Board"];
            Dictionary<Tuple<int, int>, Square> squares = board.Squares;
            List<Square> castableSquares = new List<Square>();

            foreach (KeyValuePair<Tuple<int, int>, Square> entry in squares)
            {
                if (entry.Value.MarkType == MarkType.Castable)
                {
                    castableSquares.Add(entry.Value);
                }
            }
            if (castableSquares.Count > 0)
            {
                Square s = castableSquares[R.Next(castableSquares.Count)];
                MarkAffectable(s);
            }
        }

        public void OnAffect()
        {
            Chessboard board = (Chessboard)App.Current.Properties["Board"];
            Dictionary<Tuple<int, int>, Square> squares = board.Squares;

            foreach (KeyValuePair<Tuple<int, int>, Square> entry in squares)
            {
                if (entry.Value.MarkType == MarkType.Affectable)
                {
                    if (entry.Value.IsEmpty())
                    {
                        if (AffectEmpty)
                        {
                            Effect(entry.Value);
                        }
                    }
                    else
                    {
                        Effect(entry.Value);
                    }
                }
            }
            CurrentCooldown = Cooldown;
        }

        virtual public void OnMarkCast()
        {
            if (!IsAutoSkill) {
                CastRangeModifier = 0;
                AffectRangeModifier = 0;
                CasterSquare.Piece.CheckPerks(PerkType.OnSkill);
            }
        }

        abstract public void Effect(Square s);
    }
}
