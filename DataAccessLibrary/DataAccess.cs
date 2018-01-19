using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using System.Collections;

namespace DataAccessLibrary
{
    public class DataAccess
    {
        //Class to hold coin info used in List
        public class CoinInfoClass
        {
            public string theCoinsName;
            public double theCoinsAmount;
        }

        //Initilizes SQLite Database
        public static void InitializeDatabase()
        {
            using (SqliteConnection db =
            new SqliteConnection("Filename=CryptFolio_Database.db"))
            {
                db.Open();
    
                String tableCommand = "CREATE TABLE IF NOT " +
                    "EXISTS Portfolio (CoinName VARCHAR(50) PRIMARY KEY, " +
                    "AmountOwned DOUBLE(50,25) NOT NULL)";

                SqliteCommand createTable = new SqliteCommand(tableCommand, db);
                createTable.ExecuteReader();
                db.Close();
            }
        }

        //Adds entries into the database
        public static void AddDataEntries(string nameOfCoin, double amountOfCoin)
        {
            using (SqliteConnection db =
                new SqliteConnection("Filename=CryptFolio_Database.db"))
            {
                db.Open();

                //Check if already in the database. If it is already an entry update Amount Owned. Else add a new coin and amount owned 
                SqliteCommand selectCommand = new SqliteCommand ("SELECT CoinName from Portfolio", db);
                SqliteDataReader queryCoinName = selectCommand.ExecuteReader();
                bool alreadyInList = false;
                while (queryCoinName.Read())
                {
                    if (queryCoinName.GetString(0) == nameOfCoin)
                    {
                        alreadyInList = true;
                        break;
                    }
                        
                }

                if (alreadyInList == false)         //Not in list, add new entry
                {
                    SqliteCommand insertCommand = new SqliteCommand();
                    insertCommand.Connection = db;

                    // Use parameterized query to prevent SQL injection attacks
                    insertCommand.CommandText = "INSERT INTO Portfolio (CoinName, AmountOwned) VALUES (@Val1, @Val2);";
                    insertCommand.Parameters.AddWithValue("@Val1", nameOfCoin);
                    insertCommand.Parameters.AddWithValue("@Val2", amountOfCoin);
                    insertCommand.ExecuteNonQuery();
                }
                else                            //In list update Entry
                {
                    SqliteCommand updateCommand = new SqliteCommand();
                    updateCommand.Connection = db;
                    updateCommand.CommandText = "UPDATE Portfolio SET AmountOwned = @Val1 WHERE CoinName = @Val2;";
                    updateCommand.Parameters.AddWithValue("@Val1", amountOfCoin);
                    updateCommand.Parameters.AddWithValue("@Val2", nameOfCoin);
                    updateCommand.ExecuteNonQuery();
                }
                db.Close();
            }

        }

        //Returns all data within the database
        public static List<CoinInfoClass> GetDataEntries()
        {
            //List of classes that holds all coins and values
            List<CoinInfoClass> listOfCoins = new List<CoinInfoClass>();
           
            using (SqliteConnection db =
                new SqliteConnection("Filename=CryptFolio_Database.db"))
            {
                db.Open();
                //Gets all coin names
                SqliteCommand selectStringCommand = new SqliteCommand("SELECT CoinName FROM Portfolio", db);
                SqliteDataReader stringQuery = selectStringCommand.ExecuteReader();
                //Gets all coin amounts
                SqliteCommand selectNumCommand = new SqliteCommand("SELECT AmountOwned from Portfolio", db);
                SqliteDataReader numQuery = selectNumCommand.ExecuteReader();

                while (stringQuery.Read() && numQuery.Read())
                {
                    //Add coin name and coin amount to each item in List
                   listOfCoins.Add(new CoinInfoClass { theCoinsName = stringQuery.GetString(0), theCoinsAmount = numQuery.GetDouble(0)});
                }

                db.Close();
            }
            // return listOfCoins;
            return listOfCoins;
        }

        //Deletes entry of table based on name of coin
        public static void DeleteDataEntries(string coinToDelete)
        {
            using (SqliteConnection db =
               new SqliteConnection("Filename=CryptFolio_Database.db"))
            {
                db.Open();
                //SQL delete command
                SqliteCommand deleteCommand = new SqliteCommand();
                deleteCommand.Connection = db;
                // Use parameterized query to prevent SQL injection attacks
                deleteCommand.CommandText = "DELETE FROM Portfolio WHERE CoinName = @Val;";
                deleteCommand.Parameters.AddWithValue("@Val", coinToDelete);
                deleteCommand.ExecuteNonQuery();

                db.Close();
            }
               
        }
    }
}
