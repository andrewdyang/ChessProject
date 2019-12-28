using ChessProject.BoardActions;
using ChessProject.Perks;
using ChessProject.Pieces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ChessProject
{
    public enum TurnOwner { Player, Enemy }
    public enum StageStatus { Battle, Shop }
    class Chessboard
    {
        private Grid ChessGrid { get; set; }
        public Dictionary<Tuple<int, int>, Square> Squares = new Dictionary<Tuple<int, int>, Square>();
        public List<Piece> Army = new List<Piece>();
        public ObservableCollection<Piece> ArmyAvailable = new ObservableCollection<Piece>();
        private Canvas AnimationCanvas;
        public StageStatus StageStatus;
        public ObservableCollection<Perk> PerkPool = new ObservableCollection<Perk>();
        public int Currency {
            get {
                return _currency;
            }
            set {
                _currency = value;

                Label currencyLabel = (Label)App.Current.Properties["CurrencyLabel"];
                currencyLabel.Content = _currency;
            }
        }
        private int _currency;
        public int ShopMaxPerkCount = 1;
        public int ShopPieceCount = 3;
        public ObservableCollection<BoardAction> ShopAction = new ObservableCollection<BoardAction>();
        public ObservableCollection<BoardAction> ShopActionKeep = new ObservableCollection<BoardAction>();

        private ListView Inventory { get; set; }
        private int Difficulty { get; set; }
        public int Col { get; set; }
        public int Row { get; set; }
        public int CurrentTurn { get; set; }
        public Random R = new Random();

        public TurnOwner TurnOwner { get; set; }

        bool MoveMade = false;
        public bool StopAutoQueue { get; set; }

        public bool IsAnimationQueueOnGoing { get; set; }

        List<Object> AnimationQueue = new List<object>();

        public Chessboard(Grid grid, ListView inv, int col, int row)
        {
            ChessGrid = grid;
            Inventory = inv;
            Inventory.ItemsSource = ArmyAvailable;
            Col = col;
            Row = row;
            AnimationCanvas = (Canvas)App.Current.Properties["AnimationCanvas"];
            App.Current.Properties["NextPieceID"] = 1;
            App.Current.Properties["Board"] = this;
            App.Current.Properties["AutoSkillPerkQueue"] = AnimationQueue;
            App.Current.Properties["Currency"] = Currency;

            for (int x = 0; x < col; x++)
            {
                for (int y = 0; y < row; y++)
                {
                    Square s;
                    if ((x + y) % 2 == 0)
                    {
                        s = new Square(50, 50, Colors.Ivory, x, y);
                    }
                    else
                    {
                        s = new Square(50, 50, Colors.RosyBrown, x, y);
                    }

                    ChessGrid.Children.Add(s.Tile);
                    ChessGrid.Children.Add(s.Mark);
                    ChessGrid.Children.Add(s.Label);
                    Squares.Add(Tuple.Create(x, y), s);
                }
            }

            Currency = 10;

            CreatePerkPool();
            ShopActionKeep.Add(new IncreaseMaxPerkCount());
            ShopActionKeep.Add(new IncreasePieceCount());
            /*
            Piece p;
            
            Piece p = new Placeholder();
            p.Name = "Placeholder";
            Army.Add(p);
            p = new King();
            p.Name = "King";
            Army.Add(p);
            p = new Pawn();
            p.Name = "Pawn";
            Army.Add(p);
            p = new Rook();
            p.Name = "Rook";
            Army.Add(p);
            p = new Bishop();
            p.Name = "Bishop";
            Army.Add(p);
            p = new Queen();
            p.Name = "Queen";
            Army.Add(p);
            p = new Knight();
            p.Name = "Knight";
            Army.Add(p);
            */

            RefreshArmyAvailable();

            Difficulty = 10;
            NewMatch();
        }
        public void CreatePerkPool()
        {
            PerkPool.Add(new AffectRangeUpPerk());
            PerkPool.Add(new CastRangeUpPerk());
            PerkPool.Add(new ExtraMovPerk());
            PerkPool.Add(new GrantBlizzardPerk());
            PerkPool.Add(new GrantExplosionPerk());
            PerkPool.Add(new GrantSummonPawnPerk());
            PerkPool.Add(new GrantPiercePerk());
            PerkPool.Add(new MayJumpPerk());
            PerkPool.Add(new MovAfterKillPerk());
            PerkPool.Add(new OnSummonAutoTauntPerk());
            PerkPool.Add(new OnSummonExplodeOnDeathPerk());
            PerkPool.Add(new OnSummonRangeUpPerk());
            PerkPool.Add(new RangeUp2Perk());
            PerkPool.Add(new RangeUpPerk());
            PerkPool.Add(new SummonPawnPerk());
        }
        public void RefreshArmyAvailable()
        {
            ArmyAvailable.Clear();
            foreach (Piece p in Army)
            {
                ArmyAvailable.Add(p);
            }
        }
        public void ShowArmyAvailable()
        {
            Inventory.ItemsSource = ArmyAvailable;
        }

        public void PlacePiece(Piece p, int col, int row)
        {
            if (Squares.TryGetValue(Tuple.Create(col, row), out Square s))
            {
                s.Piece = p;
                ChessGrid.Children.Add(s.Piece.PieceImage);
            }
            else
            {
                return;
            }
        }
        public void PlacePieceFromAvailable(Square s)
        {
            Piece selectedPiece = (Piece)Inventory.SelectedItem;
            Piece copiedPiece = selectedPiece.Copy();
            s.Piece = copiedPiece;
            ArmyAvailable.Remove(selectedPiece);
            ChessGrid.Children.Add(s.Piece.PieceImage);

            s.Piece.OnPlace();
            RunAnimationQueue(null);
        }
        public void KillPiece(Piece piece)
        {
            DeathAnimation(Grid.GetColumn(piece.PieceImage), Grid.GetRow(piece.PieceImage));
            ChessGrid.Children.Remove(piece.PieceImage);
        }
        public void RemovePiece(Piece piece)
        {
            piece.GetCurrentSquare().RemovePiece();
            ChessGrid.Children.Remove(piece.PieceImage);
        }

        public void MarkSelectable()
        {
            foreach (KeyValuePair<Tuple<int, int>, Square> entry in Squares)
            {
                if (!entry.Value.IsEmpty() && entry.Value.Piece.MovCountLeft > 0)
                {
                    entry.Value.MarkType = MarkType.Selectable;
                }
            }
        }

        public bool CheckGameEnd()
        {
            bool win = true;
            bool lose = true;

            if (ArmyAvailable.Count > 0)
            {
                lose = false;
            }

            foreach (KeyValuePair<Tuple<int, int>, Square> entry in Squares)
            {
                if (!entry.Value.IsEmpty())
                {
                    if (entry.Value.Piece.IsEnemy)
                    {
                        win = false;
                    }
                    else
                    {
                        lose = false;
                    }
                    if (!win && !lose)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public void EndCurrentGame()
        {
            bool win = true;
            bool lose = true;

            if (ArmyAvailable.Count > 0)
            {
                lose = false;
            }

            foreach (KeyValuePair<Tuple<int, int>, Square> entry in Squares)
            {
                if (!entry.Value.IsEmpty())
                {
                    if (entry.Value.Piece.IsEnemy)
                    {
                        win = false;
                    }
                    else
                    {
                        lose = false;
                    }
                    if (!win && !lose)
                    {
                        return;
                    }
                }
            }

            if (lose)
            {
                Lose();
            }
            else if (win)
            {
                Win();
            }

        }

        public void ReadyShop()
        {
            ShowArmyAvailable();

            foreach (KeyValuePair<Tuple<int, int>, Square> entry in Squares)
            {
                entry.Value.ClearLabel();
                entry.Value.ClearAllEvent();

                if (entry.Value.IsEmpty())
                {
                    entry.Value.MarkType = MarkType.Unbuyable;
                }
                else
                {
                    entry.Value.AddDescription();
                    entry.Value.MarkType = MarkType.Buyable;
                }
            }
        }
        public void ReadyShopWithoutDescription()
        {
            ShowArmyAvailable();

            foreach (KeyValuePair<Tuple<int, int>, Square> entry in Squares)
            {
                entry.Value.ClearLabel();
                entry.Value.ClearAllEvent();

                if (entry.Value.IsEmpty())
                {
                    entry.Value.MarkType = MarkType.Unbuyable;
                }
                else
                {
                    entry.Value.MarkType = MarkType.Buyable;
                }
            }
        }

        public void ReadyBoard()
        {
            ShowArmyAvailable();

            foreach (KeyValuePair<Tuple<int, int>, Square> entry in Squares)
            {
                entry.Value.ClearLabel();
                entry.Value.ClearAllEvent();

                if (entry.Value.IsFrozen)
                {
                    entry.Value.MarkType = MarkType.Unmovable;
                    entry.Value.Frozen();
                    continue;
                }

                if (entry.Value.IsEmpty())
                {
                    entry.Value.MarkType = MarkType.Unmovable;
                }
                else
                {
                    if (TurnOwner == TurnOwner.Player)
                    {
                        entry.Value.AddDescription();
                        if (entry.Value.Piece.IsEnemy)
                        {
                            entry.Value.MarkType = MarkType.Unmovable;
                            entry.Value.AddMoveIndicator();
                        }
                        else
                        {
                            if (entry.Value.Piece.IsAuto)
                            {
                                entry.Value.AddMoveIndicator();
                                entry.Value.MarkType = MarkType.UnmovableSelectable;
                            }
                            else if (entry.Value.Piece.MovCountLeft > 0 || (entry.Value.Piece.CastCountLeft > 0 && entry.Value.Piece.IsSkillAvailable()))
                            {
                                entry.Value.MarkType = MarkType.Selectable;
                            }
                            else
                            {
                                entry.Value.MarkType = MarkType.UnmovableSelectable;
                            }
                        }
                    }
                    else if (TurnOwner == TurnOwner.Enemy)
                    {
                        if (!entry.Value.Piece.IsEnemy)
                        {
                            entry.Value.MarkType = MarkType.Unmovable;
                            entry.Value.AddDescription();
                        }
                        else
                        {
                            entry.Value.AddDescription();
                            if (entry.Value.Piece.MovCountLeft > 0 || (entry.Value.Piece.CastCountLeft > 0 && entry.Value.Piece.Skills.Count > 0))
                            {
                                entry.Value.MarkType = MarkType.Selectable;
                            }
                            else
                            {
                                entry.Value.MarkType = MarkType.Unmovable;
                            }
                        }
                    }
                }
            }
            RunAnimationQueue(null);
        }
        public void Win()
        {
            if (AnimationQueue.Count > 0)
            {
                StopAutoQueue = true;
            }
            if (Difficulty - CurrentTurn < 5)
            {
                Currency += 5;
            }
            else {
                Currency += Difficulty - CurrentTurn;
            }

            Difficulty += 5;
            /*
            foreach (Piece p in Army)
            {
                p.PerkPoint++;
            }
            */
            NewMatch();
        }
        public void Lose()
        {
            if (AnimationQueue.Count > 0)
            {
                StopAutoQueue = true;
            }
            Difficulty -= 3;
            NewMatch();
        }
        public void NewMap()
        {
            ChessGrid.Children.Clear();
            Squares.Clear();

            //Create empty board
            for (int x = 0; x < Col; x++)
            {
                for (int y = 0; y < Row; y++)
                {
                    Square sq;
                    if ((x + y) % 2 == 0)
                    {
                        sq = new Square(50, 50, Colors.Ivory, x, y);
                    }
                    else
                    {
                        sq = new Square(50, 50, Colors.RosyBrown, x, y);
                    }
                    ChessGrid.Children.Add(sq.Tile);
                    ChessGrid.Children.Add(sq.Mark);
                    ChessGrid.Children.Add(sq.Label);
                    Squares.Add(Tuple.Create(x, y), sq);
                }
            }

            if (StageStatus == StageStatus.Battle)
            {
                List<Piece> enemyPool = new List<Piece>();
                List<Square> squarePool = new List<Square>();
                int leftPoint = Difficulty;
                int row = Row - 3;

                enemyPool.Add(new EPawn());
                enemyPool.Add(new ERook());
                enemyPool.Add(new EBishop());
                enemyPool.Add(new EKnight());
                enemyPool.Add(new EKing());

                squarePool = Squares.Where(square => square.Key.Item2 < row).Select(square => square.Value).ToList();

                Piece p;
                Square s;
                while (leftPoint > 0 && squarePool.Count > 0)
                {
                    enemyPool.RemoveAll(piece => piece.DiffPoint > leftPoint);
                    p = enemyPool[R.Next(enemyPool.Count)];
                    s = squarePool[R.Next(squarePool.Count)];
                    PlacePiece(p.Copy(), s.Col, s.Row);
                    leftPoint -= p.DiffPoint;
                    squarePool.Remove(s);
                }
                ListView ShopActionView = (ListView)App.Current.Properties["ShopActionView"];
                ShopActionView.ItemsSource = null;
            }
            else if (StageStatus == StageStatus.Shop)
            {
                List<Piece> piecePool = new List<Piece>();
                List<Square> squarePool = new List<Square>();

                piecePool.Add(new Rook());
                piecePool.Add(new Bishop());
                piecePool.Add(new Knight());
                piecePool.Add(new King());
                piecePool.Add(new Queen());

                squarePool = Squares.Select(square => square.Value).ToList();

                Piece p;
                Square s;
                for (int i = 0; i < ShopPieceCount; i++)
                {
                    p = piecePool[R.Next(piecePool.Count)].Copy();
                    s = squarePool[R.Next(squarePool.Count)];
                    p.PerkPoint = R.Next(ShopMaxPerkCount + 1);
                    SpendPerkPoints(p);
                    PlacePiece(p, s.Col, s.Row);
                    squarePool.Remove(s);
                }

                ListView ShopActionView = (ListView)App.Current.Properties["ShopActionView"];
                ReadyShopAction();
                ShopActionView.ItemsSource = ShopAction;
            }
        }
        public void ReadyShopAction()
        {
            ShopAction.Clear();
            foreach (var item in ShopActionKeep) {
                ShopAction.Add(item);
            }
        }
        public bool ChooseRandomPerk(Piece piece, List<Perk> perkpool)
        {
            if (piece.PerkPoint <= 0)
            {
                return false;
            }

            /*
            List<Perk> lp = new List<Perk>();
            List<Perk> pool = new List<Perk>();

            pool = perkpool.Where(pe => pe.ObtainablePieces.Contains(piece.PieceType)).ToList();

            for (int i = lp.Count - 1; i >= 0; i--)
            {
                foreach (Perk obtainedperk in piece.ObtainedPerks)
                {
                    if (obtainedperk.GetType() == lp[i].GetType())
                    {
                        lp.RemoveAt(i);
                    }
                }
            }

            foreach (Perk perk in pool)
            {
                int matchingPerkCount = 0;
                foreach (Type preperk in perk.PrerequisitePerk)
                {
                    foreach (Perk obtainedperk in piece.ObtainedPerks)
                    {
                        if (obtainedperk.GetType() == preperk)
                        {
                            matchingPerkCount++;
                            break;
                        }
                    }
                }
                if (matchingPerkCount == perk.PrerequisitePerk.Count)
                {
                    lp.Add((Perk)Activator.CreateInstance(perk.GetType()));
                }
            }
            */

            List<Perk> lp = new List<Perk>();
            bool obtained;

            foreach (Perk perk in perkpool)
            {
                if (perk.ObtainablePieces.Contains(piece.PieceType))
                {
                    obtained = false;
                    foreach (Perk obtainedperk in piece.ObtainedPerks)
                    {
                        if (obtainedperk.GetType() == perk.GetType())
                        {
                            obtained = true;
                            break;
                        }
                    }

                    if (!obtained)
                    {
                        int matchingPerkCount = 0;
                        foreach (Type preperk in perk.PrerequisitePerk)
                        {
                            foreach (Perk obtainedperk in piece.ObtainedPerks)
                            {
                                if (obtainedperk.GetType() == preperk)
                                {
                                    matchingPerkCount++;
                                    break;
                                }
                            }
                        }
                        if (matchingPerkCount == perk.PrerequisitePerk.Count)
                        {
                            lp.Add((Perk)Activator.CreateInstance(perk.GetType()));
                        }
                    }
                }
            }

            if (lp.Count > 0)
            {
                piece.ObtainedPerks.Add((Perk)Activator.CreateInstance(lp[R.Next(lp.Count)].GetType()));
                piece.PerkPoint -= 1;
                piece.CheckPerks(PerkType.OnObtain);
                return true;
            }
            else
            {
                return false;
            }
        }

        public void SpendPerkPoints(Piece p)
        {
            List<Perk> perklist = PerkPool.ToList();
            while (ChooseRandomPerk(p, perklist)) ;
        }

        public void NewMatch()
        {
            Button b = (Button)App.Current.Properties["EndTurnButton"];
            if (StageStatus == StageStatus.Battle)
            {
                StageStatus = StageStatus.Shop;
                b.Content = "Exit Shop";
                NewMap();
                RefreshArmyAvailable();
                AnimationCanvas.Children.Clear();
                SetUIInteractivity(true);
                ReadyShop();
            }
            else if (StageStatus == StageStatus.Shop)
            {
                StageStatus = StageStatus.Battle;
                b.Content = "End Turn";
                NewMap();
                RefreshArmyAvailable();
                CurrentTurn = 0;
                TurnOwner = TurnOwner.Enemy;
                AnimationCanvas.Children.Clear();
                StartNewTurn();
            }
        }

        public void EndTurnButtonClick()
        {
            if (StageStatus == StageStatus.Battle)
            {
                if (CheckGameEnd())
                {
                    EndCurrentGame();
                    return;
                }
                AutoTurn(EndTurn);
            }
            else if (StageStatus == StageStatus.Shop)
            {
                NewMatch();
            }
        }

        public void UnfreezeBoard()
        {
            foreach (KeyValuePair<Tuple<int, int>, Square> entry in Squares)
            {
                if (entry.Value.FreezeDuration > 0)
                {
                    entry.Value.FreezeDuration--;
                }
            }
        }
        public void StartNewTurn()
        {
            if (CheckGameEnd())
            {
                EndCurrentGame();
                return;
            }

            UnfreezeBoard();
            ReadyBoard();
            CurrentTurn++;
            if (TurnOwner == TurnOwner.Enemy)
            {
                TurnOwner = TurnOwner.Player;
            }
            else if (TurnOwner == TurnOwner.Player)
            {
                TurnOwner = TurnOwner.Enemy;
            }
            foreach (KeyValuePair<Tuple<int, int>, Square> entry in Squares)
            {
                if (entry.Value.IsFrozen)
                {
                    continue;
                }
                if (TurnOwner == TurnOwner.Player && !entry.Value.Piece.IsEnemy)
                {
                    entry.Value.Piece.OnStartTurn();
                }
                else if (TurnOwner == TurnOwner.Enemy && entry.Value.Piece.IsEnemy)
                {
                    entry.Value.Piece.OnStartTurn();
                }
            }

            ReadyBoard();
            if (TurnOwner == TurnOwner.Enemy)
            {
                RunAnimationQueue(EnemyTurn);
            }
            else
            {
                RunAnimationQueue(null);
            }
        }

        public void EndTurn()
        {
            if (CheckGameEnd())
            {
                EndCurrentGame();
                return;
            }
            foreach (KeyValuePair<Tuple<int, int>, Square> entry in Squares)
            {
                if (entry.Value.IsFrozen)
                {
                    continue;
                }
                if (TurnOwner == TurnOwner.Player && !entry.Value.Piece.IsEnemy)
                {
                    entry.Value.Piece.OnEndTurn();
                }
                else if (TurnOwner == TurnOwner.Enemy && entry.Value.Piece.IsEnemy)
                {
                    entry.Value.Piece.OnEndTurn();
                }
            }
            RunAnimationQueue(StartNewTurn);
        }

        public void SetUIInteractivity(bool isInteractable)
        {
            SetBoardInteractivity(isInteractable);
            SetEndTurnButtonInteractivity(isInteractable);
            SetInventoryInteractivity(isInteractable);
        }
        public void SetBoardInteractivity(bool isInteractable)
        {
            ChessGrid.IsEnabled = isInteractable;
        }
        public void SetEndTurnButtonInteractivity(bool isInteractable)
        {
            Button EndTurnButton = (Button)App.Current.Properties["EndTurnButton"];

            EndTurnButton.IsEnabled = isInteractable;
        }
        public void SetInventoryInteractivity(bool isInteractable)
        {
            Inventory.IsEnabled = isInteractable;
        }

        public void AutoTurn(Action continueWith)
        {
            SetUIInteractivity(false);
            foreach (KeyValuePair<Tuple<int, int>, Square> entry in Squares)
            {
                if (!entry.Value.Piece.IsEnemy && entry.Value.Piece.IsAuto && entry.Value.Piece.MovCountLeft > 0 && !entry.Value.IsFrozen)
                {
                    AnimationQueue.Add(entry.Value.Piece);
                    //PiecesToMove.Add(entry.Value.Piece);
                }
            }
            if (AnimationQueue.Count > 0)
            {
                RunAnimationQueue(continueWith);
            }
            else
            {
                continueWith();
            }
        }

        public void EnemyTurn()
        {
            SetUIInteractivity(false);
            foreach (KeyValuePair<Tuple<int, int>, Square> entry in Squares)
            {
                if (entry.Value.Piece.IsEnemy && entry.Value.Piece.MovCountLeft > 0 && !entry.Value.IsFrozen)
                {
                    AnimationQueue.Add(entry.Value.Piece);
                }
            }
            if (AnimationQueue.Count > 0)
            {
                RunAnimationQueue(EndTurn);
            }
            else
            {
                EndTurn();
            }
        }

        public async void RunAnimationQueue(Action continueWith)
        {
            int delay = 300;
            if (!IsAnimationQueueOnGoing)
            {
                IsAnimationQueueOnGoing = true;
                SetUIInteractivity(false);

                int originalSize = AnimationQueue.Count();
                int loopCount = 0;

                while (AnimationQueue.Count > 0)
                {
                    //Auto Skill case
                    if (AnimationQueue.First().GetType().IsSubclassOf(typeof(AutoSkillPerk)))
                    {
                        AutoSkillPerk currentPerk = (AutoSkillPerk)AnimationQueue.First();
                        currentPerk.MarkCast();
                        if (!currentPerk.IsSelfCast())
                        {
                            await Task.Delay(delay);
                        }
                        currentPerk.MarkAffect();
                        await Task.Delay(delay);
                        currentPerk.CastSkill();
                        ReadyBoard();
                        AnimationQueue.Remove(currentPerk);
                    }
                    //Piece case
                    else if (AnimationQueue.First().GetType().IsSubclassOf(typeof(Piece)))
                    {
                        loopCount++;
                        Piece currentPiece = (Piece)AnimationQueue.First();
                        Square currentSquare = currentPiece.GetCurrentSquare();

                        if (currentSquare != null)
                        {
                            currentSquare.Piece.TargetAndMarkDecision(currentSquare);
                            await Task.Delay(delay);
                            MoveMade = currentSquare.Piece.SimulateEvent(currentSquare.Piece.DecideMove(currentSquare.Col, currentSquare.Row)) || MoveMade;
                            ReadyBoard();

                            if (currentSquare.Piece.MovCountLeft > 0)
                            {
                                AnimationQueue.Remove(currentPiece);
                                AnimationQueue.Insert(AnimationQueue.Count, currentPiece);
                            }
                            else
                            {
                                AnimationQueue.Remove(currentPiece);
                            }

                            if (loopCount >= originalSize)
                            {
                                if (MoveMade)
                                {
                                    loopCount = 0;
                                    originalSize = AnimationQueue.Count();
                                    MoveMade = false;
                                }
                                else
                                {
                                    ReadyBoard();
                                    break;
                                }
                            }
                        }
                        else
                        {
                            AnimationQueue.Remove(currentPiece);
                        }
                    }
                }

                MoveMade = false;
                AnimationQueue.Clear();
                SetUIInteractivity(true);
                IsAnimationQueueOnGoing = false;
                continueWith?.Invoke();
            }
        }
        /*
        public async void RunAnimationQueue()
        {
            if (!IsAnimationQueueOnGoing)
            {
                IsAnimationQueueOnGoing = true;
                SetUIInteractivity(false);

                int originalSize = AnimationQueue.Count();
                int loopCount = 0;

                while (AnimationQueue.Count > 0)
                {
                    if (AnimationQueue.First().GetType().IsSubclassOf(typeof(AutoSkillPerk)))
                    {
                        AutoSkillPerk currentPerk = (AutoSkillPerk)AnimationQueue.First();
                        currentPerk.MarkCast();
                        if (!currentPerk.IsSelfCast())
                        {
                            await Task.Delay(500);
                        }
                        currentPerk.MarkAffect();
                        await Task.Delay(500);
                        currentPerk.CastSkill();
                        ReadyBoard();
                        AnimationQueue.Remove(currentPerk);
                    }
                    else if (AnimationQueue.First().GetType().IsSubclassOf(typeof(Piece)))
                    {
                        loopCount++;
                        Piece currentPiece = (Piece)AnimationQueue.First();
                        Square currentSquare = currentPiece.GetCurrentSquare();

                        if (currentSquare != null)
                        {
                            currentSquare.Piece.TargetAndMarkDecision(currentSquare);
                            await Task.Delay(500);
                            MoveMade = currentSquare.Piece.SimulateEvent(currentSquare.Piece.DecideMove(currentSquare.Col, currentSquare.Row)) || MoveMade;

                            if (currentSquare.Piece.MovCountLeft > 0)
                            {
                                AnimationQueue.Remove(currentPiece);
                                AnimationQueue.Insert(AnimationQueue.Count, currentPiece);
                            }
                            else
                            {
                                AnimationQueue.Remove(currentPiece);
                            }

                            if (loopCount >= originalSize)
                            {
                                if (MoveMade)
                                {
                                    loopCount = 0;
                                    originalSize = AnimationQueue.Count();
                                    MoveMade = false;
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                        else
                        {
                            AnimationQueue.Remove(currentPiece);
                        }
                    }
                }

                MoveMade = false;
                AnimationQueue.Clear();
                SetUIInteractivity(true);
                IsAnimationQueueOnGoing = false;
            }
        }
        */
        public void ShowSkills(Piece p)
        {
            Inventory.ItemsSource = p.Skills;
        }
        public void ReadyBoardWithoutEvent()
        {
            foreach (KeyValuePair<Tuple<int, int>, Square> entry in Squares)
            {
                entry.Value.ClearLabel();
                entry.Value.ClearAllEvent();
                entry.Value.MarkType = MarkType.Unmovable;
                if (entry.Value.IsFrozen)
                {
                    entry.Value.Frozen();
                }
            }
        }
        public void ShowDescription()
        {
            Label PieceNameLabel = (Label)App.Current.Properties["PieceNameLabel"];
            Image PieceImage = (Image)App.Current.Properties["PieceImage"];
            ListView ObtainedPerks = (ListView)App.Current.Properties["ObtainedPerks"];
            ListView AvailablePerks = (ListView)App.Current.Properties["AvailablePerks"];
            Piece p = (Piece)App.Current.Properties["DescriptionPiece"];

            PieceNameLabel.Content = p.Name;
            var converter = new ImageSourceConverter();
            PieceImage.Source = new BitmapImage(
                    new Uri(p.ImagePath, UriKind.Relative));
            ObtainedPerks.ItemsSource = p.ObtainedPerks;
            //AvailablePerks.ItemsSource = null;
        }

        public void ShowAvailablePerks()
        {
            ListView AvailablePerks = (ListView)App.Current.Properties["AvailablePerks"];
            Piece p = (Piece)Inventory.SelectedItem;

            if (p.PerkPoint > 0)
            {
                if (p.AvailablePerks.Count == 0)
                {
                    List<Perk> lp = new List<Perk>();
                    bool obtained;

                    foreach (Perk perk in PerkPool)
                    {
                        if (perk.ObtainablePieces.Contains(p.PieceType))
                        {
                            obtained = false;
                            foreach (Perk obtainedperk in p.ObtainedPerks)
                            {
                                if (obtainedperk.GetType() == perk.GetType())
                                {
                                    obtained = true;
                                    break;
                                }
                            }

                            if (!obtained)
                            {
                                int matchingPerkCount = 0;
                                foreach (Type preperk in perk.PrerequisitePerk)
                                {
                                    foreach (Perk obtainedperk in p.ObtainedPerks)
                                    {
                                        if (obtainedperk.GetType() == preperk)
                                        {
                                            matchingPerkCount++;
                                            break;
                                        }
                                    }
                                }
                                if (matchingPerkCount == perk.PrerequisitePerk.Count)
                                {
                                    lp.Add((Perk)Activator.CreateInstance(perk.GetType()));
                                }
                            }
                        }
                    }

                    int i = 0;
                    while (i < 3 && lp.Count > 0)
                    {
                        int r = R.Next(lp.Count);
                        p.AvailablePerks.Add(lp[r]);
                        lp.RemoveAt(r);
                        i++;
                    }
                    AvailablePerks.ItemsSource = p.AvailablePerks;
                }
                else
                {
                    AvailablePerks.ItemsSource = p.AvailablePerks;
                }
            }
        }

        public void AddSellAction(Piece p)
        {
            ReadyShopAction();
            ShopAction.Add(new SellPiece(3, p));
            //ListView ShopActionView = (ListView)App.Current.Properties["ShopActionView"];
            //ShopActionView.Items.Refresh();
        }
        public void AddBuyAction(Piece p)
        {
            ReadyShopAction();
            ShopAction.Add(new BuyPiece(5, p));
            //ListView ShopActionView = (ListView)App.Current.Properties["ShopActionView"];
            //ShopActionView.Items.Refresh();
        }

        private void DeathAnimation(int col, int row)
        {
            int left = col * 40 + 20;
            int top = row * 40 + 20;
            for (int i = 0; i < 50; i++)
            {
                var xAnimation = new DoubleAnimation();
                xAnimation.By = R.Next(-200, 200);
                xAnimation.Duration = new Duration(TimeSpan.FromSeconds(1));

                var yAnimation = new DoubleAnimation();
                yAnimation.By = R.Next(-200, 200);
                yAnimation.Duration = new Duration(TimeSpan.FromSeconds(1));

                var opacityAnimation = new DoubleAnimation();
                opacityAnimation.To = 0;
                opacityAnimation.Duration = new Duration(TimeSpan.FromSeconds(1));

                Rectangle r = new Rectangle();

                r.Width = 2;
                r.Height = 2;
                r.Fill = new SolidColorBrush(Colors.Black);
                r.IsHitTestVisible = false;

                Canvas.SetLeft(r, left);
                Canvas.SetTop(r, top);

                AnimationCanvas.Children.Add(r);

                r.BeginAnimation(Canvas.LeftProperty, xAnimation);
                r.BeginAnimation(Canvas.TopProperty, yAnimation);
                r.BeginAnimation(Rectangle.OpacityProperty, opacityAnimation);
            }
        }
    }
}
