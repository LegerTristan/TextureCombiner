# Check that all unit tests are passed during prebuild of the application
Set-ExecutionPolicy Unrestricted -Scope Process

if ($PSScriptRoot) 
{

    # Se déplacer vers le répertoire du script
    Set-Location -Path $PSScriptRoot
}

dotnet test

Set-ExecutionPolicy Default -Scope Process