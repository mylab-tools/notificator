using System.Collections;
using System.Collections.Generic;
using LinqToDB.Mapping;

namespace MyLab.Notifier.Share.Dal
{
    /// <summary>
    /// DB model fro topic to subject binding
    /// </summary>
    [Table("topic_bindings")]
    public class TopicBindingDb
    {
        /// <summary>
        /// Subject identifier
        /// </summary>
        [Column("subject_id", IsPrimaryKey = true)] 
        public string? SubjectId { get; set; }

        /// <summary>
        /// Topic identifier
        /// </summary>
        [Column("topic_id", IsPrimaryKey = true)] 
        public string? TopicId { get; set; }

        /// <summary>
        /// Associated contacts
        /// </summary>
        [Association(ThisKey = nameof(SubjectId), OtherKey = nameof(ContactDb.SubjectId))]
        public IEnumerable<ContactDb> Contacts { get; set; }
    }
}