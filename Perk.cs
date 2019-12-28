using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessProject
{
    enum PerkType {OnObtain,OnPlace, StartOfTurn, EndOfTurn, OnCapture,OnKill, OnSkill,AfterSkill, OnDeath, AfterMove, OnSummon, OnAutoSkill}
    abstract class Perk
    {
        public PerkType PerkType { get; set; }
        public bool Available { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<PieceType> ObtainablePieces = new List<PieceType>();
        public List<Type> PrerequisitePerk = new List<Type>();

        public Perk() {
            Available = true;
        }
        public void OnAffect(Piece p) {
            if (Available)
            {
                Effect(p);
            }
        }
        abstract public void Effect(Piece p);
    }
}
