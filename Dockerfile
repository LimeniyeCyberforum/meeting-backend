FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/MeetingServer.Grpc/MeetingGrpc.Server/MeetingGrpc.Server.csproj", "src/MeetingServer.Grpc/MeetingGrpc.Server/"]
COPY ["src/MeetingServer.Grpc/MeetingGrpc.Protos/MeetingGrpc.Protos.csproj", "src/MeetingServer.Grpc/MeetingGrpc.Protos/"]
RUN dotnet restore "src/MeetingServer.Grpc/MeetingGrpc.Server/MeetingGrpc.Server.csproj"
COPY . .
WORKDIR "/src/src/MeetingServer.Grpc/MeetingGrpc.Server"
RUN dotnet build "MeetingGrpc.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MeetingGrpc.Server.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MeetingGrpc.Server.dll"]