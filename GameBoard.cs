using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FBGFXDemo
{
    class GameBoard
    {
        public int width, height;

        public Tile tileAt(int x, int y)
        {
            return new Tile();
        }

        public void generatePiece(Piece newPiece)
        {
            /*
             calls the "inavade" function at tile closest to newPieces Owner
             
             */
        }

        public bool inBounds()
        {
            return true;
        }

    }
}
