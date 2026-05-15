using System.Threading;
using System.Threading.Tasks;

namespace Soenneker.ZipCodes.Runners.GeoNames.Utils.Abstract;

public interface IFileOperationsUtil
{
    ValueTask<string> BuildZipCodeGeometryFile(string zipFilePath, CancellationToken cancellationToken = default);
}
