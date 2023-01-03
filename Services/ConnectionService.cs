using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Linq;

namespace NotesApp1.Services
{
    public class ConnectionService
    {
        private string connectionString = "Server=localhost;Port=3306;Database=NotesApp1;Uid=root;Pwd=my_database2;";
        private MySqlConnection connection;

        public MySqlConnection GetMySqlConnection()
        {
            if (connection == null)
            {
                connection = new MySqlConnection(connectionString);
                connection.Open();
            }
            if (!(CheckTableExists("NoteUsers") && CheckTableExists("NoteItems")))
            {
                DropAllTables();
                CreateTables();
            }
            return connection;
        }

        private bool CheckTableExists(string tableName)
        {
            string showTablesString = "SHOW TABLES;";
            MySqlCommand mySqlCommand = new MySqlCommand(showTablesString, connection);
            MySqlDataReader mySqlDataReader = mySqlCommand.ExecuteReader();
            while (mySqlDataReader.Read())
            {
                string name = mySqlDataReader.GetString(0);
                if (name == tableName)
                {
                    return true;
                }
            }
            return false;
        }

        private void DropAllTables()
        {
            string showTablesString = "SHOW TABLES;";
            MySqlCommand mySqlCommand = new MySqlCommand(showTablesString, connection);
            MySqlDataReader mySqlDataReader = mySqlCommand.ExecuteReader();
            while (mySqlDataReader.Read())
            {
                string tableName = mySqlDataReader.GetString(0);
                string dropTableString = $"DROP TABLE {tableName}";
                MySqlCommand mySqlCommand1 = new MySqlCommand(dropTableString, connection);
                mySqlCommand1.ExecuteNonQuery();
            }
        }

        private void CreateTables()
        {
            List<Table> tables = new List<Table>
            {
                new Table
                {
                    Name = "NoteUsers",
                    Columns = new List<Column>
                    {
                        new Column
                        {
                            Name = "Id",
                            Type = MySqlDbType.Guid,
                            IsNullable = false,
                            IsPrimaryKey = true
                        },
                        new Column
                        {
                            Name = "UserName",
                            Type = MySqlDbType.String,
                            IsNullable = false,
                            IsUnique = true
                        },
                        new Column
                        {
                            Name = "Email",
                            Type = MySqlDbType.String,
                            IsNullable = false,
                            IsUnique = true
                        },
                        new Column
                        {
                            Name = "Password",
                            Type = MySqlDbType.String,
                            IsNullable = false
                        },
                        new Column
                        {
                            Name = "CreatedAt",
                            Type = MySqlDbType.DateTime,
                            IsNullable = false
                        },
                        new Column
                        {
                            Name = "UpdatedAt",
                            Type = MySqlDbType.DateTime,
                            IsNullable = false
                        }
                    }
                },
                new Table
                {
                    Name = "NoteItems",
                    Columns = new List<Column>
                    {
                        new Column
                        {
                            Name = "Id",
                            Type = MySqlDbType.Int32,
                            IsNullable = false,
                            IsPrimaryKey = true
                        },
                        new Column
                        {
                            Name = "NoteUserId",
                            Type = MySqlDbType.Guid,
                            IsNullable = false
                        },
                        new Column
                        {
                            Name = "Title",
                            Type = MySqlDbType.String,
                            IsNullable = false
                        },
                        new Column
                        {
                            Name = "Image",
                            Type = MySqlDbType.String,
                            IsNullable = true
                        },
                        new Column
                        {
                            Name = "Description",
                            Type = MySqlDbType.String,
                            IsNullable = false
                        },
                        new Column
                        {
                            Name = "CreatedAt",
                            Type = MySqlDbType.DateTime,
                            IsNullable = false
                        },
                        new Column
                        {
                            Name = "UpdatedAt",
                            Type = MySqlDbType.DateTime,
                            IsNullable = false
                        },
                    },
                    ForeignKeys = new List<ForeignKey>
                    {
                        new ForeignKey
                        {
                            ColumnName = "NoteUserId",
                            ReferencedTableName = "NoteUsers",
                            ReferencedColumnName = "Id"
                        }
                    }
                }
            };
            foreach (Table table in tables)
            {
                string createTableSql = GetCreateTableSql(table);
                MySqlCommand mySqlCommand = new MySqlCommand(createTableSql, connection);
                mySqlCommand.ExecuteNonQuery();
            }
        }
        
        private string GetCreateTableSql(Table table)
        {
            string columnsSql = string.Join(", ", table.Columns.Select(x => GetColumnSql(x));
            string primaryKeySql = table.Columns.FirstOrDefault(x => x.IsPrimaryKey)?.Name;
            string foreignKeySql = string.Join(", ", table.ForeignKeys.Select(x => GetForeignKeySql(x));
            return $"CREATE TABLE {table.Name} ({columnsSql}, " +
                $"{(string.IsNullOrWhiteSpace(primaryKeySql) ? string.Empty : $"PRIMARY KEY ({primaryKeySql}), ")}" +
                $"{foreignKeySql});";
        }

        private string GetColumnSql(Column column)
        {
            return $"{column.Name} {GetMySqlDataType(column.Type)} " +
                $"{(column.IsNullable ? "NULL" : "NOT NULL")} " +
                $"{(column.IsUnique ? "UNIQUE" : string.Empty)}" +
                $"{(column.IsAutoIncrement ? " AUTO_INCREMENT" : string.Empty)}";
        }

        private string GetForeignKeySql(ForeignKey foreignKey)
        {
            return $"FOREIGN KEY ({foreignKey.ColumnName}) REFERENCES " +
                $"{foreignKey.ReferencedTableName}({foreignKey.ReferencedColumnName})";
        }

        private string GetMySqlDataType(MySqlDbType mySqlDbType)
        {
            switch (mySqlDbType)
            {
                case MySqlDbType.Guid:
                    return "CHAR(36)";
                case MySqlDbType.DateTime:
                    return "DATETIME";
                case MySqlDbType.String:
                    return "TEXT";
                case MySqlDbType.Int32:
                    return "INT";
                default:
                    throw new ArgumentException("Invalid data type.");
            }
        }
    }
}
