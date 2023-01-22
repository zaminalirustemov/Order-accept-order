﻿namespace Pustok_book_sales_app.Models
{
    public class OrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int BookId { get; set; }

        public string BookName { get; set; }
        public double CostPrice { get; set; }
        public double SalePrice { get; set; }
        public double DiscountPrice { get; set; }
        public int Count { get; set; }



        public Order Order { get; set; }
        public Book Book { get; set; }
    }
}
