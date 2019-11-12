#Stage 1 - Build Phase
FROM mcr.microsoft.com/dotnet/core/sdk:2.2.402-bionic AS build
WORKDIR /src
COPY ["CatalogAPI.csproj", "./"]
RUN dotnet restore
COPY . .
RUN dotnet build -c Release -o /bin
RUN dotnet publish -c Release -o /dist

#Stage 2 - Runtime
FROM mcr.microsoft.com/dotnet/core/aspnet:2.2.7-bionic AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

ENV MongoSettings:ConnectionStrings=mongodb://megha-cosmos-mongodb:a7g9b8RLuxmygDRRUQrpTZ5bczDLuoND2a077OH0YULGRhvXhRBMSUxRQlWWeZsuFwsmDNUUvOuCBVdREM6hnw==@megha-cosmos-mongodb.mongo.cosmos.azure.com:10255/?ssl=true&replicaSet=globaldb&maxIdleTimeMS=120000&appName=@megha-cosmos-mongodb@&retrywrites=false
ENV MongoSettings:Database=CatalogDB
ENV ConnectionStrings:StorageConnection=DefaultEndpointsProtocol=https;AccountName=meghastorageaccount;AccountKey=PL7HTOJPqA5Gm1Q9loNEOoqAa9WhspFQR4/zUFtqhCJMpAIqgxhOONV0tC9Kf7KY5H74qS826w3tvqdJoI4IMQ==;EndpointSuffix=core.windows.net
ENV ConnectionStrings:TableConnection=DefaultEndpointsProtocol=https;AccountName=megha-cosmos-table;AccountKey=tZimfk21cWB0XjpPTJcjg1GB5zqfaih0ntFKOI71yNzzrLpZr7iAg0r0qNRhBeFvypnSkPaKaFSYsA4jtTChkA==;TableEndpoint=https://megha-cosmos-table.table.cosmos.azure.com:443/;

COPY --from=build /dist .
ENTRYPOINT ["dotnet", "CatalogAPI.dll"]

