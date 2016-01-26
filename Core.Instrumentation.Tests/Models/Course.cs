using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Instrumentation.Tests.Models
{
    using Core.Instrumentation.Tracking;
    using PostSharp.Extensibility;

    [TraceMethodAspect(AttributeTargetMemberAttributes = MulticastAttributes.UserGenerated, AttributePriority = 0)]
    [TraceMethodAspect(AttributeExclude = true, AttributeTargetMembers = "regex:^get_|^set_", AttributePriority = 2)]
    public class Course
    {
        public string Name { get; set; }
        public Dictionary<string, int> Grades { get; set; }

        public Course(string courseName, List<StudentGrade> studentGrades)
        {
            this.Name = courseName;
            this.Grades = studentGrades.Where(s => s.Subject.EqualsIgnoreCase(courseName))
                .ToDictionary(s => s.Name, s => s.Grade);
        }

        public int? GetAverage()
        {
            int count = GetCount(this.Grades);
            int total = GetTotal(this.Grades);
            if (count > 0)
                return total / count;
            return null;
        }

        private int GetTotal(Dictionary<string, int> grades)
        {
            return grades.Where(g => g.Value >= 0 && g.Value <= 100).Sum(g => g.Value);
        }

        private int GetCount(Dictionary<string, int> grades)
        {
            return grades.Count(g => g.Value >= 0 && g.Value <= 100);
        }
    }
}
