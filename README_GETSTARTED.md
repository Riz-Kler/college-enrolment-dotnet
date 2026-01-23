# CollegeEnrolment (.NET 8) — College FE Enrolment + AI Analysis (Demo)

A lightweight ASP.NET Core MVC demo showing:
- Students, courses, course offerings, enrolments
- Reporting (capacity utilisation)
- AI Analysis area (historical outcomes + demand forecasting + success signals)

This is designed to be demo-friendly: clean UI, seeded data, and clear extension points for future AI/ML.

---

## Tech Stack

- .NET 8 (SDK)
- ASP.NET Core MVC
- EF Core (SQL Server)
- JavaScript charts (dashboard)
- Optional future: ML.NET / Azure OpenAI / Power BI embedding

---

## Prerequisites

### 1) Install .NET SDK
Install **.NET 8 SDK**.

Verify:
```bash
dotnet --version


2) EF Core CLI tools

Install EF Core tools (if not already):

dotnet tool install --global dotnet-ef


Verify:

dotnet ef --version

Configure Database

Update connection string in:

src/CollegeEnrolment.Web/appsettings.json (or appsettings.Development.json)

Example:

"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=CollegeEnrolment;Trusted_Connection=True;TrustServerCertificate=True;"
}

Build

From repo root:

dotnet build .\src\CollegeEnrolment.sln

Migrations + Database Update
Apply existing migrations:
dotnet ef database update ^
  -p .\src\CollegeEnrolment.Data\CollegeEnrolment.Data.csproj ^
  -s .\src\CollegeEnrolment.Web\CollegeEnrolment.Web.csproj

Create a new migration (only if you changed the data model):
dotnet ef migrations add <MigrationName> ^
  -p .\src\CollegeEnrolment.Data\CollegeEnrolment.Data.csproj ^
  -s .\src\CollegeEnrolment.Web\CollegeEnrolment.Web.csproj


Then apply:

dotnet ef database update ^
  -p .\src\CollegeEnrolment.Data\CollegeEnrolment.Data.csproj ^
  -s .\src\CollegeEnrolment.Web\CollegeEnrolment.Web.csproj

Run
dotnet run --project .\src\CollegeEnrolment.Web\CollegeEnrolment.Web.csproj


App runs (by default) on:

http://localhost:5024


Notes
Migration name already exists

If EF says a migration name is already used (e.g. AddStudentResults), that means it already exists in src/CollegeEnrolment.Data/Migrations.
Use a new name for the next migration (e.g. AddStudentResultsV2) or delete the unused migration only if it was never applied.

