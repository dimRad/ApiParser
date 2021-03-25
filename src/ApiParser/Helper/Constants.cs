namespace ApiParser.Helper
{
    public class Constants
    {
        public const string ErrorResponseMessage = "{\"response\":\"No matching records\"}";

        public const string ColumnNameResponse = "Api Response";

        public const string LogProcessingInfo = "Processing row {0}, data {1}";

        public const string LogWarning = "Row {0} column {1} has no data.";

        public const string LogErrorProcessing = "Error while processing row {0}, data {1}";

        public const string LogEndProcessing = "Processing finished, skipped: {0}, processed: {1}, took: {2} mins or {3} seconds";
    }
}
