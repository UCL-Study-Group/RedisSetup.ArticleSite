using System.ComponentModel.DataAnnotations.Schema;

namespace TruthNews.Common.Models;

[Table("Comments")]
public class Comment
{
    [Column("Id")]
    public required int Id { get; set; }
    [Column("ArticleId")]
    public required int ArticleId { get; set; }
    [Column("UserId")]
    public required int UserId { get; set; }
    [Column("Content")]
    public required string Content { get; set; }
    [Column("CreatedAt")]
    public required DateTime CreatedAt { get; set; }
}