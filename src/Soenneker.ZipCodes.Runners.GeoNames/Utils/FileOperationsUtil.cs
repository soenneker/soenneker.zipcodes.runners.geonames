using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Soenneker.Utils.Directory.Abstract;
using Soenneker.Utils.File.Abstract;
using Soenneker.ZipCodes.Runners.GeoNames.Utils.Abstract;

namespace Soenneker.ZipCodes.Runners.GeoNames.Utils;

///<inheritdoc cref="IFileOperationsUtil"/>
public sealed class FileOperationsUtil : IFileOperationsUtil
{
    private readonly ILogger<FileOperationsUtil> _logger;
    private readonly IFileUtil _fileUtil;
    private readonly IDirectoryUtil _directoryUtil;

    public FileOperationsUtil(ILogger<FileOperationsUtil> logger, IFileUtil fileUtil, IDirectoryUtil directoryUtil)
    {
        _logger = logger;
        _fileUtil = fileUtil;
        _directoryUtil = directoryUtil;
    }

    public async ValueTask<string> BuildZipCodeGeometryFile(string zipFilePath, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Extracting GeoNames ZIP code data from {ZipFilePath}...", zipFilePath);

        string workingDirectory = await _directoryUtil.CreateTempDirectory(cancellationToken);
        string resultFilePath = Path.Combine(workingDirectory, Constants.FileName);

        await using FileStream zipStream = _fileUtil.OpenRead(zipFilePath);
        await using var archive = new ZipArchive(zipStream, ZipArchiveMode.Read);

        ZipArchiveEntry? sourceEntry = archive.GetEntry(Constants.SourceFileName);

        if (sourceEntry == null)
            throw new FileNotFoundException($"Could not find {Constants.SourceFileName} in GeoNames ZIP file", Constants.SourceFileName);

        await using Stream sourceStream = await sourceEntry.OpenAsync(cancellationToken);
        using var reader = new StreamReader(sourceStream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true);

        await using FileStream resultStream = _fileUtil.OpenWrite(resultFilePath);
        await using var writer = new StreamWriter(resultStream, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));

        var lineNumber = 0;
        var written = 0;
        var skipped = 0;

        while (await reader.ReadLineAsync(cancellationToken) is { } line)
        {
            cancellationToken.ThrowIfCancellationRequested();
            lineNumber++;

            if (string.IsNullOrWhiteSpace(line))
                continue;

            string[] columns = line.Split('\t');

            if (columns.Length < 11)
                throw new InvalidDataException($"Unexpected GeoNames format at line {lineNumber}. Expected at least 11 tab-delimited columns.");

            string zipCode = columns[1];
            string city = columns[2];
            string state = columns[4];
            string latitude = columns[9];
            string longitude = columns[10];

            if (string.IsNullOrWhiteSpace(state) && TryGetMilitaryState(zipCode, city, out string militaryCity, out string militaryState))
            {
                city = militaryCity;
                state = militaryState;
            }

            if (string.IsNullOrWhiteSpace(zipCode) || string.IsNullOrWhiteSpace(city) || string.IsNullOrWhiteSpace(state) ||
                string.IsNullOrWhiteSpace(latitude) || string.IsNullOrWhiteSpace(longitude))
            {
                _logger.LogWarning("Skipping GeoNames line {LineNumber} because it is missing required geometry output values", lineNumber);
                skipped++;
                continue;
            }

            await writer.WriteLineAsync($"{zipCode}\t{city}\t{state}\t{latitude}\t{longitude}".AsMemory(), cancellationToken);
            written++;
        }

        _logger.LogInformation("Wrote {Count} GeoNames ZIP code geometry rows to {ResultFilePath}. Skipped {SkippedCount} rows.", written, resultFilePath,
            skipped);

        return resultFilePath;
    }

    private static bool TryGetMilitaryState(string zipCode, string city, out string militaryCity, out string militaryState)
    {
        militaryCity = null!;
        militaryState = null!;

        string[] parts = city.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        if (parts.Length == 0)
            return false;

        if (parts[0] is not ("APO" or "FPO" or "DPO"))
            return false;

        militaryCity = parts[0];

        if (parts.Length > 1 && parts[^1] is "AA" or "AE" or "AP")
        {
            militaryState = parts[^1];
            return true;
        }

        if (zipCode.StartsWith("09", StringComparison.Ordinal))
            militaryState = "AE";
        else if (zipCode.StartsWith("340", StringComparison.Ordinal))
            militaryState = "AA";
        else if (zipCode.StartsWith("96", StringComparison.Ordinal))
            militaryState = "AP";

        return militaryState != null;
    }
}
