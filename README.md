# agilemind1
Mind1 csapat

https://picbookweb20171121053051.azurewebsites.net

Telepítés:

1) appsettings beállítása Web projektben (db connection string Connections/DefaultConnection-be)
2) sslcert/self-signed-cert.pfx Web projektbe berakása
3) Update-Database parancs kiadása entityframework csomagban

appsettings(.development).json
{
  "Logging": {
    "IncludeScopes": false,
    "LogLevel": {
      "Default": "Debug",
      "System": "Information",
      "Microsoft": "Information"
    }
  },
  "Authentication": {
    "Facebook": {
      "AppId": "xxx",
      "AppSecret": "xxx"
    },
    "Google": {
      "client_id": "xxx",
      "client_secret": "xxx"
    },
    "Twitter": {
      "ConsumerKey": "xxx",
      "ConsumerSecret": "xxx"
    },
    "Microsoft": {
      "ApplicationId": "xxx",
      "Password": "xxx"
    }
  },
  "AzureStorage": {
    "ConnectionString": "xxx"
  },
  "Tag": {
    "ConnectionString": "xxx"
  },
  "Connections": {
    "DefaultConnection": "xxx"
  }
}
