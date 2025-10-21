using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrinterApp.Models.Entities;

public class SystemSetting
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Key { get; set; }

    [Required]
    public string Value { get; set; }

    [StringLength(200)]
    public string Description { get; set; }

    public DateTime LastModified { get; set; }
}
