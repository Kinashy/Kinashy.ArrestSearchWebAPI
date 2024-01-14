#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM sergeydz/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Kinashy.ArrestSearchWebAPI/Kinashy.ArrestSearchWebAPI.csproj", "Kinashy.ArrestSearchWebAPI/"]
COPY ["/Samba/Samba.csproj", "Samba/"]
RUN dotnet restore "Kinashy.ArrestSearchWebAPI/Kinashy.ArrestSearchWebAPI.csproj"
COPY . .
WORKDIR "/src/Kinashy.ArrestSearchWebAPI"
RUN dotnet build "Kinashy.ArrestSearchWebAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Kinashy.ArrestSearchWebAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Kinashy.ArrestSearchWebAPI.dll"]

ENV TZ=+07:00
RUN ln -snf /usr/share/zoneinfo/$TZ /etc/localtime && echo $TZ > /etc/timezone