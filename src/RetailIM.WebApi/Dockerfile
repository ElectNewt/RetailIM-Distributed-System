FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
ENV ASPNETCORE_ENVIRONMENT = "Production"


WORKDIR /app/
COPY src/RetailIM.WebApi/. ./RetailIM.WebApi/
COPY src/RetailIM.WebApi.Dispatcher/. ./RetailIM.WebApi.Dispatcher/
COPY src/RetailIM.Model/. ./RetailIM.Model/
COPY src/Shared.MessageBus/. ./Shared.MessageBus/
COPY src/Shared.Common/. ./Shared.Common/
COPY src/Shared.ROP/. ./Shared.ROP/
COPY src/RetailIM.Services/. ./RetailIM.Services/


#Build for release
WORKDIR /app/
RUN dotnet publish ./RetailIM.WebApi -c Release -o /app/publish


FROM base
WORKDIR /app/dist
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "RetailIM.WebApi.dll"]