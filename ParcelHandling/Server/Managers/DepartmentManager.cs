using ParcelHandling.Shared;

namespace ParcelHandling.Server.Managers
{

    public class DepartmentManager
    {
        /// <summary>
        /// Creates a basic Dispatcher for parcels to departments based on a simple ruleset.
        /// 
        /// The read stream contains one of more departments, and for each department the expected format is as follows:
        /// the first line contains a Department name.
        /// The second line specifies zero or more actions the department can perform on a parcel and what the resulting parcel state will be.
        /// The next zero or more lines each contain expressions a parcel should ALL comply with to be assigned to the Department. Each line contains 
        /// one or more conditions of which any one should me met. These conditions state that some characteristic of the parcel (like Weight) should be 
        /// within some interval or should be equal to an exact value.
        /// 
        /// Example:
        /// 
        /// Finance
        /// Handle Package -> Handled, Scrap a Package -> Handled
        /// Value in <*, 1000] or Weight in [10,*> 
        /// Handled = false
        /// </summary>
        /// <param name="reader">The TextReader that contains a definition of Department and Handling rules.</param>
        /// <returns>A SimpleDepartmentDispatcher</returns>
        /// <exception cref="ArgumentException"></exception>
        public static Dispatcher<Department> Create(TextReader reader)
        {
            try
            {
                var result = new Dispatcher<Department>();

                string? read;
                while ((read = reader.ReadLine()) != null)
                {
                    var dept = new Department() { Name = read };
                    dept.Actions.AddRange(ReadActions(reader));
                    result.AddDispatchRule(dept, ReadDispatchRules(reader));
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Invalid department definition", ex);
            }
        }

        private static IExpression ReadDispatchRules(TextReader reader)
        {
            var result = new AndExpression();

            string? read;
            while ((read = reader.ReadLine()) != null && read != "")
            {
                var rule = new OrExpression();
                result.AddTerm(rule);

                foreach (var part in read.Split(" or "))
                {
                    if (part.Contains('='))
                    {
                        rule.AddTerm(EqualityCondition.Parse(part));
                    }
                    else if (part.Contains(" in "))
                    {
                        rule.AddTerm(IntervalCondition.Parse(part));
                    }
                    else
                    {
                        throw new ArgumentException($"Illegal expression: {part}");
                    }
                }
            }

            return result;
        }

        private static IEnumerable<ParcelAction> ReadActions(TextReader reader)
        {
            var result = new List<ParcelAction>();

            string? read = reader.ReadLine();
            if (read != null && read.Length > 0)
            {
                var handlingActions = read.Split(',');
                foreach (var action in handlingActions)
                {
                    var actionElement = action.Split("->");
                    var handlingAction = new ParcelAction() { Action = actionElement[0].Trim(), Result = Enum.Parse<ParcelState>(actionElement[1]) };
                    result.Add(handlingAction);
                }
            }

            return result;
        }

        /// <summary>
        /// Saves a basic Dispatcher for parcels to departments based on a simple ruleset to a TextWriter.
        /// </summary>
        /// <param name="writer">The writer to write saved data to.</param>
        /// <param name="dispatcher">The Dispatcher to be saved.</param>
        public static void Save(TextWriter writer, Dispatcher<Department> dispatcher)
        {
            foreach (var dept in dispatcher.Targets)
            {
                writer.WriteLine(dept.Name);

                writer.WriteLine(string.Join(", ", dept.Actions.Select(action => $"{action.Action} -> {action.Result}")));

                var rule = dispatcher.GetRule(dept);
                if (rule != null && rule is AndExpression andExpression)
                {
                    foreach (var term in andExpression.Terms)
                    {
                        if (term != null && term is OrExpression orExpression)
                        {
                            writer.WriteLine(string.Join(" or ", orExpression.Terms.Select(condition => condition.ToString())));
                        }
                    }
                }

                writer.WriteLine("");
            }
        }

        public static IEnumerable<Department> GetDepartments(IConfiguration configuration)
        {
            var departmentConfigFilename = AppManager.GetConfiguredPath(configuration, "DepartmentConfig");

            if (!File.Exists(departmentConfigFilename)) throw new ArgumentException("Departments not defined");

            IEnumerable<Department> result = new List<Department>();

            try
            {
                using StreamReader file = new(departmentConfigFilename);
                result = Create(file).Targets;
            }
            catch (Exception)
            {
                //Do nothing
            }

            return result;
        }
    }
}
