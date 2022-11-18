using LinqToDB.Mapping;

namespace MyLab.Notifier.Dal
{
    [Table("labels")]
    public class ContactLabelDb
    {
        [Column("contact_id", IsPrimaryKey = true)]
        public int ContactId { get; set; }

        [Column("name", IsPrimaryKey = true)] public string? Name { get; set; }
        [Column("value")] public string? Value { get; set; }
    }
}