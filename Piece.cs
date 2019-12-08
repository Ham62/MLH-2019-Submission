using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FBGFXDemo
{
	class Piece
	{
		public enum MOVEMODE
		{
			PAWN = 0,//only infront
			KNIGHT = 1,
			KING = 2,//only around
			ROOK = 3,//max horz and vert
			BISHOP = 4,//max diag
			QUEEN = 5,//combine both ROOK and BISHOP
			NONE = 6,//no movement
		};
		public static Pos[][] MOVE_LIST = new Pos[][]
		{
			new Pos[]{new Pos(0,1),new Pos(0,2) },//pawn
			new Pos[]{new Pos(-1,2),new Pos(-2,1),new Pos(1,2),new Pos(2,1),new Pos(2,-1),new Pos(1,-2),new Pos(-2,-1),new Pos(-1,-2)},//knight
			new Pos[]{new Pos(-1,1),new Pos(0,1),new Pos(1,1),new Pos(1,0),new Pos(1,-1),new Pos(0,-1),new Pos(-1,-1),new Pos(-1,0)},//king
			new Pos[]{},//rook, special case(global)
			new Pos[]{},//bishop, special case(global)
			new Pos[]{},//Queen, special case(global)
			new Pos[]{},//none
		};



		public Player owner { get; set; }
		public int cost { get; set; }
		public readonly String imgPath = "";
		private MOVEMODE moveMode;



		public Piece(String imgPath, int cost, Player owner, MOVEMODE moveMode)
		{
			this.imgPath = imgPath;
			this.cost = cost;
			this.owner = owner;
			this.moveMode = moveMode;
		}


		public virtual void pieceFunction(GameBoard board, Tile owner)
		{
			Pos coord = board.getCoord(owner);
			showPossibleMoves(board, coord);

		}

		//private helper used to show possible moves, checks to see if new move is valid, if so set that valid tile to canMove
		// returns false if we hit a piece going that direction and can't move past
		private bool possibleMove(GameBoard board, Pos position, int plusX, int plusY)
		{
			Tile tile = board.tileAt(position.x + plusX, position.y + plusY);
			if (tile != null)
			{
				if (tile.piece != null)
				{
					// If it's not your piece you can take it over, otherwise can't move there
					if (tile.piece.owner != owner)
						tile.canMove = true;

					return false;
				}
				tile.canMove = true;
				return true;
			}
			return false;
		}

		//hard coded for now, change back to "plus" array for each piece, then loop thru arr
		private void showPossibleMoves(GameBoard board, Pos pos)
		{
			if (moveMode <= MOVEMODE.KING)
			{
				for (int i = 0; i < MOVE_LIST[(int)moveMode].Length; i++)
				{
					int plusX = MOVE_LIST[(int)moveMode][i].x;
					int plusY = MOVE_LIST[(int)moveMode][i].y;
					possibleMove(board, pos, plusX, plusY);
				}
			}
			else//else piece is "special" global moving piece 2 ifs because queen = bishop+rook
			{
				if (moveMode == MOVEMODE.BISHOP || moveMode == MOVEMODE.QUEEN)
				{
					bool[] moves = { true, true, true, true };
					for (int i = 1; i <= (board.TILES_WIDE + board.TILES_HIGH) / 2; i++)
					{
						if (moves[0])
							moves[0] = possibleMove(board, pos, i, i);
						if (moves[1])
							moves[1] = possibleMove(board, pos, -i, -i);
						if (moves[2])
							moves[2] = possibleMove(board, pos, -i, i);
						if (moves[3])
							moves[3] = possibleMove(board, pos, i, -i);
					}
				}

				if (moveMode == MOVEMODE.ROOK || moveMode == MOVEMODE.QUEEN)
				{
					bool[] moves = { true, true, true, true };
					for (int i = 1; i <= (board.TILES_WIDE + board.TILES_HIGH) / 2; i++)
					{
						if (moves[0])
							moves[0] = possibleMove(board, pos, 0, i);
						if (moves[1])
							moves[1] = possibleMove(board, pos, 0, -i);
						if (moves[2])
							moves[2] = possibleMove(board, pos, -i, 0);
						if (moves[3])
							moves[3] = possibleMove(board, pos, i, 0);
					}
				}
			}
		}









	}
}
