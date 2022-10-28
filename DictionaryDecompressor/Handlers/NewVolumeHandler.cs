using DictionaryDecompressor.EventArgs;

namespace DictionaryDecompressor.Handlers;

/// <summary>
/// Represents the method that will handle new volume events
/// </summary>
public delegate void NewVolumeHandler(object sender, NewVolumeEventArgs e);
