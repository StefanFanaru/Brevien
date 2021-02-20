using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Posting.Infrastructure.Data.Repositories
{
    public static class RepositoryHelpers
    {
        public static string Schema { get; set; }

        public static string GetTableName<T>()
        {
            var tableName = $"{typeof(T).Name}s";
            if (!string.IsNullOrEmpty(Schema))
            {
                tableName = $"{Schema}.{tableName}";
            }

            return tableName;
        }

        public static List<string> GetEntityProperties<T>()
        {
            return typeof(T).GetProperties().Select(x => x.Name).ToList();
        }

        public static string GenerateAnyQuery<T>((string column, string value, string conditionOperator)[] conditions)
        {
            if (!string.IsNullOrWhiteSpace(conditions[0].conditionOperator))
            {
                throw new Exception("First condition should not have an operator");
            }

            if (conditions.ToList().Skip(1).Any(x => string.IsNullOrWhiteSpace(x.conditionOperator)))
            {
                throw new Exception("Mission condition operator");
            }

            var query = new StringBuilder(
                @"SELECT CASE WHEN EXISTS (SELECT * FROM");

            query.AppendLine($" {GetTableName<T>()}");

            for (int i = 0; i < conditions.Length; i++)
            {
                if (i == 0)
                {
                    query.AppendLine($"WHERE {conditions[i].column} = '{conditions[i].value}'");
                    continue;
                }

                query.AppendLine($"{conditions[i].conditionOperator} {conditions[i].column} = '{conditions[i].value}'");
            }

            query.Append(
                @") THEN CAST (1 AS BIT)
                  ELSE CAST (0 AS BIT)
                  END");

            var result = query.ToString();
            return query.ToString();
        }

        public static string GenerateInsertQuery<T>()
        {
            var insertQuery = new StringBuilder($"INSERT INTO {GetTableName<T>()} ");

            insertQuery.Append("(");

            GetEntityProperties<T>().ForEach(prop => insertQuery.Append($"[{prop}],"));

            insertQuery
                .Remove(insertQuery.Length - 1, 1)
                .Append(") VALUES (");

            GetEntityProperties<T>().ForEach(prop => insertQuery.Append($"@{prop},"));

            insertQuery
                .Remove(insertQuery.Length - 1, 1)
                .Append(")");

            return insertQuery.ToString();
        }

        public static string GenerateUpdateQuery<T>()
        {
            var updateQuery = new StringBuilder($"UPDATE {GetTableName<T>()} SET ");

            GetEntityProperties<T>().ForEach(property =>
            {
                if (!property.Equals("Id"))
                {
                    updateQuery.Append($"{property}=@{property},");
                }
            });

            updateQuery.Remove(updateQuery.Length - 1, 1); //remove last comma
            updateQuery.Append(" WHERE Id=@Id");

            return updateQuery.ToString();
        }
    }
}
