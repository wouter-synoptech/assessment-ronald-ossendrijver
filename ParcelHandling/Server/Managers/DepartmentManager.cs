using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ParcelHandling.Shared;

namespace ParcelHandling.Server.Managers
{
    public class DepartmentManager
    {
        /// <summary>
        /// SimpleDispatchingRuleset structure:
        /// 
        /// (value of x1 in interval a1 || value of x2 in interval a2 || ...)
        /// &&
        /// (value of x3 in interval b1 || value of x4 in interval b2 || ...)
        /// &&
        /// ...
        /// 
        /// </summary>
        /// 

        public static Dispatcher<Department> Create(TextReader reader)
        {
            var result = new Dispatcher<Department>();

            string? read;
            while ((read = reader.ReadLine()) != null)
            {
                var rulesetForDepartment = new SimpleAndExpression();
                var dept = new Department() { Name = read };

                read = reader.ReadLine();
                dept.HandlingResult = Enum.Parse<ParcelState>(read);

                while ((read = reader.ReadLine()) != null && read != "")
                {
                    var rule = new SimpleOrExpression();
                    rulesetForDepartment.AddTerm(rule);

                    foreach (var part in read.Split(" or "))
                    {
                        if (part.Contains('='))
                        {
                            rule.AddTerm(SimpleEqualityCondition.Parse(part));
                        }
                        else if (part.Contains(" in "))
                        {
                            rule.AddTerm(SimpleIntervalCondition.Parse(part));
                        }
                        else
                        {
                            throw new ArgumentException($"Illegal expression: {part}");
                        }
                    }
                }

                result.AddDispatchRule(dept, rulesetForDepartment);
            }

            return result;
        }

        public static IEnumerable<Department> Save(TextWriter writer, Dispatcher<Department> dispatcher)
        {
            var result = new List<Department>();

            foreach (var dept in dispatcher.Targets)
            {
                writer.WriteLine(dept.Name);
                writer.WriteLine(dept.HandlingResult);

                var rule = dispatcher.GetRule(dept);
                if (rule != null && rule is SimpleAndExpression andExpression)
                {
                    foreach (var term in andExpression.Terms)
                    {
                        if (term != null && term is SimpleOrExpression orExpression)
                        {
                            writer.WriteLine(string.Join(" or ", orExpression.Terms.Select(condition => condition.ToString())));
                        }
                    }
                }

                writer.WriteLine("");
            }

            return result;
        }

        public static IEnumerable<Department> GetDepartments()
        {
            IEnumerable<Department> result = new List<Department>();

            try
            {
                using (StreamReader file = new("departmentconfig.txt"))
                {
                    result = Create(file).Targets;
                }
            }
            catch (Exception)
            {
                //Do nothing
            }

            return result;
        }
    }
}
