# Import

## Import a collection

```bash
mongoimport --host=localhost --port=27017 --username=root --password=password --db=users --collection=users --authenticationDatabase=admin --authenticationMechanism=SCRAM-SHA-256 --file=users.json
```