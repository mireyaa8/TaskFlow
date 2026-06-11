# TaskFlow

TaskFlow is an ASP.NET Core MVC project tracker inspired by Trello/Jira.

## Main Features

- ASP.NET Core 8.0 MVC
- Entity Framework Core with SQL Server
- ASP.NET Core Identity with a custom `ApplicationUser`
- Administrator role with seeded admin user
- Projects, boards, tasks, labels, comments, and project members
- Service layer for business logic
- API controller for AJAX task status updates
- Bootstrap-based responsive UI
- TempData success/error messages
- Custom error pages
- NUnit test project skeleton

## Default Admin Account

> Change this password before real deployment.

- Email: `admin@taskflow.local`
- Password: `Admin123!`

## Setup

1. Open the solution in Visual Studio 2022 or newer.
2. Set `TaskFlow.Web` as the startup project.
3. Update the connection string in `TaskFlow.Web/appsettings.json`.
4. Run migrations:

```bash
dotnet ef migrations add InitialCreate --project TaskFlow.Data --startup-project TaskFlow.Web
dotnet ef database update --project TaskFlow.Data --startup-project TaskFlow.Web
```

5. Run the app.

