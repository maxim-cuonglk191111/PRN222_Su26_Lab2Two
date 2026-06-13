# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["BusinessObjects/BusinessObjects.csproj", "BusinessObjects/"]
COPY ["DataAccessObjects/DataAccessObjects.csproj", "DataAccessObjects/"]
COPY ["Repositories/Repositories.csproj", "Repositories/"]
COPY ["Services/Services.csproj", "Services/"]
COPY ["ProductManagementRazorPages/ProductManagementRazorPages.csproj", "ProductManagementRazorPages/"]

RUN dotnet restore "ProductManagementRazorPages/ProductManagementRazorPages.csproj"

COPY . .

RUN dotnet publish "ProductManagementRazorPages/ProductManagementRazorPages.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "ProductManagementRazorPages.dll"]
