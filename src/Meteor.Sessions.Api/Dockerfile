FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/Meteor.Sessions.Api/Meteor.Sessions.Api.csproj", "src/Meteor.Sessions.Api/"]
RUN dotnet restore "src/Meteor.Sessions.Api/Meteor.Sessions.Api.csproj"
COPY . .
WORKDIR "/src/src/Meteor.Sessions.Api"
RUN dotnet build "Meteor.Sessions.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Meteor.Sessions.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Meteor.Sessions.Api.dll"]
