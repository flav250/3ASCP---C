## Installation du projet IBayApi2
### Prérequis

Installation des outils :

- [Docker](https://docs.docker.com/desktop/install/windows-install/)
- [SDK 8.0.101](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) 

###  Installation des NuGet

- Microsoft.AspNetCore.OpenApi 8.0.0
- Swashbuckle.AspNetCore 6.4.0
- System.Linq.Dynamic.Core 1.3.8
- Microsoft.AspNetCore.Authentication.JwtBearer 8.0.1
- Microsoft.AspNetCore.Hosting.WindowsServices 8.0.1
- Microsoft.EntityFrameworkCore 8.0.1
- Microsoft.EntityFrameworkCore.SqlServer 8.0.1
- Microsoft.IdentityModel.Tokens 7.3.1

###  Création du container

Ouvrez un terminal où se situe le fichier docker-compose

```
cd IBayApi2
```

Faites la commande ci-dessous dans le terminal :

```
docker compose up -d
```

Cette commande va permettre de créer le container SQL Server.

Une fois le container créé, il vous suffira de vous connecter à SQL Server avec votre IDE :

###  Connexion à la Database

Voici les identifiants et mot de passe :

- Username : sa
- Password : 051203-Fl

Host et le port SQL Server :

- Host : localhost
- Port : 1433
 
Une fois la connexion établie, il vous suffit d'exécuter le [Script.sql](Data/script.sql) sur la branche localhost/master.

**Attention** : SQL Server ne doit pas être lancé mis à part sur docker pour éviter un conflit de port.

Il ne vous reste plus qu'à build le projet.

Pour essayer notre API, vous pouvez vous servir du fichier [IBayApi2.http](IBayApi2.http) pour toutes les requêtes nécessitantes une authentification, il vous suffit de modifier le token dans le fichier [http-client.private.env.json](http-client.private.env.json).

**Point très important :** Il vous faudra run le fichier [IBayApi2.http](IBayApi2.http) en environnement dev (situé en haut avec un menu déroulant).

Avec le lien, ci-dessous vous pourrez tester toutes les routes, mais aussi celle qui nécessite le token. 

Lien PostMan : https://app.getpostman.com/join-team?invite_code=4f2976ceb1ac76bd642d2e2cfaa4a2f5&target_code=6aaa3f24b4725544a03d18173c0d9744

**Attention** : Pensez à bien récupérer le token lorsque vous vous connectez à un compte pour tester les routes qui ont besoin d'un token. Ensuite quand vous êtes sur une requête, aller dans Authorization, dans le menu déroulant Type sélectionnez Bearer Token et rentrer le token que vous avez obtenu. Maintenant à vous de tester notre API.