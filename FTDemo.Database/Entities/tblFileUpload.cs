using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace FTDemo.Database.Entities
{
    public class tblFileUpload
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int FileId { get; set; }

        [StringLength(255)]
        public string FileName { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
