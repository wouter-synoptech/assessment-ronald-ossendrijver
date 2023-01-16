using ParcelHandling.Server.Managers;
using ParcelHandling.Shared;
using System.Xml.Serialization;

namespace Tests
{
    [TestClass]
    public class UnitTests
    {
        [TestMethod]
        public void TestDispatcher()
        {
            var dispatcher = GetTestDispatcher();
            var depts = dispatcher.Targets;
            var deptA = depts.First(d => d.Name == "A");
            var deptB = depts.First(d => d.Name == "B");
            var deptC = depts.First(d => d.Name == "C");
            var deptF = depts.First(d => d.Name == "F");

            var recipient = new Recipient()
            {
                Name = "A",
                Address = new Address() { City = "Rotterdam", PostalCode = "1234 AB", HouseNumber = "1", Street = "Blaak" }
            };

            var parcel1 = new Parcel() { Receipient = recipient, Value = 500f, Weight = 5f };
            var parcel2 = new Parcel() { Receipient = recipient, Value = 500f, Weight = 15f };
            var parcel3 = new Parcel() { Receipient = recipient, Value = 500f, Weight = 0.5f };
            var parcel4 = new Parcel() { Receipient = recipient, Value = 1500f, Weight = 3f };

            Assert.AreEqual(deptB, dispatcher.DetermineTarget(parcel1));
            Assert.AreEqual(deptC, dispatcher.DetermineTarget(parcel2));
            Assert.AreEqual(deptA, dispatcher.DetermineTarget(parcel3));
            Assert.AreEqual(deptF, dispatcher.DetermineTarget(parcel4));
        }

        [TestMethod]
        public void TestMixedRules()
        {
            var rule = new AndExpression();
            var rulePart1 = new OrExpression();
            rulePart1.AddTerm(new IntervalCondition("Value", 1000f, true, float.MaxValue, true));
            rule.AddTerm(rulePart1);
            var rulePart2 = new OrExpression();
            rulePart2.AddTerm(new EqualityCondition("Authorized", false));
            rule.AddTerm(rulePart2);

            var valuesToTestFalse1 = new Dictionary<string, object>() { ["Value"] = 1500f, ["Weight"] = 3f, ["Authorized"] = true };
            var valuesToTestFalse2 = new Dictionary<string, object>() { ["Value"] = 100f, ["Weight"] = 3f, ["Authorized"] = false };
            var valuesToTestTrue = new Dictionary<string, object>() { ["Value"] = 1500f, ["Weight"] = 3f, ["Authorized"] = false };
            Assert.IsFalse(rule.Evaluate(valuesToTestFalse1));
            Assert.IsFalse(rule.Evaluate(valuesToTestFalse2));
            Assert.IsTrue(rule.Evaluate(valuesToTestTrue));

        }

        [TestMethod]
        public void TestSimpleEqualityCondition()
        {
            var condition = EqualityCondition.Parse("Test = 10");
            var values1 = new Dictionary<string, object>() { ["Test"] = 10f };
            var values2 = new Dictionary<string, object>() { ["Test"] = 9f };

            Assert.IsTrue(condition.Evaluate(values1));
            Assert.IsFalse(condition.Evaluate(values2));
            Assert.AreEqual("Test = 10", condition.ToString());
        }

        [TestMethod]
        public void TestIntervalParsing()
        {
            var condition = IntervalCondition.Parse("Test in <9,10]");
            var values1 = new Dictionary<string, object>() { ["Test"] = 10f };
            var values2 = new Dictionary<string, object>() { ["Test"] = 9f };
            var values3 = new Dictionary<string, object>() { ["Test"] = 9.5f };
            var values4 = new Dictionary<string, object>() { ["Test"] = 10.5f };

            Assert.IsTrue(condition.Evaluate(values1), "10 in <9,10]");
            Assert.IsFalse(condition.Evaluate(values2), "9 in <9,10]");
            Assert.IsTrue(condition.Evaluate(values3), "9.5 in <9,10]");
            Assert.IsFalse(condition.Evaluate(values4), "10.5 in <9,10]");
            Assert.AreEqual("Test in <9,10]", condition.ToString());
        }

        [TestMethod]
        public void SaveAndLoadTestDepartments()
        {
            using StreamWriter file1 = new("departmentconfig.txt");
            var departmentsIn = GetTestDispatcher();
            DepartmentManager.Save(file1, departmentsIn);

            using StreamReader file2 = new("departmentconfig.txt");
            var departmentsOut = DepartmentManager.Create(file2);
            Assert.AreEqual(4, departmentsOut.Targets.Count());
        }

        [TestMethod]
        public void ReadContainers()
        {
            var serializer = new XmlSerializer(typeof(Container));
            var allParcels = new List<Parcel>();

            foreach (var containerFile in Directory.GetFiles("./ParcelContainers"))
            {
                using StreamReader sr = new(containerFile);
                var container = (Container?)serializer.Deserialize(sr);

                if (container != null && container.Parcels != null)
                {
                    allParcels.AddRange(container.Parcels);
                }
            }

            Assert.AreEqual(17, allParcels.Count);
        }

        [TestMethod]
        public void TestIntervals()
        {
            var a = new Interval(0, true, 10, false);
            Assert.IsTrue(a.Contains(5));
            Assert.IsTrue(a.Contains(5f));
            Assert.IsFalse(a.Contains(10f - 0.00001f));
            Assert.IsFalse(a.Contains(10f));

            var b = new Interval(0d, true, 10d, false);
            Assert.IsTrue(b.Contains(5));
            Assert.IsTrue(b.Contains(5f));
            Assert.IsTrue(b.Contains(10d - 0.000000001d));
            Assert.IsFalse(b.Contains(10f));
        }

        private static Dispatcher<Department> GetTestDispatcher()
        {
            var result = new Dispatcher<Department>();

            var ruleNotHandled = new OrExpression();
            ruleNotHandled.AddTerm(new EqualityCondition("Handled", false));

            var ruleDepartmentA = new AndExpression();
            var ruleDepartmentAPart1 = new OrExpression();
            ruleDepartmentAPart1.AddTerm(new IntervalCondition("Weight", null, false, 1f, true));
            var ruleDepartmentAPart2 = new OrExpression();
            ruleDepartmentAPart2.AddTerm(new IntervalCondition("Value", 0f, true, 1000f, false));
            ruleDepartmentAPart2.AddTerm(new EqualityCondition("Authorized", true));
            ruleDepartmentA.AddTerm(ruleNotHandled);
            ruleDepartmentA.AddTerm(ruleDepartmentAPart1);
            ruleDepartmentA.AddTerm(ruleDepartmentAPart2);
            var depAactions = new List<ParcelAction> { new ParcelAction() { Action = "Handle", Result = ParcelState.Handled } };
            var depA = new Department() { Name = "A", Actions = depAactions };
            result.AddDispatchRule(depA, ruleDepartmentA);

            var ruleDepartmentB = new AndExpression();
            var ruleDepartmentBPart1 = new OrExpression();
            ruleDepartmentBPart1.AddTerm(new IntervalCondition("Weight", 1f, false, 10f, true));
            var ruleDepartmentBPart2 = new OrExpression();
            ruleDepartmentBPart2.AddTerm(new IntervalCondition("Value", 0f, true, 1000f, false));
            ruleDepartmentBPart2.AddTerm(new EqualityCondition("Authorized", true));
            ruleDepartmentB.AddTerm(ruleNotHandled);
            ruleDepartmentB.AddTerm(ruleDepartmentBPart1);
            ruleDepartmentB.AddTerm(ruleDepartmentBPart2);
            var depBactions = new List<ParcelAction> { new ParcelAction() { Action = "Handle", Result = ParcelState.Handled } };
            var depB = new Department() { Name = "B", Actions = depBactions };
            result.AddDispatchRule(depB, ruleDepartmentB);

            var ruleDepartmentC = new AndExpression();
            var ruleDepartmentCPart1 = new OrExpression();
            ruleDepartmentCPart1.AddTerm(new IntervalCondition("Weight", 10f, false, null, true));
            var ruleDepartmentCPart2 = new OrExpression();
            ruleDepartmentCPart2.AddTerm(new IntervalCondition("Value", 0f, true, 1000f, false));
            ruleDepartmentCPart2.AddTerm(new EqualityCondition("Authorized", true));
            ruleDepartmentC.AddTerm(ruleNotHandled);
            ruleDepartmentC.AddTerm(ruleDepartmentCPart1);
            ruleDepartmentC.AddTerm(ruleDepartmentCPart2);
            var depCactions = new List<ParcelAction> { new ParcelAction() { Action = "Handle", Result = ParcelState.Handled } };
            var depC = new Department() { Name = "C", Actions = depCactions };
            result.AddDispatchRule(depC, ruleDepartmentC);

            var ruleDepartmentF = new AndExpression();
            var ruleDepartmentFPart1 = new OrExpression();
            ruleDepartmentFPart1.AddTerm(new IntervalCondition("Value", 1000f, true, null, true));
            var ruleDepartmentFPart2 = new OrExpression();
            ruleDepartmentFPart2.AddTerm(new EqualityCondition("Authorized", false));
            ruleDepartmentF.AddTerm(ruleNotHandled);
            ruleDepartmentF.AddTerm(ruleDepartmentFPart1);
            ruleDepartmentF.AddTerm(ruleDepartmentFPart2);
            var depFactions = new List<ParcelAction> { new ParcelAction() { Action = "Authorize", Result = ParcelState.Authorized } };
            var depF = new Department() { Name = "F", Actions = depFactions };
            result.AddDispatchRule(depF, ruleDepartmentF);

            return result;
        }
    }
}