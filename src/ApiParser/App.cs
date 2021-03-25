using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using CsvHelper;
using ApiParser.Data;
using ApiParser.Helper;
using ApiParser.Inputs;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

namespace ApiParser
{
    internal class App
    {
        private readonly ILogger<App> _logger;
        private readonly AppSettings _appSettings;
        private readonly IInputParser _parser;
        private readonly IDataGetter _dataGetter;

        public App(ILogger<App> logger, AppSettings appSettings, IInputParser parser, IDataGetter dataGetter)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
            _parser = parser ?? throw new ArgumentNullException(nameof(parser));
            _dataGetter = dataGetter ?? throw new ArgumentNullException(nameof(dataGetter));
        }

        private bool IsColumnNamesRequired { get; set; }

        public async Task Run(string inputFilePath)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            _logger.LogInformation("Running processing...");

            string outputFilePath = GetOutputFilePath(inputFilePath);

            _logger.LogInformation($"Output file: {outputFilePath}");

            FileStream outputFileStream = File.Exists(outputFilePath)
                ? File.Open(outputFilePath, FileMode.Truncate)
                : File.Create(outputFilePath);

            int skipped = 0;
            int processed = 0;

            using (var writer = new StreamWriter(outputFileStream))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                var csvData = _parser.Parse(inputFilePath);
                int i = 0;

                IsColumnNamesRequired = _appSettings.HasHeaderRecord;
                List<string> columnNames = new List<string>();


                foreach (string[] row in csvData)
                {
                    string rowData = row[_appSettings.UrlColumn];

                    if (IsColumnNamesRequired)
                    {
                        foreach (var columnName in row)
                        {
                            columnNames.Add(columnName);
                        }
                        columnNames.Add(Constants.ColumnNameResponse);
                    }

                    if (string.IsNullOrEmpty(rowData))
                    {
                        if (_appSettings.ShowNoDataWarning) _logger.LogWarning($"Row {i} column {_appSettings.UrlColumn} has no data.");
                        skipped++;
                        continue;
                    }
                    string data = string.Empty;
                    try
                    {
                        data = await _dataGetter.GetData(rowData).ConfigureAwait(false);
                        await WriteInFile(csv, data, row, columnNames);
                        processed++;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Error while processing row {i}, data {row[_appSettings.UrlColumn]}");
                        await WriteInFile(csv, data, row, columnNames, true);
                        processed++;
                    }

                    if (i % 100 == 0)
                    {
                        _logger.LogInformation($"Processed {i} rows");
                    }

                    i++;
                }
            }

            sw.Stop();

            _logger.LogInformation($"Processing finished, skipped: {skipped}, processed: {processed}, took: {sw.Elapsed.TotalMinutes} mins or {sw.Elapsed.TotalSeconds} seconds");
        }


        private string GetOutputFilePath(string filePath)
        {
            string fileName = Path.GetFileNameWithoutExtension(filePath);
            string directory = Path.GetDirectoryName(filePath);
            string newFileName = $"{fileName}-updated.csv";
            return Path.Combine(directory, newFileName);
        }

        private async Task WriteInFile(CsvWriter csv, string data, string[] row, IEnumerable<string> columnNames, bool isExceptionTrown = false)
        {
            string noMatchingRecords = Constants.ErrorResponseMessage;
            string message = isExceptionTrown ? noMatchingRecords : data;

            if (IsColumnNamesRequired)
            {
                IsColumnNamesRequired = false;
                await LoopRows(csv, columnNames.ToArray(), string.Empty);
            }
            else
            {
                await LoopRows(csv, row, message);
            }
        }
        private async Task LoopRows(CsvWriter csv, string[] rows, string message)
        {
            foreach (var o in rows)
            {
                csv.WriteField(o);
            }

            csv.WriteField(message);

            await csv.NextRecordAsync();
        }
    }
}
