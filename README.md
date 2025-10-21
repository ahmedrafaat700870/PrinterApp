# 🖨️ Printer Management System

A comprehensive **ASP.NET Core MVC** application for managing printers, users, roles, and hierarchical permissions with multi-language support (English & Arabic).

## ✨ Features

### 🔐 Authentication & Authorization
- User registration and login with ASP.NET Core Identity
- Role-based access control (Admin, Manager, User)
- Hierarchical permission system with granular roles
- Custom authorization policies

### 👥 User Management
- View all users with their roles
- Assign/change user roles
- Manage user-specific permissions
- Track user activity

### 🛡️ Permission Management
- Create and manage permissions (e.g., Users Management, Printers Management, Reports)
- Add roles to permissions (View, Create, Edit, Delete, Manage)
- Assign specific permission roles to users
- Permission code-based authorization

### 🖨️ Printer Management
- Manage printer inventory
- Track printer status and locations
- Assign printers to users/departments
- Monitor printer usage and maintenance

### 🌍 Multi-Language Support
- English and Arabic languages
- RTL (Right-to-Left) support for Arabic
- Easy language switching from navbar
- Localized resource files

### ⚙️ System Settings
- Application name configuration
- Default language selection
- Theme selection (Light/Dark)
- Session timeout configuration
- Date and time format customization
- Email notification settings

## 🏗️ Architecture

### Project Structure

PrinterApp/
├── PrinterApp.Web/             # ASP.NET Core MVC Web Application
│   ├── Controllers/            # MVC Controllers
│   ├── Views/                  # Razor Views
│   ├── Resources/              # Localization Resources
│   ├── Authorization/          # Custom Authorization Handlers
│   └── wwwroot/               # Static files (CSS, JS, Images)
├── PrinterApp.Data/            # Data Access Layer
│   ├── Repositories/           # Repository Pattern Implementation
│   ├── UnitOfWork/            # Unit of Work Pattern
│   └── Migrations/            # Entity Framework Migrations
└── PrinterApp.Models/          # Domain Models
├── Entities/              # Database Entities
└── ViewModels/            # View Models

### Design Patterns Used
- **Repository Pattern**: Abstract data access logic
- **Unit of Work Pattern**: Manage transactions
- **Dependency Injection**: Loose coupling between components
- **MVC Pattern**: Separation of concerns
- **Policy-based Authorization**: Flexible permission checking

## 🚀 Getting Started

### Prerequisites
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (LocalDB or Express)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/)

### Installation

1. **Clone the repository**
```bash
git clone https://github.com/YOUR_USERNAME/PrinterApp.git
cd PrinterApp


1.1. Update Connection String

Open PrinterApp.Web/appsettings.json and update the connection string:
jsonDownloadCopy code{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=PrinterAppDb;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}

1. Restore NuGet Packages

bashDownloadCopy codedotnet restore

1. Apply Migrations

bashDownloadCopy codecd PrinterApp.Web
dotnet ef database update --project ../PrinterApp.Data

1. Run the Application

bashDownloadCopy codedotnet run --project PrinterApp.Web
The application will be available at: https://localhost:5001
👤 Default Users
Admin Account

* Email: admin@admin.com
* Password: Admin@123
* Access: Full system access

Manager Account

* Email: manager@test.com
* Password: Manager@123
* Access: Limited permissions (View & Edit Users)

📊 Database Schema
Main Tables

* AspNetUsers: User accounts (Identity)
* AspNetRoles: System roles
* Permissions: Main permissions (e.g., Users Management, Printers Management)
* PermissionRoles: Roles within permissions (View, Create, Edit, Delete, Manage)
* UserPermissions: User-Permission-Role assignments
* SystemSettings: Application settings
* Printers: Printer inventory (future implementation)

Relationships
Permission (1) ──────< (N) PermissionRole
     │                         │
     │                         │
     └──< UserPermission >──┘
              │
              │
         ApplicationUser

🔧 Configuration
Adding New Permissions

1. Navigate to Permissions → Create New Permission
2. Enter permission details (Name, Code, Description)
3. Click Roles to add roles (View, Create, Edit, Delete, Manage)

Assigning Permissions to Users

1. Navigate to User Management
2. Click Permissions next to the user
3. Select permissions and their roles
4. Click Save Permissions

Using Permissions in Code
In Controllers:
csharpDownloadCopy code[Authorize(Policy = "Permission.USERS.Manage")]
public class UserManagementController : Controller
{
    // Only users with USERS.Manage permission can access
}

[Authorize(Policy = "Permission.PRINTERS.View")]
public class PrintersController : Controller
{
    // Only users with PRINTERS.View permission can access
}
In Views:
htmlDownloadCopy code@if (User.IsInRole("Admin"))
{
    <!-- Admin-only content -->
}
🌐 Localization
Switching Languages

* Click EN or AR buttons in the navbar
* Language preference is saved in cookies

Adding New Translations

1. Open Resources/SharedResources.en.resx
2. Add new key-value pair (English)
3. Open Resources/SharedResources.ar.resx
4. Add same key with Arabic translation

Using in Views
htmlDownloadCopy code@inject IStringLocalizer<SharedResources> Localizer

<h1>@Localizer["Home"]</h1>
🎨 Theming
The application uses custom CSS with support for:

* Light theme (default)
* Dark theme (configurable in Settings)
* RTL support for Arabic
* Responsive design
* Custom print styles for printer management

📦 Dependencies
Main Packages

* Microsoft.AspNetCore.Identity.EntityFrameworkCore (8.x)
* Microsoft.EntityFrameworkCore.SqlServer (8.x)
* Microsoft.EntityFrameworkCore.Tools (8.x)

Development Tools

* Entity Framework Core CLI
* SQL Server Management Studio (optional)

🧪 Testing
To run tests (if available):
bashDownloadCopy codedotnet test
🗺️ Roadmap
Phase 1 (Current) ✅

*  User authentication and authorization
*  Permission management system
*  Multi-language support
*  System settings

Phase 2 (Next)

*  Printer inventory management
*  Printer status tracking
*  Department management
*  Printer assignment to users/departments

Phase 3 (Future)

*  Print job monitoring
*  Usage reports and analytics
*  Maintenance scheduling
*  Cost tracking per printer
*  Email notifications for low toner/paper
*  API for mobile applications

📝 License
This project is licensed under the MIT License - see the LICENSE file for details.
👨‍💻 Author
Your Name

* GitHub: @ahmed70087

🤝 Contributing
Contributions are welcome! Please feel free to submit a Pull Request.

1. Fork the project
2. Create your feature branch (git checkout -b feature/PrinterFeature)
3. Commit your changes (git commit -m 'Add printer tracking feature')
4. Push to the branch (git push origin feature/PrinterFeature)
5. Open a Pull Request

📞 Support
If you have any questions or issues, please open an issue on GitHub.
🙏 Acknowledgments

* ASP.NET Core Team
* Entity Framework Core Team
* Bootstrap Team
* All contributors

📸 Screenshots
Dashboard

Permissions Management

User Management

Settings

Arabic Support (RTL)


⭐ If you find this project useful, please give it a star!
🔖 Tags
printer-management asp-net-core entity-framework permission-system multi-language arabic-support rtl identity authorization

## Step 3: تحديث appsettings.Example.json

**PrinterApp.Web/appsettings.Example.json**
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=PrinterAppDb;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}

