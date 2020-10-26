FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
ENV ASPNETCORE_ENVIRONMENT = "Production"
WORKDIR /app/
COPY src/RetailIM.OrderMS/. ./RetailIM.OrderMS/
COPY src/RetailIM.OrderMS.BackgroundWorkers/. ./RetailIM.OrderMS.BackgroundWorkers/
COPY src/RetailIM.Model/. ./RetailIM.Model/
COPY src/RetailIM.Services/. ./RetailIM.Services/
COPY src/RetailIM.Data/. ./RetailIM.Data/
COPY src/RetailIM.ServicesDependencies/. ./RetailIM.ServicesDependencies/
COPY src/Shared.MessageBus/. ./Shared.MessageBus/
COPY src/Shared.Common/. ./Shared.Common/
COPY src/Shared.ROP/. ./Shared.ROP/
COPY src/Shared.Data/. ./Shared.Data/

#Build for release
WORKDIR /app/
RUN dotnet publish ./RetailIM.OrderMS -c Release -o /app/publish


FROM base
WORKDIR /app/dist
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "RetailIM.OrderMS.dll"]