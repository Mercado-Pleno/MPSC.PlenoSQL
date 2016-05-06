﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using SqlEditor.Annotations;
using SqlEditor.Database;
using SqlEditor.Intellisense;

namespace SqlEditor.Databases.PostgreSql
{
    public class PostgreSqlInfoProvider : DbInfoProvider
    {
        public override IList<DatabaseInstance> GetDatabaseInstances(IDbConnection connection)
        {
            if (connection == null) throw new ArgumentNullException("connection");
            return GetDatabaseInstancesBase(connection, "SELECT datname FROM pg_database WHERE datistemplate = FALSE ORDER BY datname");
        }

        public override IList<Schema> GetSchemas(IDbConnection connection, string databaseInstance = null)
        {
            if (connection == null) throw new ArgumentNullException("connection");
            return GetSchemasBase(connection, "SELECT schema_name FROM information_schema.schemata ORDER BY schema_name");
        }

        public override IList<Table> GetTables(IDbConnection connection, string schemaName, string databaseInstanceName = null)
        {
            if (connection == null) throw new ArgumentNullException("connection");
            return GetTablesBase(connection, schemaName, "SELECT table_name FROM information_schema.tables WHERE table_type = 'BASE TABLE' AND UPPER(table_schema) = @1 ORDER BY table_name", schemaName.Trim().ToUpper());
        }

        public override IList<Column> GetTableColumns([NotNull] IDbConnection connection, [NotNull] string schemaName, [NotNull] string tableName, string databaseInstanceName = null)
        {
            if (connection == null) throw new ArgumentNullException("connection");
            if (schemaName == null) throw new ArgumentNullException("schemaName");
            if (tableName == null) throw new ArgumentNullException("tableName");
            return GetTableColumnsBase(connection, schemaName, tableName, "SELECT column_name, data_type, character_maximum_length, numeric_precision, numeric_scale, is_nullable, ordinal_position FROM information_schema.columns WHERE UPPER(table_schema) = @1 AND UPPER(table_name) = @2 ORDER BY ordinal_position", schemaName.Trim().ToUpper(), tableName.Trim().ToUpper());
        }

        public override IList<Column> GetTablePrimaryKeyColumns(IDbConnection connection, string schemaName, string tableName, string databaseInstanceName = null)
        {
            if (connection == null) throw new ArgumentNullException("connection");
            if (schemaName == null) throw new ArgumentNullException("schemaName");
            if (tableName == null) throw new ArgumentNullException("tableName");
            return GetTableColumnsBase(connection, schemaName, tableName, "SELECT c.column_name, c.data_type, c.character_maximum_length, c.numeric_precision, c.numeric_scale, c.is_nullable, c.ordinal_position   FROM information_schema.columns c INNER JOIN information_schema.key_column_usage k 	USING(table_schema,table_name,column_name) INNER JOIN information_schema.table_constraints t USING(table_schema,table_name,constraint_name) WHERE UPPER(c.table_schema) = @1 AND UPPER(c.table_name) = @2 AND UPPER(t.constraint_type) = 'PRIMARY KEY'", schemaName.Trim().ToUpper(), tableName.Trim().ToUpper());
        }

        public override IList<Partition> GetTablePartitions(IDbConnection connection, string schemaName, string tableName, string databaseInstanceName = null)
        {
            return new List<Partition>();
        }

        public override IList<View> GetViews([NotNull] IDbConnection connection, [NotNull] string schemaName, string databaseInstanceName = null)
        {
            if (connection == null) throw new ArgumentNullException("connection");
            if (schemaName == null) throw new ArgumentNullException("schemaName");
            return GetViewsBase(connection, schemaName, "SELECT table_name FROM information_schema.views WHERE UPPER(table_schema) = @1 ORDER BY table_name", schemaName.Trim().ToUpper());
        }

        public override IList<Column> GetViewColumns(IDbConnection connection, string schemaName, string viewName, string databaseInstanceName = null)
        {
            if (connection == null) throw new ArgumentNullException("connection");
            if (schemaName == null) throw new ArgumentNullException("schemaName");
            if (viewName == null) throw new ArgumentNullException("viewName");
            return GetTableColumnsBase(connection, schemaName, viewName, "SELECT column_name, data_type, character_maximum_length, numeric_precision, numeric_scale, is_nullable, ordinal_position FROM information_schema.columns WHERE UPPER(table_schema) = @1 AND UPPER(table_name) = @2 ORDER BY ordinal_position", schemaName.Trim().ToUpper(), viewName.Trim().ToUpper());
        }

        public override IList<MaterializedView> GetMaterializedViews(IDbConnection connection, string schemaName, string databaseInstanceName = null)
        {
            return new List<MaterializedView>();
        }

        public override IList<Column> GetMaterializedViewColumns(IDbConnection connection, string schemaName, string materializedViewName, string databaseInstanceName = null)
        {
            return new List<Column>();
        }

        public override IList<Index> GetIndexes([NotNull] IDbConnection connection, [NotNull] string schemaName,string databaseInstanceName = null)
        {
            if (connection == null) throw new ArgumentNullException("connection");
            if (schemaName == null) throw new ArgumentNullException("schemaName");
            return GetIndexesBase(connection, schemaName,
                                  "SELECT null as db_name, tns.nspname as table_schema, tc.relname as table_name, ins.nspname as index_schema, ic.relname as index_name, CASE WHEN ix.indisunique = 't' THEN 1 ELSE 0 END as is_unique FROM pg_index ix JOIN pg_class ic ON ix.indexrelid = ic.oid JOIN pg_namespace ins ON ic.relnamespace = ins.oid JOIN pg_class tc ON ix.indrelid = tc.oid JOIN pg_namespace tns ON tc.relnamespace = tns.oid where tc.relkind = 'r' AND ic.relkind = 'i' and UPPER(ins.nspname) = @1 ORDER BY ic.relname",
                                  schemaName.Trim().ToUpper());
        }

        public override IList<Index> GetIndexesForTable(IDbConnection connection, string schemaName, string tableName, string databaseInstanceName = null)
        {
            if (connection == null) throw new ArgumentNullException("connection");
            if (schemaName == null) throw new ArgumentNullException("schemaName");
            return GetIndexesBase(connection, schemaName,
                                  "SELECT null as db_name, tns.nspname as table_schema, tc.relname as table_name, ins.nspname as index_schema, ic.relname as index_name, CASE WHEN ix.indisunique = 't' THEN 1 ELSE 0 END as is_unique FROM pg_index ix JOIN pg_class ic ON ix.indexrelid = ic.oid JOIN pg_namespace ins ON ic.relnamespace = ins.oid JOIN pg_class tc ON ix.indrelid = tc.oid JOIN pg_namespace tns ON tc.relnamespace = tns.oid where tc.relkind = 'r' AND ic.relkind = 'i' AND UPPER(tns.nspname) = @1 AND UPPER(tc.relname) = @2 ORDER BY ic.relname",
                                  schemaName.Trim().ToUpper(), tableName.Trim().ToUpper());
        }

        public override IList<Column> GetIndexColumns([NotNull] IDbConnection connection, string tableSchemaName, string tableName, [NotNull] string indexSchemaName, [NotNull] string indexName, object indexId = null,
            string databaseInstanceName = null)
        {
            if (connection == null) throw new ArgumentNullException("connection");
            if (indexSchemaName == null) throw new ArgumentNullException("indexSchemaName");
            if (indexName == null) throw new ArgumentNullException("indexName");

            // Get table columns
            var tableColumns = GetTableColumns(connection, tableSchemaName, tableName, databaseInstanceName);
            var indexedColumns = new List<Column>();
            
            // Get indexed columns
            using (var command = connection.CreateCommand())
            {
                BuildSqlCommand(command, "SELECT a.attname as column_name FROM pg_index ix JOIN pg_class ic ON ix.indexrelid = ic.oid JOIN pg_namespace ins ON ic.relnamespace = ins.oid JOIN pg_class tc ON ix.indrelid = tc.oid JOIN pg_namespace tns ON tc.relnamespace = tns.oid JOIN pg_attribute a ON a.attrelid = tc.oid where tc.relkind = 'r' AND ic.relkind = 'i' AND a.attnum = ANY(ix.indkey) AND UPPER(tns.nspname) = @1 AND UPPER(tc.relname) = @2  AND UPPER(ins.nspname) = @3 AND UPPER(ic.relname) = @4 ORDER BY a.attnum",
                                  new object[] { tableSchemaName.Trim().ToUpper(), tableName.Trim().ToUpper(), indexSchemaName.Trim().ToUpper(), indexName.Trim().ToUpper() }) ;
                using (var dr = command.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        var indexedColumnName = dr.GetString(0);
                        var tableColumn =
                            tableColumns.FirstOrDefault(x => string.Equals(x.Name, indexedColumnName.Trim(), StringComparison.InvariantCultureIgnoreCase));
                        if (tableColumn != null)
                        {
                            indexedColumns.Add(tableColumn);
                        }
                    }
                }
            }
            return indexedColumns;
        }

        public override IList<Column> GetIndexIncludedColumns(IDbConnection connection, string tableSchemaName, string tableName, string indexSchemaName, string indexName, object indexId = null, string databaseInstanceName = null)
        {
            return new List<Column>();
        }

        public override IList<Sequence> GetSequences(IDbConnection connection, string schemaName, string databaseInstanceName = null)
        {
            if (connection == null) throw new ArgumentNullException("connection");
            if (schemaName == null) throw new ArgumentNullException("schemaName");
            return GetSequencesBase(connection, schemaName,
                                    "SELECT relname, 0, 0, 1, 0 FROM pg_class c JOIN pg_namespace n ON c.relnamespace = n.oid WHERE UPPER(relkind) = 'S'    AND UPPER(n.nspname) = @1 ORDER BY relname",
                                    schemaName.Trim().ToUpper());
        }

        public override IList<Constraint> GetConstraints(IDbConnection connection, string schemaName, string databaseInstanceName = null)
        {
            if (connection == null) throw new ArgumentNullException("connection");
            if (schemaName == null) throw new ArgumentNullException("schemaName");
            return GetConstraintsBase(connection, schemaName, "select TABLE_SCHEMA, CONSTRAINT_NAME, 'Y', CONSTRAINT_TYPE from information_schema.TABLE_CONSTRAINTS where UPPER(TABLE_SCHEMA) = @1 order by CONSTRAINT_NAME",
                                       schemaName.Trim().ToUpper());
            
        }

        public override IList<Constraint> GetConstraintsForTable(IDbConnection connection, string schemaName, string tableName,
            string databaseInstanceName = null)
        {
            if (connection == null) throw new ArgumentNullException("connection");
            if (schemaName == null) throw new ArgumentNullException("schemaName");
            return GetConstraintsBase(connection, schemaName, "select TABLE_SCHEMA, CONSTRAINT_NAME, 'Y', CONSTRAINT_TYPE from information_schema.TABLE_CONSTRAINTS where UPPER(TABLE_SCHEMA) = @1 and UPPER(TABLE_NAME) = @2 order by CONSTRAINT_NAME",
                                       schemaName.Trim().ToUpper(), tableName.Trim().ToUpper());
        }

        public override IList<Trigger> GetTriggers([NotNull] IDbConnection connection, [NotNull] string schemaName, string databaseInstanceName = null)
        {
            if (connection == null) throw new ArgumentNullException("connection");
            if (schemaName == null) throw new ArgumentNullException("schemaName");
            return GetTriggersBase(connection, schemaName,
                                   "SELECT trigger_name FROM information_schema.triggers WHERE UPPER(trigger_schema) = @1 ORDER BY trigger_name",
                                   schemaName.Trim().ToUpper());
        }

        public override IList<Synonym> GetPublicSynonyms(IDbConnection connection, string schemaName, string databaseInstanceName = null)
        {
            return new List<Synonym>();
        }

        public override IList<Synonym> GetSynonyms(IDbConnection connection, string schemaName, string databaseInstanceName = null)
        {
            return new List<Synonym>();
        }

        public override IList<StoredProcedure> GetStoredProcedures(IDbConnection connection, string schemaName, string databaseInstanceName = null)
        {
            throw new NotSupportedException("PostgreSQL does not support procedures");
            //if (connection == null) throw new ArgumentNullException("connection");
            //if (schemaName == null) throw new ArgumentNullException("schemaName");
            //var procedures = GetStoredProceduresBase(connection, schemaName,
            //                      "SELECT r.specific_name, r.routine_name, '' as routine_definition FROM information_schema.routines r WHERE UPPER(r.routine_schema) = @1 AND r.routine_type = 'PROCEDURE' ORDER BY r.routine_name",
            //                      schemaName.ToUpper());
            //using (var command = connection.CreateCommand())
            //{
            //    foreach (var storedProcedure in procedures)
            //    {
            //        command.CommandText = "SHOW CREATE PROCEDURE " + storedProcedure.FullyQualifiedName;
            //        using (var dr = command.ExecuteReader())
            //        {
            //            if (dr.Read())
            //            {
            //                storedProcedure.Definition = dr.IsDBNull(2) ? null : dr.GetString(2);
            //            }
            //        }

            //    }
            //}
            //return procedures;
        }

        public override IList<Function> GetFunctions(IDbConnection connection, string schemaName, string databaseInstanceName = null)
        {
            if (connection == null) throw new ArgumentNullException("connection");
            if (schemaName == null) throw new ArgumentNullException("schemaName");

            var functions = GetFunctionsBase(connection, schemaName,
                                  "SELECT r.specific_name, r.routine_name, '' as routine_definition FROM information_schema.routines r WHERE UPPER(r.routine_schema) = @1 AND r.routine_type = 'FUNCTION' ORDER BY r.routine_name",
                                  schemaName.ToUpper());
            using (var command = connection.CreateCommand())
            {
                foreach (var function in functions)
                {
                    command.Parameters.Clear();
                    BuildSqlCommand(command, "SELECT pg_get_functiondef(p.oid) FROM pg_proc p JOIN pg_namespace ns ON p.pronamespace = ns.oid WHERE UPPER(ns.nspname) = @1 and UPPER(p.proname) = @2", new object[] { schemaName.Trim().ToUpper(), function.Name.Trim().ToUpper() });
                    using (var dr = command.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            function.Definition = dr.IsDBNull(0) ? null : dr.GetString(0);
                        }
                    }

                }
            }
            return functions;
        }

        public override IList<ColumnParameter> GetStoredProcedureParameters(IDbConnection connection, StoredProcedure storedProcedure)
        {
            throw new NotSupportedException("PostgreSQL does not support procedures");
            //if (connection == null) throw new ArgumentNullException("connection");
            //if (storedProcedure == null) throw new ArgumentNullException("storedProcedure");

            //const string sql =
            //    "SELECT parameter_name, data_type, character_maximum_length, numeric_precision, numeric_scale, 'YES' as is_nullable, ordinal_position, parameter_mode  FROM information_schema.parameters WHERE UPPER(specific_schema) = @1 AND UPPER(specific_name) = @2 ORDER BY ordinal_position";
            //return GetStoredProcedureParametersBase(connection, storedProcedure, sql,
            //                                            storedProcedure.Parent.Name.ToUpper(),
            //                                            storedProcedure.ObjectId.ToUpper());
        }

        public override IList<ColumnParameter> GetFunctionParameters(IDbConnection connection, Function function)
        {
            if (connection == null) throw new ArgumentNullException("connection");
            if (function == null) throw new ArgumentNullException("function");

            const string sql =
                "SELECT parameter_name, data_type, character_maximum_length, numeric_precision, numeric_scale, 'YES' as is_nullable, ordinal_position, parameter_mode  FROM information_schema.parameters WHERE UPPER(specific_schema) = @1 AND UPPER(specific_name) = @2 AND parameter_mode = 'IN' ORDER BY ordinal_position";
            return GetStoredProcedureParametersBase(connection, function, sql,
                                                        function.Parent.Name.ToUpper(),
                                                        function.ObjectId.ToUpper());
        }

        public override IList<ColumnParameter> GetFunctionReturnValue(IDbConnection connection, Function function)
        {
            if (connection == null) throw new ArgumentNullException("connection");
            if (function == null) throw new ArgumentNullException("function");

            const string sql =
                "SELECT '' as parameter_name, typname, typlen, 0, 0, 'YES' as is_nullable, 1, 'OUT' FROM pg_type t JOIN pg_proc p ON t.oid = p.prorettype JOIN pg_namespace ns ON p.pronamespace = ns.oid WHERE UPPER(ns.nspname) = @1 AND UPPER(p.proname) = @2";
            return GetStoredProcedureParametersBase(connection, function, sql,
                                                        function.Parent.Name.ToUpper(),
                                                        function.Name.ToUpper());
        }

        public override IList<Package> GetPackages(IDbConnection connection, string schemaName, string databaseInstanceName = null)
        {
            throw new NotImplementedException();
        }

        public override IList<PackageProcedure> GetPackageProcedures(IDbConnection connection, string schemaName, string packageName, string databaseInstanceName = null)
        {
            throw new NotImplementedException();
        }

        public override IList<Login> GetLogins(IDbConnection connection, string databaseInstanceName = null)
        {
            return GetLoginsBase(connection, "SELECT usename FROM pg_catalog.pg_user u ORDER BY usename", null);
        }

        public override IntelisenseData GetIntelisenseData([NotNull] IDbConnection connection,
                                                           [NotNull] string currentSchemaName)
        {
            if (connection == null) throw new ArgumentNullException("connection");
            if (currentSchemaName == null) throw new ArgumentNullException("currentSchemaName");
            const string tablesSql =
                "SELECT c.table_schema, c.table_name, c.column_name FROM information_schema.columns c ORDER BY c.table_schema, c.table_name, c.column_name";
            return GetIntellisenseDataHelper(connection, currentSchemaName, tablesSql, null, null);
        }
    }
}