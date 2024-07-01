using System;
using System.Data.SQLite;


namespace SpatialiteTest
{
    class Program
    {
        static void Main(string[] args)
        {
            string connectionString = "Data Source=template.sqlite;Version=3;";

            while (true)    //Çıkış yapılmadığı takdirde seçimin tekrar yapılabilmesi için.
            {
                Console.WriteLine("Lütfen bir seçenek seçiniz:");
                Console.WriteLine("1. Bina geometrik objeleri oluştur ve kaydet");
                Console.WriteLine("2. Kapıların, binanın içinde mi dışında mı olduğunu belirle ve güncelle");
                Console.WriteLine("3. Kapı sonuçlarını görüntüle");
                Console.WriteLine("4. Çıkış");
                string secim = Console.ReadLine();

                switch (secim)
                {
                    case "1":
                        CreateAndSave(connectionString);
                        break;
                    case "2":
                        DoorInOut(connectionString);
                        break;
                    case "3":
                        DoorResult(connectionString);
                        break;
                    case "4":
                        Console.WriteLine("Programdan çıkılıyor...");
                        return;
                    default:
                        Console.WriteLine("Geçersiz seçim. Lütfen tekrar deneyin...");
                        break;
                }
            }
        }
        static void CreateAndSave(string connectionString)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                connection.LoadExtension("mod_spatialite");
                var command = connection.CreateCommand();
                command.CommandText = @"SELECT building_id, node_order, st_x(geom) as x, st_y(geom) as y FROM building_nodes";
                using (var reader = command.ExecuteReader())
                {
                    string polygonText = "POLYGON(";
                    string pointList = "";
                    string temp = "";
                    while (reader.Read())
                    {
                        var buildingId = reader.GetValue(0);
                        var nodeOrder = reader.GetValue(1);
                        double x = reader.GetDouble(2);
                        double y = reader.GetDouble(3);
                       

                        //POLYGON(10 10, 20 20, 30 30, 10 10)
                        if (temp == "")
                        {
                            temp = buildingId.ToString();
                        }
                        if(temp != buildingId.ToString())
                        {
                            polygonText += $"({pointList + pointList.Split(',')[0]}))";
                            using (var insertCmd = new SQLiteCommand("REPLACE INTO building (id, geom) VALUES (@buildingId, GeomFromText(@polygonText,4326))", connection))
                            {
                                insertCmd.Parameters.AddWithValue("@polygonText", polygonText);
                                insertCmd.Parameters.AddWithValue("@buildingId", temp);
                                insertCmd.ExecuteNonQuery();
                            }
                            temp = buildingId.ToString();
                            polygonText = "POLYGON(";
                            pointList = "";
                        }
                        pointList += $"{x.ToString().Replace(',', '.')} {y.ToString().Replace(',', '.')},";
                    }
                    
                }
            }
            Console.WriteLine("Building geometrik objeleri oluşturuldu ve kaydedildi.");
        }


        static void DoorInOut(string connectionString)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                connection.LoadExtension("mod_spatialite");
                string sql = @"UPDATE door
                                    SET inside_building = 
                                    CASE
                                        WHEN b.id IS NOT NULL AND ST_Within(d.geom, b.geom) THEN 1
                                        WHEN b.id IS NOT NULL AND NOT ST_Within(d.geom, b.geom) THEN 0
                                        ELSE NULL
                                    END
                            FROM door d
                            LEFT JOIN building b ON d.building_id = b.id;";

                /*CREATE TEMPORARY TABLE temp AS
                  SELECT d.*, 
                    CASE
                        WHEN b.id IS NOT NULL AND ST_Within(d.geom, b.geom) THEN 1
                        WHEN b.id IS NOT NULL AND NOT ST_Within(d.geom, b.geom) THEN 0
                        ELSE NULL
                    END AS inside_building
                  FROM door d
                  LEFT JOIN building b ON d.building_id = b.id;

                  UPDATE door
                  SET inside_building = (SELECT inside_building FROM temp WHERE door.id = temp.id);

                  DROP TABLE temp;*/
                using (SQLiteCommand command = new SQLiteCommand(sql, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
            Console.WriteLine("Kapıların içinde mi dışında mı olduğu belirlendi ve güncellendi.");
        }


        static void DoorResult(string connectionString)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                connection.LoadExtension("mod_spatialite");
                var command = connection.CreateCommand();
                command.CommandText = @"select id, building_id, door_no, inside_building from door";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var doorId = reader.GetValue(0);
                        var buildingId = reader.GetValue(1);
                        var doorNo = reader.GetValue(2);
                        var inside = reader.GetValue(3);
                        switch (inside)
                        {
                            case 0:
                                inside = "Hayır";
                                break;
                            case 1:
                                inside = "Evet";
                                break;
                            default:
                                inside = "Tespit Edilemedi";
                                break;
                        }
                        Console.WriteLine($"Kapi id: {doorId}, Bina id: {buildingId}, Kapi No: {doorNo}, Bina İçinde Mi: {inside}");
                    }
                }
            }
        }
    }
}
//Console.ReadLine();
