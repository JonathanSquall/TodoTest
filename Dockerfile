FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

#COPY ["TodoTest/TodoTest.csproj", "."]
#RUN dotnet restore
#COPY . .
#RUN dotnet publish -c Release -o /app/publish

COPY TodoTest/TodoTest.csproj TodoTest/
COPY TodoTest.Client/TodoTest.Client.csproj TodoTest.Client/

# Restore
RUN dotnet restore TodoTest/TodoTest.csproj

COPY . .

# Publish
RUN dotnet publish TodoTest/TodoTest.csproj -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "TodoTest.dll"]