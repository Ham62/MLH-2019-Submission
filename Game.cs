using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using FBGFX;
using WinAPI;

namespace FBGFXDemo
{
    unsafe static partial class Program
    {
        struct Region
        {
            public int top;
            public int left;
            public int bottom;
            public int right;
            public Region(int l, int t, int r, int b)
            {
                top = t;
                left = l;
                bottom = b;
                right = r;
            }
        }

        const int SCREEN_WIDTH = 640, SCREEN_HEIGHT = 480;

        static Region boardRegion;
        static Region topRegion;
        static Region leftRegion;
        static Region rightRegion;
        static Region bottomRegion;

        static GameBoard board;
        static int boardX, boardY;

        // Run a single frame of the game's internal engine
        static void runGameTick()
        {
            checkStaticKeys();
        }

        // Single press key functions we don't want to repeat
        static int[] iKeyCoolDown = new int[byte.MaxValue];
        static void checkStaticKeys()
        {
            // Toggle showing frames
            if (Multikey(Keys.F) && iKeyCoolDown['f'] <= 0)
            {
                bShowFrameRate = !bShowFrameRate;
                iKeyCoolDown['f'] = 10; // Cooldown time of 10 frames
            }

            // Decrease cooldown for each key
            for (int i = 0; i < byte.MaxValue; i++)
            {
                if (iKeyCoolDown[i] > 0)
                    iKeyCoolDown[i]--;
            }
        }


        // Initalize the graphics resources
        static void initalize()
        {
            createFonts();

            // Calculate the gameboard size
            int boardSzX = (int)(SCREEN_WIDTH / 2);
            int boardSzY = SCREEN_WIDTH / 2;

            // Create the gameboard
            boardX = SCREEN_WIDTH / 2 - boardSzX / 2;
            boardY = (int)(SCREEN_HEIGHT / 2.5) - boardSzY / 2;
            board = new GameBoard(boardX, boardY, boardSzX, boardSzY);

            // Calculate the region bounds
            boardRegion = new Region(boardX, boardY, boardX + boardSzX, boardY + boardSzY);
            topRegion = new Region(0, 0, SCREEN_WIDTH, boardRegion.top);
            bottomRegion = new Region(0, boardRegion.bottom, SCREEN_WIDTH, SCREEN_HEIGHT);
            leftRegion = new Region(0, topRegion.bottom, boardRegion.left, bottomRegion.top);
            rightRegion = new Region(boardRegion.right, topRegion.bottom, SCREEN_WIDTH, bottomRegion.top);

            Player p1 = new Player(100);
            Player p2 = new Player(100);
            Piece p = new Piece("res/Dylan_head_small.png", 1, p1, Piece.MOVEMODE.QUEEN);
            Piece p4 = new PieceShop("res/dylan_head_small.png", 1, p1, SCREEN_WIDTH, SCREEN_HEIGHT, new Pos(0, 0), 0xFFDDE000);
            board.placePiece(0, 0, p);
            board.placePiece(0, 5, p4);
        }

        static fbgfx.Font fntTitle;
        static Label lblPlayer1;
        static Label lblPlayer2;

        static void createFonts()
        {
            fntTitle = new fbgfx.Font("Vendera", 25, 0xFFFFFFFF);
        }

        static void createLabels()
        {
            lblPlayer1 = new Label(leftRegion.left, leftRegion.top, leftRegion.right - leftRegion.left, leftRegion.bottom - leftRegion.top, "Player 1", fntTitle);
        }

        // Render a frame
        static void renderFrame()
        {
            // Set background color
            fbgfx.Rectangle(0, 0, SCREEN_WIDTH, SCREEN_HEIGHT, 0xFF0000FF, true);

            drawRegions();

            /*if (PieceShop.showWindow)
                PieceShop.renderBox();*/

            // Show framerate in top/left corner
            if (bShowFrameRate)
                fbgfx.DrawString(0, 0, iFrameRate.ToString(), 0xFFFFFFFF);
        }

        static void drawRegions()
        {
            fbgfx.DrawFont(leftRegion.left, leftRegion.top, "Player 1", fntTitle);

            fbgfx.Box(topRegion.left, topRegion.top, topRegion.right, topRegion.bottom, 0, false);
            fbgfx.Box(leftRegion.left, leftRegion.top, leftRegion.right, leftRegion.bottom, 0, false);
            fbgfx.Box(rightRegion.left, rightRegion.top, rightRegion.right, rightRegion.bottom, 0, false);
            fbgfx.Box(bottomRegion.left, bottomRegion.top, bottomRegion.right, bottomRegion.bottom, 0, false);
        }
    }
}
