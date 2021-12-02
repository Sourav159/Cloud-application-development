using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using ProgrammingTask1;
using System.IO;

namespace UnitTests
{
    [TestClass]
    public class CffFileErrorsCheck
    {
        [TestMethod]
        public void TestMethod1()
        {
            // Arrange.
            string path = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + "/PT1 - Test4.taff";
            TaskAllocations taskAllocations = new TaskAllocations(path);
            Configuration configuration = new Configuration();

            string expectedError = "<br>Invalid keyword TOTAL-RAM=987<br>";
            bool actualResult, value, fileValid;
            bool expectedResult = true;


            // Act.
            fileValid = taskAllocations.GetCffFilename();
            value = configuration.Validate(taskAllocations.CffFilename);

            actualResult = configuration.Errors.Contains(expectedError);

            // Assert.
            Assert.AreEqual(expectedResult, actualResult, "Expected error is not detected by the code");
        }

        [TestMethod]
        public void TestMethod2()
        {
            // Arrange.
            string path = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + "/PT1 - Test4.taff";
            TaskAllocations taskAllocations = new TaskAllocations(path);
            Configuration configuration = new Configuration();

            string expectedError = "<br>Invalid keyword Gbps=123<br>";
            bool actualResult, value, fileValid;
            bool expectedResult = true;


            // Act.
            fileValid = taskAllocations.GetCffFilename();
            value = configuration.Validate(taskAllocations.CffFilename);

            actualResult = configuration.Errors.Contains(expectedError);

            // Assert.
            Assert.AreEqual(expectedResult, actualResult, "Expected error is not detected by the code");
        }

        [TestMethod]
        public void TestMethod3()
        {
            // Arrange.
            string path = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName + "/PT1 - Test4.taff";
            TaskAllocations taskAllocations = new TaskAllocations(path);
            Configuration configuration = new Configuration();

            string expectedError = "<h3>PT1 - Test4.cff - file errors:</h3>";
            bool actualResult, value, fileValid;
            bool expectedResult = true;


            // Act.
            fileValid = taskAllocations.GetCffFilename();
            value = configuration.Validate(taskAllocations.CffFilename);

            actualResult = configuration.Errors.Contains(expectedError);

            // Assert.
            Assert.AreEqual(expectedResult, actualResult, "Expected error is not detected by the code");
        }
    }
}
