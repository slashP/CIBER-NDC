using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace CiberNdc.Models
{
    public class Photo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public byte[] ImageStream { get; set; }
        public string Filename { get; set; }
        public string Format { get; set; }
        public int? EmployeeId { get; set; }
        public virtual Employee Employee { get; set; }

        public string UploadedBy { get; set; }

        private static readonly Random Random = new Random();

        public string RandomBackgroundColor()
        {
            var colors = new[] {"red", "mango", "blue", "purple", "green"};
            return colors[Random.Next(0, colors.Length)];
        }
    }

    public class Employee
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public string Codeword { get; set; }
        public virtual Collection<Photo> Photos { get; set; }
    }

    public class Message
    {
        public int Id { get; set; }
        public string Title { get; set; }
        [Column(TypeName = "ntext")]
        [AllowHtml]
        public string Text { get; set; }
    }
    
}