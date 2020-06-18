import json

appsettings = './appsettings.json'

def file_replacer(filename, replace_rules):
    with open(filename, 'r') as f:
        src = f.read()
    for original, target in replace_rules:
        src = src.replace(original, target)
    with open(filename, 'w+') as f:
        f.write(src)

def main():
    file_replacer(appsettings, [('localhost:5000','darrendanielday.club'), ('"IsServer": false','"IsServer": true')])


if __name__ == "__main__":
    main()