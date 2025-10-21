
namespace PrinterApp.Models.ViewModels;

public class AssignPermissionViewModel
{
    public string UserId { get; set; }
    public int PermissionId { get; set; }
    public List<int> SelectedRoleIds { get; set; } = new List<int>();
}