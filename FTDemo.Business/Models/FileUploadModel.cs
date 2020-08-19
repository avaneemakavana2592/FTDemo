using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace SkillTest.Business.Models
{
    public class FileUploadModel
    {
        public int FileId { get; set; }
        public string FileName { get; set; }
        public IFormFile File { get; set; }
    }
}
