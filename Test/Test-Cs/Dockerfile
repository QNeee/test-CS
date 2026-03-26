# --- Build Stage ---
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Копіюємо проектні файли
COPY *.csproj ./
COPY . ./

# Відновлюємо пакети
RUN dotnet restore

# Публікуємо в папку out
RUN dotnet publish -c Release -o out

# --- Runtime Stage ---
FROM mcr.microsoft.com/dotnet/runtime:8.0
WORKDIR /app

# Копіюємо опубліковані файли
COPY --from=build /app/out ./

# Копіюємо Data.json (велика буква!)
COPY --from=build /app/data.json ./data.json

# Вказуємо DLL для запуску
ENTRYPOINT ["dotnet", "Test-Cs.dll"]