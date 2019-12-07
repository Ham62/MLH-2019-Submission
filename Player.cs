using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FBGFXDemo
{
    class Player
    {
        List<Piece> pieces = new List<Piece>();
        public int gold { get; set; }
        public GameBoard board;

        public Player(int startingGold)
        {
            this.gold = startingGold;
        }

        public void addPiece(Piece newPiece, GameBoard board)
        {
            pieces.Add(newPiece);
            board.generatePiece(newPiece);
        }

        public void removePiece(Piece piece)
        {
            pieces.Remove(piece);

        }
    }
}
