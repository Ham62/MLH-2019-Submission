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
        static fbgfx.IMAGE*[] images = new fbgfx.IMAGE*[9];

		public readonly int menuWidth, menuHeight;
		public readonly Pos menuPosition;
        Region menuArea;
		public readonly uint menuColor;
		private fbgfx.IMAGE* menu;
		public bool isShowing = false;
        int selectedItem;

        const int MB_SHOP = 0,
                  MB_PAWN = 1,
                  MB_KNIGHT = 2,
                  MB_KING = 3,
                  MB_BISHOP = 4,
                  MB_ROOK = 5,
                  MB_QUEEN = 6,
                  MB_EXIT = 7,
                  MB_BUY = 8;

		public PieceShop(String imgPath, int cost, Player owner, int menuWidth, int menuHeight, Pos menuPosition,uint menuColor) : base(imgPath,cost,owner,MOVEMODE.NONE)
        {
			this.menuColor = menuColor;
			this.menuWidth = menuWidth;
			this.menuHeight = menuHeight;
			this.menuPosition = menuPosition;
            menuArea = new Region(menuPosition.x, menuPosition.y, 
                                  menuPosition.x + menuWidth, 
                                  menuPosition.y + menuHeight);

			//init buttons
			for(int i =0; i < shopItems.Length; i++)
			{
				shopItems[i] = new shopItem();
				shopItems[i].button = new GameButton(0,0,0,0);
			}

			shopItems[MB_SHOP].item = new Piece("res/shop.png", 2, owner, MOVEMODE.NONE);
            shopItems[MB_PAWN].item = new Piece("res/pawn.png", 1, owner, MOVEMODE.PAWN);
            shopItems[MB_KNIGHT].item = new Piece("res/knight.png", 3, owner, MOVEMODE.KNIGHT);
            shopItems[MB_KING].item = new Piece("res/king.png", 4, owner, MOVEMODE.KING);
            shopItems[MB_BISHOP].item = new Piece("res/bishop.png", 10, owner, MOVEMODE.BISHOP);
            shopItems[MB_ROOK].item = new Piece("res/rook.png", 10, owner, MOVEMODE.ROOK);
            shopItems[MB_QUEEN].item = new Piece("res/queen.png", 15, owner, MOVEMODE.QUEEN);


            for (int i = 0; i < shopItems.Length; i++)
            {
                if (shopItems[i].item != null)
                {
                    shopItems[i].button.clickedAction = itemButtonClicked;
                    images[i] = fbgfx.LoadPNG(shopItems[i].item.imgPath);
                }
            }

                updateMenuImage();
			shopItems[MB_EXIT].button.clickedAction = exitFunction;
            shopItems[MB_BUY].button.clickedAction = buyClicked;

		}

		private void updateMenuImage()
		{
			menu = fbgfx.ImageCreate(menuWidth, menuHeight, menuColor);

			int btnScale = 3;

			int buttonSize = menuHeight / shopItems.Length;

			for(int i =0; i < shopItems.Length; i++)
			{
				if(shopItems[i].item != null && shopItems[i].item.imgPath != "")
				{
					shopItems[i].button.x = 0;
					shopItems[i].button.y = i * buttonSize;
					shopItems[i].button.height = buttonSize;
					shopItems[i].button.width = buttonSize* btnScale;

					String imgName = shopItems[i].item.imgPath.Substring(4, shopItems[i].item.imgPath.Length - 8);
					shopItems[i].button.caption = imgName +" - $" + shopItems[i].item.cost;

				}
			}

			int optBtnSize = shopItems[0].button.width/2;//(shopItems.Length-2)*buttonSize

			shopItems[MB_EXIT].button = new GameButton(0, (shopItems.Length - 2) * buttonSize, optBtnSize, menuHeight- (shopItems.Length - 2) * buttonSize);
			shopItems[MB_EXIT].button.caption = "EXIT";
			shopItems[MB_BUY].button = new GameButton(optBtnSize, (shopItems.Length - 2) * buttonSize, optBtnSize, menuHeight - (shopItems.Length - 2) * buttonSize);
			shopItems[MB_BUY].button.caption = "BUY";
		}

		public void renderWindow()
		{
			fbgfx.Put(menuPosition.x, menuPosition.y, menu);

            if (images[selectedItem] != null)
            {
                int imgSz = menuWidth / 3;
                fbgfx.ScalePutA(images[selectedItem], 
                                menuArea.right - menuWidth / 3, 
                                menuArea.top + menuHeight / 2,
                                imgSz, imgSz);
            }
		}


		public void show()
		{
			for (int i = 0; i < shopItems.Length; i++)
			{
				WindowControls.Register(shopItems[i].button);
			}
		}

        

		GameBoard tempBoard;
        Tile tempTile;
        public override void pieceFunction(GameBoard board, Tile owner)
        {
			tempBoard = board;
            tempTile = owner;
			isShowing = true;
			tempBoard.enableBoard(false);
			show();

        }

		private void menuFunction(shopItem item)
		{

		}

		private void exitFunction(GameButton btn)
		{
            tempBoard.clearMoveflags();
            tempBoard.selectedTile = null;
            tempTile.isSelected = false;

            terminateWindow();
		}

        private void itemButtonClicked(GameButton btnClicked)
        {
            for (int i = 0; i < shopItems.Length; i++)
            {
                if (shopItems[i].button == btnClicked)
                {
                    selectedItem = i;
                    return;
                }
            }
        }

        Tile newTile;
        private void buyClicked(GameButton btnClicked)
        {
            // if have enough gold
            newTile = new Tile(-1, -1, 0, 0, tempBoard, new ButtonStyle());
            tempBoard.selectedTile = newTile;
            newTile.piece = shopItems[selectedItem].item;
            newTile.imgPath = newTile.piece.imgPath;
            
            Pos pos = tempBoard.getCoord(tempTile);
            for (int i = 0; i < tempBoard.TILES_WIDE; i++)
            {
                Tile tmp = tempBoard.tileAt(i, pos.y);
                if (tmp.piece == null)
                    tmp.canMove = true;
            }
            
            terminateWindow();
        }

        private void terminateWindow()
        {
            for (int i = 0; i < shopItems.Length; i++)
            {
                WindowControls.Unregister(shopItems[i].button);
            }

            isShowing = false;
            tempBoard.enableBoard(true);

            for (int i = 0; i < shopItems.Length; i++)
            {
                if (images[i] != null)
                {
                    fbgfx.DestroyPNG(images[i]);
                }
            }
        }
    }
}