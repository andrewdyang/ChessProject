using ChessProject.Pieces;
using ChessProject.Skills;
using ChessProject.Perks;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace ChessProject
{
    enum PieceType { None, Placeholder, EnemyPH, Pawn, Rook, Bishop, Queen, King, Knight, EPawn, ERook, EBishop, EQueen, EKing, EKnight }
    enum MoveType { Rectangle, Walk, Diagonal, Straight, Knight, Self ,Custom }
    abstract class Piece
    {
        public PieceType PieceType { get; set; }
        public List<MoveType> MoveTypes = new List<MoveType>();

        public int PieceID
        {
            get
            {
                return pieceID;
            }
            set
            {
                pieceID = (int)App.Current.Properties["NextPieceID"];
                App.Current.Properties["NextPieceID"] = (int)App.Current.Properties["NextPieceID"] + 1;
            }
        }
        private int pieceID;

        public ObservableCollection<Skill> Skills = new ObservableCollection<Skill>();
        public ObservableCollection<Perk> ObtainedPerks = new ObservableCollection<Perk>();
        public ObservableCollection<Perk> AvailablePerks = new ObservableCollection<Perk>();
        public Image PieceImage { get; set; }
        public string Name { get; set; }
        public string PerkPointText { get; set; }
        public string ImagePath
        {
            get { return imagePath; }
            set
            {
                string path = "/ChessProject;component/Images/";
                path += value;
                PieceImage.Source = new BitmapImage(
                    new Uri(path, UriKind.Relative));
                imagePath = path;
            }
        }
        private string imagePath;
        public bool IsEnemy { get; set; }
        public bool IsChasing { get; set; }
        public bool IsAuto { get; set; }
        public int TargetID { get; set; }
        public int DiffPoint { get; set; }
        public int MovCount { get; set; }
        public int MovCountLeft { get; set; }
        public int CastCount { get; set; }
        public int CastCountLeft { get; set; }
        public int Range { get; set; }
        public bool MayJump { get; set; }
        public int PerkPoint
        {
            get { return perkPoint; }

            set
            {
                perkPoint = value;
                if (perkPoint == 1) {
                    PerkPointText = "Perk available";
                }
                else if (perkPoint > 0)
                {
                    PerkPointText = perkPoint + " perks available";
                }
                else {
                    PerkPointText = "";
                }
            }
        }
        private int perkPoint;
        public int PlaceableRow { get; set; }
        public bool IsLessTargetable { get; set; }

        public Type Types { get; set; }

        public Piece()
        {
            PieceImage = new Image();
            PieceImage.IsHitTestVisible = false;
            Name = "Unnamed";
            if (GetType() != typeof(None))
            {
                PieceID = 1;
            }
            PlaceableRow = 2;
            CheckPerks(PerkType.OnObtain);
        }
        public Piece Copy()
        {
            Piece p = (Piece)Activator.CreateInstance(GetType());
            p.Name = Name;
            p.PerkPoint = PerkPoint;
            p.ObtainedPerks.Clear();
            foreach (Perk perk in ObtainedPerks) {
                p.ObtainedPerks.Add((Perk)Activator.CreateInstance(perk.GetType()));
            }
            p.CheckPerks(PerkType.OnObtain);
            p.AvailablePerks = AvailablePerks;
            return p;
        }
        public void MarkMoves(int currentCol, int currentRow)
        {
            foreach (MoveType m in MoveTypes)
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
                    case MoveType.Custom:
                        MarkCustom(currentCol, currentRow);
                        break;
                }
            }
        }
        public void MarkPlaceable()
        {
            Chessboard board = (Chessboard)App.Current.Properties["Board"];
            Dictionary<Tuple<int, int>, Square> squares = board.Squares;

            int highestRow = 0;
            int highestCol = 0;
            foreach (KeyValuePair<Tuple<int, int>, Square> entry in squares)
            {
                if (entry.Key.Item1 > highestCol)
                {
                    highestCol = entry.Key.Item1;
                }
                if (entry.Key.Item2 > highestRow)
                {
                    highestRow = entry.Key.Item2;
                }
            }

            for (int j = PlaceableRow - 1; j >= 0; j--)
            {
                for (int i = 0; i <= highestCol; i++)
                {
                    if (squares.TryGetValue(Tuple.Create(i, highestRow - j), out Square s))
                    {
                        if (s.IsEmpty())
                        {
                            s.MarkType = MarkType.Placeable;
                        }
                    }
                }
            }
        }
        public void MarkTarget()
        {
            if (!IsTargetPresent()) {
                return;
            }

            Square target = FindSquareWithPieceID(TargetID);
            if (target != null)
            {
                if (target.MarkType == MarkType.Capturable)
                {
                    target.MarkType = MarkType.CapturableTarget;
                }
                else
                {
                    target.MarkType = MarkType.Target;
                }
            }
            else {
                TargetID = -1;
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

                for (int j = 1; j <= Range && CheckAndMark(currentCol + j * x, currentRow + j * y); j++);
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
                    for (int j = 1; j <= Range && CheckAndMark(currentCol + j * x, currentRow + j * y); j++);
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
            Chessboard board = (Chessboard)App.Current.Properties["Board"];
            Dictionary<Tuple<int, int>, Square> squares = board.Squares;

            Square origin = squares[Tuple.Create(currentCol, currentRow)];

            if (squares.TryGetValue(Tuple.Create(currentCol - 1, currentRow), out Square left))
            {
                MarkWalkRecursion(1, left);
            }
            if (squares.TryGetValue(Tuple.Create(currentCol + 1, currentRow), out Square right))
            {
                MarkWalkRecursion(1, right);
            }
            if (squares.TryGetValue(Tuple.Create(currentCol, currentRow - 1), out Square up))
            {
                MarkWalkRecursion(1, up);
            }
            if (squares.TryGetValue(Tuple.Create(currentCol, currentRow + 1), out Square down))
            {
                MarkWalkRecursion(1, down);
            }
        }
        private void MarkWalkRecursion(int distance, Square current)
        {
            if (!CheckAndMark(current.Col,current.Row)|| Range <= distance)
            {
                return;
            }

            Chessboard board = (Chessboard)App.Current.Properties["Board"];
            Dictionary<Tuple<int, int>, Square> squares = board.Squares;

            distance++;
            if (squares.TryGetValue(Tuple.Create(current.Col - 1, current.Row), out Square left))
            {
                MarkWalkRecursion(distance, left);
            }
            if (squares.TryGetValue(Tuple.Create(current.Col + 1, current.Row), out Square right))
            {
                MarkWalkRecursion(distance, right);
            }
            if (squares.TryGetValue(Tuple.Create(current.Col, current.Row - 1), out Square up))
            {
                MarkWalkRecursion(distance, up);
            }
            if (squares.TryGetValue(Tuple.Create(current.Col, current.Row + 1), out Square down))
            {
                MarkWalkRecursion(distance, down);
            }
        }

        private bool CheckAndMark(int col, int row)
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
        private void Mark(Square s)
        {
            if (s.MarkType != MarkType.Movable && s.MarkType != MarkType.Capturable)
            {
                if (s.IsEmpty())
                {
                    s.MarkType = MarkType.Movable;
                }
                else if (s.Piece.IsEnemy != IsEnemy)
                {
                    s.MarkType = MarkType.Capturable;
                }
            }
        }

        public virtual void MarkCustom(int currentCol, int currentRow) { }


        public bool MakeDecision(Square currentSquare)
        {
            Chessboard board = (Chessboard)App.Current.Properties["Board"];
            Dictionary<Tuple<int, int>, Square> squares = board.Squares;

            currentSquare.MarkType = MarkType.Selected;
            if (!IsTargetPresent())
            {
                FindNewTarget(currentSquare.Col, currentSquare.Row);
            }
            MarkMoves(currentSquare.Col, currentSquare.Row);
            MarkTarget();
            return SimulateEvent(DecideMove(currentSquare.Col, currentSquare.Row));
        }

        public void MarkDecision(Square currentSquare)
        {
            Chessboard board = (Chessboard)App.Current.Properties["Board"];
            Dictionary<Tuple<int, int>, Square> squares = board.Squares;

            currentSquare.MarkType = MarkType.Selected;
            MarkMoves(currentSquare.Col, currentSquare.Row);
            MarkTarget();
            DecideMove(currentSquare.Col, currentSquare.Row);
        }

        public void TargetAndMarkDecision(Square currentSquare)
        {
            Chessboard board = (Chessboard)App.Current.Properties["Board"];
            Dictionary<Tuple<int, int>, Square> squares = board.Squares;

            currentSquare.MarkType = MarkType.Selected;
            if (!IsTargetPresent())
            {
                FindNewTarget(currentSquare.Col, currentSquare.Row);
            }
            MarkMoves(currentSquare.Col, currentSquare.Row);
            MarkTarget();
        }

        public bool SimulateEvent(Square targetSquare)
        {
            if (targetSquare != null)
            {
                //targetSquare.Mark.RaiseEvent(new RoutedEventArgs(UIElement.MouseLeftButtonUpEvent));
                switch (targetSquare.MarkType)
                {
                    case MarkType.Movable:
                        targetSquare.Move();
                        break;
                    case MarkType.Capturable:
                        targetSquare.Capture();
                        break;
                    case MarkType.CapturableTarget:
                        targetSquare.Capture();
                        break;
                    default:
                        return false;
                }
                return true;
            }
            return false;
        }
        private void FindNewTarget(int currentCol, int currentRow)
        {
            Chessboard board = (Chessboard)App.Current.Properties["Board"];
            Dictionary<Tuple<int, int>, Square> squares = board.Squares;

            int closestPieceID = -1;
            int closestDistance = int.MaxValue;
            foreach (KeyValuePair<Tuple<int, int>, Square> entry in squares)
            {
                if (!entry.Value.IsEmpty() && (entry.Value.Piece.IsEnemy != IsEnemy))
                {
                    int distance = GetDistance(currentCol, currentRow, entry.Key.Item1, entry.Key.Item2);
                    if (entry.Value.Piece.IsLessTargetable) {
                        distance += 1000;
                    }
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestPieceID = entry.Value.Piece.PieceID;
                    }
                }
            }
            TargetID = closestPieceID;
        }
        private bool IsTargetPresent()
        {
            if (TargetID == -1)
            {
                return false;
            }
            Chessboard board = (Chessboard)App.Current.Properties["Board"];
            Dictionary<Tuple<int, int>, Square> squares = board.Squares;

            foreach (KeyValuePair<Tuple<int, int>, Square> entry in squares)
            {
                if (TargetID == entry.Value.Piece.PieceID)
                {
                    return true;
                }
            }
            return false;
        }
        public Square DecideMove(int currentCol, int currentRow)
        {
            Chessboard board = (Chessboard)App.Current.Properties["Board"];
            Dictionary<Tuple<int, int>, Square> squares = board.Squares;

            Square HighestSquare = null;
            int highestWeight = int.MinValue;
            foreach (KeyValuePair<Tuple<int, int>, Square> entry in squares)
            {
                int weight = 0;
                if (entry.Value.MarkType == MarkType.CapturableTarget)
                {
                    weight += 1000;
                }
                else if (entry.Value.MarkType == MarkType.Capturable)
                {
                    if (entry.Value.Piece.IsLessTargetable)
                    {
                        weight += 300;
                    }
                    else { 
                    weight += 500;

                    }
                }
                if (entry.Value.MarkType == MarkType.Movable || entry.Value.MarkType == MarkType.Capturable || entry.Value.MarkType == MarkType.CapturableTarget)
                {
                    Square targetSquare = FindSquareWithPieceID(TargetID);
                    if (targetSquare != null)
                    {
                        if (IsChasing)
                        {
                            weight += GetDistanceWeight(currentCol, currentRow, targetSquare.Col, targetSquare.Row) - GetDistanceWeight(entry.Key.Item1, entry.Key.Item2, targetSquare.Col, targetSquare.Row);
                        }
                        else
                        {
                            weight -= GetDistanceWeight(currentCol, currentRow, targetSquare.Col, targetSquare.Row) - GetDistanceWeight(entry.Key.Item1, entry.Key.Item2, targetSquare.Col, targetSquare.Row);
                        }
                    }
                    else {
                        TargetID = -1;
                    }
                    entry.Value.Label.Content = weight.ToString();

                    if (weight > highestWeight)
                    {
                        HighestSquare = entry.Value;
                        highestWeight = weight;
                    }
                }
            }
            if (HighestSquare == null)
            {
                return (Square)App.Current.Properties["SelectedSquare"];
            }
            return HighestSquare;
        }
        private int GetDistance(int c1, int r1, int c2, int r2)
        {
            return Math.Abs(c1 - c2) + Math.Abs(r1 - r2);
        }
        private int GetDistanceWeight(int c1, int r1, int c2, int r2)
        {
            int highestDiff = Math.Abs(c1 - c2);
            if (Math.Abs(r1 - r2) > highestDiff)
            {
                highestDiff = Math.Abs(r1 - r2);
            }

            return Math.Abs(c1 - c2) + Math.Abs(r1 - r2) + highestDiff;
        }
        public Square FindSquareWithPieceID(int PieceID)
        {
            Chessboard board = (Chessboard)App.Current.Properties["Board"];
            Dictionary<Tuple<int, int>, Square> squares = board.Squares;

            foreach (KeyValuePair<Tuple<int, int>, Square> entry in squares)
            {
                if (entry.Value.Piece.PieceID == PieceID)
                {
                    return entry.Value;
                }
            }
            return null;
        }
        public bool IsSkillAvailable() {
            foreach (Skill s in Skills) {
                if (s.IsReady()) {
                    return true;
                }
            }
            return false;
        }
        public void CheckPerks(PerkType pt)
        {
            foreach (Perk p in ObtainedPerks)
            {
                if (p.PerkType == pt)
                {
                    p.OnAffect(this);
                }
            }
        }
        public void OnStartTurn()
        {
            MovCountLeft = MovCount;
            CastCountLeft = CastCount;
            foreach (Skill s in Skills)
            {
                if (s.CurrentCooldown > 0)
                {
                    s.CurrentCooldown--;
                }
            }
            CheckPerks(PerkType.StartOfTurn);
        }
        public void OnEndTurn()
        {
            CheckPerks(PerkType.EndOfTurn);
            //MovCountLeft = 0;
        }
        public void OnMoveDone()
        {
            MovCountLeft--;
            CheckPerks(PerkType.AfterMove);
        }
        public void OnSkillDone()
        {
            CastCountLeft--;
            CheckPerks(PerkType.AfterSkill);
        }
        public void OnKill()
        {
            CheckPerks(PerkType.OnKill);
        }
        public void OnCapture()
        {
            CheckPerks(PerkType.OnCapture);
        }
        public void OnDeath()
        {
            Chessboard board = (Chessboard)App.Current.Properties["Board"];
            CheckPerks(PerkType.OnDeath);
            board.KillPiece(this);
        }
        public void OnPlace() {
            CheckPerks(PerkType.OnPlace);
        }
        public void OnObtain()
        {
            CheckPerks(PerkType.OnObtain);
        }
        public void OnSummon(Piece summonedPiece) {
            App.Current.Properties["SummonedPiece"] = summonedPiece;
            CheckPerks(PerkType.OnSummon);
        }

        public void RefreshPerkAvailable()
        {
            foreach (Perk p in ObtainedPerks)
            {
                p.Available = true;
            }
        }

        public Square GetCurrentSquare() {
            Chessboard board = (Chessboard)App.Current.Properties["Board"];
            Dictionary<Tuple<int, int>, Square> squares = board.Squares;

            foreach (KeyValuePair<Tuple<int, int>, Square> entry in squares)
            {
                if (entry.Value.Piece.PieceID == PieceID)
                {
                    return entry.Value;
                }
            }
            return null;
        }
    }
}
