using Microsoft.Data.SqlClient;
using Roommates.Models;
using System;
using System.Collections.Generic;

namespace Roommates.Repositories
{
    public class ChoreRepository : BaseRepository
    {
        //When a new ChoreRepository is initialized, pass the
        //  connection string along to the BaseRepository
        public ChoreRepository(string connectionString) : base(connectionString) { }

        //Get a list of all Chores in the database
        public List<Chore> GetAll()
        {
            //"use" the connection to the database.
            //"Open() and Close() the connection.
            //Being in a "using" block makes sure we disconnect even if we get an error.
            using (SqlConnection conn = Connection)
            {
                //Open the connection
                conn.Open();

                //"use" commands
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    //Setup the command with a string of SQL we want to run.
                    cmd.CommandText = "SELECT Id, Name FROM Chore";

                    //Initialize a reader that accesses the results of running the
                    // command we created above.
                    SqlDataReader reader = cmd.ExecuteReader();

                    //A list to store Chores we get from the database
                    List<Chore> chores = new List<Chore>();

                    //While there is more data to read, this loop will continue to run.
                    while (reader.Read())
                    {
                        //"Ordinal" is the numeric position of the column.
                        //"Ordinal" is similar to "Index"
                        int idColumnPosition = reader.GetOrdinal("Id");

                        //The reader has Get___ methods to get the value of the item
                        //  in the position determined by the Ordinal above
                        int idValue = reader.GetInt32(idColumnPosition);

                        int nameColumnPosition = reader.GetOrdinal("Name");
                        string nameValue = reader.GetString(nameColumnPosition);

                        //Create a new chore object with the data we collected
                        Chore chore = new Chore
                        {
                            Id = idValue,
                            Name = nameValue
                        };

                        //Add the chore object to the list
                        chores.Add(chore);
                    }

                    //Close the reader
                    reader.Close();

                    //then return the list we created
                    return chores;
                }
            }
        }

        //Returns a single chore with the given id
        public Chore GetById(int id)
        {
            //Use the connection
            using (SqlConnection conn = Connection)
            {
                //Open the connection
                conn.Open();

                //Use Commands
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    //Declare the SQL command as a string
                    // "@id" is used as a placeholder for the Id passed into the method
                    cmd.CommandText = "SELECT Name FROM Chore WHERE Id = @id";

                    //This line replaces the "@id" with the actual Id value passed in
                    cmd.Parameters.AddWithValue("@id", id);

                    //Initialize the reader, which accesses the results of the SQL query.
                    //Also runs the SQL command we declared above
                    SqlDataReader reader = cmd.ExecuteReader();

                    //Declares a chore as null in case the Id input doesn't return a chore
                    Chore chore = null;

                    //If the reader does have a result to read, this section runs
                    if (reader.Read())
                    {
                        //Change the "null" chore declared above to a new Chore object
                        chore = new Chore
                        {
                            //Pass in the id that was given as a parameter
                            Id = id,
                            //Get the string of item at the ordinal/index position where "Name" is
                            Name = reader.GetString(reader.GetOrdinal("Name"))
                        };
                    }

                    //Close the reader
                    reader.Close();

                    //then return the chore, either "null" or what matches the Id input
                    return chore;
                }
            }
        }

        //Add a chore to the database
        //We aren't getting anything from the database, so a return isn't needed
        public void Insert(Chore chore)
        {
            //Use the connection
            using (SqlConnection conn = Connection)
            {
                //Open the connection
                conn.Open();

                //Use commands
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    //Set the SQL command text
                    // output the inserted's ID because the database assigns that value
                    cmd.CommandText = @"INSERT INTO Chore (Name)
                                        OUTPUT INSERTED.Id
                                        VALUES (@name)";

                    //Replace the placeholder "@name" with the Name of the chore
                    cmd.Parameters.AddWithValue("@name", chore.Name);

                    //Executes the command text and returns the value in Row 1, Column 1
                    //In this case, the ID of what was inserted
                    int id = (int)cmd.ExecuteScalar();

                    //Set the Chore object's ID to the number that the database assigned it
                    chore.Id = id;
                }
            }
        }
    }
}
