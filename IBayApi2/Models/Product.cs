﻿using System.ComponentModel.DataAnnotations.Schema;

namespace IBayApi2.Models;
[Table("Product")]

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Image { get; set; }
    public double Price { get; set; }
    public bool Available { get; set; }
    public DateTime AddedTime { get; set; }
    public int UserId { get; set; }
    [ForeignKey("UserId")] public Member SellerMember { get; set; }
}