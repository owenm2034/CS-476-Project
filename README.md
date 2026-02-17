## Setup:
- Ensure .NET 9 is installed on your machine [here](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)
- run `dotnet dev-certs https --trust`
- In Visual Studio Code, press Ctrl+F5 to run the app without debugging.


### DB Setup:
- Ensure db is started by calling `docker compose -f compose.yaml --all-resources up` at
the root of the project

Create database if it does not exist:
- run `docker exec -it room2room "bash"` in terminal
    - allows you to access bash in container
- run `/opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P 'aStrong\!Passw0rd' -C` in the container
- run `IF DB_ID('Room2Room') IS NULL CREATE DATABASE [Room2Room];`
- create all tables from Database/Tables
- run `exit` to leave MSSQL
- run `exit` to leave the container