using MySql.Data.MySqlClient;
using NotesApp1.Models;

namespace NotesApp1.Services
{
    public class NoteItemDataService
    {
        private readonly ConnectionService connectionService;
        private readonly MySqlConnection mySqlConnection;

        public NoteItemDataService()
        {
            this.connectionService = new ConnectionService();
            this.mySqlConnection = connectionService.GetMySqlConnection();
        }

        public IList<NoteItem> GetNoteItemsForUser(NoteUser noteUser)
        {
            string selectString = "SELECT FROM NoteItems WHERE NoteUserId = '@NoteUserId';";
            MySqlCommand mySqlCommand = new MySqlCommand(selectString, mySqlConnection);
            mySqlCommand.Parameters.AddWithValue("@NoteUserId", noteUser.Id);
            List<NoteItem> noteItems = new List<NoteItem>();
            MySqlDataReader mySqlDataReader = mySqlCommand.ExecuteReader();
            while (mySqlDataReader.Read())
            {
                noteItems.Add(new NoteItem()
                {
                    Id = mySqlDataReader.GetInt32(0),
                    NoteUser = noteUser,
                    Title = mySqlDataReader.GetString(2),
                    Image = mySqlDataReader.GetString(3),
                    Description = mySqlDataReader.GetString(4),
                    CreatedAt = mySqlDataReader.GetDateTime(5),
                    UpdatedAt = mySqlDataReader.GetDateTime(6)
                });
            }
            return noteItems;
        }

        public NoteItem GetNoteItemDetails(int id)
        {
            string selectString = "SELECT FROM NoteItems WHERE Id = '@Id';";
            MySqlCommand mySqlCommand = new MySqlCommand(selectString, mySqlConnection);
            mySqlCommand.Parameters.AddWithValue("@Id", id);
            NoteItem noteItem = new NoteItem();
            MySqlDataReader mySqlDataReader = mySqlCommand.ExecuteReader();
            if (mySqlDataReader.Read())
            {
                noteItem.Id = mySqlDataReader.GetInt32("Id");
                noteItem.NoteUser.Id = mySqlDataReader.GetGuid("UserId");
                noteItem.Title = mySqlDataReader.GetString("Title");
                noteItem.Image = mySqlDataReader.GetString("Image");
                noteItem.Description = mySqlDataReader.GetString("Description");
                noteItem.CreatedAt = mySqlDataReader.GetDateTime("CreatedAt");
                noteItem.UpdatedAt = mySqlDataReader.GetDateTime("UpdatedAt");
            }
            return noteItem;
        }

        public void InsertNoteItem(NoteItem noteItem)
        {
            string insertString = "INSERT INTO NoteItems (Id, NoteUserId, Title, Image, Description, CreatedAt, UpdatedAt)"
                    + " VALUES (@Id, '@NoteUserId', '@Title', '@Image', '@Description', @CreatedAt, @UpdatedAt);";
            MySqlCommand mySqlCommand = new MySqlCommand(insertString, mySqlConnection);
            mySqlCommand.Parameters.AddWithValue("@Id", noteItem.Id);
            mySqlCommand.Parameters.AddWithValue("@NoteUserId", noteItem.NoteUser.Id);
            mySqlCommand.Parameters.AddWithValue("@Title", noteItem.Title);
            mySqlCommand.Parameters.AddWithValue("@Image", noteItem.Image);
            mySqlCommand.Parameters.AddWithValue("@Description", noteItem.Description);
            mySqlCommand.Parameters.AddWithValue("@CreatedAt", noteItem.CreatedAt);
            mySqlCommand.Parameters.AddWithValue("@UpdatedAt", noteItem.UpdatedAt);
            mySqlCommand.ExecuteNonQuery();
        }

        public void UpdateNoteItem(NoteItem noteItem)
        {
            string updateString = "Update NoteItems SET Title = '@Title', Image = '@Image', Description = '@Description', UpdatedAt = @UpdatedAt" +
                " WHERE Id = @Id;";
            MySqlCommand mySqlCommand = new MySqlCommand(updateString, mySqlConnection);
            mySqlCommand.Parameters.AddWithValue("@Id", noteItem.Id);
            mySqlCommand.Parameters.AddWithValue("@Title", noteItem.Title);
            mySqlCommand.Parameters.AddWithValue("@Image", noteItem.Image);
            mySqlCommand.Parameters.AddWithValue("@Description", noteItem.Description);
            mySqlCommand.Parameters.AddWithValue("@UpdatedAt", noteItem.UpdatedAt);
            mySqlCommand.ExecuteNonQuery();
        }

        public void DeleteNoteItem(NoteItem noteItem)
        {
            string deleteString = "DELETE FROM NoteItems WHERE Id = @Id;";
            MySqlCommand mySqlCommand = new MySqlCommand(deleteString, mySqlConnection);
            mySqlCommand.Parameters.AddWithValue("@Id", noteItem.Id);
            mySqlCommand.ExecuteNonQuery();
        }

        public void DeleteNoteItemsForUser(NoteUser noteUser)
        {
            string deleteString = "DELETE FROM NoteItems WHERE NoteUserId = @NoteUserId;";
            MySqlCommand mySqlCommand = new MySqlCommand(deleteString, mySqlConnection);
            mySqlCommand.Parameters.AddWithValue("@NoteUserId", noteUser.Id);
            mySqlCommand.ExecuteNonQuery();
        }
    }
}
