using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Providers.Entities;

namespace CiberNdc.Models
{
    public class Photo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public byte[] ImageStream { get; set; }
        public string Filename { get; set; }
        public string Format { get; set; }
    }

    public class PhotoDBContext : DbContext
    {
        public DbSet<Photo> Photos { get; set; }
    }
    
}