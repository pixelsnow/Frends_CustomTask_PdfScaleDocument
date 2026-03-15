using System.Collections.Generic;

namespace Frends.Pdf.Create.Definitions;

/// <summary>
/// Definitions for whole table.
/// </summary>
public class TableDefinition
{
    /// <summary>
    /// Does the table have header rows?
    /// </summary>
    /// <example>true</example>
    public bool HasHeaderRow { get; set; }

    /// <summary>
    /// Type of the table.
    /// Defines if the table is a normal table, or if it is in header or footer.
    /// </summary>
    /// <example>Table</example>
    public TableTypeEnum TableType { get; set; }

    /// <summary>
    /// Styles for the table.
    /// </summary>
    /// <example>Refer to TableStyles</example>
    public TableStyle StyleSettings { get; set; }

    /// <summary>
    /// Definitions for table columns.
    /// </summary>
    /// <example>List of TableColumnDefinition. Please refer to TableColumnDefinition.</example>
    public List<TableColumnDefinition> Columns { get; set; }

    /// <summary>
    /// Table data.
    /// </summary>
    /// <example>[ { firstRowKey1: firstRowvalue1, firstRowKey2: firstRowValue2 }, { secondRowKey1: secondRowvalue1, secondRowKey2: secondRowValue2} ]</example>
    public List<Dictionary<string, string>> RowData { get; set; }
}
