# Sessions microservice

Microservice responsible for managing employee sessions

## Migrations

To add migration run the following command:
```shell
dotnet ef migrations add {MIGRATION_NAME} --project src/Meteor.Sessions.Migrations --startup-project src/Meteor.Sessions.Api -n Meteor.Sessions.Migrations -o ./
```

## Docker Build/Push

To build the image run the following command
```shell
docker build -f src/Meteor.Sessions.Api/Dockerfile -t sgermonenko/meteor-sessions:{version} --build-arg NUGET_USER={USERNAME} --build-arg NUGET_PASSWORD={PASSWORD} .
```
where:
- {version} is microservice release version
- {USERNAME} is private nuget feed username
- {PASSWORD} is private nuget feed password
