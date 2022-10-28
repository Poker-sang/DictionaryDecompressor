using System.Runtime.InteropServices;
using UnRar.Models.Enums;

namespace UnRar.Models;
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]

public struct RarHeaderData
{
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
    public string ArcName;

    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
    public string FileName;

    public ArchiveFlags Flags;
    public uint PackSize;
    public uint UnpSize;
    public HostOS HostOS;
    public uint FileCRC;
    public uint FileTime;
    public uint UnpVer;
    public PackingMethod Method;
    public uint FileAttr;

    [MarshalAs(UnmanagedType.LPStr)]
    public string CmtBuf;

    public uint CmtBufSize;
    public uint CmtSize;
    public uint CmtState;

    public void Initialize()
    {
        CmtBuf = new((char)0, 65536);
        CmtBufSize = 65536;
    }
}
