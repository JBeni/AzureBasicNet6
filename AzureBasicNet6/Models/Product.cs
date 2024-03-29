﻿using System.ComponentModel.DataAnnotations;

namespace AzureBasicNet6.Models
{
    public class Product
    {
        public Product()
        {
            CreatedDate = DateTime.Now;
        }

        public int ProductId { get; set; }

        [Required(ErrorMessage = "Please Enter Name")]
        public string Name { get; set; }

        public decimal UnitPrice { get; set; }

        [Required(ErrorMessage = "Please Enter Description")]
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
