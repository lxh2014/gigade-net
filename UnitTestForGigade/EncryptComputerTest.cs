using BLL.gigade.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace UnitTestForGigade
{
    
    
    /// <summary>
    ///这是 EncryptComputerTest 的测试类，旨在
    ///包含所有 EncryptComputerTest 单元测试
    ///</summary>
    [TestClass()]
    public class EncryptComputerTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///获取或设置测试上下文，上下文提供
        ///有关当前测试运行及其功能的信息。
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region 附加测试特性
        // 
        //编写测试时，还可使用以下特性:
        //
        //使用 ClassInitialize 在运行类中的第一个测试前先运行代码
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //使用 ClassCleanup 在运行完类中的所有测试后再运行代码
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //使用 TestInitialize 在运行每个测试前先运行代码
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //使用 TestCleanup 在运行完每个测试后运行代码
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        

        /// <summary>
        ///EncryptDecryptTextByApi 的测试
        ///</summary>
        [TestMethod()]
        public void EncryptDecryptTextByApiTest()
        {
            string plainText = "24d12"; // TODO: 初始化为适当的值
            bool isEncrypt = true; // TODO: 初始化为适当的值
            string expected = "24d12"; // TODO: 初始化为适当的值
            string actual;
            actual = EncryptComputer.EncryptDecryptTextByApi(plainText, isEncrypt);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("验证此测试方法的正确性。");
        }

        /// <summary>
        ///EncryptDecryptTextByApi 的测试
        ///</summary>
        [TestMethod()]
        public void EncryptDecryptTextByApiTest1()
        {
            string plainText = "gasdeaeasef"; // TODO: 初始化为适当的值
            bool isEncrypt = false; // TODO: 初始化为适当的值
            string expected = string.Empty; // TODO: 初始化为适当的值
            string actual;
            try
            {
                actual = EncryptComputer.EncryptDecryptTextByApi(plainText, true);
            }
            catch (Exception ex)
            {

                actual = plainText+ex.Message;
            }
            //actual2 = EncryptComputer.EncryptDecryptTextByApi(plainText, false);
            Assert.AreEqual(plainText, actual + 1);
            //Assert.Inconclusive("验证此测试方法的正确性。");
        }
    }
}
