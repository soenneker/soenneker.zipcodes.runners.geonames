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
        }

        string resultPath = await _fileOperationsUtil.BuildZipCodeGeometryFile(zipFilePath);
        string result = await File.ReadAllTextAsync(resultPath);

        await Assert.That(result.Trim()).IsEqualTo("99553\tAkutan\tAK\t54.143\t-165.7854");
    }
}
