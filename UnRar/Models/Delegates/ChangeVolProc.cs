using System.Runtime.InteropServices;

namespace UnRar.Models.Delegates;

public delegate int ChangeVolProc([MarshalAs(UnmanagedType.LPStr)] string arcName, int mode);
