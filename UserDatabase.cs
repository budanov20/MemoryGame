using System;
using System.Collections.Generic;
using System.Data.SQLite;


namespace MemoryGame
{
    public class UserDatabase : IDisposable
    {
        public SQLiteConnection _connection;

        public UserDatabase(string databasePath)
        {
            _connection = new SQLiteConnection($"Data Source={databasePath};Version=3;");
            _connection.Open();
            InitializeDatabase();
            FillTablesIfEmpty();
            AddAdminIfNotExists();
        }

        private void InitializeDatabase()
        {

            // Create the UserRole table if it doesn't exist
            string createUserRoleTableSql = @"CREATE TABLE IF NOT EXISTS UserRole (ID INTEGER PRIMARY KEY AUTOINCREMENT, RoleName TEXT NOT NULL);";
            using (var command = new SQLiteCommand(createUserRoleTableSql, _connection))
            {
                command.ExecuteNonQuery();
            }

            // Create the GameType table if it doesn't exist
            string createGameTypeTableSql = @"CREATE TABLE IF NOT EXISTS GameType (ID INTEGER PRIMARY KEY AUTOINCREMENT, TypeName TEXT NOT NULL);";
            using (var command = new SQLiteCommand(createGameTypeTableSql, _connection))
            {
                command.ExecuteNonQuery();
            }

            // Create the Game table if it doesn't exist
            string createGameTableSql = @"CREATE TABLE IF NOT EXISTS Game (ID INTEGER PRIMARY KEY AUTOINCREMENT, GameName TEXT NOT NULL, Description TEXT NOT NULL, GameTypeID INTEGER NOT NULL, FOREIGN KEY(GameTypeID) REFERENCES GameType(ID));";
            using (var command = new SQLiteCommand(createGameTableSql, _connection))
            {
                command.ExecuteNonQuery();
            }

            // Create the Gender table if it doesn't exist
            string createGenderTableSql = @"CREATE TABLE IF NOT EXISTS Gender (ID INTEGER PRIMARY KEY AUTOINCREMENT, Name TEXT NOT NULL);";
            using (var command = new SQLiteCommand(createGenderTableSql, _connection))
            {
                command.ExecuteNonQuery();
            }

            // Create the EducationLevel table if it doesn't exist
            string createEducationLevelTableSql = @"CREATE TABLE IF NOT EXISTS EducationLevel (ID INTEGER PRIMARY KEY AUTOINCREMENT, Name TEXT NOT NULL);";
            using (var command = new SQLiteCommand(createEducationLevelTableSql, _connection))
            {
                command.ExecuteNonQuery();
            }

            // Create the EducationType table if it doesn't exist
            string createEducationTypeTableSql = @"CREATE TABLE IF NOT EXISTS EducationType (ID INTEGER PRIMARY KEY AUTOINCREMENT, Name TEXT NOT NULL);";
            using (var command = new SQLiteCommand(createEducationTypeTableSql, _connection))
            {
                command.ExecuteNonQuery();
            }

            // Create the Users table if it doesn't exist
            var createTableSql = @"CREATE TABLE IF NOT EXISTS Users (Id INTEGER PRIMARY KEY AUTOINCREMENT, Name TEXT NOT NULL, Email TEXT NOT NULL, Password TEXT NOT NULL, Age INTEGER NOT NULL, RoleID INTEGER NOT NULL,
                                    GenderID INTEGER NOT NULL, EducationLevelID INTEGER NOT NULL, EducationTypeID INTEGER NOT NULL, SizeScore INTEGER NOT NULL, ColorScore INTEGER NOT NULL, OneColorScore INTEGER NOT NULL,
                                    ColorSizeScore INTEGER NOT NULL, FOREIGN KEY(RoleID) REFERENCES UserRole(ID), FOREIGN KEY(GenderID) REFERENCES Gender(ID), FOREIGN KEY(EducationLevelID) REFERENCES EducationLevel(ID),
                                    FOREIGN KEY(EducationTypeID) REFERENCES EducationType(ID));";
            using (var command = new SQLiteCommand(createTableSql, _connection))
            {
                command.ExecuteNonQuery();
            }

            // Create the Result table if it doesn't exist
            string createResultTableSql = @"CREATE TABLE IF NOT EXISTS Result (ID INTEGER PRIMARY KEY AUTOINCREMENT, UserID INTEGER NOT NULL, GameID INTEGER NOT NULL, result INTEGER NOT NULL, data TEXT NOT NULL, 
            ResultDateTime DATETIME NOT NULL, FOREIGN KEY(UserID) REFERENCES Users(Id), FOREIGN KEY(GameID) REFERENCES Game(ID));";
            using (var command = new SQLiteCommand(createResultTableSql, _connection))
            {
                command.ExecuteNonQuery();
            }
        }
        public void AddUser(User user)
        {
            // Insert a new user into the Users table
            var insertUserSql = @"INSERT INTO Users (Name, Email, Password, Age, RoleID, GenderID, EducationLevelID, EducationTypeID, SizeScore, ColorScore, OneColorScore, ColorSizeScore) 
                        VALUES (@Name, @Email, @Password, @Age, @RoleID, @GenderID, @EducationLevelID, @EducationTypeID, @SizeScore, @ColorScore, @OneColorScore, @ColorSizeScore);";
            using (var command = new SQLiteCommand(insertUserSql, _connection))
            {
                command.Parameters.AddWithValue("@Name", user.Name);
                command.Parameters.AddWithValue("@Email", user.Email);
                command.Parameters.AddWithValue("@Password", user.Password);
                command.Parameters.AddWithValue("@Age", user.Age);
                command.Parameters.AddWithValue("@RoleID", user.RoleID);
                command.Parameters.AddWithValue("@GenderID", user.GenderID);
                command.Parameters.AddWithValue("@EducationLevelID", user.EducationLevelID);
                command.Parameters.AddWithValue("@EducationTypeID", user.EducationTypeID);
                command.Parameters.AddWithValue("@SizeScore", user.SizeScore);
                command.Parameters.AddWithValue("@ColorScore", user.ColorScore);
                command.Parameters.AddWithValue("@OneColorScore", user.OneColorScore);
                command.Parameters.AddWithValue("@ColorSizeScore", user.ColorSizeScore);
                command.ExecuteNonQuery();
            }
        }
        public void UpdateUser(User user)
        {
            // Update user in the Users table
            var updateUserSql = @"UPDATE Users SET Name = @Name, Password = @Password, Age = @Age, RoleID = @RoleID, GenderID = @GenderID, EducationLevelID = @EducationLevelID, EducationTypeID = @EducationTypeID, SizeScore = @SizeScore, ColorScore = @ColorScore, OneColorScore = @OneColorScore, ColorSizeScore = @ColorSizeScore WHERE Email = @Email;";
            using (var command = new SQLiteCommand(updateUserSql, _connection))
            {
                command.Parameters.AddWithValue("@Name", user.Name);
                command.Parameters.AddWithValue("@Email", user.Email);
                command.Parameters.AddWithValue("@Password", user.Password);
                command.Parameters.AddWithValue("@Age", user.Age);
                command.Parameters.AddWithValue("@RoleID", user.RoleID);
                command.Parameters.AddWithValue("@GenderID", user.GenderID);
                command.Parameters.AddWithValue("@EducationLevelID", user.EducationLevelID);
                command.Parameters.AddWithValue("@EducationTypeID", user.EducationTypeID);
                command.Parameters.AddWithValue("@SizeScore", user.SizeScore);
                command.Parameters.AddWithValue("@ColorScore", user.ColorScore);
                command.Parameters.AddWithValue("@OneColorScore", user.OneColorScore);
                command.Parameters.AddWithValue("@ColorSizeScore", user.ColorSizeScore);
                command.ExecuteNonQuery();
            }
        }
        public List<User> GetUsers()
        {
            // Get all users in the database
            var getUsersSql = @"SELECT * FROM Users";
            List<User> users = new List<User>();
            using (var command = new SQLiteCommand(getUsersSql, _connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        User u = new User(
                            (long)reader["Id"],
                            reader["Name"].ToString(),
                            reader["Email"].ToString(),
                            reader["Password"].ToString(),
                            int.Parse(reader["Age"].ToString()),
                            int.Parse(reader["GenderID"].ToString()),
                            int.Parse(reader["RoleID"].ToString()),
                            int.Parse(reader["EducationLevelID"].ToString()),
                            int.Parse(reader["EducationTypeID"].ToString()),
                            int.Parse(reader["SizeScore"].ToString()),
                            int.Parse(reader["ColorScore"].ToString()),
                            int.Parse(reader["OneColorScore"].ToString()),
                            int.Parse(reader["ColorSizeScore"].ToString()));
                        users.Add(u);
                    }
                }
            }
            return users;
        }
        public User FindUserByEmail(string email)
        {
            // Find user by provided email
            var selectUserSql = @"SELECT * FROM Users WHERE Email = @Email;";
            using (var command = new SQLiteCommand(selectUserSql, _connection))
            {
                command.Parameters.AddWithValue("@Email", email);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        long Id = (long)reader["Id"];
                        string name = reader["Name"].ToString();
                        int age = int.Parse(reader["Age"].ToString());
                        string password = reader["Password"].ToString();
                        int genderID = int.Parse(reader["GenderID"].ToString());
                        int roleID = int.Parse(reader["RoleID"].ToString());
                        int educationLevelID = int.Parse(reader["EducationLevelID"].ToString());
                        int educationTypeID = int.Parse(reader["EducationTypeID"].ToString());
                        int sizeScore = int.Parse(reader["SizeScore"].ToString());
                        int colorScore = int.Parse(reader["ColorScore"].ToString());
                        int oneColorScore = int.Parse(reader["OneColorScore"].ToString());
                        int colorSizeScore = int.Parse(reader["ColorSizeScore"].ToString());

                        var user = new User(Id, name, email, password, age, genderID, roleID, educationLevelID, educationTypeID, sizeScore, colorScore, oneColorScore, colorSizeScore);
                        return user;
                    }
                }
            }
            return null;
        }
        public void AddUserRole(string roleName)
        {
            string insertUserRoleSql = "INSERT INTO UserRole (RoleName) VALUES (@roleName);";
            using (var command = new SQLiteCommand(insertUserRoleSql, _connection))
            {
                command.Parameters.AddWithValue("@roleName", roleName);
                command.ExecuteNonQuery();
            }
        }
        public void AddGender(string name)
        {
            string insertGenderSql = "INSERT INTO Gender (Name) VALUES (@name);";
            using (var command = new SQLiteCommand(insertGenderSql, _connection))
            {
                command.Parameters.AddWithValue("@name", name);
                command.ExecuteNonQuery();
            }
        }
        public void AddEducationLevel(string name)
        {
            string insertEducationLevelSql = "INSERT INTO EducationLevel (Name) VALUES (@name);";
            using (var command = new SQLiteCommand(insertEducationLevelSql, _connection))
            {
                command.Parameters.AddWithValue("@name", name);
                command.ExecuteNonQuery();
            }
        }
        public void AddEducationType(string name)
        {
            string insertEducationTypeSql = "INSERT INTO EducationType (Name) VALUES (@name);";
            using (var command = new SQLiteCommand(insertEducationTypeSql, _connection))
            {
                command.Parameters.AddWithValue("@name", name);
                command.ExecuteNonQuery();
            }
        }
        public bool GoodLogin(string email, string pass)
        {
            User tempuser = this.FindUserByEmail(email);
            return (tempuser != null) && (tempuser.Password == pass);
        }
        public User Login(string email, string pass)
        {
            User tempuser = this.FindUserByEmail(email);
            if( (tempuser != null) && (tempuser.Password == pass))
            {
                return tempuser;
            }
            else return null;
        }
        public void FillTablesIfEmpty()
        {
            FillUserRolesIfEmpty();
            FillGameTypesIfEmpty();
            FillGamesIfEmpty();
            FillGendersIfEmpty();
            FillEducationLevelsIfEmpty();
            FillEducationTypesIfEmpty();
        }
        private void FillUserRolesIfEmpty()
        {
            var countSql = @"SELECT COUNT(*) FROM UserRole;";
            using (var command = new SQLiteCommand(countSql, _connection))
            {
                var count = (long)command.ExecuteScalar();
                if (count == 0)
                {
                    var insertSql = @"INSERT INTO UserRole (RoleName) VALUES ('Member'), ('Admin');";
                    using (var insertCommand = new SQLiteCommand(insertSql, _connection))
                    {
                        insertCommand.ExecuteNonQuery();
                    }
                }
            }
        }
        private void FillGameTypesIfEmpty()
        {
            var countSql = @"SELECT COUNT(*) FROM GameType;";
            using (var command = new SQLiteCommand(countSql, _connection))
            {
                var count = (long)command.ExecuteScalar();
                if (count == 0)
                {
                    var insertSql = @"INSERT INTO GameType (TypeName) VALUES ('Simple'), ('Advanced');";
                    using (var insertCommand = new SQLiteCommand(insertSql, _connection))
                    {
                        insertCommand.ExecuteNonQuery();
                    }
                }
            }
        }
        private void FillGamesIfEmpty()
        {
            var countSql = @"SELECT COUNT(*) FROM Game;";
            using (var command = new SQLiteCommand(countSql, _connection))
            {
                var count = (long)command.ExecuteScalar();
                if (count == 0)
                {
                    var simpleTypeId = GetGameTypeId("Simple");
                    var advancedTypeId = GetGameTypeId("Advanced");

                    var insertSql = @"INSERT INTO Game (GameName, Description, GameTypeId) VALUES ('MemoryGame', 'This is a simple memory game', @SimpleTypeId), 
                                    ('ColorGame', 'This is an advanced memory game', @AdvancedTypeId), 
                                    ('SizeGame', 'This is an advanced memory game', @AdvancedTypeId), 
                                    ('ColorSizeGame', 'This is an advanced memory game', @AdvancedTypeId);";
                    using (var insertCommand = new SQLiteCommand(insertSql, _connection))
                    {
                        insertCommand.Parameters.AddWithValue("@SimpleTypeId", simpleTypeId);
                        insertCommand.Parameters.AddWithValue("@AdvancedTypeId", advancedTypeId);
                        insertCommand.ExecuteNonQuery();
                    }
                }
            }
        }
        private void FillGendersIfEmpty()
        {
            var countSql = @"SELECT COUNT(*) FROM Gender;";
            using (var command = new SQLiteCommand(countSql, _connection))
            {
                var count = (long)command.ExecuteScalar();
                if (count == 0)
                {
                    var insertSql = @"INSERT INTO Gender (Name) VALUES ('Male'), ('Female'), ('Other');";
                    using (var insertCommand = new SQLiteCommand(insertSql, _connection))
                    {
                        insertCommand.ExecuteNonQuery();
                    }
                }
            }
        }
        private void FillEducationLevelsIfEmpty()
        {
            var countSql = @"SELECT COUNT(*) FROM EducationLevel;";
            using (var command = new SQLiteCommand(countSql, _connection))
            {
                var count = (long)command.ExecuteScalar();
                if (count == 0)
                {
                    var insertSql = @"INSERT INTO EducationLevel (Name) VALUES ('Primary'), ('Secondary'), ('Bachelor'), ('Master'), ('PhD');";
                    using (var insertCommand = new SQLiteCommand(insertSql, _connection))
                    {
                        insertCommand.ExecuteNonQuery();
                    }
                }
            }
        }
        private void FillEducationTypesIfEmpty()
        {
            var countSql = @"SELECT COUNT(*) FROM EducationType;";
            using (var command = new SQLiteCommand(countSql, _connection))
            {
                var count = (long)command.ExecuteScalar();
                if (count == 0)
                {
                    var insertSql = @"INSERT INTO EducationType (Name) VALUES ('Technical'), ('Humanitarian');";
                    using (var insertCommand = new SQLiteCommand(insertSql, _connection))
                    {
                        insertCommand.ExecuteNonQuery();
                    }
                }
            }
        }
        private int GetGameTypeId(string typeName)
        {
            var selectSql = @"SELECT ID FROM GameType WHERE TypeName = @TypeName;";
            using (var command = new SQLiteCommand(selectSql, _connection))
            {
                command.Parameters.AddWithValue("@TypeName", typeName);
                return (int)(long)command.ExecuteScalar();
            }
        }
        public void AddResult(Result result)
        {
            var insertResultSql = @"INSERT INTO Result (UserID, GameID, Result, Data, ResultDateTime) VALUES (@UserID, @GameID, @Result, @Data, @ResultDateTime);";

            using (var command = new SQLiteCommand(insertResultSql, _connection))
            {
                command.Parameters.AddWithValue("@UserID", result.UserID);
                command.Parameters.AddWithValue("@GameID", result.GameID);
                command.Parameters.AddWithValue("@Result", result.ResultScore);
                command.Parameters.AddWithValue("@Data", result.Data);
                command.Parameters.AddWithValue("@ResultDateTime", result.ResultDateTime);

                command.ExecuteNonQuery();
            }
        }

        public List<Result> GetResultsByUserId(long userId)
        {
            List<Result> results = new List<Result>();
            var selectResultsSql = @"SELECT * FROM Result WHERE UserID = @UserID;";

            using (var command = new SQLiteCommand(selectResultsSql, _connection))
            {
                command.Parameters.AddWithValue("@UserID", userId);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Result result = new Result
                        {
                            ID = (long)reader["ID"],
                            UserID = (long)reader["UserID"],
                            GameID = (long)reader["GameID"],
                            ResultScore = (int)(long)reader["Result"],
                            Data = reader["Data"].ToString(),
                            ResultDateTime = DateTime.Parse(reader["ResultDateTime"].ToString())
                        };
                        results.Add(result);
                    }
                }
            }
            return results;
        }

        public int GetUserRank(long userId)
        {
            List<User> users = GetUsers();
            users.Sort((x, y) => y.TotalScore.CompareTo(x.TotalScore));
            int index = users.FindIndex(user => user.Id == userId);
            return index + 1;
        }
        public void AddAdminIfNotExists()
        {
            var adminUser = FindUserByEmail("admin@memorygame.com");
            if (adminUser == null)
            {
                var admin = new User
                {
                    Name = "Admin",
                    Email = "admin@memorygame.com",
                    Password = "5e884898da28047151d0e56f8dc6292773603d0d6aabbdd62a11ef721d1542d8",
                    Age = 30,
                    RoleID = 2,
                    GenderID = 1,
                    EducationLevelID = 1,
                    EducationTypeID = 1,
                    SizeScore = 0,
                    ColorScore = 0,
                    OneColorScore = 0,
                    ColorSizeScore = 0,
                };
                AddUser(admin);
            }
        }
        public List<Game> GetGames()
        {
            var selectSql = @"SELECT * FROM Game";
            var games = new List<Game>();

            using (var command = new SQLiteCommand(selectSql, _connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var game = new Game
                        {
                            ID = Convert.ToInt64(reader["ID"]),
                            GameName = reader["GameName"].ToString(),
                            Description = reader["Description"].ToString(),
                            GameTypeID = Convert.ToInt64(reader["GameTypeID"])
                        };
                        games.Add(game);
                    }
                }
            }
            return games;
        }

        public List<Result> GetResultsByGameId(long gameId)
        {
            var selectSql = @"SELECT * FROM Result WHERE GameID = @GameID";
            var results = new List<Result>();

            using (var command = new SQLiteCommand(selectSql, _connection))
            {
                command.Parameters.AddWithValue("@GameID", gameId);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var result = new Result
                        {
                            ID = Convert.ToInt64(reader["ID"]),
                            UserID = Convert.ToInt64(reader["UserID"]),
                            GameID = Convert.ToInt64(reader["GameID"]),
                            ResultScore = Convert.ToInt32(reader["Result"]),
                            Data = reader["Data"].ToString(),
                            ResultDateTime = DateTime.Parse(reader["ResultDateTime"].ToString())
                        };
                        results.Add(result);
                    }
                }
            }

            return results;
        }
        public Gender GetGenderById(long id)
        {
            var selectGenderSql = @"SELECT * FROM Gender WHERE Id = @Id;";
            using (var command = new SQLiteCommand(selectGenderSql, _connection))
            {
                command.Parameters.AddWithValue("@Id", id);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Gender
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Name = reader["Name"].ToString()
                        };
                    }
                }
            }
            return null;
        }

        public EducationType GetEducationTypeById(long id)
        {
            var selectEducationTypeSql = @"SELECT * FROM EducationType WHERE Id = @Id;";
            using (var command = new SQLiteCommand(selectEducationTypeSql, _connection))
            {
                command.Parameters.AddWithValue("@Id", id);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new EducationType
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Name = reader["Name"].ToString()
                        };
                    }
                }
            }
            return null;
        }

        public EducationLevel GetEducationLevelById(long id)
        {
            var selectEducationLevelSql = @"SELECT * FROM EducationLevel WHERE Id = @Id;";
            using (var command = new SQLiteCommand(selectEducationLevelSql, _connection))
            {
                command.Parameters.AddWithValue("@Id", id);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new EducationLevel
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Name = reader["Name"].ToString()
                        };
                    }
                }
            }
            return null;
        }
        public string GetGameNameById(long gameId)
        {
            string gameName = "";

            var selectGameSql = @"SELECT * FROM Game WHERE ID = @Id;";
            using (var command = new SQLiteCommand(selectGameSql, _connection))
            {
                command.Parameters.AddWithValue("@Id", gameId);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        gameName = reader["GameName"].ToString();
                    }
                }
            }
            return gameName;
        }



        public void Dispose()
        {
            _connection?.Dispose();
        }
    }
}
