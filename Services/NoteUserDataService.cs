using MySql.Data.MySqlClient;
using NotesApp1.Models;

namespace NotesApp1.Services
{
    public class NoteUserDataService
    {
        private readonly ConnectionService connectionService;
        private readonly MySqlConnection mySqlConnection;

        public NoteUserDataService()
        {
            this.connectionService = new ConnectionService();
            this.mySqlConnection = connectionService.GetMySqlConnection();
        }

        public IList<NoteUser> GetNoteUsers()
        {
            string selectString = "SELECT * FROM NoteUsers;";
            MySqlCommand mySqlCommand = new MySqlCommand(selectString, mySqlConnection);
            List<NoteUser> noteUsers = new List<NoteUser>();
            MySqlDataReader mySqlDataReader = mySqlCommand.ExecuteReader();
            while (mySqlDataReader.Read())
            {
                noteUsers.Add(new NoteUser()
                {
                    Id = mySqlDataReader.GetGuid(0),
                    UserName = mySqlDataReader.GetString(1),
                    Email = mySqlDataReader.GetString(2),
                    Password = mySqlDataReader.GetString(3),
                    CreatedAt = mySqlDataReader.GetDateTime(4),
                    UpdatedAt = mySqlDataReader.GetDateTime(5)
                });
            }
            return noteUsers;
        }

        public void InsertNoteUser(NoteUser noteUser)
        {
            string insertString = "INSERT INTO NoteUsers (Id, UserName, Email, Password, CreatedAt, UpdatedAt)" +
                " VALUES ('@Id', '@UserName', '@Email', '@Password', @CreatedAt, @UpdatedAt);";
            MySqlCommand mySqlCommand = new MySqlCommand(insertString, mySqlConnection);
            mySqlCommand.Parameters.AddWithValue("@Id", noteUser.Id);
            mySqlCommand.Parameters.AddWithValue("@UserName", noteUser.UserName);
            mySqlCommand.Parameters.AddWithValue("@Email", noteUser.Email);
            mySqlCommand.Parameters.AddWithValue("@Password", noteUser.Password);
            mySqlCommand.Parameters.AddWithValue("@CreatedAt", noteUser.CreatedAt);
            mySqlCommand.Parameters.AddWithValue("@UpdatedAt", noteUser.UpdatedAt);
            mySqlCommand.ExecuteNonQuery();
        }

        public void UpdateNoteUser(NoteUser noteUser)
        {
            string updateString = "UPDATE NoteUsers SET UserName = '@UserName', Email = 'Email', UpdatedAt = '@UpdatedAt'" +
                " WHERE Id = '@Id';";
            MySqlCommand mySqlCommand = new MySqlCommand(updateString, mySqlConnection);
            mySqlCommand.Parameters.AddWithValue("@UserName", noteUser.UserName);
            mySqlCommand.Parameters.AddWithValue("@Email", noteUser.Email);
            mySqlCommand.Parameters.AddWithValue("@UpdatedAt", noteUser.UpdatedAt);
            mySqlCommand.ExecuteNonQuery();
        }

        public void DeleteNoteUser(NoteUser noteUser)
        {
            string deleteString = "DELETE FROM NoteUsers WHERE Id = '@Id';";
            MySqlCommand mySqlCommand = new MySqlCommand(deleteString, mySqlConnection);
            mySqlCommand.Parameters.AddWithValue("@Id", noteUser.Id);
            mySqlCommand.ExecuteNonQuery();
        }
    }
}
