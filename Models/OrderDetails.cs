using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

public class OrderDetails {
    [Key]
    public int Id{get;set;}
    [Required]
    public int OrderHeaderId {get;set;}
    [ForeignKey("OrderHeaderId")]
    [ValidateNever]
    public OrderHeader OrderHeaders{get;set;}

    [Required]
    public int ProductId { get; set; }
    [ForeignKey("ProductId")]
    [ValidateNever]
    public Product Product { get; set; }

    public int Count { get; set; }
    public double Price { get; set; }

}