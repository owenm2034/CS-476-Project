## Setup:
- Ensure .NET 9 is installed on your machine [here](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)
- run `dotnet dev-certs https --trust`
- In Visual Studio Code, press Ctrl+F5 to run the app without debugging.


### DB Setup:
- IF you previously had the room2room db running, run `docker compose down -v`
    - THIS WILL ERASE ALL DATA FROM YOUR LOCAL DATABASE
- Start DB by calling `docker compose -f compose.yaml --all-resources up` at
the root of the project