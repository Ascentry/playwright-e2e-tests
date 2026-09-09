**Configuration des tests Playwright**

### En développement : utilisation des User Secrets

À la racine du projet de tests utilisant la librairie basé sur Playwright (là où se trouve le fichier `.csproj`) :

```powershell
dotnet user-secrets init
dotnet user-secrets set "Playwright:Product" "YNFECTIO"
dotnet user-secrets set "Playwright:Culture" "en-US"
dotnet user-secrets set "Playwright:RootUrl" "http://localhost:4200"
dotnet user-secrets set "Playwright:HeadLess" "true"
dotnet user-secrets set "Playwright:SlowMo" "300"
dotnet user-secrets set "Playwright:Password" "password"
dotnet user-secrets set "Playwright:UserName" "username"
```

### Sur une VM : utilisation des variables d'environnement

Créer les variables d'environnement suivantes :

```powershell
set Playwright__Product=YNFECTIO
set Playwright__Culture=en-US
set Playwright__RootUrl=http://ynf2601mdbtest:10000
set Playwright__HeadLess=true
set Playwright__SlowMo=300
set Playwright__UserName=USER
set Playwright__Password=PASSWORD
```