using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace FTDemo.Common.Models
{
    public class FileUploadModel
    {
        public int FileId { get; set; }
        public string FileName { get; set; }
        public IFormFile File { get; set; }

        public DateTime CreatedDate { get; set; }
        public string WebHostEnvironmentPath { get; set; }
    }
}
