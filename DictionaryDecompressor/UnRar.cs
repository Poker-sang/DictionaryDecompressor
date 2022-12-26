using System.Runtime.InteropServices;
using UnRar.Models;
using UnRar.Models.Delegates;
using UnRar.Models.Enums;
using DictionaryDecompressor.EventArgs;
using DictionaryDecompressor.Handlers;
using static UnRar.UnRar;

/*  Author:  Michael A. McCloskey
 *  Company: Schematrix
 *  Version: 20040714
 *  
 *  Personal Comments:
 *  I created this unrar wrapper class for personal use 
 *  after running into a number of issues trying to use
 *  another COM unrar product via COM interop.  I hope it 
 *  proves as useful to you as it has to me and saves you
 *  some time in building your own products.
 */

namespace DictionaryDecompressor;

#region Event Delegate Definitions

#endregion

/// <summary>
/// Wrapper class for unrar DLL supplied by RARSoft.  
/// Calls unrar DLL via platform invocation services (P\Invoke).
/// DLL is available at http://www.rarlab.com/rar/UnRARDLL.exe
/// </summary>
public class UnRar : IDisposable
{
    #region Public event declarations

    /// <summary>
    /// Event that is raised when a new chunk of data has been extracted
    /// </summary>
    public event DataAvailableHandler? DataAvailable;
    /// <summary>
    /// Event that is raised to indicate extraction progress
    /// </summary>
    public event ExtractionProgressHandler? ExtractionProgress;
    /// <summary>
    /// Event that is raised when a required archive volume is missing
    /// </summary>
    public event MissingVolumeHandler? MissingVolume;
    /// <summary>
    /// Event that is raised when a new file is encountered during processing
    /// </summary>
    public event NewFileHandler? NewFile;
    /// <summary>
    /// Event that is raised when a new archive volume is opened for processing
    /// </summary>
    public event NewVolumeHandler? NewVolume;
    /// <summary>
    /// Event that is raised when a password is required before continuing
    /// </summary>
    public event PasswordRequiredHandler? PasswordRequired;

    #endregion

    #region Private fields

    /// <summary>
    /// Path and name of RAR archive to open
    /// </summary>
    private readonly string _archivePathName;
    private nint _archiveHandle;
    private readonly bool _retrieveComment = true;
    private string _password;
    private ArchiveFlags _archiveFlags = 0;
    private RarHeaderDataEx _header;
    private readonly UnRarCallback _callback;

    #endregion

    #region Object lifetime procedures

    private UnRar(string archivePathName, string password)
    {
        _callback = RarCallback;
        _archivePathName = archivePathName;
        _password = password;
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        if (_archiveHandle != 0)
        {
            _ = RarCloseArchive(_archiveHandle);
            _archiveHandle = 0;
        }
    }

    #endregion

    #region Public Properties

    /// <summary>
    /// Archive comment 
    /// </summary>
    public string Comment { get; private set; } = "";

    /// <summary>
    /// Current file being processed
    /// </summary>
    public RarFileInfo? CurrentFile { get; private set; }

    /// <summary>
    /// Password for opening encrypted archive
    /// </summary>
    public string Password
    {
        get => _password;
        set
        {
            _password = value;
            if (_archiveHandle != 0)
                RarSetPassword(_archiveHandle, value);
        }
    }

    #endregion

    #region Public Methods

    public static UnRar Open(FileInfo fileInfo, OpenMode openMode, string password = "") => Open(fileInfo.FullName, openMode, password);

    /// <summary>
    /// Opens specified archive using the specified mode.  
    /// </summary>
    /// <param name="archivePathName">Path of archive to open</param>
    /// <param name="openMode">Mode in which to open archive</param>
    /// <param name="password">password</param>
    public static UnRar Open(string archivePathName, OpenMode openMode, string password = "")
    {
        var unRar = new UnRar(archivePathName, password);

        // Prepare extended open archive struct
        var openStruct = new RarOpenArchiveDataEx(
            archivePathName + "\0",
            archivePathName + "\0"
            )
        {
            OpenMode = openMode,
            CmtBuf = unRar._retrieveComment ? new((char)0, 65536) : null,
            CmtBufSize = (uint)(unRar._retrieveComment ? 65536 : 0)
        };

        // Open archive
        var handle = RarOpenArchiveEx(ref openStruct);

        // Check for success
        switch (openStruct.OpenResult)
        {
            case RarError.Success:
                break;

            case RarError.InsufficientMemory:
                throw new OutOfMemoryException("Insufficient memory to perform operation.");

            case RarError.BadData:
                throw new IOException("Archive header broken");

            case RarError.BadArchive:
                throw new IOException("File is not a valid archive.");

            case RarError.OpenError:
                throw new IOException("File could not be opened.");

            default:
                throw new IOException(openStruct.OpenResult.ToString());
        }

        // Save handle and flags
        unRar._archiveHandle = handle;
        unRar._archiveFlags = openStruct.Flags;

        // Set callback
        RarSetCallback(unRar._archiveHandle, unRar._callback, unRar.GetHashCode());

        // If comment retrieved, save it
        if (openStruct.CmtState is 1)
            unRar.Comment = openStruct.CmtBuf ?? "";

        // If password supplied, set it
        if (unRar._password.Length is not 0)
            RarSetPassword(unRar._archiveHandle, unRar._password);

        // Fire NewVolume event for first volume
        _ = unRar.OnNewVolume(unRar._archivePathName);

        return unRar;
    }

    /// <summary>
    /// Reads the next archive header and populates CurrentFile property data
    /// </summary>
    /// <returns></returns>
    public RarError ReadHeader()
    {
        // Initialize header struct
        _header = new();

        // Read next entry
        var result = RarReadHeaderEx(_archiveHandle, ref _header);

        // Check for error or end of archive
        if (result is not RarError.Success)
            return result;
        // case RarError.EndOfArchive:
        // case RarError.BadData:
        // case RarError.BadPassword:
        // case RarError.MissingPassword:

        // Determine if new file
        if (_header.Flags is FileFlags.SplitBefore)
            CurrentFile!.ContinuedFromPrevious = true;
        else
        {
            // New file, prepare header
            CurrentFile = new(_header.FileNameW)
            {
                ContinuedOnNext = _header.Flags is FileFlags.SplitAfter,
                PackedSize = _header.PackSizeHigh != 0
                    ? (_header.PackSizeHigh * 0x100000000) + _header.PackSize
                    : _header.PackSize,
                UnpackedSize = _header.UnpSizeHigh != 0
                    ? (_header.UnpSizeHigh * 0x100000000) + _header.UnpSize
                    : _header.UnpSize,
                HostOS = _header.HostOS,
                FileCRC = _header.FileCRC,
                FileTime = FromMsDosTime(_header.FileTime),
                VersionToUnpack = (int)_header.UnpVer,
                Method = (int)_header.Method,
                FileAttributes = (int)_header.FileAttr,
                BytesExtracted = 0,
                IsDirectory = _header.Flags is FileFlags.Directory
            };
            if (NewFile is not null)
            {
                var e = new NewFileEventArgs(CurrentFile);
                NewFile(this, e);
            }
        }

        // Return success
        return RarError.Success;
    }

    /// <summary>
    /// Returns array of file names contained in archive.
    /// May need Password.
    /// </summary>
    /// <returns></returns>
    public IEnumerable<string> ListFiles()
    {
        var fileNames = new List<string>();
        while (ReadHeader() is RarError.Success)
        {
            if (!CurrentFile!.IsDirectory)
                fileNames.Add(CurrentFile.FileName);
            ProcessFileError(Skip());
        }

        return fileNames;
    }

    /// <summary>
    /// Moves the current archive position to the next available header.
    /// Only returns <see cref="RarError.Success"/> and <see cref="RarError.OpenError"/>.
    /// </summary>
    /// <returns></returns>
    public RarError Skip() => RarProcessFileW(_archiveHandle, Operation.Skip, "", "");

    /// <summary>
    /// Tests the ability to extract the current file without saving extracted data to disk
    /// </summary>
    /// <returns></returns>
    public RarError Test() => RarProcessFileW(_archiveHandle, Operation.Test, "", "");

    /// <summary>
    /// Extracts the current file to a specified destination path and filename
    /// </summary>
    /// <param name="destinationName">Path and name of extracted file</param>
    /// <returns></returns>
    public RarError Extract(string destinationName) => Extract("", destinationName);

    /// <summary>
    /// Extracts the current file to a specified directory without renaming file
    /// </summary>
    /// <param name="destinationPath"></param>
    /// <returns></returns>
    public RarError ExtractToDirectory(string destinationPath) => Extract(destinationPath, "");

    #endregion

    #region Private Methods

    private RarError Extract(string destinationPath, string destinationName) => RarProcessFileW(_archiveHandle, Operation.Extract, destinationPath, destinationName);

    private CallbackMode RarCallback(CallbackMessages msg, long userData, long p1, long p2)
    {
        var result = CallbackMode.Aborted;
        var p11 = (nint)p1;

        switch (msg)
        {
            case CallbackMessages.ProcessData:
                result = OnDataAvailable(p11, (int)p2);
                break;

            case CallbackMessages.NeedPassword:
            case CallbackMessages.NeedPasswordW:
                result = CallbackMode.Continue;// OnPasswordRequired(p11, p2);
                break;

            case CallbackMessages.VolumeChange:
            case CallbackMessages.ValumeChangeW:
                var volume = Marshal.PtrToStringAnsi(p11)!;
                switch ((VolumeMessage)p2)
                {
                    case VolumeMessage.Notify:
                        result = OnNewVolume(volume);
                        break;
                    case VolumeMessage.Ask:
                    {
                        var newVolume = OnMissingVolume(volume);
                        if (newVolume.Length is 0)
                            result = CallbackMode.Aborted;
                        else
                        {
                            if (newVolume != volume)
                            {
                                for (var i = 0; i < newVolume.Length; ++i)
                                    Marshal.WriteByte(p11, i, (byte)newVolume[i]);
                                Marshal.WriteByte(p11, newVolume.Length, 0);
                            }

                            result = CallbackMode.Continue;
                        }

                        break;
                    }
                    default:
                        throw new ArgumentOutOfRangeException(nameof(p2), p2, null);
                }

                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(msg), msg, null);
        }

        return result;
    }

    #endregion

    #region Static Methods

    private static DateTime FromMsDosTime(uint dosTime)
    {
        var hiWord = (ushort)((dosTime & 0xFFFF0000) >> 16);
        var loWord = (ushort)(dosTime & 0xFFFF);
        var year = ((hiWord & 0xFE00) >> 9) + 1980;
        var month = (hiWord & 0x01E0) >> 5;
        var day = hiWord & 0x1F;
        var hour = (loWord & 0xF800) >> 11;
        var minute = (loWord & 0x07E0) >> 5;
        var second = (loWord & 0x1F) << 1;
        return new(year, month, day, hour, minute, second);
    }

    private static void ProcessFileError(RarError result)
    {
        if (result is RarError.Success)
            return;
        throw result switch
        {
            RarError.UnknownFormat => new OutOfMemoryException("Unknown archive format."),
            RarError.BadData => new IOException("File CRC Error"),
            RarError.BadArchive => new IOException("File is not a valid archive."),
            RarError.OpenError => new IOException("File could not be opened."),
            RarError.CreateError => new IOException("File could not be created."),
            RarError.CloseError => new IOException("File close error."),
            RarError.ReadError => new IOException("File read error."),
            RarError.WriteError => new IOException("File write error."),
            // RarError.EndOfArchive => new IOException("End of archive."),
            // RarError.InsufficientMemory => new IOException("Insufficient memory."),
            // RarError.BufferTooSmall => new IOException("Buffer too small."),
            // RarError.UnknownError => new IOException("Unknown error."),
            // RarError.MissingPassword => new IOException("Missing password."),
            // RarError.ReferenceError => new IOException("Reference error."),
            RarError.BadPassword => new IOException("Bad password."),
            _ => throw new ArgumentOutOfRangeException(nameof(result), result, null)
        };
    }

    #endregion

    #region Protected Virtual (Overridable) Methods

    protected virtual CallbackMode OnPasswordRequired(nint p1, long p2)
    {
        var result = CallbackMode.Aborted;
        if (PasswordRequired is null)
            throw new IOException("Password is required for extraction.");

        var e = new PasswordRequiredEventArgs();
        PasswordRequired(this, e);

        if (e.ContinueOperation && e.Password.Length > 0)
        {
            for (var i = 0; i < e.Password.Length && i < p2; ++i)
                Marshal.WriteByte(p1, i, (byte)e.Password[i]);
            Marshal.WriteByte(p1, e.Password.Length, 0);
            result = CallbackMode.Continue;
        }

        return result;
    }

    protected virtual CallbackMode OnDataAvailable(nint p1, int p2)
    {
        var result = CallbackMode.Continue;
        if (CurrentFile is not null)
            CurrentFile.BytesExtracted += p2;
        if (DataAvailable is not null)
        {
            var data = new byte[p2];
            Marshal.Copy(p1, data, 0, p2);
            var e = new DataAvailableEventArgs(data);
            DataAvailable(this, e);
            if (!e.ContinueOperation)
                result = CallbackMode.Aborted;
        }

        if (ExtractionProgress is not null && CurrentFile is not null)
        {
            var e = new ExtractionProgressEventArgs(CurrentFile);
            ExtractionProgress(this, e);
            if (!e.ContinueOperation)
                result = CallbackMode.Aborted;
        }

        return result;
    }

    protected virtual CallbackMode OnNewVolume(string volume)
    {
        var result = CallbackMode.Continue;
        if (NewVolume is not null)
        {
            var e = new NewVolumeEventArgs(volume);
            NewVolume(this, e);
            if (!e.ContinueOperation)
                result = CallbackMode.Aborted;
        }

        return result;
    }

    protected virtual string OnMissingVolume(string volume)
    {
        var result = "";
        if (MissingVolume is not null)
        {
            var e = new MissingVolumeEventArgs(volume);
            MissingVolume(this, e);
            if (e.ContinueOperation)
                result = e.VolumeName;
        }

        return result;
    }

    #endregion
}

