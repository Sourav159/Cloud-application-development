using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using ProgrammingTask1;
using System.IO;

namespace UnitTests
{
    [TestClass]
    public class TaffFileCheck
    {
        [TestMethod]
        public void TestMethod1()
        {

            // Arrange.
            
            string path = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + "/PT1 - Test1.taff";
            TaskAllocations taskAllocations = new TaskAllocations(path);

            bool actualResult;
            bool expectedResult = true;
            
            // Act.
            actualResult = taskAllocations.Validate();

            // Assert.
           Assert.AreEqual(expectedResult, actualResult, "TAFF file does not conform to the TAFF format");
        }

        [TestMethod]
        public void TestMethod2()
        {

            // Arrange.
            string path = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + "/PT1 - Test2.taff";
            TaskAllocations taskAllocations = new TaskAllocations(path);

            bool actualResult;
            bool expectedResult = true;
            
            // Act.
            actualResult = taskAllocations.Validate();

            // Assert.
            Assert.AreEqual(expectedResult, actualResult, "TAFF file does not conform to the TAFF format");
        }

        [TestMethod]
        public void TestMethod3()
        {

            // Arrange.
            string path = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + "/PT1 - Test4.taff";
            TaskAllocations taskAllocations = new TaskAllocations(path);

            bool actualResult;
            bool expectedResult = true;

            // Act.
            actualResult = taskAllocations.Validate();

            // Assert.
            Assert.AreNotEqual(expectedResult, actualResult, "TAFF file conform to the TAFF format");
        }
    }
}
