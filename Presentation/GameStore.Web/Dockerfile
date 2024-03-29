#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

#Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
#For more information, please see https://aka.ms/containercompat

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Presentation/GameStore.Web/GameStore.Web.csproj", "Presentation/GameStore.Web/"]
COPY ["Infrastructure/GameStore.MemoryStorage/GameStore.MemoryStorage.csproj", "Infrastructure/GameStore.MemoryStorage/"]
COPY ["Domain/GameStore/GameStore.csproj", "Domain/GameStore/"]
COPY ["Infrastructure/GameStore.Data.EF/GameStore.Data.EF.csproj", "Infrastructure/GameStore.Data.EF/"]
COPY ["Application/GameStore.Web.App/GameStore.Web.App.csproj", "Application/GameStore.Web.App/"]
COPY ["Plugins/GameStore.EmulateKassa/GameStore.EmulateKassa.csproj", "Plugins/GameStore.EmulateKassa/"]
RUN dotnet restore "Presentation/GameStore.Web/GameStore.Web.csproj"
COPY . .
WORKDIR "/src/Presentation/GameStore.Web"
RUN dotnet build "GameStore.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "GameStore.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GameStore.Web.dll"]