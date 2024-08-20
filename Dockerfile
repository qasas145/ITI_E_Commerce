from mcr.microsoft.com/dotnet/sdk:8.0
WORKDIR /app
copy *.csproj . 
run dotnet restore
copy . .
run dotnet publish -c Release -o output 

EXPOSE 5158
EXPOSE 8080

CMD [ "dotnet",  "./output/ITIPROJECT.dll"]