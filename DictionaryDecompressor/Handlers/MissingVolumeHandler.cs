using DictionaryDecompressor.EventArgs;

namespace DictionaryDecompressor.Handlers;

/// <summary>
/// Represents the method that will handle missing archive volume events
/// </summary>
public delegate void MissingVolumeHandler(object sender, MissingVolumeEventArgs e);