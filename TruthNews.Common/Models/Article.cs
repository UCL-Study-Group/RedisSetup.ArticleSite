using System.ComponentModel.DataAnnotations.Schema;

namespace TruthNews.Common.Models;

[Table("Articles")]
public class Article
{
    [Column("Id")]
    public required int Id { get; set; }
    [Column("Title")]
    public required string Title { get; set; }
    [Column("Content")]
    public required string Content { get; set; }
    [Column("AuthorId")]
    public required int AuthorId { get; set; }
    [Column("CreatedAt")]
    public DateTime CreatedAt { get; set; }
}