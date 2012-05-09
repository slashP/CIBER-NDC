using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using CiberNdc.Models;

namespace CiberNdc.Context
{
    public class DataContext : DbContext 
    {
        public DbSet<Photo> Photos { get; set; } 
    }
}