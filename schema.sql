-- MySQL dump 10.13  Distrib 8.0.44, for Win64 (x86_64)
--
-- Host: s0znzigqvfehvff5.cbetxkdyhwsb.us-east-1.rds.amazonaws.com    Database: is5fptofzkxg7uah
-- ------------------------------------------------------
-- Server version	8.0.40

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8 */;
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

SET @@GLOBAL.GTID_PURGED=/*!80000 '+'*/ '';

--
-- Table structure for table `categories`
--

DROP TABLE IF EXISTS `categories`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `categories` (
  `category_id` int NOT NULL AUTO_INCREMENT,
  `category_name` varchar(100) NOT NULL,
  PRIMARY KEY (`category_id`)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `categories`
--

LOCK TABLES `categories` WRITE;
/*!40000 ALTER TABLE `categories` DISABLE KEYS */;
INSERT INTO `categories` VALUES (1,'Đồ uống'),(2,'Bánh kẹo'),(3,'Gia vị'),(4,'Đồ gia dụng'),(5,'Mỹ phẩm');
/*!40000 ALTER TABLE `categories` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `customers`
--

DROP TABLE IF EXISTS `customers`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `customers` (
  `customer_id` int NOT NULL AUTO_INCREMENT,
  `name` varchar(100) NOT NULL,
  `phone` varchar(20) DEFAULT NULL,
  `email` varchar(100) DEFAULT NULL,
  `address` text,
  `created_at` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `password` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`customer_id`)
) ENGINE=InnoDB AUTO_INCREMENT=22 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `customers`
--

LOCK TABLES `customers` WRITE;
/*!40000 ALTER TABLE `customers` DISABLE KEYS */;
INSERT INTO `customers` VALUES (1,'Khách hàng 1','0909000001','kh1@mail.com','Địa chỉ 1','2025-11-27 05:02:24',NULL),(2,'Khách hàng 2','0909000002','kh2@mail.com','Địa chỉ 2','2025-11-27 05:02:24',NULL),(3,'Khách hàng 3','0909000003','kh3@mail.com','Địa chỉ 3','2025-11-27 05:02:24',NULL),(4,'Khách hàng 4','0909000004','kh4@mail.com','Địa chỉ 4','2025-11-27 05:02:24',NULL),(5,'Khách hàng 5','0909000005','kh5@mail.com','Địa chỉ 5','2025-11-27 05:02:24',NULL),(6,'Khách hàng 6','0909000006','kh6@mail.com','Địa chỉ 6','2025-11-27 05:02:24',NULL),(7,'Khách hàng 7','0909000007','kh7@mail.com','Địa chỉ 7','2025-11-27 05:02:24',NULL),(8,'Khách hàng 8','0909000008','kh8@mail.com','Địa chỉ 8','2025-11-27 05:02:24',NULL),(9,'Khách hàng 9','0909000009','kh9@mail.com','Địa chỉ 9','2025-11-27 05:02:24',NULL),(10,'Khách hàng 10','0909000010','kh10@mail.com','Địa chỉ 10','2025-11-27 05:02:24',NULL),(11,'Khách hàng 11','0909000011','kh11@mail.com','Địa chỉ 11','2025-11-27 05:02:24',NULL),(12,'Khách hàng 12','0909000012','kh12@mail.com','Địa chỉ 12','2025-11-27 05:02:24',NULL),(13,'Khách hàng 13','0909000013','kh13@mail.com','Địa chỉ 13','2025-11-27 05:02:24',NULL),(14,'Khách hàng 14','0909000014','kh14@mail.com','Địa chỉ 14','2025-11-27 05:02:24',NULL),(15,'Khách hàng 15','0909000015','kh15@mail.com','Địa chỉ 15','2025-11-27 05:02:24',NULL),(16,'Khách hàng 16','0909000016','kh16@mail.com','Địa chỉ 16','2025-11-27 05:02:24',NULL),(17,'Khách hàng 17','0909000017','kh17@mail.com','Địa chỉ 17','2025-11-27 05:02:24',NULL),(18,'Khách hàng 18','0909000018','kh18@mail.com','Địa chỉ 18','2025-11-27 05:02:24',NULL),(19,'Khách hàng 19','0909000019','kh19@mail.com','Địa chỉ 19','2025-11-27 05:02:24',NULL),(20,'Khách hàng 20','0909000020','kh20@mail.com','Địa chỉ 20','2025-11-27 05:02:24',NULL),(21,'Thịnh Hi','0931816175','zaikaman123@gmail.com','536 Âu Cơ','2025-11-27 14:36:36','lovelybaby');
/*!40000 ALTER TABLE `customers` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `inventory`
--

DROP TABLE IF EXISTS `inventory`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `inventory` (
  `inventory_id` int NOT NULL AUTO_INCREMENT,
  `product_id` int NOT NULL,
  `quantity` int DEFAULT '0',
  `updated_at` timestamp NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`inventory_id`)
) ENGINE=InnoDB AUTO_INCREMENT=51 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `inventory`
--

LOCK TABLES `inventory` WRITE;
/*!40000 ALTER TABLE `inventory` DISABLE KEYS */;
INSERT INTO `inventory` VALUES (1,1,25,'2025-11-27 05:02:25'),(2,2,169,'2025-11-27 05:02:25'),(3,3,77,'2025-11-27 05:02:25'),(4,4,169,'2025-11-27 05:02:25'),(5,5,90,'2025-11-27 05:02:25'),(6,6,105,'2025-11-27 05:02:25'),(7,7,125,'2025-11-27 05:02:25'),(8,8,37,'2025-11-27 05:02:25'),(9,9,74,'2025-11-27 05:02:25'),(10,10,149,'2025-11-27 05:02:25'),(11,11,69,'2025-11-27 05:02:25'),(12,12,23,'2025-11-27 05:02:25'),(13,13,46,'2025-11-27 05:02:25'),(14,14,144,'2025-11-27 05:02:25'),(15,15,134,'2025-11-27 05:02:25'),(16,16,182,'2025-11-27 05:02:25'),(17,17,99,'2025-11-27 05:02:25'),(18,18,72,'2025-11-27 05:02:25'),(19,19,128,'2025-11-27 05:02:25'),(20,20,123,'2025-11-27 05:02:25'),(21,21,155,'2025-11-27 05:02:25'),(22,22,78,'2025-11-27 05:02:25'),(23,23,166,'2025-11-27 05:02:25'),(24,24,117,'2025-11-27 05:02:25'),(25,25,168,'2025-11-27 05:02:25'),(26,26,197,'2025-11-27 05:02:25'),(27,27,35,'2025-11-27 14:56:41'),(28,28,145,'2025-11-27 05:02:25'),(29,29,61,'2025-11-27 05:02:25'),(30,30,139,'2025-11-27 05:02:25'),(31,31,47,'2025-11-27 05:02:25'),(32,32,149,'2025-11-27 15:10:56'),(33,33,194,'2025-11-27 05:02:25'),(34,34,41,'2025-11-27 05:02:25'),(35,35,154,'2025-11-27 05:02:25'),(36,36,71,'2025-11-27 05:02:25'),(37,37,49,'2025-11-27 05:02:25'),(38,38,165,'2025-11-27 05:02:25'),(39,39,70,'2025-11-27 15:05:54'),(40,40,176,'2025-11-27 05:02:25'),(41,41,41,'2025-11-27 05:02:25'),(42,42,29,'2025-11-27 15:10:56'),(43,43,175,'2025-11-27 05:02:25'),(44,44,59,'2025-11-27 05:02:25'),(45,45,198,'2025-11-27 05:02:25'),(46,46,106,'2025-11-27 05:02:25'),(47,47,99,'2025-11-27 05:02:25'),(48,48,55,'2025-11-27 05:02:25'),(49,49,62,'2025-11-27 05:02:25'),(50,50,31,'2025-11-27 19:37:37');
/*!40000 ALTER TABLE `inventory` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `order_items`
--

DROP TABLE IF EXISTS `order_items`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `order_items` (
  `order_item_id` int NOT NULL AUTO_INCREMENT,
  `order_id` int DEFAULT NULL,
  `product_id` int DEFAULT NULL,
  `quantity` int NOT NULL,
  `price` decimal(10,2) NOT NULL,
  `subtotal` decimal(10,2) NOT NULL,
  PRIMARY KEY (`order_item_id`)
) ENGINE=InnoDB AUTO_INCREMENT=101 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `order_items`
--

LOCK TABLES `order_items` WRITE;
/*!40000 ALTER TABLE `order_items` DISABLE KEYS */;
INSERT INTO `order_items` VALUES (1,1,23,2,31265.00,62530.00),(2,1,5,2,205683.00,411366.00),(3,1,47,1,477948.00,477948.00),(4,1,25,2,170243.00,340486.00),(5,2,39,1,447059.00,447059.00),(6,2,14,1,51108.00,51108.00),(7,2,46,3,411147.00,1233441.00),(8,3,18,3,202167.00,606501.00),(9,3,34,1,44219.00,44219.00),(10,3,26,3,23354.00,70062.00),(11,4,24,2,10843.00,21686.00),(12,5,9,1,94180.00,94180.00),(13,6,18,3,186886.00,560658.00),(14,6,22,2,199267.00,398534.00),(15,6,42,3,215726.00,647178.00),(16,6,17,3,474268.00,1422804.00),(17,6,20,3,286499.00,859497.00),(18,7,8,2,256297.00,512594.00),(19,8,42,1,355116.00,355116.00),(20,8,43,2,129224.00,258448.00),(21,8,31,3,367155.00,1101465.00),(22,9,17,2,48755.00,97510.00),(23,9,12,2,381904.00,763808.00),(24,9,43,2,167445.00,334890.00),(25,9,19,3,429281.00,1287843.00),(26,10,25,1,232635.00,232635.00),(27,10,1,2,245362.00,490724.00),(28,10,23,2,127233.00,254466.00),(29,10,49,2,46207.00,92414.00),(30,11,3,2,347879.00,695758.00),(31,11,23,3,130215.00,390645.00),(32,11,4,1,64761.00,64761.00),(33,11,33,1,240159.00,240159.00),(34,11,7,1,141418.00,141418.00),(35,12,40,2,455428.00,910856.00),(36,12,46,2,75412.00,150824.00),(37,12,34,2,189856.00,379712.00),(38,12,25,3,114654.00,343962.00),(39,13,24,2,143251.00,286502.00),(40,13,23,2,381347.00,762694.00),(41,13,18,2,179146.00,358292.00),(42,13,9,2,90394.00,180788.00),(43,14,24,2,327016.00,654032.00),(44,14,2,1,403478.00,403478.00),(45,14,27,3,404474.00,1213422.00),(46,14,4,2,312582.00,625164.00),(47,15,18,1,105328.00,105328.00),(48,15,27,2,17303.00,34606.00),(49,15,50,2,23033.00,46066.00),(50,16,15,1,43160.00,43160.00),(51,16,16,2,18541.00,37082.00),(52,16,44,1,492698.00,492698.00),(53,16,41,1,451150.00,451150.00),(54,17,42,1,467148.00,467148.00),(55,18,30,1,64334.00,64334.00),(56,18,11,1,178454.00,178454.00),(57,18,20,3,50518.00,151554.00),(58,19,16,1,89280.00,89280.00),(59,19,23,3,404655.00,1213965.00),(60,19,11,2,331196.00,662392.00),(61,20,49,1,367325.00,367325.00),(62,20,32,2,264392.00,528784.00),(63,20,19,3,345903.00,1037709.00),(64,20,17,2,392028.00,784056.00),(65,20,19,1,171939.00,171939.00),(66,21,11,3,227666.00,682998.00),(67,21,25,2,436122.00,872244.00),(68,21,48,1,340400.00,340400.00),(69,21,10,2,58482.00,116964.00),(70,21,4,2,137900.00,275800.00),(71,22,40,2,165504.00,331008.00),(72,23,1,2,296698.00,593396.00),(73,23,16,3,384657.00,1153971.00),(74,23,40,3,135828.00,407484.00),(75,24,3,3,379562.00,1138686.00),(76,25,9,1,22063.00,22063.00),(77,25,16,2,185892.00,371784.00),(78,26,47,2,130329.00,260658.00),(79,27,37,1,448581.00,448581.00),(80,27,23,1,484618.00,484618.00),(81,28,20,3,357837.00,1073511.00),(82,28,34,1,161219.00,161219.00),(83,28,1,3,458131.00,1374393.00),(84,29,28,1,485514.00,485514.00),(85,29,7,3,487044.00,1461132.00),(86,29,42,1,235885.00,235885.00),(87,29,38,1,223761.00,223761.00),(88,30,25,1,426943.00,426943.00),(89,30,11,3,130209.00,390627.00),(90,30,5,2,73116.00,146232.00),(91,30,46,2,272220.00,544440.00),(92,30,23,3,467964.00,1403892.00),(93,31,27,1,38039.00,38039.00),(94,32,39,1,317819.00,317819.00),(95,33,39,1,317819.00,317819.00),(96,34,32,1,495429.00,495429.00),(97,35,39,1,317819.00,317819.00),(98,36,42,5,9413.00,47065.00),(99,36,32,4,495429.00,1981716.00),(100,37,50,2,328680.00,657360.00);
/*!40000 ALTER TABLE `order_items` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `orders`
--

DROP TABLE IF EXISTS `orders`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `orders` (
  `order_id` int NOT NULL AUTO_INCREMENT,
  `customer_id` int DEFAULT NULL,
  `user_id` int DEFAULT NULL,
  `promo_id` int DEFAULT NULL,
  `order_date` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `status` enum('pending','paid','canceled') DEFAULT 'pending',
  `total_amount` decimal(10,2) DEFAULT NULL,
  `discount_amount` decimal(10,2) DEFAULT '0.00',
  `transfer_content` varchar(100) DEFAULT NULL,
  PRIMARY KEY (`order_id`)
) ENGINE=InnoDB AUTO_INCREMENT=38 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `orders`
--

LOCK TABLES `orders` WRITE;
/*!40000 ALTER TABLE `orders` DISABLE KEYS */;
INSERT INTO `orders` VALUES (1,5,3,5,'2025-11-27 05:02:26','paid',1292330.00,100000.00,NULL),(2,17,3,NULL,'2025-11-27 05:02:26','paid',1731608.00,0.00,NULL),(3,8,3,NULL,'2025-11-27 05:02:26','paid',720782.00,0.00,NULL),(4,20,3,5,'2025-11-27 05:02:26','paid',21686.00,21686.00,NULL),(5,1,2,NULL,'2025-11-27 05:02:26','paid',94180.00,0.00,NULL),(6,5,3,2,'2025-11-27 05:02:26','paid',3888671.00,100000.00,NULL),(7,9,3,4,'2025-11-27 05:02:26','paid',512594.00,102518.80,NULL),(8,11,3,3,'2025-11-27 05:02:26','paid',1715029.00,171502.90,NULL),(9,11,3,NULL,'2025-11-27 05:02:26','paid',2484051.00,0.00,NULL),(10,11,3,2,'2025-11-27 05:02:26','paid',1070239.00,100000.00,NULL),(11,20,3,NULL,'2025-11-27 05:02:26','paid',1532741.00,0.00,NULL),(12,10,2,NULL,'2025-11-27 05:02:26','paid',1785354.00,0.00,NULL),(13,10,3,2,'2025-11-27 05:02:26','paid',1588276.00,100000.00,NULL),(14,6,2,2,'2025-11-27 05:02:26','paid',2896096.00,50000.00,NULL),(15,10,2,3,'2025-11-27 05:02:26','paid',186000.00,27900.00,NULL),(16,10,2,5,'2025-11-27 05:02:26','paid',1024090.00,50000.00,NULL),(17,19,3,NULL,'2025-11-27 05:02:26','paid',467148.00,0.00,NULL),(18,10,2,NULL,'2025-11-27 05:02:26','paid',394342.00,0.00,NULL),(19,8,3,4,'2025-11-27 05:02:26','paid',1965637.00,294845.55,NULL),(20,3,3,NULL,'2025-11-27 05:02:26','paid',2889813.00,0.00,NULL),(21,9,2,NULL,'2025-11-27 05:02:26','paid',2288406.00,0.00,NULL),(22,17,3,NULL,'2025-11-27 05:02:26','paid',331008.00,0.00,NULL),(23,6,3,1,'2025-11-27 05:02:26','paid',2154851.00,323227.65,NULL),(24,1,3,1,'2025-11-27 05:02:26','paid',1138686.00,170802.90,NULL),(25,2,2,5,'2025-11-27 05:02:26','paid',393847.00,100000.00,NULL),(26,15,3,1,'2025-11-27 05:02:26','paid',260658.00,52131.60,NULL),(27,4,2,NULL,'2025-11-27 05:02:26','paid',933199.00,0.00,NULL),(28,16,2,NULL,'2025-11-27 05:02:26','paid',2609123.00,0.00,NULL),(29,4,3,4,'2025-11-27 05:02:26','paid',2406292.00,481258.40,NULL),(30,1,3,NULL,'2025-11-27 05:02:26','paid',2912134.00,0.00,NULL),(31,21,NULL,NULL,'2025-11-27 14:56:40','paid',38039.00,0.00,NULL),(32,21,NULL,NULL,'2025-11-27 14:58:12','paid',317819.00,0.00,NULL),(33,21,NULL,NULL,'2025-11-27 15:01:15','paid',317819.00,0.00,NULL),(34,21,NULL,NULL,'2025-11-27 15:02:15','paid',495429.00,0.00,NULL),(35,21,NULL,NULL,'2025-11-27 15:05:53','paid',317819.00,0.00,NULL),(36,21,NULL,NULL,'2025-11-27 15:10:55','paid',2028781.00,0.00,NULL),(37,21,NULL,NULL,'2025-11-27 19:37:36','paid',657360.00,0.00,NULL);
/*!40000 ALTER TABLE `orders` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `payments`
--

DROP TABLE IF EXISTS `payments`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `payments` (
  `payment_id` int NOT NULL AUTO_INCREMENT,
  `order_id` int NOT NULL,
  `amount` decimal(10,2) NOT NULL,
  `payment_method` enum('cash','card','bank_transfer','e-wallet') DEFAULT 'cash',
  `payment_date` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `status` varchar(20) DEFAULT 'pending',
  PRIMARY KEY (`payment_id`)
) ENGINE=InnoDB AUTO_INCREMENT=38 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `payments`
--

LOCK TABLES `payments` WRITE;
/*!40000 ALTER TABLE `payments` DISABLE KEYS */;
INSERT INTO `payments` VALUES (1,1,1192330.00,'cash','2025-11-27 05:02:26','pending'),(2,2,1731608.00,'e-wallet','2025-11-27 05:02:26','pending'),(3,3,720782.00,'e-wallet','2025-11-27 05:02:26','pending'),(4,4,0.00,'card','2025-11-27 05:02:26','pending'),(5,5,94180.00,'cash','2025-11-27 05:02:26','pending'),(6,6,3788671.00,'cash','2025-11-27 05:02:26','pending'),(7,7,410075.20,'e-wallet','2025-11-27 05:02:26','pending'),(8,8,1543526.10,'cash','2025-11-27 05:02:26','pending'),(9,9,2484051.00,'cash','2025-11-27 05:02:26','pending'),(10,10,970239.00,'card','2025-11-27 05:02:26','pending'),(11,11,1532741.00,'e-wallet','2025-11-27 05:02:26','pending'),(12,12,1785354.00,'card','2025-11-27 05:02:26','pending'),(13,13,1488276.00,'card','2025-11-27 05:02:26','pending'),(14,14,2846096.00,'cash','2025-11-27 05:02:26','pending'),(15,15,158100.00,'card','2025-11-27 05:02:26','pending'),(16,16,974090.00,'cash','2025-11-27 05:02:26','pending'),(17,17,467148.00,'cash','2025-11-27 05:02:26','pending'),(18,18,394342.00,'e-wallet','2025-11-27 05:02:26','pending'),(19,19,1670791.45,'card','2025-11-27 05:02:26','pending'),(20,20,2889813.00,'card','2025-11-27 05:02:26','pending'),(21,21,2288406.00,'cash','2025-11-27 05:02:26','pending'),(22,22,331008.00,'e-wallet','2025-11-27 05:02:26','pending'),(23,23,1831623.35,'cash','2025-11-27 05:02:26','pending'),(24,24,967883.10,'e-wallet','2025-11-27 05:02:26','pending'),(25,25,293847.00,'cash','2025-11-27 05:02:26','pending'),(26,26,208526.40,'cash','2025-11-27 05:02:26','pending'),(27,27,933199.00,'cash','2025-11-27 05:02:26','pending'),(28,28,2609123.00,'card','2025-11-27 05:02:26','pending'),(29,29,1925033.60,'cash','2025-11-27 05:02:26','pending'),(30,30,2912134.00,'card','2025-11-27 05:02:26','pending'),(31,31,38039.00,'cash','2025-11-27 14:56:41','completed'),(32,32,317819.00,'cash','2025-11-27 14:58:13','completed'),(33,33,317819.00,'cash','2025-11-27 15:01:16','completed'),(34,34,495429.00,'cash','2025-11-27 15:02:16','completed'),(35,35,317819.00,'cash','2025-11-27 15:05:54','completed'),(36,36,2028781.00,'cash','2025-11-27 15:10:56','completed'),(37,37,657360.00,'bank_transfer','2025-11-27 19:37:37','completed');
/*!40000 ALTER TABLE `payments` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `products`
--

DROP TABLE IF EXISTS `products`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `products` (
  `product_id` int NOT NULL AUTO_INCREMENT,
  `category_id` int DEFAULT NULL,
  `supplier_id` int DEFAULT NULL,
  `product_name` varchar(100) NOT NULL,
  `barcode` varchar(50) DEFAULT NULL,
  `price` decimal(10,2) NOT NULL,
  `unit` varchar(20) DEFAULT 'pcs',
  `created_at` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `image_url` varchar(500) DEFAULT NULL,
  PRIMARY KEY (`product_id`),
  UNIQUE KEY `barcode` (`barcode`)
) ENGINE=InnoDB AUTO_INCREMENT=52 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `products`
--

LOCK TABLES `products` WRITE;
/*!40000 ALTER TABLE `products` DISABLE KEYS */;
INSERT INTO `products` VALUES (1,1,1,'Coca Cola lon','8900000000001',314838.00,'lon','2025-11-27 12:06:22','https://res.cloudinary.com/dgxh5aruz/image/upload/v1764242884/products/weruxjfqbtiauebl2wkn.jpg'),(2,1,3,'Pepsi lon','8900000000002',114807.00,'lon','2025-11-27 12:06:28','https://res.cloudinary.com/dgxh5aruz/image/upload/v1764242894/products/t7fzynpjffnif8elm51x.jpg'),(3,1,3,'Trà Xanh 0 độ','8900000000003',415725.00,'chai','2025-11-27 12:06:34','https://res.cloudinary.com/dgxh5aruz/image/upload/v1764242904/products/ahdhy9ukccdhybu1aspg.jpg'),(4,1,1,'Sting dâu','8900000000004',351670.00,'chai','2025-11-27 12:06:40','https://res.cloudinary.com/dgxh5aruz/image/upload/v1764242915/products/ni1uojpjmnrutgrfswkx.jpg'),(5,1,2,'Red Bull','8900000000005',402179.00,'lon','2025-11-27 12:06:46','https://res.cloudinary.com/dgxh5aruz/image/upload/v1764242925/products/zysnnx1skfbqsfr7yr2y.jpg'),(6,2,2,'Bánh Oreo','8900000000006',209283.00,'gói','2025-11-27 12:06:15','https://res.cloudinary.com/dgxh5aruz/image/upload/v1764242936/products/taysny3fu4rmpyof5nth.jpg'),(7,2,3,'Bánh Chocopie','8900000000007',212528.00,'gói','2025-11-27 12:06:59','https://res.cloudinary.com/dgxh5aruz/image/upload/v1764242945/products/jvrpqp3n0707yckfctrx.jpg'),(8,2,2,'Kẹo Alpenliebe','8900000000008',34313.00,'pcs','2025-11-27 12:07:14','https://res.cloudinary.com/dgxh5aruz/image/upload/v1764242954/products/h8rx2gfqgofzbwnymxkq.jpg'),(9,2,1,'Kẹo bạc hà','8900000000009',316289.00,'pcs','2025-11-27 12:07:22','https://res.cloudinary.com/dgxh5aruz/image/upload/v1764242963/products/kdjvfhxpvsqirsdio8ib.jpg'),(10,2,2,'Socola KitKat','8900000000010',139959.00,'gói','2025-11-27 12:07:38','https://res.cloudinary.com/dgxh5aruz/image/upload/v1764242973/products/g0cyfx3ybvuzthdamiox.jpg'),(11,3,1,'Nước mắm Nam Ngư','8900000000011',51792.00,'chai','2025-11-27 12:07:48','https://res.cloudinary.com/dgxh5aruz/image/upload/v1764242984/products/lwwwzptpwrpsvvvhdc0h.jpg'),(12,3,2,'Nước tương Maggi','8900000000012',462539.00,'chai','2025-11-27 12:08:00','https://res.cloudinary.com/dgxh5aruz/image/upload/v1764242997/products/qdtllkwyovde6jre1cov.jpg'),(13,3,3,'Muối i-ốt','8900000000013',173302.00,'gói','2025-11-27 12:08:15','https://res.cloudinary.com/dgxh5aruz/image/upload/v1764243008/products/nmvqxk6pvhdzqkjgtjh7.jpg'),(14,3,1,'Bột ngọt Ajinomoto','8900000000014',443069.00,'gói','2025-11-27 12:08:26','https://res.cloudinary.com/dgxh5aruz/image/upload/v1764243019/products/dwdsmgkjhrjfb48avls1.jpg'),(15,3,2,'Dầu ăn Tường An','8900000000015',281354.00,'chai','2025-11-27 12:08:36','https://res.cloudinary.com/dgxh5aruz/image/upload/v1764243029/products/mxxbcdqqzf6dhkzuebpg.jpg'),(16,4,1,'Nồi cơm điện','8900000000016',405347.00,'pcs','2025-11-27 12:08:51','https://res.cloudinary.com/dgxh5aruz/image/upload/v1764243052/products/ezy81yoe1z4zizskumbf.jpg'),(17,4,3,'Ấm siêu tốc','8900000000017',113087.00,'pcs','2025-11-27 12:09:02','https://res.cloudinary.com/dgxh5aruz/image/upload/v1764243067/products/xliuquknvmjaupaouck2.jpg'),(18,4,2,'Quạt máy','8900000000018',69968.00,'pcs','2025-11-27 12:09:11','https://res.cloudinary.com/dgxh5aruz/image/upload/v1764243079/products/a4fty45qbhdpg2exhusc.jpg'),(19,4,1,'Bếp gas mini','8900000000019',416845.00,'pcs','2025-11-27 12:09:20','https://res.cloudinary.com/dgxh5aruz/image/upload/v1764243183/products/qqgjzn6boyhy32j0wbc3.jpg'),(20,4,3,'Máy xay sinh tố','8900000000020',334564.00,'pcs','2025-11-27 12:09:29','https://res.cloudinary.com/dgxh5aruz/image/upload/v1764243199/products/oyw49ty24rlcsji6acoq.jpg'),(21,5,1,'Sữa rửa mặt Hazeline','8900000000021',188475.00,'chai','2025-11-27 12:09:40','https://res.cloudinary.com/dgxh5aruz/image/upload/v1764243212/products/hu9fjehlbtixevi1axe7.jpg'),(22,5,1,'Kem dưỡng da Pond\'s','8900000000022',413840.00,'hộp','2025-11-27 12:09:52','https://res.cloudinary.com/dgxh5aruz/image/upload/v1764243224/products/e1vxdme2ra9v5hrma2ia.jpg'),(23,5,3,'Dầu gội Sunsilk','8900000000023',158950.00,'chai','2025-11-27 12:10:02','https://res.cloudinary.com/dgxh5aruz/image/upload/v1764243285/products/m8zva2wjuw2phwbl5ggx.jpg'),(24,5,2,'Sữa tắm Dove','8900000000024',336928.00,'chai','2025-11-27 12:10:11','https://res.cloudinary.com/dgxh5aruz/image/upload/v1764243299/products/ogtxgcdjpdjstcxhwsyr.jpg'),(25,5,1,'Nước hoa Romano','8900000000025',352508.00,'chai','2025-11-27 12:10:24','https://res.cloudinary.com/dgxh5aruz/image/upload/v1764243315/products/ufyyjsvxhnuedyxlxfds.jpg'),(26,1,1,'Cà phê G7','8900000000026',201228.00,'gói','2025-11-27 12:10:33','https://res.cloudinary.com/dgxh5aruz/image/upload/v1764243327/products/xauj8y7tkl0g6y2y8nat.jpg'),(27,1,1,'Trà Lipton','8900000000027',38039.00,'hộp','2025-11-27 12:10:42','https://res.cloudinary.com/dgxh5aruz/image/upload/v1764243385/products/e30ubzp1aoqlldzfps1g.jpg'),(28,1,3,'Sữa Vinamilk','8900000000028',252845.00,'hộp','2025-11-27 12:10:51','https://res.cloudinary.com/dgxh5aruz/image/upload/v1764243397/products/zrwehjqwoutlt16pascd.jpg'),(29,1,1,'Sữa TH True Milk','8900000000029',35278.00,'hộp','2025-11-27 12:10:59','https://res.cloudinary.com/dgxh5aruz/image/upload/v1764243407/products/my4xpqempdjwfbosarqc.jpg'),(30,1,2,'Nước suối Lavie','8900000000030',331637.00,'chai','2025-11-27 12:11:07','https://res.cloudinary.com/dgxh5aruz/image/upload/v1764243419/products/swxqvccl77bfssh8inmo.jpg'),(31,4,3,'Khăn giấy Tempo','8900000000031',102525.00,'hộp','2025-11-27 12:11:27','https://res.cloudinary.com/dgxh5aruz/image/upload/v1764243435/products/o0rtalm2dd1mz76cimpb.jpg'),(32,4,3,'Giấy vệ sinh Pulppy','8900000000032',495429.00,'hộp','2025-11-27 12:11:35','https://res.cloudinary.com/dgxh5aruz/image/upload/v1764243447/products/bkxhzff0p5ajyxdxwrpm.jpg'),(33,4,2,'Bình nước Lock&Lock','8900000000033',354771.00,'chai','2025-11-27 12:11:46','https://res.cloudinary.com/dgxh5aruz/image/upload/v1764243460/products/q2rpxpt70i1pnuxc1ien.jpg'),(34,4,1,'Hộp nhựa Tupperware','8900000000034',297415.00,'hộp','2025-11-27 12:11:55','https://res.cloudinary.com/dgxh5aruz/image/upload/v1764243473/products/ocvdbbfshek6je1fyy5v.jpg'),(35,4,3,'Dao Inox','8900000000035',47523.00,'pcs','2025-11-27 12:12:04','https://res.cloudinary.com/dgxh5aruz/image/upload/v1764243483/products/ewdqh4ljgpvzvltlrft3.jpg'),(36,4,1,'Bàn chải Colgate','8900000000036',136417.00,'pcs','2025-11-27 12:12:14','https://res.cloudinary.com/dgxh5aruz/image/upload/v1764243495/products/fcrkmborsdpgpc2awd6m.jpg'),(37,4,2,'Kem đánh răng P/S','8900000000037',93713.00,'tuýp','2025-11-27 12:12:29','https://res.cloudinary.com/dgxh5aruz/image/upload/v1764243543/products/wrkxzihfvnzmwkpov2jn.jpg'),(38,5,3,'Nước súc miệng Listerine','8900000000038',223906.00,'chai','2025-11-27 12:12:47','https://res.cloudinary.com/dgxh5aruz/image/upload/v1764243554/products/acfzhzwolvcq4uv8sg6c.jpg'),(39,5,2,'Bông tẩy trang','8900000000039',317819.00,'gói','2025-11-27 12:13:01','https://res.cloudinary.com/dgxh5aruz/image/upload/v1764243566/products/snoa0d1zmrqxryiijbbj.jpg'),(40,4,1,'Khẩu trang 3M','8900000000040',464252.00,'hộp','2025-11-27 12:13:11','https://res.cloudinary.com/dgxh5aruz/image/upload/v1764243576/products/vvpvyochbnpivsbp2vn2.jpg'),(41,2,1,'Bánh mì sandwich','8900000000041',279350.00,'pcs','2025-11-27 12:13:30','https://res.cloudinary.com/dgxh5aruz/image/upload/v1764243588/products/olmjno0w6ghovb5hnazy.jpg'),(42,2,2,'Mì gói Hảo Hảo','8900000000042',9413.00,'gói','2025-11-27 12:13:52','https://res.cloudinary.com/dgxh5aruz/image/upload/v1764243599/products/fmb5tdamgky9vrckhedt.jpg'),(43,2,2,'Mì Omachi','8900000000043',26616.00,'gói','2025-11-27 12:14:02','https://res.cloudinary.com/dgxh5aruz/image/upload/v1764243616/products/i3wmowmosreoswlkyyht.jpg'),(44,2,2,'Bún khô','8900000000044',350911.00,'gói','2025-11-27 12:14:13','https://res.cloudinary.com/dgxh5aruz/image/upload/v1764243628/products/rwym3mwophh9mrjbe5dg.jpg'),(45,2,1,'Phở ăn liền','8900000000045',407779.00,'gói','2025-11-27 12:14:29','https://res.cloudinary.com/dgxh5aruz/image/upload/v1764243642/products/rimtmteboym9ixiiqsql.jpg'),(46,1,1,'Nước ngọt Sprite','8900000000046',230083.00,'lon','2025-11-27 12:14:38','https://res.cloudinary.com/dgxh5aruz/image/upload/v1764243652/products/a2xrjgmveatlwdi0z8ij.jpg'),(47,1,3,'Trà sữa đóng chai','8900000000047',15130.00,'chai','2025-11-27 12:14:46','https://res.cloudinary.com/dgxh5aruz/image/upload/v1764243664/products/gndmf50ddavvtoicxpkz.jpg'),(48,2,3,'Snack Oishi','8900000000048',43415.00,'gói','2025-11-27 12:14:58','https://res.cloudinary.com/dgxh5aruz/image/upload/v1764243674/products/dfjj4gljyhcdce2al0y0.jpg'),(49,2,2,'Snack Lay\'s','8900000000049',83536.00,'gói','2025-11-27 12:15:08','https://res.cloudinary.com/dgxh5aruz/image/upload/v1764243683/products/qsvs4py2jnizldylm2cg.jpg'),(50,2,2,'Kẹo dẻo Haribo','8900000000050',328680.00,'gói','2025-11-27 12:15:17','https://res.cloudinary.com/dgxh5aruz/image/upload/v1764243694/products/fex5xaiucukuzplvjjkh.jpg');
/*!40000 ALTER TABLE `products` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `promotions`
--

DROP TABLE IF EXISTS `promotions`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `promotions` (
  `promo_id` int NOT NULL AUTO_INCREMENT,
  `promo_code` varchar(50) NOT NULL,
  `description` varchar(255) DEFAULT NULL,
  `discount_type` enum('percent','fixed') NOT NULL,
  `discount_value` decimal(10,2) NOT NULL,
  `start_date` date NOT NULL,
  `end_date` date NOT NULL,
  `min_order_amount` decimal(10,2) DEFAULT '0.00',
  `usage_limit` int DEFAULT '0',
  `used_count` int DEFAULT '0',
  `status` enum('active','inactive') DEFAULT 'active',
  PRIMARY KEY (`promo_id`),
  UNIQUE KEY `promo_code` (`promo_code`)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `promotions`
--

LOCK TABLES `promotions` WRITE;
/*!40000 ALTER TABLE `promotions` DISABLE KEYS */;
INSERT INTO `promotions` VALUES (1,'SALE10','Giảm 10% cho mọi đơn hàng','percent',10.00,'2025-01-01','2025-12-31',0.00,0,0,'active'),(2,'FREESHIP50K','Giảm 50,000 cho đơn từ 300,000 trở lên','fixed',50000.00,'2025-03-01','2025-12-31',300000.00,500,0,'active'),(3,'NEWUSER','Giảm 20% cho khách hàng mới','percent',20.00,'2025-01-01','2025-06-30',0.00,1,0,'active'),(4,'SUMMER15','Giảm 15% mùa hè','percent',15.00,'2025-06-01','2025-08-31',50000.00,1000,0,'active'),(5,'VIP100K','Giảm 100,000 cho đơn từ 1 triệu','fixed',100000.00,'2025-01-01','2025-12-31',1000000.00,200,0,'active');
/*!40000 ALTER TABLE `promotions` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `suppliers`
--

DROP TABLE IF EXISTS `suppliers`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `suppliers` (
  `supplier_id` int NOT NULL AUTO_INCREMENT,
  `name` varchar(100) NOT NULL,
  `phone` varchar(20) DEFAULT NULL,
  `email` varchar(100) DEFAULT NULL,
  `address` text,
  PRIMARY KEY (`supplier_id`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `suppliers`
--

LOCK TABLES `suppliers` WRITE;
/*!40000 ALTER TABLE `suppliers` DISABLE KEYS */;
INSERT INTO `suppliers` VALUES (1,'Công ty ABC','0909123456','abc@gmail.com','Hà Nội'),(2,'Công ty XYZ','0912123456','xyz@gmail.com','TP HCM'),(3,'Công ty 123','0933123456','123@gmail.com','Đà Nẵng');
/*!40000 ALTER TABLE `suppliers` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `users`
--

DROP TABLE IF EXISTS `users`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `users` (
  `user_id` int NOT NULL AUTO_INCREMENT,
  `username` varchar(50) NOT NULL,
  `password` varchar(255) NOT NULL,
  `full_name` varchar(100) DEFAULT NULL,
  `role` enum('admin','staff') DEFAULT 'staff',
  `created_at` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`user_id`),
  UNIQUE KEY `username` (`username`)
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `users`
--

LOCK TABLES `users` WRITE;
/*!40000 ALTER TABLE `users` DISABLE KEYS */;
INSERT INTO `users` VALUES (1,'admin','123456','Quản trị viên','admin','2025-11-27 05:02:24'),(2,'staff01','123456','Nguyễn Văn A','staff','2025-11-27 05:02:24'),(3,'staff02','123456','Lê Thị B','staff','2025-11-27 05:02:24');
/*!40000 ALTER TABLE `users` ENABLE KEYS */;
UNLOCK TABLES;
SET @@SESSION.SQL_LOG_BIN = @MYSQLDUMP_TEMP_LOG_BIN;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2025-11-27 20:08:33
