$imageName = 'mcr.microsoft.com/mssql/server:2019-CU5-ubuntu-18.04'
$containerName = 'SqlDXContainer'

$filterValue = "^/$containerName$"
$containerCount = (docker container ls --all --filter name=$filterValue | Measure-Object).Count

if ($containerCount -gt 1) # >= 1 because docker outputs a header row
{
  # Container already exists
  docker container start `
  $containerName
}
else
{
  # Container does NOT exist

  docker image pull $imageName

  docker container run -d `
  -p 1433:1433 `
  --env ACCEPT_EULA=Y `
  --env MSSQL_SA_PASSWORD=P4ssw0rd! `
  --name $containerName `
  $imageName
}
