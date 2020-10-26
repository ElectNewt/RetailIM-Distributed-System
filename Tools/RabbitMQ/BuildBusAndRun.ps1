##Delete old image
docker rm $(docker stop $(docker ps -a -q --filter ancestor='rabbitmq' --format="{{.ID}}"))

##Build image
docker build -t rabbitmq Tools\RabbitMQ\.

##Run container
docker run -d  --hostname rabbitmqhost -p 5672:5672 -p 15672:15672 --name rabbitmq rabbitmq

#http://localhost:15672/ guest:guest
#http://localhost:15672/ admin:admin

