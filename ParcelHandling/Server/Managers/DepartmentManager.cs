﻿using ParcelHandling.Shared;

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
            try
            {
                var result = new Dispatcher<Department>();

                string? read;
                while ((read = reader.ReadLine()) != null)
                {
                    var rulesetForDepartment = new SimpleAndExpression();
                    var dept = new Department() { Name = read };

                    read = reader.ReadLine();
                    if (read != null && read.Length > 0)
                    {
                        var handlingActions = read.Split(',');
                        foreach (var action in handlingActions)
                        {
                            var actionElement = action.Split("->");
                            var handlingAction = new ParcelAction() { Action = actionElement[0].Trim(), Result = Enum.Parse<ParcelState>(actionElement[1]) };
                            dept.Actions.Add(handlingAction);
                        }
                    }

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
            catch (Exception ex)
            {
                throw new ArgumentException("Invalid department definition", ex);
            }
        }

        public static IEnumerable<Department> Save(TextWriter writer, Dispatcher<Department> dispatcher)
        {
            var result = new List<Department>();

            foreach (var dept in dispatcher.Targets)
            {
                writer.WriteLine(dept.Name);

                writer.WriteLine(string.Join(", ", dept.Actions.Select(action => $"{action.Action} -> {action.Result}")));

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
