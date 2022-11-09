using LinqToDB.Mapping;

namespace MyLab.Notifier.Dal
{

    [Table("topic_bindings")]
    public class TopicBindingDb
    {
        [Column("subject_id")] public string? SubjectId { get; set; }
        [Column("topic")] public string? Topic { get; set; }
    }
}