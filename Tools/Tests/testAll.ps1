#Run unit test
echo "Execute Unit test"
cd .\test\RetailIM.UnitTest\
dotnet test
cd ..\..


#Build Sql server for Integration tests
echo "Build MySQL Server"
.\Tools\ServerMySql\BuildServerAndRun.ps1

echo "Build MySQL RabbitMQ"
.\Tools\RabbitMQ\BuildBusAndRun.ps1

echo "Wait a few seconds (15) before execute the integration tests (get the DB ready)";
Start-Sleep -Seconds 15

#Execute integration tests
echo "Execute integration test"
cd .\test\retailIM.IntegrationTest\
dotnet test
cd ..\..