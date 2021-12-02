﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using ProgrammingTask1;

namespace UnitTests
{
    [TestClass]
    public class RamCheck
    {
        [TestMethod]
        public void TestMethod1()
        {
            // Arrange.
            Task task = new Task(0, 1.1, 2.2, 2, 100, 10);
            Processor processor = new Processor(0, "Intel i5", 1.8, 4, 300, 50);

            bool expectedResult = true;
            bool actualResult;
           
            // Act.
            actualResult = task.IsRamSufficient(processor);

            // Assert.
            Assert.AreEqual(expectedResult, actualResult, "Task RAM is not less than the Processor RAM");
        }

        [TestMethod]
        public void TestMethod2()
        {
            // Arrange.
            Task task = new Task(0, 1.1, 2.2, 4, 100, 10);
            Processor processor = new Processor(0, "Intel i5", 1.8, 4, 300, 50);

            bool expectedResult = true;
            bool actualResult;
         
            // Act.
            actualResult = task.IsRamSufficient(processor);

            // Assert.
            Assert.AreEqual(expectedResult, actualResult, "Task RAM is not equal to the Processor RAM");
        }

        [TestMethod]
        public void TestMethod3()
        {
            // Arrange.
            Task task = new Task(0, 1.1, 2.2, 4, 100, 10);
            Processor processor = new Processor(0, "Intel i5", 1.8, 2, 300, 50);
            bool expectedResult = true;
            bool actualResult;        

            // Act.
            actualResult = task.IsRamSufficient(processor);

            // Assert.
            Assert.AreNotEqual(expectedResult, actualResult, "Task RAM is not more than the Processor RAM");
        }


    }
}
