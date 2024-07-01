Notes:
 Nuget Packages;
 + Nuget System.Data.SQLite package install:  NuGet\Install-Package System.Data.SQLite -Version 1.0.118
 + Nuget mode_spatialilte package install: NuGet\Install-Package mod_spatialite -Version 4.3.0.1 
 
 Spatialite:
	+ Spatialite Sqlite dosya bazlı veri tabanı motorunun konumsal veri saklama ve soruglama yeteneklerinin eklenmiş halidir. 
	+ Spatialite dosyalarında sözel veriler ile birlikte geometrik özellikli(nokta, çizgi, alan) verileri ilişkisel tablolarda saklayabilir ve bu verileri üzerinden çeşitli 
		coğrafi sorgular(kesişim, en yakın mesafe, Koordinat vb.) yapılabilir. 
		 

 Referans linkler;
 + https://dominoc925.blogspot.com/2016/10/simple-c-example-for-creating-and-using.html
 + https://www.gaia-gis.it/gaia-sins/spatialite-sql-4.2.0.html
 + https://www.gaia-gis.it/gaia-sins/spatialite-cookbook-5/index.html

 QGIS:
 + template.sqlite dosyası içindeki verileri görsel olarak görmek için kullanılabilecek açık kaynak kodlu GIS yazılımı.
 + https://www.qgis.org/en/site/
 + https://www.youtube.com/watch?v=gC0z4miZmtQ

 template.sqlite: 
    + Dosya içerisinde, building, building_nodes, door isminde 3 adet tablo bulunmaktadır.
	+ Tablolar arasındaki ilişki spatialtest.jpg dosyasında mevcuttur. 

 Program Çalışma şekli;
 + Program komut satırı uygulaması olacak
 + Kullanıcı programı çalıştırdığında, karşısına 3 adet seçenek çıkacak,
 + Kullanıcı bu 3 adet seçeneği klaveyden komut girerek istediği sıra ile çalıştırabilecek, 


 + 1. Seçenek de template veri tabanındaki building_nodes tablosundaki kayıtlar okunarak, building_id ve node_order kolonlarına göre bina geometrik objeleri oluşturulacak
	ve building tablosuna kayıt edilecek, ilgili bina, tabloda zaten mevcutsa silinip tekrar eklenecek 




 + 2. Seçenek de door tablosu okunarak her bir kapının konumu, building_id'ye göre building tablosu ile karşılaştırılarak, ilgili kapının binanın içinde mi yoksa dışında mı
   olduğu spatialite kütüphanesi konumsal analiz fonksiyonları kullanılarak tespit edilecek ve sonuçlar door tablosunda inside_building kolonuna, içinde için "1", dışında için "0" olacak şekilde update edilecek. 
   Eğer ilgili kapi için building tablosunda kayıt yoksa inside_building kolonu null olarak update edilecek.



 + 3. Seçenek de door sonuçları aşağıdaki örnek şeklinde ekrana yazdırılacak;
	* Kapı id: 1, Bina id: 2, Kapı No: 2A, Bina İçinde Mi: Evet --inside_building kolonu 1 olan kayıt
	* Kapi id: 2, Bina id: 3, Kapı No: 16, Bina İçinde Mi: Hayır --inside_building kolonu 0 olan kayıt
	* Kapi id: 3, Bina id: 1, Kapı No: 12, Bina İçinde Mi: Tespit Edilemedi --inside_building kolonun null olan kayıt
