using LinqToDB.Mapping;

namespace MyLab.Notifier.Dal
{

    [Table("topic_bindings")]
    public class TopicBindingDb
    {
        [Column("subject_id", IsPrimaryKey = true)] public string? SubjectId { get; set; }
        [Column("topic_id", IsPrimaryKey = true)] public string? TopicId { get; set; }
    }
}