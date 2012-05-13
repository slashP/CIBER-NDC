﻿using System;

namespace CiberNdc.Models
{
    public class Photo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public byte[] ImageStream { get; set; }
        public string Filename { get; set; }
        public string Format { get; set; }

        public string RandomBackgroundColor()
        {
            var colors = new[] {"greenbg", "redbg", "bluebg", "brownbg"};
            return colors[DateTime.Now.Ticks%4];
        }
    }
    
}