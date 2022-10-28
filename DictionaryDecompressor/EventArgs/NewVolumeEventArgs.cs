namespace DictionaryDecompressor.EventArgs;

public class NewVolumeEventArgs
{
    public string VolumeName;
    public bool ContinueOperation = true;

    public NewVolumeEventArgs(string volumeName) => VolumeName = volumeName;
}
