using System.ComponentModel.DataAnnotations.Schema;

namespace TruthNews.Common.Models;

/// <summary>
/// Based on
/// 
/// Table users {
///     id int [pk, increment]
///     name varchar(100)
///     email varchar(255) [unique]
///     created_at datetime
/// }
/// </summary>

[Table("Users")]
public class User
{
    [Column("Id")]
    public required int Id { get; set; }
    [Column("Name")]
    public required string Name { get; set; }
    [Column("Email")]
    public required string Email { get; set; }
    [Column("CreatedAt")]
    public DateTime CreatedAt { get; set; }
}