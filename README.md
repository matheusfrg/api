# api
# Instalação do docker
https://www.docker.com/

# Instação da imagem do SQL Server 2019
https://learn.microsoft.com/pt-br/sql/linux/quickstart-install-connect-docker?view=sql-server-ver15&pivots=cs1-bash

# Baixar a imagem do SQL Server Docker
docker pull mcr.microsoft.com/mssql/server:2019-latest

# Executar a imagem do docker
docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=@Sql2019" -p 1433:1433 --name sqlserver --hostname sql1 -d mcr.microsoft.com/mssql/server:2019-latest

# Pacotes Nuggets para utilização do EntityFramework

dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
