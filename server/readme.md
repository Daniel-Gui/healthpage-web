## BackEnd
1 - Instalar SDK .net 9 para desenvolvimento
	https://dotnet.microsoft.com/pt-br/download/dotnet/9.0

2 - Talvez seja preciso baixar algumas pacotes do EF (ou um ou outro)
	dotnet tool update --global dotnet-ef 
	dotnet tool install --global dotnet-ef

3 - Instalar o Postgres ou um Docker com Postgres

4 - Segredos e Super usuario inicial
	dotnet user-secrets init          #so rodar isso na primeira vez para criar o espa�o de segredos                   
	dotnet user-secrets set "Jwt:Key" "CHAVE_JWT_----DA_PRA_GEREAR_NA_NET"
	dotnet user-secrets set "ConnectionStrings:Default" "Host=localhost;Database=dbHealth;Username=...;Password=..."	#incluir username e senha que foi criado na sua maquina
	dotnet user-secrets set "SeedAdmin:Email" "..."				#definir um email do super usuario inicial
	dotnet user-secrets set "SeedAdmin:Password" "..."			#definir a senha do super usuario inicial
	dotnet user-secrets list									#Se precisar ver a lista de segredos
	  
5 - Rodar as Migrations 
	dotnet ef migrations add InitialIdentity         # So rodar esse se for criar uma migra��o nova
	dotnet ef database update						 # Rodar esse para criar o banco e todas suas tabelas


----------------------------------------- Rotas -------------------------------------------------
## Rotas de Autentica��o   
	## POST /api/auth/register		# Registrar um novo usuario
 JSON Body:
 {
   "Email": "string",
   "Password": "string",
   "ConfirmPassword": "string",
   "FullName": "string"
 }
 Response:
 {
	"token":"...",
	"expiresInSeconds":28799,
	"user":{
		"id":"5f061a73-1d6b-4d0c-9e28-d8b8ba8bc467",
		"userName":"email@exemplo.com",
		"email":"email@exemplo.com",
		"fullName":"Site Administrator",
		"isActive":true,
		"roles":["admin"]
		}
 }
 Erro:
 {
	"status":400,
	"code":"ValidationError",
	"title":"Erro de valida��o",
	"message":"Um ou mais campos est�o inv�lidos.",
	"errors":{
			"email":["E-mail inv�lido."]
			},
	"traceId":"0HNFHCTF8GL8I:00000006",
	"path":"/api/auth/register",
	"timestamp":"2025-09-11T19:32:55.3118863+00:00"
 }


	### POST /api/auth/login		# Login do usuario
JSON Body
{
    "UserNameOrEmail": "exemplo@exemplo.com",
    "Password": "senhaTOP1"
}
Response:
 {
	"token":"...",
	"expiresInSeconds":28799,
	"user":{
		"id":"5f061a73-1d6b-4d0c-9e28-d8b8ba8bc467",
		"userName":"email@exemplo.com",
		"email":"email@exemplo.com",
		"fullName":"Site Administrator",
		"isActive":true,
		"roles":["admin"]
		}
 }
 Erro:
 {
	"status":401,
	"code":"Unauthorized",
	"title":"N�o autenticado",
	"message":"Usu�rio ou senha inv�lidos.",
	"errors":null,
	"traceId":"0HNFHCTF8GL8I:00000009",
	"path":"/api/auth/login",
	"timestamp":"2025-09-11T19:35:51.0638249+00:00"
 }

	### POST /api/auth/me		# Para teste de autentica��o, retorna os dados do usuario logado
 Enviar com o Token Bearer
 JSON Body
 {
	"id":"e2c2d6e5-29e0-44a6-aae6-65795e4d3d30",
	"userName":"gabriel@gmail.com",
	"email":"gabriel@gmail.com",
	"fullName":"Gabriel Baia Brasil",
	"isActive":true,
	"createdAtUtc":"2025-09-11T18:46:51.996541Z",
	"roles":["user"]
  }

--------------------------------------------Roles
		Todas as rotdas de Roles precisam esta logadas com Admin
## Rotas de Roles
	### Get api/admin/roles/users/{id:guid}				#Obter Roles do usuaior
Subistituir {id:guid} pelo id do usuario
Response:
{
    "id": "e2c2d6e5-29e0-44a6-aae6-65795e4d3d30",
    "userName": "gabriel@gmail.com",
    "roles": [
        "user"
    ]
}
Erro:
{
    "status": 401,
    "code": "Unauthorized",
    "title": "N�o autenticado",
    "message": "Token ausente ou inv�lido.",
    "errors": null,
    "traceId": "0HNFHCTF8GL8I:0000000E",
    "path": "/api/admin/roles/users/e2c2d6e5-29e0-44a6-aae6-65795e4d3d30",
    "timestamp": "2025-09-11T19:42:51.6290484+00:00"
}

	### POST api/admin/roles/assign					#Cadastrar Roles de um usuaior

 JSON Body
{
    "userId": "e2c2d6e5-29e0-44a6-aae6-65795e4d3d30",
    "role": "Admin"
}
Response:
{
	"message":"Role atribu�da.",
	"userId":"e2c2d6e5-29e0-44a6-aae6-65795e4d3d30",
	"role":"doctor"
}
Erro:
{
	"status":400,
	"code":"BadRequest",
	"title":"Erro de valida��o",
	"message":"Role 'Admin1' n�o existe.",
	"errors":null,
	"traceId":"0HNFHCTF8GL8I:00000014",
	"path":"/api/admin/roles/assign",
	"timestamp":"2025-09-11T19:50:03.2592687+00:00"
}


	### POST api/admin/roles/remove					#Remover Roles de um usuaior

 JSON Body
{
    "userId": "e2c2d6e5-29e0-44a6-aae6-65795e4d3d30",
    "role": "Admin"
}
Response:
{
	"message":"Role removida.",
	"userId":"e2c2d6e5-29e0-44a6-aae6-65795e4d3d30",
	"role":"doctor"
}
Erro:
{
	"status":400,
	"code":"BadRequest",
	"title":"Erro de valida��o",
	"message":"Role 'Admin1' n�o existe.",
	"errors":null,
	"traceId":"0HNFHCTF8GL8I:00000014",
	"path":"/api/admin/roles/assign",
	"timestamp":"2025-09-11T19:50:03.2592687+00:00"
}