using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using ProgrammingTask1;
using System.IO;

namespace UnitTests
{
    [TestClass]
    public class TaffFileErrorsCheck
    {
        [TestMethod]
        public void TestMethod1()
        {
            // Arrange.
            string path = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + "/PT1 - Test4.taff";
            TaskAllocations taskAllocations = new TaskAllocations(path);
            
            string expectedError = "<br>Invalid keyword LOCATIONS<br>";
            bool expectedResult = true;
            bool actualResult;

            // Act.
            bool value = taskAllocations.Validate();
            actualResult =  taskAllocations.Errors.Contains(expectedError);

            // Assert.
            Assert.AreEqual(expectedResult, actualResult, "Expected error is not detected by the code");
        }

        [TestMethod]
        public void TestMethod2()
        {
            // Arrange.
            string path = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + "/PT1 - Test4.taff";
            TaskAllocations taskAllocations = new TaskAllocations(path);

            string expectedError = "<br>More than expected number of allocations of 2 at ID=3<br>";
            bool expectedResult = true;
            bool actualResult;

            // Act.
            bool value = taskAllocations.Validate();
            actualResult = taskAllocations.Errors.Contains(expectedError);

            // Assert.
            Assert.AreEqual(expectedResult, actualResult, "Expected error is not detected by the code");
        }

        [TestMethod]
        public void TestMethod3()
        {
            // Arrange.
            string path = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + "/PT1 - Test4.taff";
            TaskAllocations taskAllocations = new TaskAllocations(path);

            string expectedError = "<br>Invalid number of tasks 4 for Allocation ID 6<br>";
            bool expectedResult = true;
            bool actualResult;

            // Act.
            bool value = taskAllocations.Validate();
            actualResult = taskAllocations.Errors.Contains(expectedError);

            // Assert.
            Assert.AreEqual(expectedResult, actualResult, "Expected error is not detected by the code");
        }
    }
}
