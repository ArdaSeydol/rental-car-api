using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ardaRetake.Models.DTOs;

public class ClientRequestDto
{
    [Required]
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    [JsonPropertyName("firstName")]
    public string FirstName { get; set; }

    [Required]
    [StringLength(100)]
    [JsonPropertyName("lastName")]
    public string LastName { get; set; }

    [Required]
    [StringLength(100)]
    [JsonPropertyName("address")]
    public string Adress { get; set; }

    [Required]
    [MinLength(0)]
    [JsonPropertyName("rentals")]
    public List<Rental> rentals { get; set; }
}

public class Rental
{
    [Required]
    [StringLength(17)]
    [JsonPropertyName("vin")]
    public string Vin { get; set; }

    [Required]
    [StringLength(100)]
    [JsonPropertyName("color")]
    public string Color { get; set; }

    [Required]
    [StringLength(100)]
    [JsonPropertyName("model")]
    public string Model { get; set; }

    [Required]
    [DataType(DataType.Date)]
    [JsonPropertyName("dateFrom")]
    public DateTime DateFrom { get; set; }

    [Required]
    [DataType(DataType.Date)]
    [JsonPropertyName("dateTo")]
    public DateTime DateTo { get; set; }

    [Required]
    [Range(typeof(decimal), "0.01", "99999999.99")]
    [JsonPropertyName("totalPrice")]
    public decimal TotalPrice { get; set; }
}