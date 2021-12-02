using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using ProgrammingTask1;
using System.IO;

namespace UnitTests
{
    [TestClass]
    public class CffFileCheck
    {
        [TestMethod]
        public void TestMethod1()
        {
            // Arrange.
            string path = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + "/PT1 - Test1.taff";
            TaskAllocations taskAllocations = new TaskAllocations(path);
            Configuration configuration = new Configuration();

            bool actualResult, value;
            bool expectedResult = true;

            // Act.
            value = taskAllocations.GetCffFilename();
            actualResult = configuration.Validate(taskAllocations.CffFilename);

            // Assert.
            Assert.AreEqual(expectedResult, actualResult, "CFF file does not conform to the CFF format");
        }

        [TestMethod]
        public void TestMethod2()
        {
            // Arrange.
            string path = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + "/PT1 - Test2.taff";
            TaskAllocations taskAllocations = new TaskAllocations(path);
            Configuration configuration = new Configuration();

            bool actualResult, value;
            bool expectedResult = true;
            
            // Act.
            value = taskAllocations.GetCffFilename();
            actualResult = configuration.Validate(taskAllocations.CffFilename);

            // Assert.
            Assert.AreEqual(expectedResult, actualResult, "CFF file does not conform to the CFF format");
        }

        [TestMethod]
        public void TestMethod3()
        {
            // Arrange.
            string path = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + "/PT1 - Test4.taff";
            TaskAllocations taskAllocations = new TaskAllocations(path);
            Configuration configuration = new Configuration();

            bool actualResult, value;
            bool expectedResult = true;
            
            // Act.
            value = taskAllocations.GetCffFilename();
            actualResult = configuration.Validate(taskAllocations.CffFilename);

            // Assert.
            Assert.AreNotEqual(expectedResult, actualResult, "CFF file conform to the CFF format");
        }
    }
}
