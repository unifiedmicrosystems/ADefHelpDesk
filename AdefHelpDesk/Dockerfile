#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

# Use command below to display files and folders
# RUN echo "Displaying /" && cd / && dir

FROM node:latest AS node_base

RUN echo "NODE Version:" && node --version
RUN echo "NPM Version:" && npm --version



FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base

WORKDIR /app
EXPOSE 80
EXPOSE 443



FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build

COPY --from=node_base . .
WORKDIR /src
COPY . .
RUN dotnet restore "AdefHelpDeskBase.csproj"
RUN dotnet build "AdefHelpDeskBase.csproj" -c Release -o /app/build



FROM build AS publish

WORKDIR /src
RUN dotnet publish "AdefHelpDeskBase.csproj" -c Release -o /app/publish



FROM base AS final

WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AdefHelpDeskBase.dll"]