FROM openfaas/of-watchdog:0.8.0 as watchdog

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 as tester

ENV DOTNET_CLI_TELEMETRY_OPTOUT 1

# Optimize for Docker builder caching by adding projects first.

RUN mkdir -p /home/app/src/function/src
WORKDIR /home/app/src/function/src
COPY ./function/src/Function.fsproj  .

RUN mkdir -p /home/app/src/function/tests
WORKDIR /home/app/src/function/tests
COPY ./function/tests/FunctionTests.fsproj  .

WORKDIR /home/app/src/
COPY ./root.fsproj  .
RUN dotnet restore ./root.fsproj

COPY .  .

ENV ASPNETCORE_ENVIRONMENT Test
RUN dotnet test ./function/tests/FunctionTests.fsproj

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 as builder

ENV DOTNET_CLI_TELEMETRY_OPTOUT 1

# Optimize for Docker builder caching by adding projects first.

RUN mkdir -p /home/app/src/function/src
WORKDIR /home/app/src/function/src
COPY ./function/src/Function.fsproj  .

WORKDIR /home/app/src/
COPY ./root.fsproj  .
RUN dotnet restore ./root.fsproj

COPY .  .

RUN dotnet publish -c release -o published

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim

COPY --from=watchdog /fwatchdog /usr/bin/fwatchdog
RUN chmod +x /usr/bin/fwatchdog

# Create a non-root user
RUN addgroup --system app \
    && adduser --system --ingroup app app

WORKDIR /home/app/
COPY --from=builder /home/app/src/published .
RUN chown app:app -R /home/app

USER app

ENV fprocess="dotnet ./root.dll"
ENV cgi_headers="true"
ENV mode="http"
ENV ASPNETCORE_URLS=http://*:5000
ENV upstream_url="http://localhost:5000"
RUN ASPNETCORE_ENVIRONMENT=Production
EXPOSE 8080

HEALTHCHECK --interval=3s CMD [ -e /tmp/.lock ] || exit 1

CMD ["fwatchdog"]
