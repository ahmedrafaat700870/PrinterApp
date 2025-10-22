using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace PrinterApp.Models.ViewModels
{
    public class ProductViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Product name is required")]
        [StringLength(200, ErrorMessage = "Product name cannot exceed 200 characters")]
        [Display(Name = "Product Name")]
        public string ProductName { get; set; }

        [Required(ErrorMessage = "Product code is required")]
        [StringLength(50, ErrorMessage = "Product code cannot exceed 50 characters")]
        [Display(Name = "Product Code")]
        public string ProductCode { get; set; }

        [Required(ErrorMessage = "Please specify if the product is printed or not")]
        [Display(Name = "Is Printed")]
        public bool IsPrinted { get; set; }

        [Required(ErrorMessage = "Raw material is required")]
        [Display(Name = "Raw Material")]
        public int RawMaterialId { get; set; }

        [Display(Name = "Raw Material Name")]
        public string RawMaterialName { get; set; }

        [Required(ErrorMessage = "Supplier is required")]
        [Display(Name = "Supplier")]
        public int SupplierId { get; set; }

        [Display(Name = "Supplier Name")]
        public string SupplierName { get; set; }

        [Display(Name = "Supplier Code")]
        public string SupplierCode { get; set; }

        [StringLength(500)]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Manufacturing Additions")]
        public List<int> SelectedAdditionIds { get; set; } = new List<int>();

        [Display(Name = "Selected Additions")]
        public List<string> SelectedAdditionNames { get; set; } = new List<string>();

        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }

        [Display(Name = "Last Modified")]
        public DateTime? LastModified { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; }

        // For dropdown lists
        public List<SelectListItem> RawMaterials { get; set; }
        public List<SelectListItem> Suppliers { get; set; }
        public List<ManufacturingAdditionCheckboxViewModel> AvailableAdditions { get; set; }
    }

    public class ManufacturingAdditionCheckboxViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsSelected { get; set; }
    }
}