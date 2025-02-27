# OPSS Primary Authority Register
The Primary Authority Register (PAR) is a secure service supporting the operation of Primary Authority (PA), 
which allows businesses to receive assured and tailored advice to meet regulations through either direct or 
co-ordinated partnerships with local authorities, including fire and rescue authorities.

Local regulators can use this service to view, share or manage information on these partnerships, guiding 
enforcement work across the UK. Businesses or organisations representing them can view the details of their 
specific partnerships.

## Docker Desktop Setup:
the database container's hostname needs to be pointed to localhost, so run:
```
Add-Content -Path $env:windir\System32\drivers\etc\hosts -Value "`n127.0.0.1`tdb" -Force
```
This will add the nessasary entry to the hosts file. (This is primarily required for database migrations)

-------------

I ran into a few problems getting the docker-compose to execute from Visual Studio. The following describes
the error messages I receieved and how I resolved them:

### Volume sharing is not enabled
#### Step 1: Enable Windows Subsystem for Linux (WSL)
Run the following command with administrator privileges in cmd or Powershell:


`dism.exe /online /enable-feature /featurename:Microsoft-Windows-Subsystem-Linux /all /norestart`

- `dism.exe`: Windows Deployment Image Service Management Tool, a tool for managing Windows features.
- `/online`: Performs the operation on the currently running operating system.
- `/enable-feature`: Enables the specified feature, in this case, WSL.
- `/featurename`:Microsoft-Windows-Subsystem-Linux: Specifies the name of the feature to enable, which is WSL.
- `/all`: Performs the operation on all available features.
- `/norestart`: Prevents the system from restarting after completing the operation.

#### Step 2: Enable Virtual Machine Platform
Run the following command with administrator privileges:

`dism.exe /online /enable-feature /featurename:VirtualMachinePlatform /all /norestart`

#### Step 4: Download Linux Kernel Update Package
Run the following commands with administrator privileges:

`wsl.exe --install`

`wsl.exe --update`

#### Step 4: Set WSL 2 as Default Version
Run the following command with administrator privileges:

`wsl --set-default-version 2`

#### Step 5: Configure Docker Desktop
In Docker Desktop, go to Settings > General > Check "Use the WSL 2 based engine."

#### Step 6: Restart Your Computer
#### Step 7: Run Docker Compose Again
After completing all the steps, restart your computer and try running Docker Compose with your volume.

## Database Migrations
- Set your starup project to `Opss.PrimaryAuthorityRegister.Api`. 
- Then using the `Package Manager Console` set the default project to `Api\Infrastructure\Opss.PrimaryAuthorityRegister.Persistance`.
- Create the migration: `Add-Migration <Migration Name>`
- You do not need to apply the migration `Update-Database` as this will be done automatically when you run the solution

See the README.md in `Opss.PrimaryAuthorityRegister.Api.Persistance\Migrations` for information on customising migrations.