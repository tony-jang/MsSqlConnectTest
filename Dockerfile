FROM mcr.microsoft.com/dotnet/sdk:6.0

WORKDIR /root
COPY . /root

RUN dotnet publish -r linux-x64 --self-contained false -c Release
WORKDIR /root/MsSqlConnectTest/bin/Release/net6.0/linux-x64/publish

CMD ["./MsSqlConnectTest"]
