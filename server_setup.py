import json

appsettings = './appsettings.json'
startup = './Startup.cs'


with open(appsettings, 'r') as f:
    src = f.read()


with open(appsettings, 'w+') as f:
    src = src.replace('port=3333', 'port=3306')
    settings = json.loads(src)
    settings['Host'] = 'darrendanielday.club'
    f.write(json.dumps(settings))


with open(startup, 'r') as f:
    src = f.read()


with open(startup, 'w+') as f:
    local_version = 'Version(8, 0, 3)'
    server_version = 'Version(5, 7, 29)'
    f.write(src.replace(local_version, server_version))