using System.Diagnostics;
using AdvancedSharpAdbClient;
using AdvancedSharpAdbClient.Models;

namespace Playground;

class Program
{
    const string LocalDir = @"D:\AFS\Test";
    const string RemotePath = @"/data/local/tmp/com.topjohnwu.magisk_27.0.tar";

    static async Task Main(string[] args)
    {
        var server = AdbServer.Instance;
        var result = server.StartServer("adb", true);
        var client = AdbClient.Instance;
        var devices = client.GetDevices();

        foreach (var device in devices)
        {
            Console.WriteLine(device);
        }

        var adbDevice = devices.First();

        Directory.Delete(LocalDir, true);
        Directory.CreateDirectory(LocalDir);

        var destPath = DownloadFile(RemotePath, adbDevice);

        // foreach (var syncFlag in Enum.GetValues<SyncFlag>())
        // {
        //     try
        //     {
        //         var destPathV2 = DownloadFileV2(RemotePath, syncFlag, adbDevice);
        //     }
        //     catch (Exception ex)
        //     {
        //         Console.WriteLine($"Exception thrown for {nameof(SyncFlag)}{syncFlag}: {ex.Message}");
        //     }
        // }

        var downloadCount = 100;
        var results = new List<string>();
        foreach (var compressionType in Enum.GetValues<CompressionType>())
        {
            if (compressionType == CompressionType.Any)
            {
                continue;
            }

            var sw = Stopwatch.StartNew();
            Parallel.For(0, downloadCount, i =>
            {
                try
                {
                    var destPathV2 = DownloadFileV2(RemotePath, compressionType, adbDevice, i);
                    // if (Path.Exists(destPathV2))
                    // {
                    //     File.Delete(destPathV2);
                    // }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception thrown for {nameof(CompressionType)}.{compressionType}: {ex.Message}");
                }
            });
            // for (var i = 0; i < downloadCount; i++)
            // {
            //     try
            //     {
            //         var destPathV2 = DownloadFileV2(RemotePath, compressionType, adbDevice, i);
            //         // if (Path.Exists(destPathV2))
            //         // {
            //         //     File.Delete(destPathV2);
            //         // }
            //     }
            //     catch (Exception ex)
            //     {
            //         Console.WriteLine($"Exception thrown for {nameof(CompressionType)}.{compressionType}: {ex.Message}");
            //     }
            // }

            var resultString = $"Downloaded {downloadCount} files with {compressionType} compression in {sw.ElapsedMilliseconds}ms";

            Console.WriteLine(resultString);

            results.Add(resultString);
        }

        foreach (var resultString in results)
        {
            Console.WriteLine(resultString);
        }
    }

    static string DownloadFile(string path, DeviceData adbDevice)
    {
        var sw = Stopwatch.StartNew();
        var destPath = Path.GetFullPath(Path.Combine(@"D:\AFS\Test", Path.GetFileName(path)));
        using var syncService = new SyncService(adbDevice);
        using var stream = File.OpenWrite(destPath);

        syncService.Pull(path, stream);
        Console.WriteLine($"{path} downloaded to {destPath} in {sw.ElapsedMilliseconds}ms");

        return destPath;
    }

    static string DownloadFileV2(string path, CompressionType compressionType, DeviceData adbDevice, int? num)
    {
        var sw = Stopwatch.StartNew();
        var fileNum = num.HasValue ? $"({num.Value})": string.Empty;

        var destPath = Path.GetFullPath(Path.Combine(@"D:\AFS\Test",
            $"{Path.GetFileNameWithoutExtension(path)}_{nameof(CompressionType)}.{compressionType}_{fileNum}{Path.GetExtension(path)}"));
        using var syncService = new SyncService(adbDevice);
        using var stream = File.OpenWrite(destPath);

        syncService.PullV2(path, compressionType, stream);
        Console.WriteLine($"{path} downloaded to {destPath} with {compressionType} compression in {sw.ElapsedMilliseconds}ms");

        return destPath;
    }

    static string DownloadFileV2(string path, SyncFlag syncFlag, DeviceData adbDevice)
    {
        var sw = Stopwatch.StartNew();

        var destPath = Path.GetFullPath(Path.Combine(@"D:\AFS\Test",
            $"{Path.GetFileNameWithoutExtension(path)}_{nameof(SyncFlag)}.{syncFlag}_{Path.GetExtension(path)}"));
        using var syncService = new SyncService(adbDevice);
        using var stream = File.OpenWrite(destPath);

        syncService.PullV2(path, syncFlag, stream);
        Console.WriteLine($"{path} downloaded to {destPath} with {syncFlag} compression in {sw.ElapsedMilliseconds}ms");

        return destPath;
    }
}