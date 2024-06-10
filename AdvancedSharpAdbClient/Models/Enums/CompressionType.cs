namespace AdvancedSharpAdbClient.Models;

/// <summary>
/// Compression type enum used for sync V2.
/// </summary>
public enum CompressionType
{
    None,
    Any,
    Brotli,
    LZ4,
    Zstd,
}