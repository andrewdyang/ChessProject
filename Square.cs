using ChessProject.Pieces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ChessProject
{
    enum MarkType { Selectable, Unmovable, Movable, Capturable, Selected, Target, CapturableTarget, Placeable, Affectable, Castable, UnmovableSelectable, Buyable, Sellable, Unbuyable ,BuySelected}
    class Square
    {
        public MarkType MarkType
        {
            get { return markType; }
            set
            {
                markType = value;
                switch (markType)
                {
                    case MarkType.Selectable:
                        Mark.Fill = new SolidColorBrush(Colors.Aquamarine);
                        Mark.Fill.Opacity = 1.0;
                        Mark.MouseLeftButtonUp += Selectable;
                        break;
                    case MarkType.Unmovable:
                        Mark.Fill = new SolidColorBrush(Colors.Transparent);
                        Mark.Fill.Opacity = 0.3;
                        Mark.MouseLeftButtonUp += Unmovable;
                        break;
                    case MarkType.UnmovableSelectable:
                        Mark.Fill = new SolidColorBrush(Colors.Transparent);
                        Mark.Fill.Opacity = 0.3;
                        Mark.MouseLeftButtonUp += Selectable;
                        break;
                    case MarkType.Movable:
                        Mark.Fill = new SolidColorBrush(Colors.Green);
                        Mark.Fill.Opacity = 0.3;
                        Mark.MouseLeftButtonUp += Movable;
                        break;
                    case MarkType.Capturable:
                        Mark.Fill = new SolidColorBrush(Colors.Red);
                        Mark.Fill.Opacity = 0.3;
                        Mark.MouseLeftButtonUp += Capturable;
                        Mark.MouseEnter -= AutoIndicationEnter;
                        Mark.MouseLeave -= AutoIndicationLeave;
                        break;
                    case MarkType.Selected:
                        Mark.Fill = new SolidColorBrush(Colors.Purple);
                        Mark.Fill.Opacity = 1.0;
                        App.Current.Properties["SelectedSquare"] = this;
                        App.Current.Properties["SelectedPiece"] = Piece;
                        Chessboard board = (Chessboard)App.Current.Properties["Board"];
                        board.ShowSkills(Piece);
                        Mark.MouseLeftButtonUp += Unmovable;
                        break;
                    case MarkType.Target:
                        Mark.Fill = new SolidColorBrush(Colors.Red);
                        Mark.Fill.Opacity = 0.5;
                        break;
                    case MarkType.CapturableTarget:
                        Mark.Fill = new SolidColorBrush(Colors.Red);
                        Mark.Fill.Opacity = 1.0;
                        break;
                    case MarkType.Placeable:
                        Mark.Fill = new SolidColorBrush(Colors.SkyBlue);
                        Mark.Fill.Opacity = 0.5;
                        Mark.MouseLeftButtonUp += Placeable;
                        break;
                    case MarkType.Castable:
                        Mark.Fill = new SolidColorBrush(Colors.SkyBlue);
                        Mark.Fill.Opacity = 0.5;
                        Mark.MouseLeftButtonUp -= Unmovable;
                        Mark.MouseLeftButtonUp += Castable;
                        Mark.MouseEnter += CastableEnter;
                        break;
                    case MarkType.Affectable:
                        Mark.Fill = new SolidColorBrush(Colors.DarkRed);
                        Mark.Fill.Opacity = 0.5;
                        break;
                    case MarkType.Buyable:
                        Mark.Fill = new SolidColorBrush(Colors.Green);
                        Mark.Fill.Opacity = 0.3;
                        Mark.MouseLeftButtonUp += Buyable;
                        break;
                    case MarkType.Unbuyable:
                        Mark.Fill = new SolidColorBrush(Colors.Transparent);
                        Mark.MouseLeftButtonUp += Unbuyable;
                        break;
                    case MarkType.BuySelected:
                        Mark.Fill = new SolidColorBrush(Colors.Purple);
                        Mark.Fill.Opacity = 1.0;
                        Mark.MouseLeftButtonUp += Unbuyable;
                        break;
                }
            }
        }
        private MarkType markType;

        public int Col
        {
            get { return col; }
            set
            {
                col = value;
                Grid.SetColumn(Tile, col);
                Grid.SetColumn(Mark, col);
                Grid.SetColumn(Label, col);
            }
        }
        private int col;

        public int Row
        {
            get { return row; }
            set
            {
                row = value;
                Grid.SetRow(Tile, row);
                Grid.SetRow(Mark, row);
                Grid.SetRow(Label, row);
            }
        }
        private int row;

        public Rectangle Tile { get; set; }
        public Rectangle Mark { get; set; }
        public Label Label { get; set; }
        public Button Clickable { get; set; }

        public Piece Piece
        {
            get { return piece; }
            set
            {
                piece = value;
                if (!IsEmpty())
                {
                    Grid.SetColumn(piece.PieceImage, Col);
                    Grid.SetRow(piece.PieceImage, Row);
                }
            }
        }
        private Piece piece = new None();

        public Color TileColor { get; set; }
        public bool IsFrozen
        {
            get
            {
                return isFrozen;
            }
            set
            {
                isFrozen = value;
                if (isFrozen)
                {
                    Frozen();
                }
                else {
                    UnFrozen();
                }
            }
        }
        private bool isFrozen;
        public int FreezeDuration
        {
            get { return freezeDuration; }

            set
            {
                freezeDuration = value;
                if (freezeDuration > 0)
                {
                    isFrozen = true;
                }
                else {
                    isFrozen = false;
                }
            }
        }
        private int freezeDuration;

        public Square(int width, int height, Color color, int col, int row)
        {
            Tile = new Rectangle();
            Tile.Width = width;
            Tile.Height = height;
            TileColor = color;
            Tile.Fill = new SolidColorBrush(color);

            Mark = new Rectangle();
            Mark.Width = width;
            Mark.Height = height;
            MarkType = MarkType.Unmovable;
            Mark.Fill.Opacity = 0.3;

            Mark.MouseRightButtonUp += Unmovable;

            Label = new Label();
            Label.Padding = new Thickness(1, 1, 1, 1);
            Label.VerticalAlignment = VerticalAlignment.Bottom;
            Label.IsHitTestVisible = false;
            Label.Content = "";
            Label.FontSize = 8;
            Label.Foreground = new SolidColorBrush(Colors.Black);

            Col = col;
            Row = row;
        }

        public void Frozen()
        {
            Mark.Fill = new SolidColorBrush(Colors.Blue);
            Mark.Fill.Opacity = 0.5;
        }
        public void UnFrozen()
        {
            
        }
        void Movable(object sender, MouseButtonEventArgs e)
        {
            Move();
        }
        public void Move()
        {
            Square selected = (Square)App.Current.Properties["SelectedSquare"];
            Piece p = selected.Piece;
            selected.Piece = new None();
            Piece = p;
            MoveDone();
        }
        void Capturable(object sender, MouseButtonEventArgs e)
        {
            Capture();
        }
        public void Capture()
        {
            Square selected = (Square)App.Current.Properties["SelectedSquare"];
            Piece p = selected.Piece;

            selected.Piece = new None();
            Kill();
            Piece = p;
            p.OnCapture();
            MoveDone();
        }
        public void Kill()
        {
            Square selected = (Square)App.Current.Properties["SelectedSquare"];
            Piece p = selected.Piece;

            p.OnKill();
            Piece.OnDeath();
            Piece = new None();
        }
        public void RemovePiece() {
            Piece = new None();
        }
        void Selectable(object sender, MouseButtonEventArgs e)
        {
            Chessboard board = (Chessboard)App.Current.Properties["Board"];
            board.ReadyBoardWithoutEvent();
            Mark.MouseLeftButtonUp -= Selectable;
            if (Piece.MovCountLeft > 0 && !Piece.IsAuto)
            {
                Piece.MarkMoves(Col, Row);
            }
            MarkType = MarkType.Selected;
        }
        void Unmovable(object sender, MouseButtonEventArgs e)
        {
            Chessboard board = (Chessboard)App.Current.Properties["Board"];
            board.ReadyBoard();
            AddMoveIndicator();
        }
        void Placeable(object sender, MouseButtonEventArgs e)
        {
            Chessboard board = (Chessboard)App.Current.Properties["Board"];
            board.PlacePieceFromAvailable(this);
            board.ReadyBoard();
        }

        void MoveDone()
        {
            Piece selectedPiece = (Piece)App.Current.Properties["SelectedPiece"];
            selectedPiece.OnMoveDone();
            Chessboard board = (Chessboard)App.Current.Properties["Board"];
            board.ReadyBoard();
        }
        void SkillDone()
        {
            Piece selectedPiece = (Piece)App.Current.Properties["SelectedPiece"];
            selectedPiece.OnSkillDone();
            Chessboard board = (Chessboard)App.Current.Properties["Board"];
            board.ReadyBoard();
        }
        void AutoIndicationEnter(object sender, MouseEventArgs e)
        {
            Chessboard board = (Chessboard)App.Current.Properties["Board"];
            board.ReadyBoard();
            Piece.MarkDecision(this);
        }
        void AutoIndicationLeave(object sender, MouseEventArgs e)
        {
            Chessboard board = (Chessboard)App.Current.Properties["Board"];
            board.ReadyBoard();
        }
        void ShowDescriptionEnter(object sender, MouseEventArgs e)
        {
            Chessboard board = (Chessboard)App.Current.Properties["Board"];
            App.Current.Properties["DescriptionPiece"] = Piece;
            board.ShowDescription();
        }
        public void AddDescription()
        {
            Mark.MouseEnter += ShowDescriptionEnter;
        }
        public void RemoveDescription()
        {
            Mark.MouseEnter -= ShowDescriptionEnter;
        }

        public void ClearLabel()
        {
            Label.Content = "";
        }
        public void AddMoveIndicator()
        {
            Mark.MouseEnter -= AutoIndicationEnter;
            Mark.MouseLeave -= AutoIndicationLeave;
            Mark.MouseEnter += AutoIndicationEnter;
            Mark.MouseLeave += AutoIndicationLeave;
        }

        public void Castable(object sender, MouseButtonEventArgs e)
        {
            Skill s = (Skill)App.Current.Properties["SelectedSkill"];
            s.OnAffect();
            SkillDone();
        }
        public void CastableEnter(object sender, MouseEventArgs e)
        {
            Chessboard board = (Chessboard)App.Current.Properties["Board"];
            board.ReadyBoardWithoutEvent();
            Skill s = (Skill)App.Current.Properties["SelectedSkill"];
            Square sq = (Square)App.Current.Properties["SelectedSquare"];
            s.MarkCastable(sq);
            s.MarkAffectable(this);
        }

        void Buyable(object sender, MouseButtonEventArgs e)
        {
            Chessboard board = (Chessboard)App.Current.Properties["Board"];
            App.Current.Properties["DescriptionPiece"] = Piece;
            board.ShowDescription();
            board.AddBuyAction(Piece);
            board.ReadyShopWithoutDescription();
            Mark.MouseLeftButtonUp -= Buyable;
            MarkType = MarkType.BuySelected;
        }

        void Unbuyable(object sender, MouseButtonEventArgs e)
        {
            Chessboard board = (Chessboard)App.Current.Properties["Board"];
            board.ReadyShop();
            board.ReadyShopAction();
        }
        public void ClearAllEvent()
        {
            Mark.MouseLeftButtonUp -= Movable;
            Mark.MouseLeftButtonUp -= Selectable;
            Mark.MouseLeftButtonUp -= Unmovable;
            Mark.MouseLeftButtonUp -= Capturable;
            Mark.MouseLeftButtonUp -= Placeable;
            Mark.MouseLeftButtonUp -= Castable;
            Mark.MouseLeftButtonUp -= Buyable;
            Mark.MouseLeftButtonUp -= Unbuyable;
            Mark.MouseEnter -= ShowDescriptionEnter;
            Mark.MouseEnter -= CastableEnter;
            Mark.MouseEnter -= AutoIndicationEnter;
            Mark.MouseLeave -= AutoIndicationLeave;
        }

        public bool IsEmpty() {
            if (Piece.PieceType == PieceType.None)
            {
                return true;
            }
            else {
                return false;
            }
        }
    }
}
