# Export

## Export a collection

### Using host, port, username and password

```bash
mongoexport --host=localhost --port=27017 --username=root --password=password --db=users --collection=users --authenticationDatabase=admin --authenticationMechanism=SCRAM-SHA-256  --out=users.json
```

### Using uri

```bash
mongoexport --uri="mongodb://root:password@localhost:27017/" --db=users --collection=users --authenticationDatabase=admin --authenticationMechanism=SCRAM-SHA-256  --out=users.json
```