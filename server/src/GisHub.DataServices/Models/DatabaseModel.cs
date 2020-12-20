namespace Beginor.GisHub.DataServices.Models {

    public class TableModel {

        public string TableName { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }

    }

    public class ColumnModel {

        public string ColumnName { get; set; }
        public string Description { get; set; }
        public string DataType { get; set; }
        public int Length { get; set; }
        public bool IsNullable { get; set; }

    }

}
