using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FBGFXDemo
{
    class PieceShop : Piece
    {
        //(String imgPath, int cost, Pos movePath, Pos xy, Player owner)
        public PieceShop(String imgPath, int cost, Pos xy, Player owner) : base(imgPath,cost,xy,owner,MOVEMODE.NONE)
        {

        }

        public override void pieceFunction()
        {
            //prompt user shop to buy piece
        }
    }
}
