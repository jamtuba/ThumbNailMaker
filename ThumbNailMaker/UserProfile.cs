using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace ThumbNailMaker
{
    class UserProfile : TableEntity
    {
        public UserProfile(string firstName, string lastName)
        {
            PartitionKey = "p1";
            RowKey = Guid.NewGuid().ToString();
            FirstName = firstName;
            LastName = lastName;
        }
        public UserProfile() { }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
