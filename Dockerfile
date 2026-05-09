FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine

RUN apk add --no-cache bash git curl

WORKDIR /app

EXPOSE 8080

CMD ["sh"]