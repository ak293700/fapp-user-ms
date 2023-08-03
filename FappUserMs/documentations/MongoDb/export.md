# Export

## Export a collection

```bash
mongoexport --host=localhost --port=27017 --username=root --password=password --db=users --collection=users --authenticationDatabase=admin --authenticationMechanism=SCRAM-SHA-256  --out=users.json
```