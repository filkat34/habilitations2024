﻿using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace habilitations2024.dal.Tests
{
    [TestClass()]
    public class AccessTests
    {
        private static readonly Access access = Access.GetInstance();

        [TestMethod()]
        public void GetInstanceTest()
        {
            Assert.IsNotNull(access, "devrait réussir si connexion à la BDD correcte");
        }
    }
}