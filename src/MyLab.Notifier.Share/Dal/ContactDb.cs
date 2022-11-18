using LinqToDB.Mapping;

namespace MyLab.Notifier.Share.Dal
{
    [Table("contacts")]
    public class ContactDb
    {
        [Column("id", IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
        [Column("value")]
        public string? Value { get; set; }
        [Column("channel_id")]
        public string? ChannelId { get; set; }
        [Column("subject_id")]
        public string? SubjectId { get; set; }

        [Association(ThisKey = nameof(Id), OtherKey = nameof(ContactLabelDb.ContactId))]
        public virtual ContactLabelDb[]? Labels { get; set; }
    }
}
