﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ToDoAPI.API.Models
{
    public class ToDoViewModel
    {
        public int Todoid { get; set; }
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Priority { get; set; }

        

        public virtual CategoryViewModel Category { get; set; }
    }

    public class CategoryViewModel
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
    }
}