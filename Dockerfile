# -------------------------------------
# Stage 01: Build
# -------------------------------------
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /app

# Copy the entire repository:
COPY . .

# Move to the solution directory:
WORKDIR /app/src/LedgerLiteUsersAPI

# Restore dependencies:
RUN dotnet restore LedgerLiteUsersAPI.slnx

# -------------------------------------
# Stage 02: Publish
# -------------------------------------
FROM build AS publish-api

# Publishes the Persistence project containing the API:
RUN dotnet publish Users.Persistence/Users.Persistence.csproj \
    -c Release \
    -o /app/publish/api \
    --no-restore

# -------------------------------------
# Stage 03: Runtime
# -------------------------------------
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS ledger-lite-users-api
WORKDIR /app
COPY --from=publish-api /app/publish/api .
EXPOSE 8080

ENTRYPOINT [ "dotnet", "Employee.Infrastructure.dll" ]