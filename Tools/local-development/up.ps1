docker network create retailimnetwork

echo "Build MySQL Server"
.\Tools\ServerMySql\BuildServerAndRun.ps1


echo "build rabbitMQ"
.\Tools\RabbitMQ\BuildBusAndRun.ps1


echo ".............Completed................"
echo "MySqlServer running in localhost:4306"
echo "RabbitMQ running in http://localhost:15672/"