using FTDemo.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace FTDemo.Database.Interfaces
{
    public interface IFileUpload
    {
        bool AddUpdateBranch(FileUploadModel fileUploadModel);
        FileUploadModel GetFileUploadById(int Id);
        List<FileUploadModel> GetAllFileUpload();
    }
}
