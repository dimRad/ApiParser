using System.Collections.Generic;
using System.Globalization;
using System.IO;
using CsvHelper;

namespace ApiParser.Inputs
{
    public class CsvParser : IInputParser
    {
        private readonly AppSettings _appSettings;

        public CsvParser(AppSettings appSettings)
        {
            _appSettings = appSettings;
        }

        /// <summary>
        /// Parses CSV file and returns each row as an array
        /// </summary>
        /// <param name="inputFilePath"></param>
        /// <returns></returns>
        public IEnumerable<string[]> Parse(string inputFilePath)
        {
            if (!File.Exists(inputFilePath))
            {
                throw new FileNotFoundException("File does not exist.", inputFilePath);
            }

            using (var reader = new StreamReader(inputFilePath))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                //csv.Configuration.HasHeaderRecord = _appSettings.HasHeaderRecord;

                //if (_appSettings.HasHeaderRecord)
                //{
                //    //csv.Read();
                //    //csv.ReadHeader();
                //}

                while (csv.Read())
                {
                    List<string> rowData = new List<string>();

                    for (int i = 0; i < 100; i++)
                    {
                        if (!csv.TryGetField<string>(i, out string c))
                        {
                            break;
                        }

                        rowData.Add(c);
                    }

                    yield return rowData.ToArray();
                }
            }
        }
    }
}
