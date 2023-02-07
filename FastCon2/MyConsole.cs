using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32.SafeHandles;

namespace FastCon2;

public class MyConsole
{
    [DllImport("Kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    static extern SafeFileHandle CreateFile(
        string fileName,
        [MarshalAs(UnmanagedType.U4)] uint fileAccess,
        [MarshalAs(UnmanagedType.U4)] uint fileShare,
        IntPtr securityAttributes,
        [MarshalAs(UnmanagedType.U4)] FileMode creationDisposition,
        [MarshalAs(UnmanagedType.U4)] int flags,
        IntPtr template);

    [DllImport("kernel32.dll", SetLastError = true)]
    static extern bool WriteConsoleOutputW(
      SafeFileHandle hConsoleOutput, 
      CharInfo[] lpBuffer, 
      Coord dwBufferSize, 
      Coord dwBufferCoord, 
      ref SmallRect lpWriteRegion);

    [StructLayout(LayoutKind.Sequential)]
    public struct Coord
    {
      public short X;
      public short Y;

      public Coord(short X, short Y)
      {
        this.X = X;
        this.Y = Y;
      }
    };

    [StructLayout(LayoutKind.Explicit)]
    public struct CharUnion
    {
      [FieldOffset(0)] public ushort UnicodeChar;
      [FieldOffset(0)] public byte AsciiChar;
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct CharInfo
    {
      [FieldOffset(0)] public CharUnion Char;
      [FieldOffset(2)] public short Attributes;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SmallRect
    {
      public short Left;
      public short Top;
      public short Right;
      public short Bottom;
    }

    private const int WindowWidth = 120;
    private const int WindowHeight = 45;
    private CharInfo[] buf;
    private SmallRect rect;
    private SafeFileHandle h;

    [STAThread]
    public bool SetupCon()
    {
      h = CreateFile("CONOUT$", 0x40000000, 2, IntPtr.Zero, FileMode.Open, 0, IntPtr.Zero);

      if (!h.IsInvalid)
      {
        Console.WindowHeight = WindowHeight;
        Console.WindowWidth = WindowWidth;
        buf = new CharInfo[WindowWidth * WindowHeight];
        rect = new SmallRect() { Left = 0, Top = 0, Right = WindowWidth, Bottom = WindowHeight };


        // for (byte character = 65; character < 65 + 26; ++character)
        // {
        //   for (short attribute = 0; attribute < 15; ++attribute)
        //   {
        //     for (int i = 0; i < buf.Length; ++i)
        //     {
        //       buf[i].Attributes = attribute;
        //       buf[i].Char.AsciiChar = character;
        //     }
        //     bool b = WriteConsoleOutputW(h, buf, new Coord() { X = WindowWidth, Y = WindowHeight }, new Coord() { X = 0, Y = 0 }, ref rect);
        //   }
        // }
      }
      return !h.IsInvalid;
    }

    public void WriteAt(char c, int XPos, int YPos)
    {
      buf[(YPos * WindowWidth) + XPos].Attributes = 7;
      buf[(YPos * WindowWidth) + XPos].Char.UnicodeChar = (ushort) c;
      bool b = WriteConsoleOutputW(h, buf, new Coord() { X = WindowWidth, Y = WindowHeight }, new Coord() { X = 0, Y = 0 }, ref rect);
    }

    public void WriteAt(string s, int XPos, int YPos)
    {
      for (int i = 0; i < s.Length; i++)
      {
        buf[(YPos * WindowWidth) + XPos + i].Attributes = 7;
        buf[(YPos * WindowWidth) + XPos + i].Char.AsciiChar = Encoding.ASCII.GetBytes(s)[i];
      }
      bool b = WriteConsoleOutputW(h, buf, new Coord() { X = WindowWidth, Y = WindowHeight }, new Coord() { X = 0, Y = 0 }, ref rect);
    }
}
  