using DictionaryDecompressor.EventArgs;

namespace DictionaryDecompressor.Handlers;

/// <summary>
/// Represents the method that will handle password required events
/// </summary>
public delegate void PasswordRequiredHandler(object sender, PasswordRequiredEventArgs e);