using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FBGFXDemo
{
	class GameBoard
	{
		public readonly int TILES_WIDE = 16, TILES_HIGH = 16;

		int boardWidth = 0, boardHeight = 0;
		int boardX, boardY;
		int tileSzX, tileSzY;

		public Tile selectedTile; // Current highlighted piece to move to
		Tile[,] gameBoard;

		private GameBoard() { }
		public GameBoard(int x, int y, int iWid, int iHei)
		{
			boardX = x;
			boardY = y;
			boardWidth = iWid;
			boardHeight = iHei;
			tileSzX = iWid / TILES_WIDE;
			tileSzY = iHei / TILES_HIGH;

			gameBoard = new Tile[TILES_WIDE, TILES_HIGH];
			for (int iX = 0; iX < TILES_WIDE; iX++)
			{
				for (int iY = 0; iY < TILES_HIGH; iY++)
				{
					gameBoard[iX, iY] = new Tile(x + iX * tileSzX, y + iY * tileSzY,
												 tileSzX, tileSzY, this,
												 generateStyle(((iX + iY) & 1) != 0));
					gameBoard[iX, iY].stretchBG = true;
				}
			}
		}


		ButtonStyle generateStyle(bool isEvenEven)
		{
			ButtonStyle bs = new ButtonStyle();
			bs.button3DStyle = false;

			bs.borderWidth = 1;

			// even x & y is lighter than tiles that have an odd one
			if (isEvenEven)
				bs.bgColor = 0xFF00FF21;
			else
				bs.bgColor = 0xFF007F0E;

			return bs;
		}

		public void movePieceTo(Tile destination)
		{
			//checks to see if piece is occupied
			if(destination.piece != null)
			{
				Player temp = destination.piece.owner;//used to remove from player array
				temp.removePiece(destination.piece);
			}

			// Move piece to destination
			destination.piece = selectedTile.piece;
			destination.imgPath = selectedTile.imgPath;

			// Remove piece from currently selected tile
			selectedTile.imgPath = " ";
			selectedTile.piece = null;
			selectedTile.isSelected = false;

			// Unselect tile and clear highlights
			selectedTile = null;
			clearMoveflags();
		}

		// Return tile from x/y on board, return false else
		public Tile tileAt(int iX, int iY)
		{
			if (iX >= 0 && iX < TILES_WIDE && iY >= 0 && iY < TILES_HIGH)
				return gameBoard[iX, iY];

			return null;
		}

		// Clear all the movement flags on the tiles
		public void clearMoveflags()
		{
			for (int iX = 0; iX < TILES_WIDE; iX++)
			{
				for (int iY = 0; iY < TILES_HIGH; iY++)
				{
					gameBoard[iX, iY].canMove = false;
				}
			}
		}

		public void enableBoard(bool isEnabled)
		{
			for (int iX = 0; iX < TILES_WIDE; iX++)
			{
				for (int iY = 0; iY < TILES_HIGH; iY++)
				{
					if (isEnabled)
						WindowControls.Register(gameBoard[iX, iY]);
					else
						WindowControls.Unregister(gameBoard[iX, iY]);
				}
			}
		}

		public GameBoard getOpponentBoard()
		{
			GameBoard opponentBoard = new GameBoard(this.boardX,this.boardY,this.boardWidth,this.boardHeight);

			for (int iX = 0; iX < TILES_WIDE; iX++)
				for (int iY = 0; iY < TILES_HIGH; iY++)
					opponentBoard.gameBoard[iX, iY] = gameBoard[TILES_WIDE - iX - 1, TILES_HIGH - iY - 1];

			return opponentBoard;
		}

		public void placePiece(int iX, int iY, Piece piece)
		{
			gameBoard[iX, iY].placePiece(piece);
		}

		public Pos getCoord(Tile tile)
		{
			for (int iX = 0; iX < TILES_WIDE; iX++)
			{
				for (int iY = 0; iY < TILES_HIGH; iY++)
				{
					if (gameBoard[iX, iY] == tile)
						return new Pos(iX, iY);
				}
			}
			return null;
		}
	}
}
