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
            db.Server       = Constants.Server;
            db.Password     = Constants.Password;
            db.UserName     = Constants.UserName;
            db.DatabaseName = Constants.DatabaseName;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var db = DBConnection.Instance();

            DBConnection.InitializeDBConnection(db);

            if (!db.IsConnect())
            {
                Console.WriteLine("Ошибка подключения к базе данных");
                return;
            }
            MySqlDataReader reader = null;
           
            while (true)
            {
                Console.WriteLine("=======================================================");
                Console.WriteLine("Выберите номер действия:");
                Console.WriteLine("1 - Вывести подходящие стикеры для сообщения с текстом...\n" +
                                  "2 - Добавить/Редактировать/Удалить пользователя\n" +
                                  "3 - Добавить/Редактировать/Удалить диалог \n" +
                                  "4 - Показать все сообщения пользователя с именем...\n" + 
                                  "5 - Показать все вложения диалога с названием...\n" +
                                  "6 - Показать всех друзей для пользователя..." + 
                                  "Exit - to exit from app");
                Console.WriteLine("=======================================================");

                try
                {
                    switch (Console.ReadLine())
                    {
                        case "1":
                            {
                                Console.WriteLine("Введите текст для подбора стикеров");
                                string text = Console.ReadLine();
                                string request = $"select *	from stickers where text_sticker like '%{text}%'; ";
                                reader = db.ExecuteRequest(request);

                                while (reader?.Read() ?? false)
                                    Console.WriteLine("|" + reader?.GetString(0) + "|" + reader?.GetString(1) + "|" + reader?.GetString(2) + "|" + reader?.GetString(3) + "|");
                               
                                reader?.Close();
                                Console.ReadLine();
                                break;
                            }
                        case "2":
                            {
                                Console.WriteLine("1 - Добавить \n 2 - Редактировать \n 3 - Удалить");
                                string request;

                                switch (Console.ReadLine())
                                {
                                    case "1":
                                        {
                                            Console.WriteLine("Введите ваше имя");
                                            string name = Console.ReadLine();

                                            Console.WriteLine("Введите пароль");
                                            string pass = Console.ReadLine();

                                            request = $"insert user (name, password) values ('{name}', '{pass}');";
                                            break;
                                        }
                                    case "2":
                                        {
                                            Console.WriteLine("Введите ваше прошлое имя");
                                            string name = Console.ReadLine();

                                            Console.WriteLine("Введите ваше новое имя");
                                            string newname = Console.ReadLine();

                                            request = $"update user set name = '{newname}' where name = '{name}'";
                                            break;
                                        }
                                    default:
                                        {
                                            Console.WriteLine("Введите имя аккаунта, который вы хотите удалить:");
                                            string name = Console.ReadLine();
                                            request = $"delete from user where name = '{name}'";
                                            break;
                                        }
                                }
                                reader = db.ExecuteRequest(request);
                                reader?.Close();
                                break;
                            }
                        case "3":
                            {
                                Console.WriteLine("1 - Добавить \n 2 - Редактировать \n 3 - Удалить");
                                string request;

                                switch (Console.ReadLine())
                                {
                                    case "1":
                                        {
                                            Console.WriteLine("Введите имя диалога");
                                            string name = Console.ReadLine();

                                            Console.WriteLine("Введите идентификатор пользователя, который будет администратором");
                                            string admin = Console.ReadLine();

                                            request = $"insert dialog (name, id_admin) values ('{name}', '{admin}');";
                                            break;
                                        }
                                    case "2":
                                        {
                                            Console.WriteLine("Введите старое название диалога");
                                            string name = Console.ReadLine();

                                            Console.WriteLine("Введите новое название диалога");
                                            string newname = Console.ReadLine();

                                            request = $"update dialog set name = '{newname}' where name = '{name}'";
                                            break;
                                        }
                                    default:
                                        {
                                            Console.WriteLine("Введите название диалога, который вы хотите удалить:");
                                            string name = Console.ReadLine();
                                            request = $"delete from dialog where name = '{name}'";
                                            break;
                                        }
                                }
                                reader = db.ExecuteRequest(request);
                                reader?.Close();
                                break;
                            }
                        case "4":
                            {
                                Console.WriteLine("Введите имя человека");
                                string name = Console.ReadLine();

                                string request = "select message.text_message from user " +
                                    "join user_dialog on id_user = user.id " +
                                    "join dialog on id_dialog = dialog.id " +
                                    "join message on dialog_id = dialog.id " +
                                    $"where user.name = '{name}' and text_message is not null;";

                                reader = db.ExecuteRequest(request);
                                
                                while (reader.Read())
                                    Console.WriteLine("|" + reader.GetString(0) ?? "null" + "|" + reader.GetString(1) ?? "null" + "|");
                                reader.Close();
                                break;
                            }
                        case "5":
                            {
                                Console.WriteLine("Введите название диалога");
                                string name = Console.ReadLine();

                                string request = "select ref_to_content from content " +
                                                        "join message on message_id = message.id " +
                                                        "join dialog on dialog_id = dialog.id " +
                                                         $"where dialog.name = '{name}'; ";
                                reader = db.ExecuteRequest(request);

                                while (reader?.Read() ?? false)
                                    Console.WriteLine("|" + reader?.GetString(0) + "|");

                                reader?.Close();
                                break;
                            }
                        case "6":
                            {
                                Console.WriteLine("Введите имя пользователя");
                                string name = Console.ReadLine();

                                string request = " select user1.name " +
                                                        "from user as user1 " +
                                                        "join user_dialog on user1.id = user_dialog.id_user " +
                                                        "join user as user2 " +
                                                        "where id_dialog in " +
                                                        "(select id_dialog from user_dialog " +
                                                        "join user on user_dialog.id_user = user.id " +
                                                        $"where user.name = '{name}') and user1.name != '{name}' and user2.name = '{name}';";

                                reader = db.ExecuteRequest(request);

                                while (reader?.Read() ?? false)
                                    Console.WriteLine("|" + reader?.GetString(0) + "|");
                                reader?.Close();
                                break;
                            }
                        case "Exit":
                            {
                                Console.WriteLine("Выход из приложения...");
                                return;
                                break;
                            }
                        default:
                            {
                                Console.WriteLine("Вы ввели неверное действие, повторите снова...");
                                break;
                            }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    reader?.Close();
                }



            }
            db.Close();
        }

    }
}
