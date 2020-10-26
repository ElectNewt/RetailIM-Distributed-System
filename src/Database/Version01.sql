
CREATE DATABASE IF NOT EXISTS `retailim`;
USE `retailim`;
CREATE USER 'personaluser' IDENTIFIED BY 'personalpass';
GRANT ALL PRIVILEGES ON * . * TO 'personaluser';

CREATE TABLE `product` (
	`productid` VARCHAR(36) NOT NULL,
	`name` VARCHAR(50) NOT NULL,
	`totalstock` INT NOT NULL,
	`allocatedstock` INT NOT NULL,
	PRIMARY KEY (`productid`)
);

CREATE TABLE `individualorder` (
	`orderid` VARCHAR(36) NOT NULL,
	`creationtimeutc` DATETIME NOT NULL,
	PRIMARY KEY (`orderid`)
);

CREATE TABLE `orderproducts` (
	`orderid` VARCHAR(36) NOT NULL,
	`productid` VARCHAR(36) NOT NULL,
	`quantity` INT NULL,
	PRIMARY KEY (`orderid`, `productid`)
);

CREATE TABLE `delivery` (
	`deliveryid` VARCHAR(36) NOT NULL,
	`country` VARCHAR(50) NULL,
	`city` VARCHAR(50) NULL,
	`street` VARCHAR(50) NULL,
	PRIMARY KEY (`deliveryid`)
);



INSERT INTO `retailim`.`product` (`productid`, `name`, `totalstock`, `allocatedstock`) VALUES ('331866d3-25f7-425f-9c75-90f21f5a606c', 'produt', '100', '0');
INSERT INTO `retailim`.`product` (`productid`, `name`, `totalstock`, `allocatedstock`) VALUES ('491b9c74-db55-4d21-a377-409903e5d30f', 'peeetaaaadl', '100', '0');