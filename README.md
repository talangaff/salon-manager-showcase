# Salon Manager

> Showcase copy. Credentials, hostnames, branding and deployment details have been removed.

A tip-tracking and client color-history app for salon staff, built with ASP.NET Core 9 MVC, PostgreSQL (EF Core) and ASP.NET Identity.

## Features
- **Tip tracking** per employee.
- **Client records** with color and toner formula history (`Client`, `ClientColor`, `ClientToner`).
- **Admin / user roles**, with an admin screen to create users.
- **Security**: cookie auth, strong password policy (12+ characters), salted password hashes, and a filter that forces a password change on first login.

## Running it
1. Create a PostgreSQL database and set `ConnectionStrings:DefaultConnection` in `appsettings.json` (or via environment variables / user secrets).
2. Set `SeedAdminPassword` (environment variable or user secret) to create the initial `Admin` user, who must change it at first login.
3. `dotnet run`

No real data or credentials are included. Never commit real credentials.
