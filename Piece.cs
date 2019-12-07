using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FBGFXDemo
{
	abstract class Piece
	{
		public enum MOVEMODE
		{
			PAWN=0,//only infront
			KNIGHT=1,
			KING=2,//only around
			ROOK=3,//max horz and vert
			BISHOP=4,//max diag
			QUEEN=5,//combine both ROOK and BISHOP
			NONE=6,//no movement
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
        public Pos position { get; set; }
        String imgPath = "";
        private MOVEMODE moveMode;



        public Piece(String imgPath, int cost, Pos xy, Player owner, MOVEMODE moveMode)
        {
            this.imgPath = imgPath;
            this.cost = cost;
            this.position = xy;
            this.owner = owner;
            this.moveMode = moveMode;
        }

        //default function to move to given x, y. When calling this function you determine the possible moves based off of movePath
        public Boolean move(GameBoard board,int newX, int newY)
        {
            if(newX < board.width && newX >= 0 && newY <= board.width && newY >=0)
            {
                position.x = newX;
                position.y = newY;

                board.tileAt(newX, newY).invade(this);//updates the tile(handles piece moving ex remove old updating tile)

                return true;
            }
            return false;
        }


        public virtual void pieceFunction(GameBoard board)
        {
			//highlight posible moves(calling button.highlight)
			//prompt user to click on possible piece(clicking on tile returns position)
			//if possible piece, move piece
			//moving this piece will call a made up function "invade" what handles updating the tile
			/*
             1.hight possible moves
             2.prompt user for click
             3.if possible move piece
             */
			showPossibleMoves(board);

        }

		//private helper used to show possible moves, checks to see if new move is valid, if so set that valid tile to canMove
        private void possibleMove(GameBoard board, int plusX, int plusY)
        {
            Pos newPosition = board.tileAt(position.x+plusX, position.y + plusY).piece.position;
            if (newPosition != null)//newPosition is set to null
                board.tileAt(newPosition.x, newPosition.y).canMove = true;
        }

        //hard coded for now, change back to "plus" array for each piece, then loop thru arr
        private void showPossibleMoves(GameBoard board)
        {
			if(moveMode <= MOVEMODE.KING)
			{
				for(int i =0; i < MOVE_LIST[(int)moveMode].Length; i++)
				{
					int plusX = MOVE_LIST[(int)moveMode][i].x;
					int plusY = MOVE_LIST[(int)moveMode][i].y;
					possibleMove(board, plusX, plusY);
				}
			}
			else//else piece is "special" global moving piece 2 ifs because queen = bishop+rook
			{
				if(moveMode == MOVEMODE.BISHOP || moveMode == MOVEMODE.QUEEN)
				{
					for(int i =1; i <= (board.width+board.height)/2;i++)
					{
						possibleMove(board, i, i);
						possibleMove(board, -i, -i);
						possibleMove(board, -i, i);
						possibleMove(board, i, -i);
					}
				}

				if(moveMode == MOVEMODE.ROOK || moveMode == MOVEMODE.QUEEN)
				{
					for (int i = 1; i <= (board.width + board.height) / 2; i++)
					{
						possibleMove(board, 0, i);
						possibleMove(board, 0, -i);
						possibleMove(board, -i, 0);
						possibleMove(board, i, -0);

					}
				}
			}
        }


        


        



    }
}
