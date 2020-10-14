using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace ThumbNailMaker
{
    class UserProfile : TableEntity
    {
        public UserProfile(string firstName, string lastName, string profilePicUrl, string email)
        {
            PartitionKey = "p1";
            RowKey = Guid.NewGuid().ToString();
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            ProfilePicUrl = profilePicUrl;
        }
        public UserProfile() { }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string ProfilePicUrl { get; set; }
    }
}
