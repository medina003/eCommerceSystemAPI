﻿namespace eCommerceRESTfulAPI.Domain.Entities
{
    public class Customer
    {
        public int Id { get; set; } 
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        public ICollection<Order>? Orders { get; set; } 
    }
}
