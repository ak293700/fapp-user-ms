# Import

## Import a collection

### Using host, port, username and password

```bash
mongoimport --host=localhost --port=27017 --username=root --password=password --db=users --collection=users --authenticationDatabase=admin --authenticationMechanism=SCRAM-SHA-256 --file=users.json
```

### Using uri

```bash
mongoimport --uri="mongodb://root:password@localhost:27017/" --db=users --collection=users --authenticationDatabase=admin --authenticationMechanism=SCRAM-SHA-256 --file=users.json
```