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
    /// <param name="zipFilePath">The zip file path.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task containing the result of the operation.</returns>
    ValueTask<string> BuildZipCodeGeometryFile(string zipFilePath, CancellationToken cancellationToken = default);
}
