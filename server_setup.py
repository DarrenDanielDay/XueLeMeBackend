import json

appsettings = './appsettings.json'
startup = './Startup.cs'
dbinit = './Data/DbInitializer.cs'

def file_replacer(filename, replace_rules):
    with open(filename, 'r') as f:
        src = f.read()
    for original, target in replace_rules:
        src = src.replace(original, target)
    with open(filename, 'w+') as f:
        f.write(src)

def main():
    file_replacer(appsettings, [('port=3333', 'port=3306'),('localhost:5000','darrendanielday.club')])
    file_replacer(dbinit, [('context.Database.EnsureDeleted();','')])
    file_replacer(startup,[('Version(8, 0, 3)','Version(5, 7, 29)')] )

if __name__ == "__main__":
    main()