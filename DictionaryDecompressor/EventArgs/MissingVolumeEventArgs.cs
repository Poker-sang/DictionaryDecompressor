namespace DictionaryDecompressor.EventArgs;

public class MissingVolumeEventArgs
{
    public string VolumeName;
    public bool ContinueOperation = false;

    public MissingVolumeEventArgs(string volumeName) => VolumeName = volumeName;
}