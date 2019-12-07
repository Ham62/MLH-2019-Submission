using FBGFX;
using System;

namespace FBGFXDemo
{
	public class ButtonStyle
	{
		public uint bgColor;
		public uint borderColor;
		public int borderWidth;
		public bool button3DStyle = true;

		public fbgfx.Font font;
		public bool fontAutoSize = true;

		public ButtonStyle(fbgfx.Font font, uint bgColor, uint borderColor, int borderWidth, bool fontAutoSize)
		{
			this.font = font;
			this.bgColor = bgColor;
			this.borderColor = borderColor;
			this.borderWidth = borderWidth;
			this.fontAutoSize = fontAutoSize;
		}

		public ButtonStyle() : this(new fbgfx.Font("Arial", 10, 0xFFFFFFFF), 0xFF828282, 0xFFFFFFFF,2,true)
		{

		}

		public ButtonStyle(uint bgColor) : this(new fbgfx.Font("Arial", 10, 0xFFFFFFFF),bgColor, 0xFFFFFFFF, 2, true)
		{

		}

		public ButtonStyle clone()
		{
			return new ButtonStyle(font, bgColor, borderColor, borderWidth, fontAutoSize);
		}
	}

	public unsafe class GameButton
	{
		private fbgfx.IMAGE* imgRendered;
		private fbgfx.IMAGE* imgFade;

		public int x, y;

		protected int _width, _height;
		public int width { get { return _width; } set { _width = value; renderButton(); } }
		public int height { get { return _height; } set { _height = value; renderButton(); } }

		private string _caption;
		public string caption { get { return _caption; } set { _caption = value; renderButton(); } }

		public int clickCooldown = 0; // minimum cool down time, adds delay before renenabling
		public bool isEnabled = true;

		/*Added Code, allows for changing the "highlight properties" (ex: when clicked/enabled). 
		 and added feature to turn on highlighting without havning to disable the button, Remove comment later*/
		public bool highlight = false;

		//sanitises value
		private int _highlightAlpha;
		public int highlightAlpha { get { return _highlightAlpha; }
			set {
				if(value > 255)
					_highlightAlpha = 255;
				else if(value < 0)
					_highlightAlpha = 0;
				else
					_highlightAlpha = value;
			}
		}

		private uint _highlightColor;//updates the imgFade when value is changed
		public uint highlightColor { 
            get {return _highlightColor; } 
            set {
                _highlightColor = value;
                if (imgFade == null)
                    fbgfx.ImageDestroy(imgFade);
                imgFade = fbgfx.ImageCreate(width, height, _highlightColor); } 
        }
		/*--------------------------------------------*/

		private ButtonStyle _style;
		public ButtonStyle style { get { renderButton(); return _style; } set { _style = value; renderButton(); } }

		public Action<GameButton> clickedAction = null;
		private int coolDownCount = 0;

		private fbgfx.IMAGE* imgBgn;
		private string _imgPath = " ";
		public string imgPath { get {return _imgPath; } set {_imgPath = value;setImage(value); } }


        private bool _stretchBG = false;
        public bool stretchBG { get { return _stretchBG; } set { _stretchBG = value; renderButton(); } }


		public GameButton(int x_, int y_, int width_, int height_, string text, ButtonStyle bs, Action<GameButton> btnClicked)
		{
			this.x = x_; this.y = y_;
			_width = width_;
			_height = height_;
			_caption = text;
			_style = bs;
			clickedAction = btnClicked;

			_highlightColor = 0xA07F7F7F;//default
			_highlightAlpha = 128;//default

			imgBgn = fbgfx.ImageCreate(_width, _height, fbgfx.RGB(0xFF, 0, 0xFF));

			renderButton();
		}

		public GameButton(int x_, int y_, int width_, int height_, string text, ButtonStyle bs) : this(x_, y_, width_, height_, text, bs, null){}
		public GameButton(int x_, int y_, int width_, int height_, string text, Action<GameButton> btnClicked) : this(x_, y_, width_, height_, text, new ButtonStyle(), btnClicked){}
		public GameButton(int x_, int y_, int width_, int height_, string text) : this(x_, y_, width_, height_, text, new ButtonStyle(), null){}
		public GameButton(int x_, int y_, int width_, int height_) : this(x_, y_, width_, height_, " ", new ButtonStyle(), null) { }

		public WindowControls.Rect getRect()
		{
			return new WindowControls.Rect(x, y, width, height);
		}

		public void clicked()
		{
			if(coolDownCount > 0)
				return;

			if(isEnabled && clickedAction != null) {
				coolDownCount = clickCooldown;
				clickedAction(this);
				isEnabled = false;
			}
		}

		public void runCoolDown()
		{
			if(coolDownCount > 0)
				coolDownCount--;
			else
				isEnabled = true;
		}

		public void drawButton()
		{
			fbgfx.Put(x, y, imgRendered);

			if(!isEnabled || highlight)
				fbgfx.Put(x, y, imgFade, fbgfx.DrawMode.ALPHA, highlightAlpha);

		}

		protected void renderButton()
		{
			// Check if we need to re-size the image
			if(imgRendered != null) {
				if(imgRendered->width != width || imgRendered->height != height) {
					// to avoid memory leaks
					fbgfx.ImageDestroy(imgRendered);
					imgRendered = fbgfx.ImageCreate(width, height, _style.bgColor);
					imgFade = fbgfx.ImageCreate(width, height, _highlightColor);
					updateImage(imgPath);
				}
			}
			else // Image doesn't exist, create it
			{
				imgRendered = fbgfx.ImageCreate(width, height, _style.bgColor);
				imgFade = fbgfx.ImageCreate(width, height, _highlightColor);
				updateImage(imgPath);
			}

			// Fill background color
			fbgfx.Rectangle(imgRendered, 0, 0, width, height, _style.bgColor, true);

			
			drawImage();
			if(caption != "")
				drawFont();
			drawBorder();
		}

		private void drawImage()
		{
			updateImage(imgPath);
            if (imgPath[imgPath.Length - 1] == 'g' || imgPath[imgPath.Length - 1] == 'G')
                if (_stretchBG)
                    fbgfx.ScalePutA(imgRendered, imgBgn, width / 2, height / 2, width, height);
                else
                    fbgfx.RotoZoomA(imgRendered, imgBgn, width / 2, height / 2, 0, 1, 1);
            else
                fbgfx.Put(imgRendered, 0, 0, imgBgn, fbgfx.DrawMode.TRANS);
		}

		private void setImage(String sBitmap)
		{
			updateImage(sBitmap);

			renderButton();
		}

		private void updateImage(String sBitmap)
		{
            bool isPng;

            // check if we need to delete old image
            if (imgBgn != null)
            {
                isPng = imgPath[imgPath.Length - 1] == 'g' || imgPath[imgPath.Length - 1] == 'G';
                if (isPng)
                    fbgfx.DestroyPNG(imgBgn);
                else
                    fbgfx.ImageDestroy(imgBgn);
            }

            isPng = sBitmap[sBitmap.Length - 1] == 'g' || imgPath[sBitmap.Length - 1] == 'G';

            if (isPng)
            {
                imgBgn = fbgfx.LoadPNG(sBitmap);
            }
            else
            {
                imgBgn = fbgfx.ImageCreate(width, height, fbgfx.RGB(0xFF, 0, 0xFF));
                fbgfx.LoadBitmap(sBitmap, imgBgn);
            }
		}

		public void destroy()
		{
			fbgfx.ImageDestroy(imgRendered);
			fbgfx.ImageDestroy(imgBgn);
			fbgfx.ImageDestroy(imgFade);
		}

		private void drawFont()
		{
			// Font size
			int iSzX = 0, iSzY = 0;

			// auto size font?
			if(_style.fontAutoSize) {
				_style.font.size = 0;
				do {
					_style.font.size++;
					fbgfx.CalcTextSize(caption, _style.font, out iSzX, out iSzY);
				} while(iSzX < width - ((_style.borderWidth + 4) * 2) &&
						 iSzY < height - ((_style.borderWidth + 4) * 2));
				_style.font.size--;
			}

			// Center text in middle of button
			fbgfx.CalcTextSize(caption, _style.font, out iSzX, out iSzY);
			int fontX = (width / 2) - (iSzX / 2);
			int fontY = (int)((height / 2) - (iSzY / 2));

			fbgfx.DrawFont(imgRendered, fontX, fontY, caption, _style.font);
		}

		private void drawBorder()
		{
			// Draw a border
			if(_style.button3DStyle) {
				uint clrUpper = 0xFF929292;
				uint clrLower = 0xFF717171;

				for(int i = 0; i < _style.borderWidth; i++) {
					fbgfx.Line(imgRendered, 0, i, width - i - 1, i, clrUpper);
					fbgfx.Line(imgRendered, i, 0, i, height - i - 1, clrUpper);

					int wid = width - i - 1, hei = height - i - 1;
					fbgfx.Line(imgRendered, i, hei, wid, hei, clrLower);
					fbgfx.Line(imgRendered, wid, i, wid, hei, clrLower);
				}
			}
			// 2D border
			else {
				for(int i = 0; i < _style.borderWidth; i++)
					fbgfx.Rectangle(imgRendered, i, i, width - 1 - i * 2, height - 1 - i * 2, 0, false);
			}
		}
	}

    public class Label : GameButton
    {
        public Label(int iX, int iY, int iWid, int iHei, string sCaption, fbgfx.Font fnt)
            : base(iX, iY, iWid, iHei, sCaption)
        {
            style = new ButtonStyle(fnt, 0, 0, 0, true);
        }
    }
}