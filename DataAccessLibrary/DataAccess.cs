using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

namespace DataAccessLibrary
{
    public class DataAccess
    {
        public static void InitializeDatabase()
        {
            using (SqliteConnection db =
                new SqliteConnection("Filename=CryptFolio_Database.db"))
            {
                db.Open();

                String tableCommand = "CREATE TABLE IF NOT " +
                    "EXISTS MyTable (Primary_Key INTEGER PRIMARY KEY AUTOINCREMENT, " +
                    "Text_Entry NVARCHAR(2048) NULL)";


/*CREATE TABLE [Portfolio] (
  [CoinName] TEXT NOT NULL
, [AmountOwned] REAL DEFAULT 0 NOT NULL
, CONSTRAINT [sqlite_autoindex_Portfolio_1] PRIMARY KEY ([CoinName])
, CONSTRAINT [PK_Portfolio] PRIMARY KEY ([CoinName])
);   
                 */

                SqliteCommand createTable = new SqliteCommand(tableCommand, db);

                createTable.ExecuteReader();
            }
        }
    }
}
