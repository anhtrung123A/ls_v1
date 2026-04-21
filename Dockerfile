FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine

RUN apk add --no-cache bash git curl

WORKDIR /src

EXPOSE 8081

CMD ["sh"]
