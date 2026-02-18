import json

# write from json to a big text list
with open(r"helpers/universityDomains/world_universities_and_domains.json", 'r') as file:
    data = json.load(file)
    with open(r"Database/Migrations/AddUniversities.sql", "w") as output:
        lineCount = 0
        for i,obj in enumerate(data):
            if lineCount % 999 == 0:
                output.write("Insert into Universities (Domain, Name) VALUES\n")
                lineCount += 1
                
            
            name = obj["name"]
            output.write(f"\t('{obj["domains"][0]}','{name.replace("'", "''")}')")
            lineCount += 1
            if (i != len(data) - 1 and lineCount % 999 != 0) or lineCount == 0:
                output.write(',')
            elif lineCount % 999 == 0:
                output.write(';')
    
            output.write('\n')
            