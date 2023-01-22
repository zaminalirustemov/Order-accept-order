﻿namespace Pustok_book_sales_app.Models
{
    public class BasketItem
    {
        public int Id { get; set; }
        public string AppUserId { get; set; }
        public int BookId { get; set; }
        public int Count { get; set; }
        public  bool IsDeleted { get; set; }

        public AppUser? AppUser { get; set; }
        public Book? Book { get; set; }
    }
}
