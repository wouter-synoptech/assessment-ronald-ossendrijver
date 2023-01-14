using ParcelHandling.Server.Managers;
using ParcelHandling.Shared;
//using System.ComponentModel;
using System.Data;
using System.Text.Json;
using System.Xml.Serialization;
using static ParcelHandling.Shared.IExpression;

namespace Tests
{
    [TestClass]
    public class UnitTest1
    {
        public Dispatcher<Department> GetTestDispatcher()
        {
            var result = new Dispatcher<Department>();

            var ruleDepartmentA = new SimpleAndExpression();
            var ruleDepartmentAPart1 = new SimpleOrExpression();
            ruleDepartmentAPart1.AddTerm(new SimpleIntervalCondition("Weight", null, false, 1f, true));
            var ruleDepartmentAPart2 = new SimpleOrExpression();
            ruleDepartmentAPart2.AddTerm(new SimpleIntervalCondition("Value", 0f, true, 1000f, false));
            ruleDepartmentAPart2.AddTerm(new SimpleEqualityCondition("Authorized", true));
            ruleDepartmentA.AddTerm(ruleDepartmentAPart1);
            ruleDepartmentA.AddTerm(ruleDepartmentAPart2);
            var depA = new Department() { Name = "A" };
            result.AddDispatchRule(depA, ruleDepartmentA);

            var ruleDepartmentB = new SimpleAndExpression();
            var ruleDepartmentBPart1 = new SimpleOrExpression();
            ruleDepartmentBPart1.AddTerm(new SimpleIntervalCondition("Weight", 1f, false, 10f, true));
            var ruleDepartmentBPart2 = new SimpleOrExpression();
            ruleDepartmentBPart2.AddTerm(new SimpleIntervalCondition("Value", 0f, true, 1000f, false));
            ruleDepartmentBPart2.AddTerm(new SimpleEqualityCondition("Authorized", true));
            ruleDepartmentB.AddTerm(ruleDepartmentBPart1);
            ruleDepartmentB.AddTerm(ruleDepartmentBPart2);
            var depB = new Department() { Name = "B" };
            result.AddDispatchRule(depB, ruleDepartmentB);

            var ruleDepartmentC = new SimpleAndExpression();
            var ruleDepartmentCPart1 = new SimpleOrExpression();
            ruleDepartmentCPart1.AddTerm(new SimpleIntervalCondition("Weight", 10f, false, null, true));
            var ruleDepartmentCPart2 = new SimpleOrExpression();
            ruleDepartmentCPart2.AddTerm(new SimpleIntervalCondition("Value", 0f, true, 1000f, false));
            ruleDepartmentCPart2.AddTerm(new SimpleEqualityCondition("Authorized", true));
            ruleDepartmentC.AddTerm(ruleDepartmentCPart1);
            ruleDepartmentC.AddTerm(ruleDepartmentCPart2);
            var depC = new Department() { Name = "C" };
            result.AddDispatchRule(depC, ruleDepartmentC);

            var ruleDepartmentF = new SimpleAndExpression();
            var ruleDepartmentFPart1 = new SimpleOrExpression();
            ruleDepartmentFPart1.AddTerm(new SimpleIntervalCondition("Value", 1000f, true, null, true));
            var ruleDepartmentFPart2 = new SimpleOrExpression();
            ruleDepartmentFPart2.AddTerm(new SimpleEqualityCondition("Authorized", false));
            ruleDepartmentF.AddTerm(ruleDepartmentFPart1);
            ruleDepartmentF.AddTerm(ruleDepartmentFPart2);
            var depF = new Department() { Name = "F" };
            result.AddDispatchRule(depF, ruleDepartmentF);

            return result;
        }

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
            var rule = new SimpleAndExpression();
            var rulePart1 = new SimpleOrExpression();
            rulePart1.AddTerm(new SimpleIntervalCondition("Value", 1000f, true, float.MaxValue, true));
            rule.AddTerm(rulePart1);
            var rulePart2 = new SimpleOrExpression();
            rulePart2.AddTerm(new SimpleEqualityCondition("Authorized", false));
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
            var condition = SimpleEqualityCondition.Parse("Test = 10");
            var values1 = new Dictionary<string, object>() { ["Test"] = 10f };
            var values2 = new Dictionary<string, object>() { ["Test"] = 9f };

            Assert.IsTrue(condition.Evaluate(values1));
            Assert.IsFalse(condition.Evaluate(values2));
            Assert.AreEqual("Test = 10", condition.ToString());
        }

        [TestMethod]
        public void TestSimpleIntervalCondition()
        {
            var condition = SimpleIntervalCondition.Parse("Test in <9,10]");
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
            using (StreamWriter file1 = new("departmentconfig.txt"))
            {
                var departmentsIn = GetTestDispatcher();
                DepartmentManager.Save(file1, departmentsIn);
            }

            using (StreamReader file2 = new("departmentconfig.txt"))
            {
                var departmentsOut = DepartmentManager.Create(file2);
                Assert.AreEqual(4, departmentsOut.Targets.Count());
            }
        }

        [TestMethod]
        public void ReadContainers()
        {
            var serializer = new XmlSerializer(typeof(Container));
            var allParcels = new List<Parcel>();

            foreach (var containerFile in Directory.GetFiles("./ParcelContainers"))
            {
                using (StreamReader sr = new(containerFile))
                {
                    var container = (Container?)serializer.Deserialize(sr);

                    if (container != null)
                    {
                        Console.WriteLine($"Container {container.Id} - {container.ShippingDate}, #parcels: {container.Parcels.Count()}");
                        foreach (var parcel in container.Parcels)
                        {
                            Console.WriteLine($"{parcel.Receipient} - {parcel.Weight} - {parcel.Value}");
                        }
                        allParcels.AddRange(container.Parcels);
                    }
                }
            }

            Assert.AreEqual(17, allParcels.Count());
        }

    }
}