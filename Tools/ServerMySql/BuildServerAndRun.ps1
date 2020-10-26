##Copy DB files
$source = "src/Database"
$destination = "Tools/ServerMysql"

Copy-Item -Path $source -Filter "*.sql" -Recurse -Destination $destination -Container -force

##Delete old image
docker rm $(docker stop $(docker ps -a -q --filter ancestor='server-mysql' --format="{{.ID}}"))


##Build image
docker build -t server-mysql Tools\ServerMysql\.

##Run container
docker run -d --network retailimnetwork --hostname mysqlserver -p 4306:3306 --name MySql server-mysql