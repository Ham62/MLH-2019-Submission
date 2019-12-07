' FBGFX Font Render by Mysoft

'' DrawFont([BUFFER],[POSX],[POSY],"STRING","Font Name",FontSize,[Color],[Style],[Charset])

'' this define allow you to set some effect while using anti-alias/blur (1 to 1.6)
#define GAMMA 1.5

#include "windows.bi"
#include "fbgfx.bi"

enum Charsets 'already denfined
  NULL_CHARSET = 0
  'ANSI_CHARSET
  'DEFAULT_CHARSET
  'SYMBOL_CHARSET
  'SHIFTJIS_CHARSET
  'GB2312_CHARSET
  'HANGEUL_CHARSET
  'CHINESEBIG5_CHARSET
  'OEM_CHARSET
end enum

enum TextStyle
  FS_BOLD = 2
  FS_ITALIC = 4 
  FS_ANTIALIAS = 8
  FS_BLUR = 16+8
end enum

declare sub DrawFont(BUFFER as any ptr=0,POSX as integer, POSY as integer, _
FTEXT as string, FNAME as string,FSIZE as integer, _
FCOLOR as uinteger=rgba(255,255,255,0), FSTYLE as integer=0, CHARSET as integer=DEFAULT_CHARSET)
declare sub DrawFontW(BUFFER as any ptr=0,POSX as integer, POSY as integer, _
FTEXT as wstring, FNAME as string,FSIZE as integer, _
FCOLOR as uinteger=rgba(255,255,255,0), FSTYLE as integer=0, CHARSET as integer=DEFAULT_CHARSET)


' *************************************************************
' ************************ EXAMPLE ****************************
' *************************************************************
#if 0
screenres 640,480,32,5
if screenptr=0 then end

for iY as integer = -1 to 1
  for iX as integer = -1 to 1
    if (iX or iY) then DrawFont(,30+iX,4+iY,"Testing ITALIC","Times New Roman",32,rgb(85,85,255),FS_ITALIC)    
  next iX
next iY

scope
  var pTemp = ImageCreate(256,38,rgb(255,255,255))
  for iY as integer = -1 to 1
    for iX as integer = -1 to 1
      if (iX or iY) then DrawFont(pTemp,iX,iY,"Testing Multiple","Arial",24,rgb(0,0,0),FS_BOLD or FS_ANTIALIAS)
    next iX  
  next iY
  DrawFont(pTemp,0,0,"Testing Multiple","Arial",24,rgb(255,255,255),FS_BOLD or FS_ANTIALIAS)  
  for N as integer = 0 to 25 step 1
    DrawFont(,320+N,N,"Testing Multiple","Arial",24,rgb(128+N*5,128,255-N*5),FS_BOLD)
  next N
  put (345,25),pTemp,and
  ImageDestroy(pTemp)
end scope

DrawFont(,30,4,"Testing ITALIC","Times New Roman",32,rgb(0,0,128),FS_ITALIC)
DrawFont(,110,360,"Microsoft","Tahoma",64,rgb(255,255,255),FS_BOLD or FS_ANTIALIAS)
DrawFont(,5,60,"Testing heuaheuea","Script",50,rgb(85,255,85))
DrawFont(,325,60,"Testing heuaheuea","Script",50,rgb(85,255,85),FS_ANTIALIAS)
DrawFont(,30,125,"¯±ÈÞ","Webdings",100,rgb(32,64,255),FS_BLUR)
DrawFontW(,-8,240,wstr(!"\u2654\u2655\u2656\u2657\u2658\u2659\u2603"),"Segoe UI Symbol",72,rgb(128,64,255))

sleep
#endif
' ***********************************************************************
' ***********************************************************************
' ***********************************************************************

#macro DrawFontBody( _WorA_ ) 
  ' allocating as static for speed
  static FINIT as integer
  static as hdc THEDC
  static as hbitmap THEBMP
  static as any ptr THEPTR
  static as fb.image ptr FBBLK  
  static as integer TXTSZ,COUNT,RESU,RESUU
  static as any ptr SRCBUF,DSTBUF
  static as hfont THEFONT
  static as integer FW,FI,TXYY,FCOR
  static DSKWND as hwnd, DSKDC as hdc
  static MYBMPINFO as BITMAPINFO
  static as TEXTMETRIC MYTXINFO
  static as SIZE TXTSIZE
  static as RECT RCT
 
  #define FontSize(PointSize) -MulDiv(PointSize, GetDeviceCaps(THEDC, LOGPIXELSY), 72) 
 
  if FINIT = 0 then   
    '' allocating things and starting the "engine"
    FINIT = 1   
    with MYBMPINFO.bmiheader
      .biSize = sizeof(BITMAPINFOHEADER)
      .biWidth = 2048
      .biHeight = -513
      .biPlanes = 1
      .biBitCount = 32
      .biCompression = BI_RGB
    end with   
    ' creating a DC and a bitmap that will receive the rendered font
    DSKWND = GetDesktopWindow()
    DSKDC = GetDC(DSKWND)
    THEDC = CreateCompatibleDC(DSKDC)
    THEBMP = CreateDIBSection(THEDC,@MYBMPINFO,DIB_RGB_COLORS,@THEPTR,null,null)   
    ReleaseDC(DSKWND,DSKDC)   
  end if
 
  ' creating the font
  if (FSTYLE and FS_BOLD) then FW = FW_BOLD else FW = FW_NORMAL   
  if (FSTYLE and FS_ITALIC) then FI = True else FI = False   
 
  THEFONT = CreateFont(FontSize(FSIZE),0,0,0,FW,FI,0,0,CHARSET,0,0,NONANTIALIASED_QUALITY,0,cast(any ptr,strptr(FNAME)))   
 
  ' selecting it
  SelectObject(THEDC,THEBMP)
  SelectObject(THEDC,THEFONT)
 
  GetTextMetrics(THEDC,@MYTXINFO)
   
  ' get text width/height
  GetTextExtentPoint32##_WorA_(THEDC,FTEXT,len(FTEXT),@TXTSIZE) 
  TXTSZ = TXTSIZE.CX
  TXYY = TXTSIZE.CY
  if (FSTYLE and FS_ITALIC) then
    if MYTXINFO.tmOverhang then
      TXTSZ += MYTXINFO.tmOverhang
    else
      TXTSZ += 1+(FSIZE/2)
    end if
    TXYY += 1+(FSIZE/8)
  end if
  if (FSTYLE and FS_ANTIALIAS) then
    #if GAMMA>1 and GAMMA <= 2
    TXTSZ += GAMMA*2
    #endif
  end if
  with RCT
    .LEFT = 0
    .TOP = 1
    .RIGHT = TXTSZ
    .BOTTOM = TXYY+1
  end with
  TXTSZ -= 1
  TXYY -= 1
 
  ' RGB to BGR
  asm
    mov eax,[FCOLOR]
    and eax,0xFFFFFF
    mov [FCOR],eax
    bswap eax
    ror eax,8
    mov [FCOLOR],eax
  end asm
 
  ' Set Colors
  SetBkColor(THEDC,rgba(255,0,255,0))
  SetTextColor(THEDC,FCOLOR)
   
  ExtTextOut##_WorA_(THEDC,0,1,ETO_CLIPPED or ETO_OPAQUE,@RCT,FTEXT,len(FTEXT),null)
  
  ' filling FBGFX header
  FBBLK = THEPTR+(2048*4)-sizeof(fb.image)
  FBBLK->type = 7
  FBBLK->bpp = 4
  FBBLK->width = 2048
  FBBLK->height = 512
  FBBLK->pitch = 2048*4
 
  ' blitting the rendered font to destion
  if (FSTYLE and FS_ANTIALIAS) then
    dim as any ptr MYBLK
    MYBLK = THEPTR+(2048*4)
    asm
      mov ecx,2048*511
      mov ebx,[FCOR]
      mov esi,[MYBLK]     
      1:
      cmp [esi], dword ptr 0xFF00FF     
      je 2f
      mov [esi+3], byte ptr 0xFF     
      2f:     
      and [esi], dword ptr 0xFF000000
      or [esi], ebx
      add esi,4
      dec ecx
      jnz 1b
    end asm
   
    dim as integer TX,TY
    dim as integer ALP
    #define GetAlpha(PX,PY) peek(MYBLK+((PY)*8192)+((PX)*4)+3)
    #define SetAlpha(PX,PY,NA) poke(MYBLK+((PY)*8192)+((PX)*4)+3),NA
   
    if (FSTYLE and FS_BLUR) = FS_BLUR then
      ' blur primeira linha
      for TX = 1 to TXTSZ-1
        ALP = (GetAlpha(TX,0)+GetAlpha(TX+1,0)+GetAlpha(TX-1,0)+ _
        GetAlpha(TX,1)+GetAlpha(TX-1,1)+GetAlpha(TX+1,1)) / 6
        #if GAMMA>1 and GAMMA <= 1.6
        ALP *= (GAMMA+.5)
        if ALP > 255 then ALP = 255
        #endif
        SetAlpha(TX,TY,ALP)
      next TX
      ' blur conteudo
      for TX = 1 to TXTSZ-1
        for TY = 1 to TXYY-1         
          ALP = (GetAlpha(TX,TY)+GetAlpha(TX+1,TY)+GetAlpha(TX-1,TY)+ _
          GetAlpha(TX,TY-1)+GetAlpha(TX,TY+1) + _
          GetAlpha(TX-1,TY-1)+GetAlpha(TX-1,TY+1)+ _
          GetAlpha(TX+1,TY-1)+GetAlpha(TX+1,TY+1)) / 9
          #if GAMMA>1 and GAMMA <= 1.6
        ALP *= (GAMMA+.5)
        if ALP > 255 then ALP = 255
        #endif
          SetAlpha(TX,TY,ALP)
        next TY
      next TX
      ' blur ultima linha
      for TX = 1 to TXTSZ-1
        ALP = (GetAlpha(TX,TY)+GetAlpha(TX+1,TY)+GetAlpha(TX-1,TY)+ _
        GetAlpha(TX,TY-1)+GetAlpha(TX-1,TY-1)+GetAlpha(TX+1,TY-1)) / 6
        #if GAMMA>1 and GAMMA <= 1.6
        ALP *= (GAMMA+.5)
        if ALP > 255 then ALP = 255
        #endif
        SetAlpha(TX,TY,ALP)
      next TX
    else     
      ' antialias primeira linha
      for TX = 1 to TXTSZ-1
        ALP = (GetAlpha(TX,0)+GetAlpha(TX+1,0)+_
        GetAlpha(TX-1,0)+GetAlpha(TX,1))/4       
        #if GAMMA>1 and GAMMA <= 2
        ALP *= GAMMA
        if ALP > 255 then ALP = 255
        #endif       
        SetAlpha(TX,TY,ALP)
      next TX
      ' antialias conteudo
      for TX = 1 to TXTSZ-1
        for TY = 1 to TXYY-1
          ALP = (GetAlpha(TX,TY)+GetAlpha(TX+1,TY)+GetAlpha(TX-1,TY)+_
          GetAlpha(TX,TY-1)+GetAlpha(TX,TY+1))/5
          #if GAMMA>1 and GAMMA <= 2
          ALP *= GAMMA
          if ALP > 255 then ALP = 255
          #endif         
          SetAlpha(TX,TY,ALP)         
        next TY
      next TX
      ' antialias ultima linha
      for TX = 1 to TXTSZ-1
        ALP = (GetAlpha(TX,TY)+GetAlpha(TX+1,TY)+ _
        GetAlpha(TX-1,TY)+GetAlpha(TX,TY-1))/4       
        #if GAMMA>1 and GAMMA <= 2
        ALP *= GAMMA
        if ALP > 255 then ALP = 255
        #endif       
        SetAlpha(TX,TY,ALP)
      next TX
    end if
   
    put BUFFER,(POSX,POSY),FBBLK,(0,0)-(TXTSZ-1,TXYY),alpha
  else 
    put BUFFER,(POSX,POSY),FBBLK,(0,0)-(TXTSZ-1,TXYY),trans
  end if
 
  ' cleanning up things
  DeleteObject(THEFONT)

#endmacro
sub DrawFont(BUFFER as any ptr=0,POSX as integer, POSY as integer, _
  FTEXT as string, FNAME as string,FSIZE as integer, _
  FCOLOR as uinteger=rgba(255,255,255,0), FSTYLE as integer=0, CHARSET as integer=DEFAULT_CHARSET )
 
  DrawFontBody(A)
 
end sub
sub DrawFontW(BUFFER as any ptr=0,POSX as integer, POSY as integer, _
  FTEXT as wstring, FNAME as string,FSIZE as integer, _
  FCOLOR as uinteger=rgba(255,255,255,0), FSTYLE as integer=0, CHARSET as integer=DEFAULT_CHARSET )
  
  DrawFontBody(W)
  
end sub