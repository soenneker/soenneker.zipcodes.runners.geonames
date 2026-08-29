using System.Threading;
using System.Threading.Tasks;

namespace Soenneker.ZipCodes.Runners.GeoNames.Utils.Abstract;

/// <summary>
/// Defines the file operations util contract.
/// </summary>
public interface IFileOperationsUtil
{
    /// <summary>
    /// Builds zip code geometry file.
    /// </summary>
    /// <param name="zipFilePath">Path of the zip file to use.</param>
    /// <param name="cancellationToken">Token used to cancel the operation.</param>
    /// <returns>A task whose result is the text returned by build Zip Code Geometry File.</returns>
    ValueTask<string> BuildZipCodeGeometryFile(string zipFilePath, CancellationToken cancellationToken = default);
}
