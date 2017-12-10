using System;
using System.Collections.Generic;
using System.Text;

namespace PicBook.ApplicationService
{
    public class TagConnection : ITagConnection
    {
        public string ConnectionString { get; set; }
        public TagConnection(string connectionstring)
        {
            this.ConnectionString = connectionstring;
        }
    }
}
