# ?? Guia de User Secrets - Gerenciamento Seguro de Configurações

## O que são User Secrets?

User Secrets é um recurso do .NET que permite armazenar informações sensíveis (como connection strings, senhas, chaves de API) **fora do código-fonte** durante o desenvolvimento.

## Por que usar User Secrets?

? **Segurança**: Evita commit acidental de senhas no Git  
? **Conveniência**: Fácil de configurar e usar  
? **Isolamento**: Cada desenvolvedor pode ter suas próprias configurações  
? **Padrão .NET**: Integrado ao ASP.NET Core Configuration  

## ?? Configuração Rápida

### 1. Inicializar User Secrets

```bash
cd PropostaService.Api
dotnet user-secrets init
```

Isso adiciona um `UserSecretsId` ao arquivo `.csproj`:

```xml
<PropertyGroup>
  <UserSecretsId>aspnet-PropostaService-12345678</UserSecretsId>
</PropertyGroup>
```

### 2. Adicionar Connection String

```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=PropostaServiceDb;Username=postgres;Password=postgres123"
```

### 3. Verificar Configuração

```bash
# Listar todos os secrets
dotnet user-secrets list

# Ver conteúdo do arquivo (Windows)
type %APPDATA%\Microsoft\UserSecrets\<UserSecretsId>\secrets.json

# Ver conteúdo do arquivo (Linux/Mac)
cat ~/.microsoft/usersecrets/<UserSecretsId>/secrets.json
```

## ?? Comandos Úteis

### Gerenciar Secrets

```bash
# Adicionar secret
dotnet user-secrets set "ChaveApi" "valor-secreto"

# Adicionar secret com JSON
dotnet user-secrets set "Smtp:Host" "smtp.gmail.com"
dotnet user-secrets set "Smtp:Port" "587"
dotnet user-secrets set "Smtp:User" "user@gmail.com"
dotnet user-secrets set "Smtp:Password" "senha"

# Listar secrets
dotnet user-secrets list

# Remover secret específico
dotnet user-secrets remove "ChaveApi"

# Limpar todos os secrets
dotnet user-secrets clear
```

### Localização dos Arquivos

**Windows:**
```
%APPDATA%\Microsoft\UserSecrets\<UserSecretsId>\secrets.json
```

**Linux/Mac:**
```
~/.microsoft/usersecrets/<UserSecretsId>/secrets.json
```

## ?? Estrutura do secrets.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=PropostaServiceDb;Username=postgres;Password=postgres123"
  },
  "Smtp": {
    "Host": "smtp.gmail.com",
    "Port": 587,
    "User": "user@gmail.com",
    "Password": "senha"
  },
  "ApiKeys": {
    "ExternalService": "key-123456"
  }
}
```

## ?? Como Funciona no Código

O ASP.NET Core carrega automaticamente os User Secrets em **ambiente de desenvolvimento**:

```csharp
// Program.cs - Não precisa de código adicional!
var builder = WebApplication.CreateBuilder(args);

// Os secrets já estão disponíveis em builder.Configuration
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
```

### Ordem de Precedência (do menor para o maior)

1. `appsettings.json`
2. `appsettings.{Environment}.json`
3. **User Secrets** (apenas em Development)
4. Environment Variables
5. Command-line arguments

Isso significa que User Secrets **sobrescreve** valores do appsettings.json!

## ?? Boas Práticas

### ? O QUE FAZER

1. **Use para informações sensíveis em desenvolvimento**
   ```bash
   dotnet user-secrets set "ConnectionStrings:DefaultConnection" "..."
   dotnet user-secrets set "ApiKeys:PaymentGateway" "..."
   ```

2. **Configure valores padrão no appsettings.json**
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Host=localhost;Database=MyDb"
     }
   }
   ```

3. **Documente as secrets necessárias**
   - Crie um `secrets.json.example` no repositório
   - Liste as configurações necessárias no README

4. **Use para cada projeto**
   ```bash
   cd PropostaService.Api
   dotnet user-secrets init
   
   cd ../ContratacaoService.Api
   dotnet user-secrets init
   ```

### ? O QUE NÃO FAZER

1. **Não use em produção**
   - User Secrets é **apenas para desenvolvimento**
   - Em produção, use Azure Key Vault, AWS Secrets Manager, etc.

2. **Não commite o UserSecretsId**
   - O ID pode ser commitado (está no .csproj)
   - O conteúdo dos secrets NÃO deve ser commitado

3. **Não use para configurações públicas**
   - Use appsettings.json para URLs, timeouts, etc.
   - Reserve secrets apenas para dados sensíveis

## ?? Migração de .env para User Secrets

Se você estava usando `.env`:

```bash
# Antes (.env)
DB_HOST=localhost
DB_PASSWORD=senha123

# Depois (User Secrets)
dotnet user-secrets set "Database:Host" "localhost"
dotnet user-secrets set "Database:Password" "senha123"
```

## ?? Produção

Para produção, **NÃO use User Secrets**. Use:

### Azure App Service

```bash
# Via Azure CLI
az webapp config appsettings set \
  --name myapp \
  --resource-group mygroup \
  --settings ConnectionStrings__DefaultConnection="..."
```

### Azure Key Vault

```csharp
// Program.cs
builder.Configuration.AddAzureKeyVault(
    new Uri($"https://{keyVaultName}.vault.azure.net/"),
    new DefaultAzureCredential());
```

### Docker

```bash
docker run -e ConnectionStrings__DefaultConnection="..." myapp
```

### Kubernetes

```yaml
apiVersion: v1
kind: Secret
metadata:
  name: app-secrets
type: Opaque
data:
  connectionString: <base64-encoded-value>
```

## ?? Testes

Para testes, você pode:

```csharp
// Criar configuração in-memory
var inMemorySettings = new Dictionary<string, string>
{
    {"ConnectionStrings:DefaultConnection", "Host=localhost;Database=TestDb"}
};

var configuration = new ConfigurationBuilder()
    .AddInMemoryCollection(inMemorySettings)
    .Build();
```

## ?? Troubleshooting

### Secret não está sendo carregado

```bash
# 1. Verificar se está em Development
echo $ASPNETCORE_ENVIRONMENT  # Linux/Mac
echo %ASPNETCORE_ENVIRONMENT%  # Windows

# 2. Verificar UserSecretsId
cat PropostaService.Api/PropostaService.Api.csproj | grep UserSecretsId

# 3. Verificar secrets configurados
dotnet user-secrets list

# 4. Reiniciar aplicação
```

### Arquivo secrets.json corrompido

```bash
# Limpar e reconfigurar
dotnet user-secrets clear
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "..."
```

### Múltiplos projetos compartilhando secrets

```xml
<!-- Adicione o mesmo UserSecretsId em ambos os .csproj -->
<PropertyGroup>
  <UserSecretsId>shared-secrets-id</UserSecretsId>
</PropertyGroup>
```

## ?? Referências

- [Safe storage of app secrets in development in ASP.NET Core](https://docs.microsoft.com/aspnet/core/security/app-secrets)
- [Configuration in ASP.NET Core](https://docs.microsoft.com/aspnet/core/fundamentals/configuration/)
- [Azure Key Vault configuration provider](https://docs.microsoft.com/aspnet/core/security/key-vault-configuration)

## ? Exemplo Completo

```bash
# 1. Inicializar
cd PropostaService.Api
dotnet user-secrets init

# 2. Configurar database
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5432;Database=PropostaServiceDb;Username=postgres;Password=postgres123"

# 3. Configurar outros secrets
dotnet user-secrets set "Jwt:SecretKey" "my-super-secret-key-12345"
dotnet user-secrets set "Jwt:Issuer" "PropostaService"
dotnet user-secrets set "Jwt:Audience" "PropostaServiceClients"

# 4. Verificar
dotnet user-secrets list

# 5. Executar aplicação
dotnet run
```

---

**Dica:** Sempre que trabalhar em um novo computador ou clonar o repositório, execute `dotnet user-secrets init` e configure novamente seus secrets!
