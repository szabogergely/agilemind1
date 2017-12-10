using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace PicBook.Domain
{
    public class Tag : Entity
    {
        public string ImageIdentifier { get; set; }
        public string TagName { get; set; }
    }
}
