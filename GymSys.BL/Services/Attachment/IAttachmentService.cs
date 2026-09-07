using GymSys.BLL.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSys.BLL.Services.Attachment
{
    public interface IAttachmentService
    {
        Task<Result<string?>> UploadAsync(Stream fileStream, string folderName, string fileName, CancellationToken ct = default);
        Result Delete(string folderName, string fileName);
        Result<(Stream stream, string contentType)?> GetFile(string folderName, string fileName);
    }
}
