FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["OrderConsumerService.Worker/OrderConsumerService.Worker.csproj", "OrderConsumerService.Worker/"]
COPY ["OrderConsumerService.Application/OrderConsumerService.Application.csproj", "OrderConsumerService.Application/"]
COPY ["OrderConsumerService.Domain/OrderConsumerService.Domain.csproj", "OrderConsumerService.Domain/"]
COPY ["OrderConsumerService.Infrastructure/OrderConsumerService.Infrastructure.csproj", "OrderConsumerService.Infrastructure/"]
RUN dotnet restore "OrderConsumerService.Worker/OrderConsumerService.Worker.csproj"
COPY . .
WORKDIR "/src/OrderConsumerService.Worker"
RUN dotnet build -c Release -o /app/build
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:80
ENTRYPOINT ["dotnet", "OrderConsumerService.Worker.dll"]
