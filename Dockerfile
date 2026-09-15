# -------------------------------------
# Migrator (temporary, until there is a WebAPI)
# -------------------------------------
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS migrator

WORKDIR /app
COPY . .

WORKDIR /app/src/LedgerLiteUsersAPI

RUN dotnet restore LedgerLiteUsersAPI.slnx

RUN dotnet tool install --global dotnet-ef --version 10.0.12
ENV PATH="$PATH:/root/.dotnet/tools"

ENTRYPOINT [ "dotnet", "ef", "database", "update", "--project", "Users.Persistence" ]