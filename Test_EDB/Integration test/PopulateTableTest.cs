using DBMS;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Test_EDB.Integration_test
{
    public class PopulateTableTest
    {
        [Fact]
        public void CreateAndInsertSuppliersTest()
        {
            string path = "C:\\temp\\dbms\\", name = "my_store";
            var db = new Database(path, name);
            db.CreateTable(SuppliersTableTest.CreateTableStatement);
            foreach (var query in SuppliersTableTest.InsertStatements)
            {
                db.InsertIntoTable(query);
            }
        }

        [Fact]
        public void CreateAndInsertCategoriesTest()
        {
            string path = "C:\\temp\\dbms\\", name = "my_store";
            var db = new Database(path, name);
            db.CreateTable(CategoriesTableTest.CreateTableStatement);
            foreach (var query in CategoriesTableTest.InsertStatements)
            {
                db.InsertIntoTable(query);
            }
        }

        [Fact]
        public void CreateAndInsertProductsTest()
        {
            string path = "C:\\temp\\dbms\\", name = "my_store";
            var db = new Database(path, name);
            db.CreateTable(ProductsTableTest.CreateTableStatement);
            foreach (var query in ProductsTableTest.InsertStatements)
            {
                db.InsertIntoTable(query);
            }
        }
    }

    public static class SuppliersTableTest
    {
        private static string _createStatement = "CREATE TABLE Suppliers (SupplierID INT index SupplierID_INDEX ,SupplierName VARCHAR(255),ContactName VARCHAR(255),Address VARCHAR(255),City VARCHAR(255),PostalCode VARCHAR(20),Country VARCHAR(255),Phone VARCHAR(20))";

        public static string CreateTableStatement { get { return _createStatement; } }

        private static List<string> _insertStatements = new List<string>
        {
            "INSERT INTO Suppliers VALUES (1,'Exotic Liquid','Charlotte Cooper','49 Gilbert St.','Londona','EC1 4SD','UK','(171) 555-2222')",
            "INSERT INTO Suppliers VALUES (2,'New Orleans Cajun Delights','Shelley Burke','P.O. Box 78934','New Orleans','70117','USA','(100) 555-4822')",
            "INSERT INTO Suppliers VALUES (3,'Grandma Kelly\'s Homestead','Regina Murphy','707 Oxford Rd.','Ann Arbor','48104','USA','(313) 555-5735')",
            "INSERT INTO Suppliers VALUES (4,'Tokyo Traders','Yoshi Nagase','9-8 Sekimai Musashino-shi','Tokyo','100','Japan','(03) 3555-5011')",
            "INSERT INTO Suppliers VALUES (5,'Cooperativa de Quesos \'Las Cabras\'','Antonio del Valle Saavedra ','Calle del Rosal 4','Oviedo','33007','Spain','(98) 598 76 54')",
            "INSERT INTO Suppliers VALUES (6,'Mayumi\'s','Mayumi Ohno','92 Setsuko Chuo-ku','Osaka','545','Japan','(06) 431-7877')",
            "INSERT INTO Suppliers VALUES (7,'Pavlova Ltd.','Ian Devling','74 Rose St. Moonie Ponds','Melbourne','3058','Australia','(03) 444-2343')",
            "INSERT INTO Suppliers VALUES (8,'Specialty Biscuits Ltd.','Peter Wilson','29 King\'s Way','Manchester','M14 GSD','UK','(161) 555-4448')",
            "INSERT INTO Suppliers VALUES (9,'PB Knäckebröd AB','Lars Peterson','Kaloadagatan 13','Göteborg','S-345 67','Sweden ','031-987 65 43')",
            "INSERT INTO Suppliers VALUES (10,'Refrescos Americanas LTDA','Carlos Diaz','Av. das Americanas 12.890','São Paulo','5442','Brazil','(11) 555 4640')",
            "INSERT INTO Suppliers VALUES (11,'Heli Süßwaren GmbH &amp; Co. KG','Petra Winkler','Tiergartenstraße 5','Berlin','10785','Germany','(010) 9984510')",
            "INSERT INTO Suppliers VALUES (12,'Plutzer Lebensmittelgroßmärkte AG','Martin Bein','Bogenallee 51','Frankfurt','60439','Germany','(069) 992755')",
            "INSERT INTO Suppliers VALUES (13,'Nord-Ost-Fisch Handelsgesellschaft mbH','Sven Petersen','Frahmredder 112a','Cuxhaven','27478','Germany','(04721) 8713')",
            "INSERT INTO Suppliers VALUES (14,'Formaggi Fortini s.r.l.','Elio Rossi','Viale Dante 75','Ravenna','48100','Italy','(0544) 60323')",
            "INSERT INTO Suppliers VALUES (15,'Norske Meierier','Beate Vileid','Hatlevegen 5','Sandvika','1320','Norway','(0)2-953010')",
            "INSERT INTO Suppliers VALUES (16,'Bigfoot Breweries','Cheryl Saylor','3400 - 8th Avenue Suite 210','Bend','97101','USA','(503) 555-9931')",
            "INSERT INTO Suppliers VALUES (17,'Svensk Sjöföda AB','Michael Björn','Brovallavägen 231','Stockholm','S-123 45','Sweden','08-123 45 67')",
            "INSERT INTO Suppliers VALUES (18,'Aux joyeux ecclésiastiques','Guylène Nodier','203 Rue des Francs-Bourgeois','Paris','75004','France','(1) 03.83.00.68')",
            "INSERT INTO Suppliers VALUES (19,'New England Seafood Cannery','Robb Merchant','Order Processing Dept. 2100 Paul Revere Blvd.','Boston','02134','USA','(617) 555-3267')",
            "INSERT INTO Suppliers VALUES (20,'Leka Trading','Chandra Leka','471 Serangoon Loop Suite #402','Singapore','0512','Singapore','555-8787')",
            "INSERT INTO Suppliers VALUES (21,'Lyngbysild','Niels Petersen','Lyngbysild Fiskebakken 10','Lyngby','2800','Denmark','43844108')",
            "INSERT INTO Suppliers VALUES (22,'Zaanse Snoepfabriek','Dirk Luchte','Verkoop Rijnweg 22','Zaandam','9999 ZZ','Netherlands','(12345) 1212')",
            "INSERT INTO Suppliers VALUES (23,'Karkki Oy','Anne Heikkonen','Valtakatu 12','Lappeenranta','53120','Finland','(953) 10956')",
            "INSERT INTO Suppliers VALUES (24,'Gday Mate','Wendy Mackenzie','170 Prince Edward Parade Hunter\'s Hill','Sydney','2042','Australia','(02) 555-5914')",
            "INSERT INTO Suppliers VALUES (25,'Ma Maison','Jean-Guy Lauzon','2960 Rue St. Laurent','Montréal','H1J 1C3','Canada','(514) 555-9022')",
            "INSERT INTO Suppliers VALUES (26,'Pasta Buttini s.r.l.','Giovanni Giudici','Via dei Gelsomini 153','Salerno','84100','Italy','(089) 6547665')",
            "INSERT INTO Suppliers VALUES (27,'Escargots Nouveaux','Marie Delamare','22 rue H. Voiron','Montceau','71300','France','85.57.00.07')",
            "INSERT INTO Suppliers VALUES (28,'Gai pâturage','Eliane Noz','Bat. B 3 rue des Alpes','Annecy','74000','France','38.76.98.06')",
            "INSERT INTO Suppliers VALUES (29,'Forêts dérables','Chantal Goulet','148 rue Chasseur','Ste-Hyacinthe','J2S 7S8','Canada','(514) 555-2955')"
        };

        public static List<string> InsertStatements { get { return _insertStatements; } }
    }

    public static class CategoriesTableTest
    {
        private static string _createStatement = "CREATE TABLE Categories (CategoryID INT,CategoryName VARCHAR(255),Description VARCHAR(255))";

        public static string CreateTableStatement { get { return _createStatement; } }

        private static List<string> _insertStatements = new List<string>
        {
            "INSERT INTO Categories VALUES (1, 'Beverages','Soft drinks, coffees, teas, beers, and ales')",
            "INSERT INTO Categories VALUES (2, 'Condiments','Sweet and savory sauces, relishes, spreads, and seasonings')",
            "INSERT INTO Categories VALUES (3, 'Confections','Desserts, candies, and sweet breads')",
            "INSERT INTO Categories VALUES (4, 'Dairy Products','Cheeses')",
            "INSERT INTO Categories VALUES (5, 'Grains/Cereals','Breads, crackers, pasta, and cereal')",
            "INSERT INTO Categories VALUES (6, 'Meat/Poultry','Prepared meats')",
            "INSERT INTO Categories VALUES (7, 'Produce','Dried fruit and bean curd')",
            "INSERT INTO Categories VALUES (8, 'Seafood','Seaweed and fish')"
        };

        public static List<string> InsertStatements { get { return _insertStatements; } }
    }

    public static class ProductsTableTest
    {
        private static string _createStatement = "CREATE TABLE Products (ProductID INT index ProductID_INDEX,ProductName VARCHAR(255),SupplierID INT index SupplierID_INDEX,CategoryID INT,Unit VARCHAR(255),Price INT)";

        public static string CreateTableStatement { get { return _createStatement; } }

        private static List<string> _insertStatements = new List<string>
        {
            "INSERT INTO Products VALUES (1,'Chais',1,1,'10 boxes x 20 bags',18)",
            "INSERT INTO Products VALUES (2,'Chang',1,1,'24 - 12 oz bottles',19)",
            "INSERT INTO Products VALUES (3,'Aniseed Syrup',1,2,'12 - 550 ml bottles',10)",
            "INSERT INTO Products VALUES (4,'Chef Anton\'s Cajun Seasoning',2,2,'48 - 6 oz jars',22)",
            "INSERT INTO Products VALUES (5,'Chef Anton\'s Gumbo Mix',2,2,'36 boxes',21.35)",
            "INSERT INTO Products VALUES (6,'Grandma\'s Boysenberry Spread',3,2,'12 - 8 oz jars',25)",
            "INSERT INTO Products VALUES (7,'Uncle Bob\'s Organic Dried Pears',3,7,'12 - 1 lb pkgs.',30)",
            "INSERT INTO Products VALUES (8,'Northwoods Cranberry Sauce',3,2,'12 - 12 oz jars',40)",
            "INSERT INTO Products VALUES (9,'Mishi Kobe Niku',4,6,'18 - 500 g pkgs.',97)",
            "INSERT INTO Products VALUES (10,'Ikura',4,8,'12 - 200 ml jars',31)",
            "INSERT INTO Products VALUES (11,'Queso Cabrales',5,4,'1 kg pkg.',21)",
            "INSERT INTO Products VALUES (12,'Queso Manchego La Pastora',5,4,'10 - 500 g pkgs.',38)",
            "INSERT INTO Products VALUES (13,'Konbu',6,8,'2 kg box',6)",
            "INSERT INTO Products VALUES (14,'Tofu',6,7,'40 - 100 g pkgs.',23.25)",
            "INSERT INTO Products VALUES (15,'Genen Shouyu',6,2,'24 - 250 ml bottles',15.5)",
            "INSERT INTO Products VALUES (16,'Pavlova',7,3,'32 - 500 g boxes',17.45)",
            "INSERT INTO Products VALUES (17,'Alice Mutton',7,6,'20 - 1 kg tins',39)",
            "INSERT INTO Products VALUES (18,'Carnarvon Tigers',7,8,'16 kg pkg.',62.5)",
            "INSERT INTO Products VALUES (19,'Teatime Chocolate Biscuits',8,3,'10 boxes x 12 pieces',9.2)",
            "INSERT INTO Products VALUES (20,'Sir Rodney\'s Marmalade',8,3,'30 gift boxes',81)",
            "INSERT INTO Products VALUES (21,'Sir Rodney\'s Scones',8,3,'24 pkgs. x 4 pieces',10)",
            "INSERT INTO Products VALUES (22,'Gustaf\'s Knäckebröd',9,5,'24 - 500 g pkgs.',21)",
            "INSERT INTO Products VALUES (23,'Tunnbröd',9,5,'12 - 250 g pkgs.',9)",
            "INSERT INTO Products VALUES (24,'Guaraná Fantástica',10,1,'12 - 355 ml cans',4.5)",
            "INSERT INTO Products VALUES (25,'NuNuCa Nuß-Nougat-Creme',11,3,'20 - 450 g glasses',14)",
            "INSERT INTO Products VALUES (26,'Gumbär Gummibärchen',11,3,'100 - 250 g bags',31.23)",
            "INSERT INTO Products VALUES (27,'Schoggi Schokolade',11,3,'100 - 100 g pieces',43.9)",
            "INSERT INTO Products VALUES (28,'Rössle Sauerkraut',12,7,'25 - 825 g cans',45.6)",
            "INSERT INTO Products VALUES (29,'Thüringer Rostbratwurst',12,6,'50 bags x 30 sausgs.',123.79)",
            "INSERT INTO Products VALUES (30,'Nord-Ost Matjeshering',13,8,'10 - 200 g glasses',25.89)",
            "INSERT INTO Products VALUES (31,'Gorgonzola Telino',14,4,'12 - 100 g pkgs',12.5)",
            "INSERT INTO Products VALUES (32,'Mascarpone Fabioli',14,4,'24 - 200 g pkgs.',32)",
            "INSERT INTO Products VALUES (33,'Geitost',15,4,'500 g',2.5)",
            "INSERT INTO Products VALUES (34,'Sasquatch Ale',16,1,'24 - 12 oz bottles',14)",
            "INSERT INTO Products VALUES (35,'Steeleye Stout',16,1,'24 - 12 oz bottles',18)",
            "INSERT INTO Products VALUES (36,'Inlagd Sill',17,8,'24 - 250 g jars',19)",
            "INSERT INTO Products VALUES (37,'Gravad lax',17,8,'12 - 500 g pkgs.',26)",
            "INSERT INTO Products VALUES (38,'Côte de Blaye',18,1,'12 - 75 cl bottles',263.5)",
            "INSERT INTO Products VALUES (39,'Chartreuse verte',18,1,'750 cc per bottle',18)",
            "INSERT INTO Products VALUES (40,'Boston Crab Meat',19,8,'24 - 4 oz tins',18.4)",
            "INSERT INTO Products VALUES (41,'Jack\'s New England Clam Chowder',19,8,'12 - 12 oz cans',9.65)",
            "INSERT INTO Products VALUES (42,'Singaporean Hokkien Fried Mee',20,5,'32 - 1 kg pkgs.',14)",
            "INSERT INTO Products VALUES (43,'Ipoh Coffee',20,1,'16 - 500 g tins',46)",
            "INSERT INTO Products VALUES (44,'Gula Malacca',20,2,'20 - 2 kg bags',19.45)",
            "INSERT INTO Products VALUES (45,'Røgede sild',21,8,'1k pkg.',9.5)",
            "INSERT INTO Products VALUES (46,'Spegesild',21,8,'4 - 450 g glasses',12)",
            "INSERT INTO Products VALUES (47,'Zaanse koeken',22,3,'10 - 4 oz boxes',9.5)",
            "INSERT INTO Products VALUES (48,'Chocolade',22,3,'10 pkgs.',12.75)",
            "INSERT INTO Products VALUES (49,'Maxilaku',23,3,'24 - 50 g pkgs.',20)",
            "INSERT INTO Products VALUES (50,'Valkoinen suklaa',23,3,'12 - 100 g bars',16.25)",
            "INSERT INTO Products VALUES (51,'Manjimup Dried Apples',24,7,'50 - 300 g pkgs.',53)",
            "INSERT INTO Products VALUES (52,'Filo Mix',24,5,'16 - 2 kg boxes',7)",
            "INSERT INTO Products VALUES (53,'Perth Pasties',24,6,'48 pieces',32.8)",
            "INSERT INTO Products VALUES (54,'Tourtière',25,6,'16 pies',7.45)",
            "INSERT INTO Products VALUES (55,'Pâté chinois',25,6,'24 boxes x 2 pies',24)",
            "INSERT INTO Products VALUES (56,'Gnocchi di nonna Alice',26,5,'24 - 250 g pkgs.',38)",
            "INSERT INTO Products VALUES (57,'Ravioli Angelo',26,5,'24 - 250 g pkgs.',19.5)",
            "INSERT INTO Products VALUES (58,'Escargots de Bourgogne',27,8,'24 pieces',13.25)",
            "INSERT INTO Products VALUES (59,'Raclette Courdavault',28,4,'5 kg pkg.',55)",
            "INSERT INTO Products VALUES (60,'Camembert Pierrot',28,4,'15 - 300 g rounds',34)",
            "INSERT INTO Products VALUES (61,'Sirop d\'érable',29,2,'24 - 500 ml bottles',28.5)",
            "INSERT INTO Products VALUES (62,'Tarte au sucre',29,3,'48 pies',49.3)",
            "INSERT INTO Products VALUES (63,'Vegie-spread',7,2,'15 - 625 g jars',43.9)",
            "INSERT INTO Products VALUES (64,'Wimmers gute Semmelknödel',12,5,'20 bags x 4 pieces',33.25)",
            "INSERT INTO Products VALUES (65,'Louisiana Fiery Hot Pepper Sauce',2,2,'32 - 8 oz bottles',21.05)",
            "INSERT INTO Products VALUES (66,'Louisiana Hot Spiced Okra',2,2,'24 - 8 oz jars',17)",
            "INSERT INTO Products VALUES (67,'Laughing Lumberjack Lager',16,1,'24 - 12 oz bottles',14)",
            "INSERT INTO Products VALUES (68,'Scottish Longbreads',8,3,'10 boxes x 8 pieces',12.5)",
            "INSERT INTO Products VALUES (69,'Gudbrandsdalsost',15,4,'10 kg pkg.',36)",
            "INSERT INTO Products VALUES (70,'Outback Lager',7,1,'24 - 355 ml bottles',15)",
            "INSERT INTO Products VALUES (71,'Fløtemysost',15,4,'10 - 500 g pkgs.',21.5)",
            "INSERT INTO Products VALUES (72,'Mozzarella di Giovanni',14,4,'24 - 200 g pkgs.',34.8)",
            "INSERT INTO Products VALUES (73,'Röd Kaviar',17,8,'24 - 150 g jars',15)",
            "INSERT INTO Products VALUES (74,'Longlife Tofu',4,7,'5 kg pkg.',10)",
            "INSERT INTO Products VALUES (75,'Rhönbräu Klosterbier',12,1,'24 - 0.5 l bottles',7.75)",
            "INSERT INTO Products VALUES (76,'Lakkalikööri',23,1,'500 ml ',18)",
            "INSERT INTO Products VALUES (77,'Original Frankfurter grüne Soße',12,2,'12 boxes',13)"
        };

        public static List<string> InsertStatements { get { return _insertStatements; } }
    }

}
