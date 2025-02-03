# Custom Scripting
If you need to run custom scripts alongside migrations, once you've generated your EntityFramework migration using
`Add-Migration`, modify the generated migration class:

## Simple Scripts:
Simple changes can be made using the `migrationBuilder.Sql()` function:

```c# 
migrationBuilder.Sql("DROP DATABASE [master]");
```

## Larger Scripts:
More complicated changes can be created as a SQL script file. Please name this file the same as the migration class
with `_up` and `_down` in the filename. I.e. adding a SQL script for migration `20250113154211_TestData.cs` should 
be scripted in `20250113154211_TestData.up.sql` and if a revert script is also needed to revert the changes for the 
`down` function, it should be created as `20250113154211_TestData.down.sql`. These files will require their "Copy to 
output directory" property set to "Copy alyways" or "Copy if newer"; this ensures the file is available to the
database migration process.

These can then be implemented:

```c#
migrationBuilder.Sql(MigrationHelper.ReadMigrationFile("20250113154211_TestData.up.sql"))
```

## Note
When customising the databse migrations, please _always_ provide a `Down` reversion script to ensure databse roll-backs
are correctly applied.