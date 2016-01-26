using System;
using TechTalk.SpecFlow;

namespace Core.Instrumentation.Tests.Steps
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Core.Instrumentation.Tests.Models;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Core.Instrumentation.Tracking;
    using PostSharp.Extensibility;

    [Binding]
    public class TrackMethodCallSteps
    {
        [Given(@"a student grades")]
        public void GivenAStudentGrades(Table table)
        {
            var students = table.ToModelList<StudentGrade>();
            Assert.IsNotNull(students);
            Assert.IsTrue(students.Count>0);
            Assert.IsTrue(students.Count(s=>s.Subject.EqualsIgnoreCase("Math"))>0);
            Assert.IsTrue(students.Count(s => s.Subject.EqualsIgnoreCase("Literature")) > 0);
            ScenarioContext.Current.Set(students);
        }

        [When(@"I caculate grade average by subject ""(.*)""")]
        public void WhenICaculateGradeAverageBySubject(string subject)
        {
            var students = ScenarioContext.Current.Get<List<StudentGrade>>();
            var course = new Course(subject, students);
            var avg = course.GetAverage();
            ScenarioContext.Current.Set(avg,"Average");
        }
        
        [Then(@"^the average should be (\d+)$")]
        public void ThenTheResultShouldBe(int average)
        {
            var avg = ScenarioContext.Current.Get<int>("Average");
            Assert.AreEqual(average, avg);
        }
    }
}
