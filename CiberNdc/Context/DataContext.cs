using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using CiberNdc.Models;
using CodeFirstMembershipSharp;

namespace CiberNdc.Context
{
    public class DataContext : DbContext 
    {
        public DbSet<Photo> Photos { get; set; }
        public DbSet<Employee> Employees { get; set; }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
    }

}