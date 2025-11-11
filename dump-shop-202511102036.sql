-- MySQL dump 10.13  Distrib 8.0.19, for Win64 (x86_64)
--
-- Host: localhost    Database: shop
-- ------------------------------------------------------
-- Server version	9.5.0

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8mb4 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;
SET @MYSQLDUMP_TEMP_LOG_BIN = @@SESSION.SQL_LOG_BIN;
SET @@SESSION.SQL_LOG_BIN= 0;

--
-- GTID state at the beginning of the backup 
--

SET @@GLOBAL.GTID_PURGED=/*!80000 '+'*/ '35ff60c3-be3b-11f0-9ff5-dc4a3e4f2049:1-88';

--
-- Table structure for table `__efmigrationshistory`
--

DROP TABLE IF EXISTS `__efmigrationshistory`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `__efmigrationshistory` (
  `MigrationId` varchar(150) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `ProductVersion` varchar(32) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (`MigrationId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `__efmigrationshistory`
--

LOCK TABLES `__efmigrationshistory` WRITE;
/*!40000 ALTER TABLE `__efmigrationshistory` DISABLE KEYS */;
/*!40000 ALTER TABLE `__efmigrationshistory` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `categories`
--

DROP TABLE IF EXISTS `categories`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `categories` (
  `Id` varchar(36) NOT NULL,
  `Name` varchar(255) NOT NULL,
  `ParentCategoryId` varchar(36) DEFAULT NULL,
  `Description` text,
  `ProfitMarginTarget` decimal(5,2) DEFAULT NULL,
  `CreatedAt` datetime NOT NULL,
  `CreatedBy` varchar(36) NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `ParentCategoryId` (`ParentCategoryId`),
  KEY `IX_Categories_CreatedBy` (`CreatedBy`),
  CONSTRAINT `categories_ibfk_1` FOREIGN KEY (`ParentCategoryId`) REFERENCES `categories` (`Id`),
  CONSTRAINT `categories_ibfk_2` FOREIGN KEY (`CreatedBy`) REFERENCES `users` (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `categories`
--

LOCK TABLES `categories` WRITE;
/*!40000 ALTER TABLE `categories` DISABLE KEYS */;
/*!40000 ALTER TABLE `categories` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `customers`
--

DROP TABLE IF EXISTS `customers`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `customers` (
  `Id` varchar(36) NOT NULL,
  `Name` varchar(255) NOT NULL,
  `Phone` varchar(20) DEFAULT NULL,
  `Email` varchar(255) DEFAULT NULL,
  `Address` text,
  `TotalPurchases` decimal(15,2) DEFAULT '0.00',
  `TotalTransactions` int DEFAULT '0',
  `LastPurchaseDate` datetime DEFAULT NULL,
  `CreatedAt` datetime NOT NULL,
  `CreatedBy` varchar(36) NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_Customers_CreatedBy` (`CreatedBy`),
  CONSTRAINT `customers_ibfk_1` FOREIGN KEY (`CreatedBy`) REFERENCES `users` (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `customers`
--

LOCK TABLES `customers` WRITE;
/*!40000 ALTER TABLE `customers` DISABLE KEYS */;
/*!40000 ALTER TABLE `customers` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `producthistory`
--

DROP TABLE IF EXISTS `producthistory`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `producthistory` (
  `Id` varchar(36) NOT NULL,
  `ProductId` varchar(36) NOT NULL,
  `DateTime` datetime NOT NULL,
  `TransactionType` varchar(50) NOT NULL,
  `QuantityChanged` int NOT NULL,
  `StockBefore` int NOT NULL,
  `StockAfter` int NOT NULL,
  `UnitPrice` decimal(15,2) DEFAULT NULL,
  `TotalValue` decimal(15,2) DEFAULT NULL,
  `Notes` text,
  `CreatedBy` varchar(36) NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `ProductId` (`ProductId`),
  KEY `CreatedBy` (`CreatedBy`),
  CONSTRAINT `producthistory_ibfk_1` FOREIGN KEY (`ProductId`) REFERENCES `products` (`Id`),
  CONSTRAINT `producthistory_ibfk_2` FOREIGN KEY (`CreatedBy`) REFERENCES `users` (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `producthistory`
--

LOCK TABLES `producthistory` WRITE;
/*!40000 ALTER TABLE `producthistory` DISABLE KEYS */;
/*!40000 ALTER TABLE `producthistory` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `products`
--

DROP TABLE IF EXISTS `products`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `products` (
  `Id` varchar(36) NOT NULL,
  `Name` varchar(255) NOT NULL,
  `Barcode` varchar(255) DEFAULT NULL,
  `CategoryId` varchar(36) NOT NULL,
  `BuyingPrice` decimal(15,2) NOT NULL,
  `SellingPrice` decimal(15,2) NOT NULL,
  `CurrentStock` int NOT NULL,
  `MinStockLevel` int DEFAULT '10',
  `SupplierId` varchar(36) DEFAULT NULL,
  `CreatedAt` datetime NOT NULL,
  `IsActive` tinyint(1) DEFAULT '1',
  `CreatedBy` varchar(36) NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `SupplierId` (`SupplierId`),
  KEY `IX_Products_CategoryId` (`CategoryId`),
  KEY `IX_Products_CreatedBy` (`CreatedBy`),
  CONSTRAINT `products_ibfk_1` FOREIGN KEY (`CategoryId`) REFERENCES `categories` (`Id`),
  CONSTRAINT `products_ibfk_2` FOREIGN KEY (`SupplierId`) REFERENCES `suppliers` (`Id`),
  CONSTRAINT `products_ibfk_3` FOREIGN KEY (`CreatedBy`) REFERENCES `users` (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `products`
--

LOCK TABLES `products` WRITE;
/*!40000 ALTER TABLE `products` DISABLE KEYS */;
/*!40000 ALTER TABLE `products` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `saleitems`
--

DROP TABLE IF EXISTS `saleitems`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `saleitems` (
  `Id` varchar(36) NOT NULL,
  `SaleId` varchar(36) NOT NULL,
  `ProductId` varchar(36) NOT NULL,
  `ProductName` varchar(255) NOT NULL,
  `Quantity` int NOT NULL,
  `UnitBuyingPrice` decimal(15,2) NOT NULL,
  `UnitSellingPrice` decimal(15,2) NOT NULL,
  `TotalAmount` decimal(15,2) NOT NULL,
  `TotalCost` decimal(15,2) NOT NULL,
  `TotalProfit` decimal(15,2) NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `ProductId` (`ProductId`),
  KEY `IX_SaleItems_SaleId` (`SaleId`),
  CONSTRAINT `saleitems_ibfk_1` FOREIGN KEY (`SaleId`) REFERENCES `sales` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `saleitems_ibfk_2` FOREIGN KEY (`ProductId`) REFERENCES `products` (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `saleitems`
--

LOCK TABLES `saleitems` WRITE;
/*!40000 ALTER TABLE `saleitems` DISABLE KEYS */;
/*!40000 ALTER TABLE `saleitems` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `sales`
--

DROP TABLE IF EXISTS `sales`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `sales` (
  `Id` varchar(36) NOT NULL,
  `DateTime` datetime NOT NULL,
  `CustomerId` varchar(36) DEFAULT NULL,
  `CustomerName` varchar(255) DEFAULT NULL,
  `CustomerPhone` varchar(20) DEFAULT NULL,
  `PaymentMethod` varchar(50) NOT NULL,
  `TotalAmount` decimal(15,2) NOT NULL,
  `TotalCost` decimal(15,2) NOT NULL,
  `TotalProfit` decimal(15,2) NOT NULL,
  `CreatedAt` datetime NOT NULL,
  `CreatedBy` varchar(36) NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `CustomerId` (`CustomerId`),
  KEY `IX_Sales_DateTime` (`DateTime`),
  KEY `IX_Sales_CreatedBy` (`CreatedBy`),
  CONSTRAINT `sales_ibfk_1` FOREIGN KEY (`CustomerId`) REFERENCES `customers` (`Id`),
  CONSTRAINT `sales_ibfk_2` FOREIGN KEY (`CreatedBy`) REFERENCES `users` (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `sales`
--

LOCK TABLES `sales` WRITE;
/*!40000 ALTER TABLE `sales` DISABLE KEYS */;
/*!40000 ALTER TABLE `sales` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `suppliers`
--

DROP TABLE IF EXISTS `suppliers`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `suppliers` (
  `Id` varchar(36) NOT NULL,
  `Name` varchar(255) NOT NULL,
  `ContactPerson` varchar(255) DEFAULT NULL,
  `Phone` varchar(20) DEFAULT NULL,
  `Email` varchar(255) DEFAULT NULL,
  `Address` text,
  `TotalPurchases` decimal(15,2) DEFAULT '0.00',
  `TotalProducts` int DEFAULT '0',
  `LastPurchaseDate` datetime DEFAULT NULL,
  `CreatedAt` datetime NOT NULL,
  `CreatedBy` varchar(36) NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_Suppliers_CreatedBy` (`CreatedBy`),
  CONSTRAINT `suppliers_ibfk_1` FOREIGN KEY (`CreatedBy`) REFERENCES `users` (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `suppliers`
--

LOCK TABLES `suppliers` WRITE;
/*!40000 ALTER TABLE `suppliers` DISABLE KEYS */;
/*!40000 ALTER TABLE `suppliers` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `users`
--

DROP TABLE IF EXISTS `users`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `users` (
  `Id` varchar(36) NOT NULL,
  `Email` varchar(255) NOT NULL,
  `Name` varchar(255) NOT NULL,
  `Phone` varchar(20) DEFAULT NULL,
  `ShopName` varchar(255) NOT NULL,
  `PasswordHash` varchar(255) NOT NULL,
  `CreatedAt` datetime NOT NULL,
  `IsEmailVerified` tinyint(1) DEFAULT '0',
  `RefreshToken` varchar(500) DEFAULT NULL,
  `RefreshTokenExpiryTime` datetime DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `Email` (`Email`),
  KEY `IX_Users_Email` (`Email`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `users`
--

LOCK TABLES `users` WRITE;
/*!40000 ALTER TABLE `users` DISABLE KEYS */;
INSERT INTO `users` VALUES ('81d72bf7-ef80-4537-8792-4bf8db16fb2f','mdkamrulhasanewu@gmail.com','Kamrul','01758290421','Marzahan Cosmetics','$2a$11$G008zem/84UsPLaCKTFDn.xC4wOo.NpIozS4yPEOYtZtxmJ0GHA/q','2025-11-10 14:18:19',0,'GCWsJn9WTtRaIAt6IFlfNyisRqFaHI62Z3qggdYd8obY6X4OTvtxTG3/krpRiFZv9lw2HlvF3Bc8e5vF9lR9eQ==','2025-11-17 14:23:19');
/*!40000 ALTER TABLE `users` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Dumping routines for database 'shop'
--
SET @@SESSION.SQL_LOG_BIN = @MYSQLDUMP_TEMP_LOG_BIN;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2025-11-10 20:36:01
