## Setup:
- Ensure .NET 10 is installed on your machine [here](https://dotnet.microsoft.com/en-us/download/dotnet/10.0)
- run `dotnet dev-certs https --trust`
- In Visual Studio Code, press Ctrl+F5 to run the app without debugging.


### DB Setup:
- IF you previously had the room2room db running, run `docker compose down -v`
    - THIS WILL ERASE ALL DATA FROM YOUR LOCAL DATABASE
- Start DB by calling `docker compose -f compose.yaml --all-resources up` at
the root of the project
    - This will run `Database/init.sql`
    - This will run `Database/Migrations/AddUniversities.sql`
    - To add a migration script to setup see `compose.yml`


## Accounts
Db Setup generates these 5 accounts automatically
| Email | Password | IsAdmin | University |
| --- | --- | --- | --- |
| 476user@uregina.ca | cs476password | false | University of Regina |
| 476admin@uregina.ca | cs476password | true | University of Regina |
| 476user@usask.ca | cs476password | false | University of Saskatchewan |
| 476admin@usask.ca | cs476password | true | University of Saskatchewan |
| 476other@nagasaki-u.ac.jp | cs476password | false | Nagasaki University |


## Generate Publish Zip
Ensure you are in the root of the project
```bash
dotnet publish Room2Room/Room2Room.csproj -c Release -o ./site;
cd site;
zip -r ../Archive.zip .;
cd ..
rm -rf site;
```