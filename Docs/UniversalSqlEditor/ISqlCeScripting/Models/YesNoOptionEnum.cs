﻿namespace ErikEJ.SqlCeScripting
{
    public enum YesNoOption
    {
        YES
        , NO
    }

    public enum Scope
    {
        Schema,
        SchemaData,
        SchemaDataBlobs,
        SchemaDataAzure,
        DataOnly,
        DataOnlyForSqlServer,
        SchemaDataSQLite
    }

    public enum CommandExecute
    {
        Undefined,
        DataTable,
        NonQuery,
        NonQuerySchemaChanged
    }
}
