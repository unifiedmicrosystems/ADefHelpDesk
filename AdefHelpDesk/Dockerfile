#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

## START: get nodejs
FROM node:latest AS node_base

RUN echo "NODE Version:" && node --version
RUN echo "NPM Version:" && npm --version
## END: get nodejs



FROM mcr.microsoft.com/dotnet/core/aspnet:2.1-stretch-slim AS base

COPY --from=node_base . .
WORKDIR /app
EXPOSE 80
EXPOSE 443



FROM mcr.microsoft.com/dotnet/core/sdk:2.1-stretch AS build

COPY --from=node_base . .
WORKDIR /src
COPY ["AdefHelpDesk/AdefHelpDeskBase.csproj", "AdefHelpDesk/"]
RUN dir
RUN dotnet restore "AdefHelpDesk/AdefHelpDeskBase.csproj"
COPY . .
WORKDIR "/src/AdefHelpDesk"
RUN dir
RUN dotnet build "AdefHelpDeskBase.csproj" -c Release -o /app/build



FROM build AS publish
RUN dotnet publish "AdefHelpDeskBase.csproj" -c Release -o /app/publish



FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
RUN dir
ENTRYPOINT ["dotnet", "AdefHelpDeskBase.dll"]