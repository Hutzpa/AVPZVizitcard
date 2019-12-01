﻿using AVPZCard.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AVPZCard.ViewModels
{
    public class PostViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Body { get; set; } = "";
        public string CurrentImage { get; set; } = "";
        public decimal Price { get; set; }
        public bool IsAble { get; set; }


        public string Description { get; set; } = "";
        public string Tags { get; set; } = "";
        public Category Category { get; set; }
        public IFormFile Image { get; set; } = null;
    }
}
