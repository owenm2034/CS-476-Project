import json

# write from json to a big text list
with open(r"helpers/universityDomains/world_universities_and_domains.json", 'r') as file:
    data = json.load(file)
    with open(r"Database/Migrations/AddUniversities.sql", "w") as output:
        lineCount = 0
        output.write('SET NOCOUNT ON\nGO\n')
        output.write('IF EXISTS (SELECT 1 FROM Universities) BEGIN\n')
        output.write("PRINT('Adding Universities...')\n")
        output.write('INSERT INTO Universities SELECT * FROM (VALUES')
        for i,obj in enumerate(data):            
            name = obj["name"]
            output.write(f"\t('{obj["domains"][0]}','{name.replace("'", "''")}')")
            lineCount += 1
            
            if (i != len(data) - 1):
                output.write(',\n')


        output.write(') v(Name,Domain);\n')
        output.write('END\n')
        output.write("ELSE PRINT('Universities already added')")