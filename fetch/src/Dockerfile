FROM public.ecr.aws/lambda/dotnet:6 AS base

FROM mcr.microsoft.com/dotnet/sdk:6.0-bullseye-slim as build
WORKDIR /src
COPY ["DailyTide.csproj", "DailyTide/"]
RUN dotnet restore "DailyTide/DailyTide.csproj"

WORKDIR "/src/DailyTide"
COPY . .
RUN dotnet build "DailyTide.csproj" --configuration Release --output /app/build

FROM build AS publish
RUN dotnet publish "DailyTide.csproj" \
            --configuration Release \ 
            --runtime linux-x64 \
            --self-contained false \ 
            --output /app/publish \
            -p:PublishReadyToRun=true  

FROM base AS final
WORKDIR /var/task
COPY --from=publish /app/publish .

CMD ["DailyTide::DailyTide.Function::FunctionHandler"]