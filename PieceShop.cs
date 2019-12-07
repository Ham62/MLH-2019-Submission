using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FBGFX;

namespace FBGFXDemo
{
    unsafe class PieceShop : Piece
    {
		private class shopItem
		{
			public Piece item;
			public GameButton button;
		}

		readonly shopItem[] shopItems = new shopItem[9];//6 items in chess

		const int EXIT = 7, BUY = 8;//pos in shopItems


		public readonly int menuWidth, menuHeight;
		public readonly Pos menuPosition;
		public readonly uint menuColor;
		private fbgfx.IMAGE* menu;
		public bool isShowing = false;


		public PieceShop(String imgPath, int cost, Player owner, int menuWidth, int menuHeight, Pos menuPosition,uint menuColor) : base(imgPath,cost,owner,MOVEMODE.NONE)
        {
			this.menuColor = menuColor;
			this.menuWidth = menuWidth;
			this.menuHeight = menuHeight;
			this.menuPosition = menuPosition;

			//init buttons
			for(int i =0; i < shopItems.Length; i++)
			{
				shopItems[i] = new shopItem();
				shopItems[i].button = new GameButton(0,0,0,0);
			}

			shopItems[0].item = new Piece("shop.png", 2, owner, MOVEMODE.NONE);
			shopItems[1].item = new Piece("pawn.png", 1, owner, MOVEMODE.PAWN);
			shopItems[2].item = new Piece("knight.png",3,owner,MOVEMODE.KNIGHT);
			shopItems[3].item = new Piece("king.png",4,owner,MOVEMODE.KING);
			shopItems[4].item = new Piece("bishop.png",10,owner,MOVEMODE.BISHOP);
			shopItems[5].item = new Piece("rook.png", 10,owner,MOVEMODE.ROOK);
			shopItems[6].item = new Piece("queen.png",15,owner,MOVEMODE.QUEEN);
			shopItems[EXIT].item = new Piece("",0,null,MOVEMODE.NONE);
			shopItems[BUY].item = new Piece("", 0, null, MOVEMODE.NONE);
			updateMenuImage();

			
		}

		private void updateMenuImage()
		{
			menu = fbgfx.ImageCreate(menuWidth, menuHeight, menuColor);

			int btnScale = 3;

			int buttonSize = menuHeight / shopItems.Length;

			for(int i =0; i < shopItems.Length; i++)
			{
				if(shopItems[i].item.imgPath != "")
				{
					shopItems[i].button.x = 0;
					shopItems[i].button.y = i * buttonSize;
					shopItems[i].button.height = buttonSize;
					shopItems[i].button.width = buttonSize* btnScale;

					String imgName = shopItems[i].item.imgPath.Substring(0, shopItems[i].item.imgPath.Length - 4);
					shopItems[i].button.caption = imgName +" - $" + shopItems[i].item.cost;

				}
			}

			int optBtnSize = shopItems[0].button.width/2;//(shopItems.Length-2)*buttonSize

			shopItems[EXIT].button = new GameButton(0, (shopItems.Length - 2) * buttonSize, optBtnSize, menuHeight- (shopItems.Length - 2) * buttonSize);
			shopItems[EXIT].button.caption = "EXIT";
			shopItems[BUY].button = new GameButton(optBtnSize, (shopItems.Length - 2) * buttonSize, optBtnSize, menuHeight - (shopItems.Length - 2) * buttonSize);
			shopItems[BUY].button.caption = "BUY";
		}

		public void renderBox()
		{
			fbgfx.Put(menuPosition.x, menuPosition.y, menu);
		}


		public void show()
		{
			for (int i = 0; i < shopItems.Length; i++)
			{
				WindowControls.Register(shopItems[i].button);
			}
		}

        public override void pieceFunction(GameBoard board)
        {
			isShowing = true;
			board.enableBoard(false);
			show();

        }

		private void menuFunction(shopItem item)
		{

		}

		private void exitFunction(GameButton btn)
		{
			isShowing = false;

			for (int i = 0; i < shopItems.Length; i++)
			{
				WindowControls.Unregister(shopItems[i].button);
			}
		}
    }
}
