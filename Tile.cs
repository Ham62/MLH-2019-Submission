using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FBGFXDemo
{
    class Tile : GameButton
    {
        GameBoard ownerBoard;

        public bool isSelected = false;

        private bool _canMove = false;
        public bool canMove { 
            get { return _canMove; }
            set { 
                _canMove = value;
                highlight = value;
                renderButton();
            } 
        }

        public Piece piece;

        public Tile(int iX, int iY, int iWid, int iHei, GameBoard board, ButtonStyle tileStyle) : base(iX, iY, iWid, iHei, " ", tileStyle) {
            ownerBoard = board;
            highlightAlpha = 128;
            highlightColor = 0xFFFFFFFF;

            clickedAction = tileClicked;
            WindowControls.Register(this);
        }

        private void tileClicked(GameButton tileID) {
            // if you click the selected piece it unselects itself
            if (isSelected)
            {
                isSelected = false;
                ownerBoard.clearMoveflags();
                ownerBoard.selectedTile = null;
                return;
            }

            if (ownerBoard.selectedTile == null)
            {
                if (piece != null)
                {
                    isSelected = true;
                    highlight = true;
                    ownerBoard.selectedTile = this;
                    piece.pieceFunction(ownerBoard, this);
                }
            }
            else
            {
                if (_canMove)
                {
                    ownerBoard.movePieceTo(this);
                }
            }
        }

        public void placePiece(Piece newPiece)
        {
            piece = newPiece;
            imgPath = piece.imgPath;
            Console.WriteLine(imgPath);
            
        }
    }
}
