using MediaRatingPlatform_DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaRatingPlatform_BusinessLogicLayer
{
    
    public class DatabaseService
    {
        private readonly DBConnection _dBConnection;
        
        public DatabaseService(string connString)
        {
            _dBConnection = new DBConnection(connString);
        }

        public async Task TestDatabaseAsync()
        {
            await _dBConnection.ConnectToDatabaseAsync();
        }


    }
}
