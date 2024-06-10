using System;

namespace AdvancedSharpAdbClient.Models;

/// <summary>
/// Flags used by sync V2 commands.
/// </summary>
/// [Flags]
public enum SyncFlag : UInt32
{
    None = 0,
    Brotli = 1,
    LZ4 = 2,
    Zstd = 4,
    DryRun = 0x8000_0000u,
}