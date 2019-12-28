using ChessProject.Pieces;
using ChessProject.Skills;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ChessProject
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Chessboard Chessboard;
        public MainWindow()
        {
            InitializeComponent();

            App.Current.Properties["Description"] = Description;
            App.Current.Properties["PieceNameLabel"] = PieceNameLabel;
            App.Current.Properties["PieceImage"] = PieceImage;
            App.Current.Properties["ObtainedPerks"] = ObtainedPerks;
            App.Current.Properties["ShopActionView"] = AvailableActions;
            App.Current.Properties["AnimationCanvas"] = AnimationCanvas;
            App.Current.Properties["EndTurnButton"] = EndTurnButton;
            App.Current.Properties["CurrencyLabel"] = CurrencyLabel;

            Chessboard = new Chessboard(ChessGrid, Inventory, 10, 10);
        }

        private void EndTurnButton_Click(object sender, RoutedEventArgs e)
        {
            Chessboard.EndTurnButtonClick();
        }

        private void Item_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var selectedItem = Inventory.SelectedItem;
            if (selectedItem == null)
            {
                return;
            }
            else
            {
                Piece p;
                Skill s;
                if (selectedItem.GetType().IsSubclassOf(typeof(Piece)))
                {
                    p = (Piece)selectedItem;
                    App.Current.Properties["DescriptionPiece"] = (Piece)selectedItem;
                    Chessboard.ShowDescription();
                    //c.ShowAvailablePerks();
                    if (Chessboard.StageStatus == StageStatus.Shop)
                    {
                        Chessboard.AddSellAction(p);
                    }
                    else if (Chessboard.StageStatus == StageStatus.Battle)
                    {
                        Chessboard.ReadyBoardWithoutEvent();
                        p.MarkPlaceable();
                    }
                }
                else if (selectedItem.GetType().IsSubclassOf(typeof(Skill)))
                {
                    if (Chessboard.StageStatus != StageStatus.Battle)
                    {
                        return;
                    }
                    s = (Skill)selectedItem;
                    p = (Piece)App.Current.Properties["SelectedPiece"];
                    if (p.CastCountLeft > 0 && s.CurrentCooldown <= 0)
                    {
                        App.Current.Properties["SelectedSkill"] = selectedItem;
                        Square sq = (Square)App.Current.Properties["SelectedSquare"];
                        Chessboard.ReadyBoardWithoutEvent();
                        s.MarkCastable(sq);
                    }
                }
            }
        }

        private void AvailableItem_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            /*
            var selectedItem = AvailablePerks.SelectedItem;
            if (selectedItem == null)
            {
                return;
            }
            else
            {
                Perk perk = (Perk)selectedItem;
                if (selectedItem.GetType().IsSubclassOf(typeof(Perk)))
                {
                    Chessboard c = (Chessboard)App.Current.Properties["Board"];
                    Piece piece = (Piece)App.Current.Properties["DescriptionPiece"];
                    piece.ObtainedPerks.Add((Perk)Activator.CreateInstance(selectedItem.GetType()));
                    piece.AvailablePerks.Clear();
                    piece.PerkPoint -= 1;
                    piece.CheckPerks(PerkType.OnObtain);
                    Inventory.Items.Refresh();
                }
            }
            */
            var selectedItem = AvailableActions.SelectedItem;
            if (selectedItem == null)
            {
                return;
            }
            else
            {
                BoardAction action = (BoardAction)selectedItem;
                if (action.DoAction()) {
                    Chessboard.ReadyShop();
                    Chessboard.ReadyShopAction();
                }
            }
        }
    }
}
