using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using Soenneker.Tests.HostedUnit;
using Soenneker.ZipCodes.Runners.GeoNames.Utils.Abstract;

namespace Soenneker.ZipCodes.Runners.GeoNames.Tests;

[ClassDataSource<Host>(Shared = SharedType.PerTestSession)]
public sealed class ZipCodesGeoNameRunnerTests : HostedUnitTest
{
    private readonly IFileOperationsUtil _fileOperationsUtil;

    public ZipCodesGeoNameRunnerTests(Host host) : base(host)
    {
        _fileOperationsUtil = Resolve<IFileOperationsUtil>(true);
    }

    [Test]
    public async Task Builds_zip_code_geometry_file()
    {
        string zipFilePath = Path.Combine(Path.GetTempPath(), $"{nameof(Builds_zip_code_geometry_file)}.zip");

        if (File.Exists(zipFilePath))
            File.Delete(zipFilePath);

        await using (FileStream zipStream = File.Create(zipFilePath))
        {
            using var archive = new ZipArchive(zipStream, ZipArchiveMode.Create);
            ZipArchiveEntry entry = archive.CreateEntry(Constants.SourceFileName);

            await using Stream entryStream = entry.Open();
            await using var writer = new StreamWriter(entryStream);
            await writer.WriteLineAsync("US\t99553\tAkutan\tAlaska\tAK\tAleutians East\t013\t\t\t54.143\t-165.7854\t1");
            await writer.WriteLineAsync("US\t09001\tAPO AA\t\t\t\t\t\t\t38.1105\t15.6613\t");
            await writer.WriteLineAsync("US\t96365\tAPO STA\t\t\t\t\t\t\t26.3652\t127.7586\t");
            await writer.WriteLineAsync("US\t00000\tNowhere\t\t\t\t\t\t\t\t\t");
        }

        string resultPath = await _fileOperationsUtil.BuildZipCodeGeometryFile(zipFilePath);
        string result = (await File.ReadAllTextAsync(resultPath)).Replace("\r\n", "\n");

        await Assert.That(result.Trim()).IsEqualTo("""
                                                   99553	Akutan	AK	54.143	-165.7854
                                                   09001	APO	AA	38.1105	15.6613
                                                   96365	APO	AP	26.3652	127.7586
                                                   """);
    }
}
