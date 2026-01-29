# -------- Build stage --------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
# 1) Önce csproj kopyala (cache için)
COPY *.csproj ./
RUN dotnet restore
# 2) Sonra tüm kaynak kodu kopyala
COPY . ./
# 3) Publish al (çalışır paket)
RUN dotnet publish -c Release -o /app/publish
# -------- Runtime stage --------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish ./
# Container içi port
EXPOSE 8080
# Kestrel'i 8080'de dinlet (opsiyonel ama net olsun diye iyi)
ENV ASPNETCORE_URLS=http://+:8080
# ❗ Burayı kendi DLL adına göre değiştir
ENTRYPOINT ["dotnet", "Frontend.dll"]