# Distributed .NET System

The application is built using `NetCore 3.1` and `Docker` is needed to run it correclty.

## Architecture  of the aplication
![Arquitecture of the application](https://www.netmentor.es/imagen/61e7a9ff-8fbf-4afc-a6c6-4f37c89483a4.jpg)


The architecture consists in an API (`RetailIM.WebApi`) which is an event generator for the queues.

The queues are divided by its name like `{order-creation-queue}` and the response queue with `{order-creation-queue}-{response}`.

Each API endpoint contains a different pair of queues. 

The Queues are hosted/Managed in RabbitMQ.


A background worker per endpoint/flow is located in the order microservice (`RetailIM.OrderMS`) and it will trigger the service flow and interact with the database.
A response to `{queue-name}-{response}` is made at this point.

The API which was waiting for the response receives it and shows it to the user. 




## Test
To run the tests  from the `root` folder execute the next script: `.\Tools\Tests\testAll.ps1` you need to have `Docker` installed for the execution of the Integration test.

In case that you don't have docker, you can install `MySQL` in your own machine and update the `connectionstring`.
You have to install as well `RabbitMQ` in order to be able to use the messagebus.


### Test_Full_Order_Workflow
There is a test into the integration tests which checkes all the workflow in the next order:
1. Create order
2. Update delivery address
3. Update products 
4. List all orders
5. Get Order
6. Delete the order


## Development
To be able to debug correctly `RabbitMQ` and `MySql` should be installed in the system. I use the `Docker` approach.

To build Containers with `RabbitMQ` and `MySQL` simply execute the next script `.\Tools\local-development\up.ps1` once completed the next endpoints will be available:
* MySqlServer running in localhost:4306
* RabbitMQ running in http://localhost:15672/



## Production-ready
To simulate a production environment execute in the root of the repositroy the next command `docker-compose up`.

Once it finish, the next endpoints will be availables 
* WebAPI: http://localhost:8080
* OrderMS: http://localhost:8180
* RabbitMQ Admin: http://localhost:15672
* MySql: localhost:4306

Note: Two products are inserted along with the database in the product table (100 free stock). 
- `331866d3-25f7-425f-9c75-90f21f5a606c` 
- `491b9c74-db55-4d21-a377-409903e5d30f`

## API Endpoints
The api contains the next endpoints

### Create order
#### Request
POST http://localhost:8080/order/create

Body:
````
{
	"Delivery" : {
		"Country" : "IE",
		"City": "DUB",
		"Street" : "Street name"
	},
	"Products" : [
			{
				"ProductId" : "331866d3-25f7-425f-9c75-90f21f5a606c",
				"Quantity" : 1
			},
			{
				"ProductId" : "491b9c74-db55-4d21-a377-409903e5d30f",
				"Quantity" : 1
			}
		]
}
````
#### Response
````
{
    "error": null,
    "result": {
        "orderId": "117dacc3-a46f-4b00-8eae-d79a966715f9"
    }
}
````

### Update order
#### Request
PUT http://localhost:8080/order/updateproducts/117dacc3-a46f-4b00-8eae-d79a966715f9

Body:
````
[{
	"ProductId" : "331866d3-25f7-425f-9c75-90f21f5a606c",
	"Quantity" : 8
},
{
	"ProductId" : "491b9c74-db55-4d21-a377-409903e5d30f",
	"Quantity" : 6
}]
````

#### Response:
````
{
    "error": null,
    "result": {
        "success": true
    }
}
````

### Update Delivery
#### Request
PUT http://localhost:8080/order/updatedelivery/117dacc3-a46f-4b00-8eae-d79a966715f9
Body:
````
{
	"Country": "IE",
	"City" : "Dublin",
	"Street" : "street 1"
}
````
#### Response:
````
{
    "error": null,
    "result": {
        "success": true
    }
}
````

### Get order
#### Request
Get http://localhost:8080/order/117dacc3-a46f-4b00-8eae-d79a966715f9
#### Response:
````
{
    "error": null,
    "result": {
        "order": {
            "orderId": "117dacc3-a46f-4b00-8eae-d79a966715f9",
            "creationDateUtc": "2020-10-26T17:31:59",
            "deliveryDto": {
                "country": "IE",
                "city": "Dublin",
                "street": "street 1"
            },
            "products": [
                {
                    "productId": "331866d3-25f7-425f-9c75-90f21f5a606c",
                    "name": "produt",
                    "quantity": 8
                },
                {
                    "productId": "491b9c74-db55-4d21-a377-409903e5d30f",
                    "name": "peeetaaaadl",
                    "quantity": 6
                }
            ]
        }
    }
}
````

### Get List of order paginated
#### Request
GET http://localhost:8080/order/page/1

#### Response
````
{
    "error": null,
    "result": {
        "orders": [
            {
                "orderId": "117dacc3-a46f-4b00-8eae-d79a966715f9",
                "creationDateUtc": "2020-10-26T17:31:59",
                "deliveryDto": {
                    "country": "IE",
                    "city": "Dublin",
                    "street": "street 1"
                },
                "products": [
                    {
                        "productId": "331866d3-25f7-425f-9c75-90f21f5a606c",
                        "name": "produt",
                        "quantity": 8
                    },
                    {
                        "productId": "491b9c74-db55-4d21-a377-409903e5d30f",
                        "name": "peeetaaaadl",
                        "quantity": 6
                    }
                ]
            }
        ]
    }
}
````
### Delete order
#### Request
Delete http://localhost:8080/order/117dacc3-a46f-4b00-8eae-d79a966715f9

#### Response
````
{
    "error": null,
    "result": {
        "success": true
    }
}
````
