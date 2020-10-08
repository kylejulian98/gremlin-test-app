# Gremlin Test App

When setting up for development, you will need to add User Secrets to the Gremlin Test App project. The following settings need to be populated in the User Secrets:

```json
{
  "Gremlin": {
    "Host": "",
    "PrimaryKey": "",
    "Database": "",
    "Container": "",
    "Port": 443
  }
}
```

To add sample Vertices to your database you can use the following:

```
g.addV('person').property('name', <PersonName>).property('pk', <PartitionKey>)
```

To add a Edge to a Vertex you can use the following:

```
g.V(<Person1_Id>).addE('knows').to(g.V(<Person2_Id>))
```