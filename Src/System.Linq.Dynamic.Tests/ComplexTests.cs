﻿using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq.Dynamic.Tests.Helpers;
using System.Linq;

namespace System.Linq.Dynamic.Tests
{
    [TestClass]
    public class ComplexTests
    {
        /// <summary>
        /// The purpose of this test is to verify that after a group by of a dynamically created
        /// key, the Select clause can access the key's members
        /// </summary>
        [TestMethod]
        public void GroupByAndSelect_TestDynamicSelectMember()
        {
            //Arrange
            var testList = User.GenerateSampleModels(100);
            var qry = testList.AsQueryable();

            //Act
            var byAgeReturnAll = qry.GroupBy("new (Profile.Age)");
            var selectQry = byAgeReturnAll.Select("new (Key.Age, Sum(Income) As TotalIncome)");

            //Real Comparison
            var realQry = qry.GroupBy(x => new { x.Profile.Age }).Select(x => new { x.Key.Age, TotalIncome = x.Sum(y => y.Income) });

            //Assert
            Assert.AreEqual(realQry.Count(), selectQry.Count());
#if NET35
            CollectionAssert.AreEqual(
                realQry.Select(x => x.Age).ToArray(),
                selectQry.Cast<object>().Select(x => ((object)x).GetDynamicProperty<int?>("Age")).ToArray());

            CollectionAssert.AreEqual(
                realQry.Select(x => x.TotalIncome).ToArray(),
                selectQry.Cast<object>().Select(x => ((object)x).GetDynamicProperty<int>("TotalIncome")).ToArray());
#else
            CollectionAssert.AreEqual(
                realQry.Select(x => x.Age).ToArray(),
                selectQry.AsEnumerable().Select(x => x.Age).ToArray());

            CollectionAssert.AreEqual(
                realQry.Select(x => x.TotalIncome).ToArray(),
                selectQry.AsEnumerable().Select(x => x.TotalIncome).ToArray());
#endif
        }
    }
}
