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
		public bool isAltPlayer = false;
        //private GameBoard board;

        public Player(int startingGold)
        {
            this.gold = startingGold;
        }


        public void addPiece(int x, int y,Piece newPiece, GameBoard board)
        {
			if(isAltPlayer)//if its alt player flip board to place the piece
				board.getOpponentBoard().placePiece(x, y, newPiece);
			else
				board.placePiece(x,y,newPiece);
            pieces.Add(newPiece);
        }


        public void removePiece(Piece piece)
        {
            pieces.Remove(piece);

        }

		public PieceShop getShop()
		{
			return (PieceShop)pieces[0];
		}
    }
}
