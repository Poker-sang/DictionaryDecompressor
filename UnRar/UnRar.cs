using System;
using System.Runtime.InteropServices;
using UnRar.Models;
using UnRar.Models.Delegates;
using UnRar.Models.Enums;

namespace UnRar;

public partial class UnRar
{
    public const string Path = @"C:\WorkSpace\DictionaryDecompressor\UnRar\UnRAR64.dll";

    /// <summary>
    /// Open Rar archive and allocate memory structures.
    /// <para>
    /// This function is obsolete. It does not support Unicode names and
    /// does not allow to specify the callback function.
    /// It is recommended to use <see cref="RarOpenArchiveEx"/> instead.
    /// </para>
    /// </summary>
    /// <param name="archiveData">Points to <see cref="RarOpenArchiveData"/> structure. </param>
    /// <returns>Archive handle or NULL in case of error.</returns>
    [Obsolete("It does not support Unicode names and does not allow to specify the callback function. It is recommended to use RarOpenArchiveEx instead.")]
    [DllImport(Path, EntryPoint = "RAROpenArchive")]
    public static extern nint RarOpenArchive(ref RarOpenArchiveData archiveData);

    /// <summary>
    /// Open Rar archive and allocate memory structures.
    /// Replaces the obsolete <see cref="RarOpenArchive"/>
    /// providing more options and Unicode names support.
    /// </summary>
    /// <param name="archiveData">Points to <see cref="RarOpenArchiveDataEx"/> structure.</param>
    /// <returns>Archive handle or NULL in case of error.</returns>
    [DllImport(Path, EntryPoint = "RAROpenArchiveEx")]
    public static extern nint RarOpenArchiveEx(ref RarOpenArchiveDataEx archiveData);

    /// <summary>
    /// Close Rar archive and release allocated memory.
    /// It must be called when archive processing is finished,
    /// even if the archive processing was stopped due to an error.
    /// </summary>
    /// <param name="hArcData">
    /// This parameter should contain the archive handle obtained from
    /// <see cref="RarOpenArchive"/> or
    /// <see cref="RarOpenArchiveEx"/> function call.
    /// </param>
    /// <returns></returns>
    [LibraryImport(Path, EntryPoint = "RARCloseArchive")]
    public static partial RarError RarCloseArchive(nint hArcData);

    /// <summary>
    /// Read a file header from archive.
    /// <para>
    /// This function is obsolete. It does not support Unicode names and 64 bit file sizes.
    /// It is recommended to use <see cref="RarReadHeaderEx"/> instead.
    /// </para>
    /// </summary>
    /// <param name="hArcData">
    /// This parameter should contain the archive handle obtained from
    /// <see cref="RarOpenArchive"/> or
    /// <see cref="RarOpenArchiveEx"/> function call.
    /// </param>
    /// <param name="headerData">Points to <see cref="RarHeaderData"/> structure.</param>
    /// <returns></returns>
    [Obsolete("It does not support Unicode names and 64 bit file sizes. It is recommended to use RarReadHeaderEx instead.")]
    [DllImport(Path, EntryPoint = "RARReadHeader")]
    public static extern int RarReadHeader(nint hArcData, ref RarHeaderData headerData);

    /// <summary>
    /// Read a file header from archive.
    /// </summary>
    /// <param name="hArcData">
    /// This parameter should contain the archive handle obtained from
    /// <see cref="RarOpenArchive"/> or
    /// <see cref="RarOpenArchiveEx"/> function call.
    /// </param>
    /// <param name="headerData">Points to <see cref="RarHeaderDataEx"/> structure.</param>
    /// <returns></returns>
    [DllImport(Path, EntryPoint = "RARReadHeaderEx")]
    public static extern RarError RarReadHeaderEx(nint hArcData, ref RarHeaderDataEx headerData);

    /// <summary>
    /// Performs the user defined action and moves the current position in the archive to the next file.
    /// <para>
    /// If archive is opened in <see cref="OpenMode.Extract"/> mode,
    /// this function extracts or tests the current file and sets the archive position to the next file.
    /// </para>
    /// <para>
    /// If open mode is <see cref="OpenMode.List"/>,
    /// then a call to this function will skip the current file and
    /// set the archive position to the next file.
    /// </para>
    /// <para>
    /// It is recommended to use <see cref="RarProcessFileW"/>
    /// instead of this function,
    /// because <see cref="RarProcessFileW"/> supports Unicode.
    /// </para>
    /// </summary>
    /// <param name="hArcData">
    /// This parameter should contain the archive handle obtained from
    /// <see cref="RarOpenArchive"/> or
    /// <see cref="RarOpenArchiveEx"/> function call.
    /// </param>
    /// <param name="operation"></param>
    /// <param name="destPath">
    /// This parameter should point to a zero terminated string,
    /// containing the destination directory to place the extracted files to.
    /// If DestPath is equal to NULL, it means extracting to the current directory.
    /// This parameter has meaning only if DestName is NULL.
    /// <para>This parameter must be in OEM encoding.</para>
    /// </param>
    /// <param name="destName">
    /// This parameter should point to a zero terminated string,
    /// containing the full path and name to assign to extracted file or
    /// it can be NULL to use the default name.
    /// If DestName is defined (not NULL),
    /// it overrides both the original file name stored in the archive and
    /// path specified in DestPath setting.
    /// <para>
    /// This parameter must be in OEM encoding.
    /// </para>
    /// </param>
    /// <returns></returns>
    [LibraryImport(Path, EntryPoint = "RARProcessFile")]
    public static partial RarError RarProcessFile(nint hArcData, Operation operation,
        [MarshalAs(UnmanagedType.LPStr)] string destPath,
        [MarshalAs(UnmanagedType.LPStr)] string destName);

    /// <summary>
    /// Performs the user defined action and moves the current position in the archive to the next file.
    /// <para>
    /// If archive is opened in <see cref="OpenMode.Extract"/> mode,
    /// this function extracts or tests the current file and sets the archive position to the next file.
    /// </para>
    /// <para>
    /// If open mode is <see cref="OpenMode.List"/>,
    /// then a call to this function will skip the current file and
    /// set the archive position to the next file.
    /// </para>
    /// </summary>
    /// <param name="hArcData">
    /// This parameter should contain the archive handle obtained from
    /// <see cref="RarOpenArchive"/> or
    /// <see cref="RarOpenArchiveEx"/> function call.
    /// </param>
    /// <param name="operation"></param>
    /// <param name="destPath">
    /// This parameter should point to a zero terminated Unicode string,
    /// containing the destination directory to place the extracted files to.
    /// If DestPath is equal to NULL, it means extracting to the current directory.
    /// This parameter has meaning only if DestName is NULL.
    /// </param>
    /// <param name="destName">
    /// This parameter should point to a zero terminated Unicode string,
    /// containing the full path and name to assign to extracted file or
    /// it can be NULL to use the default name.
    /// If DestName is defined (not NULL),
    /// it overrides both the original file name stored in the archive and
    /// path specified in DestPath setting.
    /// </param>
    /// <returns></returns>
    [LibraryImport(Path, EntryPoint = "RARProcessFileW")]
    public static partial RarError RarProcessFileW(nint hArcData, Operation operation,
        [MarshalAs(UnmanagedType.LPWStr)] string destPath,
        [MarshalAs(UnmanagedType.LPWStr)] string destName);

    /// <summary>
    /// Set a user defined <see cref="UnRarCallback"/> function to process UnRAR events.
    /// <para>
    /// <see cref="RarSetCallback"/> is obsolete and
    /// less preferable way to specify the callback function.
    /// Recommended approach is to set <see cref="RarOpenArchiveDataEx.Callback"/> and
    /// <see cref="RarOpenArchiveDataEx.UserData"/> when calling <see cref="RarOpenArchiveDataEx"/>.
    /// If you use <see cref="RarSetCallback"/>,
    /// you will not be able to read the archive comment in archives with encrypted headers.
    /// If you do not need the archive comment, you can continue to use
    /// <see cref="RarSetCallback"/>.
    /// </para>
    /// </summary>
    /// <param name="hArcData">
    /// This parameter should contain the archive handle obtained from
    /// <see cref="RarOpenArchive"/> or
    /// <see cref="RarOpenArchiveEx"/> function call.
    /// </param>
    /// <param name="callback">
    /// Address of user defined <see cref="UnRarCallback"/> to process UnRAR events.
    /// <para>
    /// Set it to NULL if you do not want to define the callback function.
    /// Callback function is required to process multi-volume and encrypted archives properly.
    /// </para>
    /// </param>
    /// <param name="userData">
    /// User defined value, which will be passed to <see cref="UnRarCallback"/>.
    /// </param>
    [LibraryImport(Path, EntryPoint = "RARSetCallback")]
    public static partial void RarSetCallback(nint hArcData, UnRarCallback callback, long userData);

    [Obsolete("Use the callback function instead.")]
    [LibraryImport(Path, EntryPoint = "RARSetChangeVolProc")]
    public static partial void RarSetChangeVolProc(nint hArcData, ChangeVolProc changeVolProc);

    [Obsolete("Use the callback function instead.")]
    [LibraryImport(Path, EntryPoint = "RARSetProcessDataProc")]
    public static partial void RarSetProcessDataProc(nint hArcData, ProcessDataProc processDataProc);

    /// <summary>
    /// Set a password to decrypt files.
    /// <para>
    /// This function does not allow to process archives with encrypted headers.
    /// It can be used only for archives with encrypted file data and unencrypted headers.
    /// So the recommended way to set a password is <see cref="CallbackMessages.NeedPasswordW"/>
    /// message in user defined callback function.
    /// Unlike <see cref="RarSetPassword"/>,
    /// <see cref="CallbackMessages.NeedPasswordW"/> can be used for all types of encrypted Rar archives.
    /// </para>
    /// </summary>
    /// <param name="hArcData">
    /// This parameter should contain the archive handle obtained from
    /// <see cref="RarOpenArchive"/> or
    /// <see cref="RarOpenArchiveEx"/> function call.
    /// </param>
    /// <param name="password">Zero terminated string containing a password.</param>
    [LibraryImport(Path, EntryPoint = "RARSetPassword")]
    public static partial void RarSetPassword(nint hArcData,
        [MarshalAs(UnmanagedType.LPStr)] string password);

    /// <summary>
    /// Returns API version.
    /// </summary>
    /// <returns>
    /// Returns an integer value denoting UnRAR.dll API version,
    /// which is also defined in unrar.h as Rar_DLL_VERSION.
    /// API version number is incremented only in case of noticeable changes in UnRAR.dll API.
    /// Do not confuse it with version of UnRAR.dll stored in DLL resources,
    /// which is incremented with every DLL rebuild.
    /// <para>
    /// If <see cref="RarGetDllVersion()"/> returns a value lower than UnRAR.dll,
    /// which your application was designed for,
    /// it may indicate that DLL version is too old and
    /// it may fail to provide all necessary functions to your application.
    /// </para>
    /// <para>
    /// This function is missing in very old versions of UnRAR.dll,
    /// so it is safer to use LoadLibrary and GetProcAddress to access it.
    /// </para>
    /// </returns>
    [LibraryImport(Path, EntryPoint = "RARGetDllVersion")]
    public static partial int RarGetDllVersion();
}
