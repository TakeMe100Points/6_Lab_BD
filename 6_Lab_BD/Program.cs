using System;
using MySql.Data.MySqlClient;

namespace _6_Lab_BD
{
    class DBConnection
    {
        public DBConnection(){}

        public string Server { get; set; }
        public string DatabaseName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        public MySqlConnection Connection { get; set; }

        private static DBConnection _instance = null;
        public static DBConnection Instance()
        {
            if (_instance == null)
                _instance = new DBConnection();
            return _instance;
        }

        public bool IsConnect()
        {
            if (Connection == null)
            {
                if (String.IsNullOrEmpty(DatabaseName))
                    return false;

                string connstring = string.Format("Server={0}; database={1}; UID={2}; password={3}", Server, DatabaseName, UserName, Password);
                Connection = new MySqlConnection(connstring);
                Connection.Open();
            }

            return true;
        }

        public void Close()
        {
            Connection.Close();
        }

        public MySqlDataReader ExecuteRequest(string request)
        {
            var cmd = new MySqlCommand(request, this.Connection);
            return cmd.ExecuteReader();
        }

        public static void InitializeDBConnection(DBConnection db)
        {
            db.Server = Constants.Server;
            db.Password = Constants.Password;
            db.UserName = Constants.UserName;
            db.DatabaseName = Constants.DatabaseName;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var db = DBConnection.Instance();

            DBConnection.InitializeDBConnection(db);

            if (db.IsConnect())
            {
                //Получить список пользователей (не более 10)
                Console.WriteLine("======================================");
                Console.WriteLine("Получить список пользователей (не более 10)");
                string request = "select name from user limit 10;";
                var reader = db.ExecuteRequest(request);
                while (reader.Read())
                    Console.WriteLine(reader.GetString(0));
                reader.Close();
                Console.ReadLine();

                //Создать новый диалог
                Console.WriteLine("======================================");            
                Console.WriteLine("Создать новый диалог");
                request = "insert dialog (name, id_admin) values(\"Политех new\", \"42\"); ";
                db.ExecuteRequest(request).Close();
                Console.ReadLine();


                //Обновить название
                Console.WriteLine("======================================");
                Console.WriteLine("Изменить название диалога");
                request = "update dialog set name = \"VSTU new\" where name = \"Политех new\";";
                db.ExecuteRequest(request).Close();
                Console.ReadLine();

                //Удалить добавленный диалог
                Console.WriteLine("======================================");
                Console.WriteLine("Удалить новый диалог");
                request = "delete from dialog where name = \"VSTU new\"";
                db.ExecuteRequest(request).Close();
                Console.ReadLine();

                //Сменить пароль
                Console.WriteLine("======================================");
                Console.WriteLine("Сменить пароль");
                request = "update user set password = \"new_password131231\" where id = 25;";
                db.ExecuteRequest(request).Close();
                Console.ReadLine();

                //Отправить сообщение
                Console.WriteLine("======================================");
                Console.WriteLine("Отправить сообщение");
                request = "insert message (send_date, life_time, dialog_id, text_message) values(\"2020-11-24 09:22:30\", \"2:00:00\", \"4\", \"Привет всем!\"); ";
                db.ExecuteRequest(request).Close();
                Console.ReadLine();

                //Удалить пользователя
                Console.WriteLine("======================================");
                Console.WriteLine("Удалить пользователя");
                request = "delete from user where id = \"40\"; ";
                db.ExecuteRequest(request).Close();
                Console.ReadLine();

                //Получить среднее количество сообщений в диалоге
                Console.WriteLine("======================================");
                Console.WriteLine("Среднее количество сообщений в диалоге");
                request = "select count(*) / count(distinct dialog_id) from message;";
                reader = db.ExecuteRequest(request);
                while (reader.Read())
                    Console.WriteLine(reader.GetString(0));
                reader.Close();
                Console.ReadLine();


                //Получить среднее количество пользователей в диалоге
                Console.WriteLine("======================================");
                Console.WriteLine("Cреднее количество пользователей в диалоге");
                request = "select count(*) / count(distinct id_user) from user_dialog;";
                reader = db.ExecuteRequest(request);
                while (reader.Read())
                    Console.WriteLine(reader.GetString(0));
                reader.Close();
                Console.ReadLine();


                db.Close();

            }
        }

    }
}
