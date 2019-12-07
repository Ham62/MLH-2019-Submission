#define fbc -dll -x "FBGFX.dll"
#include once "inc\fbpng.bi"
#include "inc\FBDrawFont.bas"
#include "inc\Rotation7-1.bas"
#include "inc\rz.bas"
#include "crt.bi"
#include "fbgfx.bi"
Using FB

extern "windows-ms"

function ScreenRes_ (wid as integer, hei as integer, depth as integer, flags as integer) as integer export
    return ScreenRes (wid, hei, depth,, flags)
end function

sub ScreenClose() export
    ' Close graphics screen preserving console
    ' &H80000000 is the undocumented value you pass to achive this
    #define SCREEN_EXIT &h80000000
    Screen 0, 0, 0, SCREEN_EXIT
end sub

function GetWindowHandle() as integer export
    dim as integer hWnd
    ScreenControl(GET_WINDOW_HANDLE, hWnd)
    return hWnd
end function

sub SetWindowTitle (title as zstring) export
    WindowTitle(title)
end sub

sub ScreenLock_ export
    ScreenLock()
end sub

sub ScreenUnlock_ export
    ScreenUnlock()
end sub

function ScreenPtr_ () as any ptr export
    return ScreenPtr
end function

function ImageCreate_ (wid as integer, hei as integer, clr as uinteger) as any ptr export
    return ImageCreate(wid, hei, clr)
end function

sub ImageDestroy_ (image as any ptr) export
    ImageDestroy(image)
end sub

function LoadBitmap_ (filename as zString, dest as any ptr, pal as any ptr) as integer export
    return Bload(filename, dest, pal)
end function

sub GetDimensionsPNG(szFile as zString, byref w as integer, byref h as integer) export
    png_dimensions(szFile, w, h)
end sub

function LoadPNG(szFilename as zString) as any ptr export
    return png_load(szFilename, PNG_TARGET_FBNEW)
end function

sub DestroyPNG(pBuff as any ptr) export
    png_destroy(pBuff)
end sub

'-------------- Get/Put ---------------'
enum PutFlags
    FLAG_PSET
    FLAG_TRANS
    FLAG_AND
    FLAG_OR
    FLAG_XOR
    FLAG_ALPHA
    FLAG_ADD
end enum

sub Put_ (target as any ptr, x as integer, y as integer, source as any ptr, _
          flag as integer, value as integer) export
    select case flag
    case FLAG_PSET
        Put target, (x, y), source, pset
            
    case FLAG_TRANS
        Put target, (x, y), source, trans
            
    case FLAG_AND
        Put target, (x, y), source, and
    
    case FLAG_OR
        Put target, (x, y), source, or
            
    case FLAG_XOR
        Put target, (x, y), source, xor
            
    case FLAG_ALPHA
        Put target, (x, y), source, alpha, value 
            
    case FLAG_ADD
        Put target, (x, y), source, add, value
    end select
end sub

sub PutPart (target as any ptr, x as integer, y as integer, source as any ptr, _
             srcX as integer, srcY as integer, wid as integer, hei as integer, _
             flag as integer, value as integer) export
    select case flag
    case FLAG_PSET
        Put target, (x, y), source, (srcX, srcY)-STEP(wid, hei), pset
        
    case FLAG_TRANS
        Put target, (x, y), source, (srcX, srcY)-STEP(wid, hei), trans
            
    case FLAG_AND
        Put target, (x, y), source, (srcX, srcY)-STEP(wid, hei), and
    
    case FLAG_OR
        Put target, (x, y), source, (srcX, srcY)-STEP(wid, hei), or
        
    case FLAG_XOR
        Put target, (x, y), source, (srcX, srcY)-STEP(wid, hei), xor

    case FLAG_ALPHA
        Put target, (x, y), source, (srcX, srcY)-STEP(wid, hei), alpha, value
        
    case FLAG_ADD
        Put target, (x, y), source, (srcX, srcY)-STEP(wid, hei), add, value
    end select
end sub

sub RotoZoomAlpha(dest as any ptr, src as any ptr, x as integer, y as integer, _
                  angle as single, scaleX as single, scaleY as single) export
    RotateScaleHQ(dest, src, x, y, angle, scaleX, scaleY)
end sub

sub RotoZoom32_(dest as any ptr, src as any ptr, x as integer, y as integer, _
              angle as single, zoomX as single, zoomY as single, transcol as uinteger) export
    rotozoom(dest, src, x, y, angle, zoomX, zoomY, transcol)
end sub


sub Get_ (src as any ptr, x as integer, y as integer, wid as integer, _
          hei as integer, dest as any ptr) export
    Get src, (x, y)-STEP(wid, hei), dest
end sub

'----------------- Drawing stuff -----------------'
sub cls_ () export
    cls
end sub

function RGB_ (R as integer, G as integer, B as integer) as uinteger export
    return RGB(R, G, B)
end function

function RGBA_ (R as integer, G as integer, B as integer, A as integer) as uinteger export
    return RGBA(R, G, B, A)
end function

'---------------- Shape Functions ------------------'
sub Circle_ (target as any ptr, x as integer, y as integer, radius as integer, _
             colr as uinteger, fillFlag as integer) export
    ' if target is null pointer use screen
    if fillFlag then
        Circle target, (x, y), radius, colr,,,,F
    else
        Circle target, (x, y), radius, colr
    end if
end sub

sub Line_ (target as any ptr, x1 as integer, y1 as integer, x2 as integer, _
           y2 as integer, colr as uinteger, style as ushort) export
    Line target, (x1, y1)-(x2, y2), colr,, style
end sub    

sub Box (target as any ptr, x1 as integer, y1 as integer, x2 as integer, _
               y2 as integer, colr as uinteger, fillFlag as integer, style as ushort) export
    if fillFlag then
        Line target, (x1, y1)-(x2, y2), colr, BF, style
    else
        Line target, (x1, y1)-(x2, y2), colr, B, style
    end if
end sub
    
sub Rectangle_ (target as any ptr, x1 as integer, y1 as integer, x2 as integer, _
               y2 as integer, colr as uinteger, fillFlag as integer, style as ushort) export
    if fillFlag then
        Line target, (x1, y1)-STEP(x2, y2), colr, BF, style
    else
        Line target, (x1, y1)-STEP(x2, y2), colr, B, style
    end if
end sub

'---------------- Input Functions ------------------'
type MouseInfo
    x as integer
    y as integer
    wheel as integer
    buttons as integer
    clip as integer
end type

Dim shared as String*2 sKey
function InKey_ () as zstring ptr export
    sKey = InKey 
    return StrPtr(sKey)
end function

function GetMouse_ (byref MI as MouseInfo) as integer export
    with MI
        return GetMouse(.x, .y, .wheel, .buttons, .clip)
    end with
end function

function SetMouse_ (byref MI as MouseInfo, visible as integer) as integer export
    with MI
        return SetMouse(.x, .y, visible, .clip)
    end with
end function

'---------------- Text Functions ------------------'
sub DrawString (target as any ptr, x as integer, y as integer, _
                text as zstring, colr as integer) export
    Draw String target, (x, y), text, colr
end sub

sub DrawFont_ (target as any ptr, x as integer, y as integer, _
               szCaption as zstring, szFntName as zstring, fntSz as integer, _
               fntClr as uinteger, fStyle as integer, charset as integer) export
    DrawFont(target, x, y, szCaption, szFntName, fntSz, fntClr, fStyle, charset)
end sub

sub DrawFontW_ (target as any ptr, x as integer, y as integer, _
               szCaption as wstring, szFntName as zstring, fntSz as integer, _
               fntClr as uinteger, fStyle as integer, charset as integer) export
    DrawFontW(target, x, y, szCaption, szFntName, fntSz, fntClr, fStyle, charset)
end sub

sub CalcTextSize(szCaption as wstring, szFntName as zString, fntSz as integer, _
                  fStyle as integer, byref x as integer, byref y as integer) export
                  
    dim as integer fw, fi
    if (fstyle and FS_BOLD) then fw = FW_BOLD else fw = FW_NORMAL   
    if (fstyle and FS_ITALIC) then fi = True else fi = False

    var hDC = GetDC(0)
    
    dim as integer fontPt = -MulDiv(fntSz, GetDeviceCaps(hDC, LOGPIXELSY), 72)
    var hFont = CreateFont(fontPt,0,0,0,fw,fi,0,0,DEFAULT_CHARSET,0,0,_
                           NONANTIALIASED_QUALITY,0,szFntName)  
                       
    SelectObject(hDC, hFont)
        
    static as TEXTMETRIC textMetrics
    static as SIZE txtSize
    
    GetTextMetrics(hDC, @textMetrics)
    GetTextExtentPoint32W(hDC, szCaption, len(szCaption),@txtSize) 
    dim as integer TXTSZ = txtSize.cx
    dim as integer TXYY = txtSize.cy
    if (fstyle and FS_ITALIC) then
        if textMetrics.tmOverhang then
            TXTSZ += textMetrics.tmOverhang
        else
            TXTSZ += 1+(fntSz/2)
        end if
        TXYY += 1+(fntSz/8)
    end if
    
    if (fstyle and FS_ANTIALIAS) then
        #if GAMMA>1 and GAMMA <= 2
            TXTSZ += GAMMA*2
        #endif
    end if

    TXTSZ -= 1
    TXYY -= 1
    
    ReleaseDC(0, hDC)
    DeleteObject(hFont)

    x = TXTSZ: y = TXYY
end sub
end extern
