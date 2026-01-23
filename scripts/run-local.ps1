$ErrorActionPreference = "Stop"

Write-Host "Building solution..." -ForegroundColor Cyan
dotnet build .\src\CollegeEnrolment.sln

Write-Host "Applying EF Core migrations..." -ForegroundColor Cyan
dotnet ef database update `
  -p .\src\CollegeEnrolment.Data\CollegeEnrolment.Data.csproj `
  -s .\src\CollegeEnrolment.Web\CollegeEnrolment.Web.csproj

Write-Host "Running web app..." -ForegroundColor Cyan
dotnet run --project .\src\CollegeEnrolment.Web\CollegeEnrolment.Web.csproj


## To run this script, open PowerShell and execute:
# .\scripts\run-local.ps1