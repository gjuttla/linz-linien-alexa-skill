FROM microsoft/dotnet:2.1.1-aspnetcore-runtime-alpine3.7
RUN apk update && apk add curl && rm -rf /var/cache/apk
ADD out /
ENTRYPOINT ["dotnet", "LinzLinienAlexaSkill.dll"]
