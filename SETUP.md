# How to Setup Project for Development

The following instructions have been developed to set up a development / trial environment for the Annotations project in a VM using Ubuntu 24.04.2 LTS. 

## 1. Get the correct .NET SDK

Run the following command to install .NET 8.0

```bash
sudo apt-get update && \
  sudo apt-get install -y dotnet-sdk-8.0
```

## 2. Get the latest Annotations project

For this next part, you will need Git.

```bash
sudo apt install git
```

Clone the latest version. I will clone it into `~/source`, a directory that I am creating for projects on this VM. 

```
git clone https://github.com/ITU-ISE2025-GROUP8-PT/Annotations.git
```

From here, we can start to prepare the solution. 

1. `dotnet restore`
2. `dotnet build`

_It took about 9 minutes to restore the project. This can largely be attributed to the use of Playwright for E2E tests._
## 3. HTTPS developer certificate

At this point, trust the developer HTTPS certificate. 

For Windows, OSX, Ubuntu and Fedora, there is simply a handy command: `dotnet dev-certs https --trust`

On Ubuntu, it is necessary to install `libnss3-tools` first, for `certutil` to be on the path.

```bash
sudo apt install libnss3-tools
```

Should the certificate not be trusted at any point, we can fix this as follows.

```bash
dotnet dev-certs https --clean
dotnet dev-certs https --trust
```

For more information, see [https://aka.ms/dotnet-https-linux](https://aka.ms/dotnet-https-linux)

## 4. Set up Azure Storage

### 4.1 Install

To develop the application, we have been using Azurite, the local Azure Storage emulator. This can be obtained over the npm package manager. 

```bash
sudo apt install npm
```

```bash
npm install -g azurite
```

For more information on how to run Azurite, see [https://learn.microsoft.com/en-us/azure/storage/common/storage-use-azurite](https://learn.microsoft.com/en-us/azure/storage/common/storage-use-azurite)

### 4.2 Run

Using the following command will start Azurite, _using the folder that the terminal is pointing to_ for storage. 

```bash
azurite run
```

### 4.3 Storage explorer

To manage storage during development, we are using Azure Storage Explorer. 

```bash
sudo snap install storage-explorer
```

### 4.4 Connection string

Azurite has a default connection string for access to the emulated storage.

```
DefaultEndpointsProtocol=http;AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;BlobEndpoint=http://127.0.0.1:10000/devstoreaccount1;QueueEndpoint=http://127.0.0.1:10001/devstoreaccount1;TableEndpoint=http://127.0.0.1:10002/devstoreaccount1;
```

- [ ] For `Annotations.API` you will need to set an Azure Storage connection string as a user secret. 

```bash
dotnet user-secrets set "AzureStorageConnection" "<ConnectionString>"
```

### 4.5 Setup blob containers

In the current version of the program, a single blob container must be created beforehand:

- `images`

This can be done using Azure Storage Explorer to access the storage. 

## 5. Set up an OIDC provider

We are using [Orchard Core](https://orchardcore.net/) during development to host a user base. This is fundamentally a CMS system, that also allows us to

- Register and manage users, including mailing and password recovery
- Setup and assign roles to users
- Have basic scope configuration for the roles
- Act as an OIDC authority with the ability to issue JWT tokens

In future development, Perfusion Tech will be able to select a commercial SSO provider, or other preferred solution. 

The following is an explanation of how to set up this service for localhost running. It is possible for a team of developers to share an online deployment of Orchard Core. The set up for this is done in the same way, except the base project is first deployed to a server. 

### 5.1 Obtain project templates

Obtain the latest version of the project templates. 

```bash
dotnet new install OrchardCore.ProjectTemplates
```

### 5.2 Create and set up Orchard Core CMS

In a new folder outside of the Annotations project, create an Orchard Core CMS project. This must be given some name other than OrchardCore. 

```bash
dotnet new occms -n OrchardUsers
```

Start the project using `dotnet run` or `dotnet watch`. 

This will download additional files to the project, and present the first time setup page.

![](docs/images/Pasted%20image%2020250507194249.png)

1. Give the site a name.
2. Select the "Headless site" recipe. 
3. Select a database. Here SQLite for local use. 
	1. Orchard Core also supports SQL Server, Postgres, and MySQL. This takes a normal connection string. 
4. Add details to create the super user. 
5. This user will have the `Administrator` role. 
	1. Note: Password recovery is an option to setup later, along with an email provider. Make sure then to keep the super user information. 
6. Finish setup

### 5.3 Prepare for use as OIDC provider

I can now login to the CMS using my super user. 

![](docs/images/Pasted%20image%2020250507194813.png)

Navigate to `Security -> OpenID Connect -> Settings -> Authorization Server`

![](docs/images/Pasted%20image%2020250507200509.png)

On this page, ensure the following:

1. "Authorization Code Flow" and "Refresh Token Flow" are allowed.
2. "Require Proof for Code Exchange" is set.
3. Set the "Authority", the base URL of the OIDC service. In this case: `https://localhost:5001/`
4. Set the "Token Format" to "JSON Web Token (JWT)"
5. Set "Disable Access Token Encryption"

- [ ] For `Annotations.API` you will need to set the authority as a user secret. 

```bash
dotnet user-secrets set "Authentication:jwt:authority" "https://localhost:5001/"
```

- [ ] For `Annotations.Blazor` you will need to set the authority as a user secret.

```bash
dotnet user-secrets set "Authentication:oidc:authority" "https://localhost:5001/"
```

![](docs/images/Pasted%20image%2020250507201233.png)

Press the button to update and reload the server settings.

### 5.4 Setup scopes for application

Navigate to `Security -> OpenID Connect -> Management -> Scopes`

![](docs/images/Pasted%20image%2020250507201418.png)

Here we must create a new scope that gives users access to the Annotations backend API. 

![](docs/images/Pasted%20image%2020250507201639.png)

It is most important that

1. "Name" is set to `api`
2. `https://localhost:7250` is added as an API audience

Save this scope, and continue with registering the application. 

- [ ] For `Annotations.API` you will need to set the audience as a user secret. 

```bash
dotnet user-secrets set "Authentication:jwt:audience" "https://localhost:7250"
```

### 5.5 Register Annotations as an application

Navigate to `Security -> OpenID Connect -> Management -> Scopes`

![](docs/images/Pasted%20image%2020250507204054.png)

1. Give the application a display name.
2. We will use the "Confidential Client" type. Tokens are delivered to our front end server, not directly to clients. 
3. The shuffle buttons on the very right of "Client Id" and "Client Secret" can be used to generate these app credentials. 
4. Set all of these checkboxes: 
	1. "Allow Authorization Code Flow", 
	2. "Allow Refresh Token Flow", 
	3. "Allow Logout Endpoint" and
	4. "Require Proof Key for Code Exchange". 
5. Set the "Redirect Uris" to `https://localhost:7238/signin-oidc`
6. Set the "Logout Redirect Uris" to `https://localhost:7238/signout-callback-oidc`
	1. This ensures correct redirection to the locally running Blazor service after a successful login.
7. Set "Consent type" to "Implicit consent". This is what our user base is for after all.
8. Set the allowed scopes. At a minimum: `email`, `profile`, `roles`, and `api`. 

- [ ] For `Annotations.Blazor` you will need to set the client ID as a user secret.

```bash
dotnet user-secrets set "authentication:oidc:clientid" "<client ID>"
```

- [ ] For `Annotations.Blazor` you will need to set the client secret as a user secret.

```bash
dotnet user-secrets set "authentication:oidc:clientsecret" "<client secret>"
```

Save the application settings.

### 5.6 Configure specific roles for Annotations

Apart from the `Administrator` super user role for Orchard Core, our project requires two additional roles to be configured:

1. `Manager`
2. `AnnotationsUser`

These are the roles for which RBAC is configured within Annotations. 

Navigate to `Security -> Roles`

![](docs/images/Screen%20Capture%202025-05-08%20195507.png)

Notice that there are already some roles defined, which are specific to CMS. 

1. Press "Add Role" to create a new role. 
2. Use this to create the `Manager` and the `AnnotationsUser` roles.

It will then be possible to assign these roles to users, using the user management interface within Orchard Core. 

## 6. Set up SQLite database connection

We are using an SQLite database to store blood vessel trees for local development. For `Annotations.API` you will need to set a user secret. An SQLite database will be created/used based on the connection string provided. I.e.:
```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "DataSource=C:\temp\annotations.db"
```

## 7. Start the app

1. Start Azurite in your storage data folder: `azurite run`
2. Ensure that Orchard Core is running: `dotnet run`
3. Start Annotations.API: `dotnet run` or `dotnet watch`
4. Start Annotations.Blazor: `dotnet run` or `dotnet watch`

It is now possible to run Annotations locally for testing. 

## 8. Supporting tools

### 8.1 `user-jwts`

This can be used with `Annotations.API` for debugging and testing endpoints using tokens created directly without Orchard Core. 

```shell
dotnet user-jwts create
```

Sets up the required user-secrets on first time use, and generates a token. 

Of course, sometimes it will be necessary to set certain claims within the token for debugging and testing. Here i.e. to create a test token with a certain user name and user ID:

```bash
dotnet user-jwts create --name DevsTestToken --claim "Sub=123abcdefg007"
```

