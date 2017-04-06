using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using Newtonsoft.Json.Linq;
using OxyPlot;
using OxyPlot.Series;

namespace SimAirportStatsViewer
{
    public class GameReport
    {
        public Int64 DayNumber;
        public double s_sentiment;
        public Int64 s_i;
        public Int64 n_carousels;
        public Int64 ScheduledPassengers_Departures;
        public Int64 MissedFlight;
        public Int64 BoardedFlight;
        public Int64 LuggageLost;
        public Int64 LuggageSucceeded;
        public Int64 FlightsCount;
        public Int64 FlightsCanceled;
        public Int64 FlightsDelayed;
    }


    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IEnumerable<GameReport> ReadReports(string inputFilename)
        {
            dynamic d = JObject.Parse(File.ReadAllText(inputFilename));
            var gameReports = d["GameReports"];
            var reports = new List<GameReport>();
            foreach (var gameReport in gameReports)
            {
                var report = new GameReport();
                report.DayNumber = Int64.Parse(gameReport.Name);
                foreach (var gameReportKeyValuePair in gameReport)
                {
                    foreach (var keyValuePair in gameReportKeyValuePair)
                    {
                        var fields = typeof(GameReport).GetFields(BindingFlags.Public | BindingFlags.Instance);
                        var field = fields.FirstOrDefault(f => f.Name == keyValuePair.Name);
                        if (field != null)
                            field.SetValue(report, keyValuePair.Value.Value);
                    }
                }
                reports.Add(report);
            }
            return reports.OrderBy(r => r.DayNumber).ToList();
        }

        public List<PlotModel> PlotModels { get; set; } = new List<PlotModel>();

        private PlotModel GeneratePlot(IEnumerable<GameReport> reports, string propertyName)
        {
            var fields = typeof(GameReport).GetFields(BindingFlags.Public | BindingFlags.Instance);
            var field = fields.FirstOrDefault(f => f.Name.Equals(propertyName));
            if (field == null)
                throw new ArgumentException();

            var plot = new PlotModel();
            var series = new LineSeries();

            foreach (var report in reports)
            {
                var value = double.Parse(field.GetValue(report).ToString());
                series.Points.Add(new DataPoint(report.DayNumber, value));
            }

            plot.Series.Add(series);

            plot.Title = propertyName;

            return plot;
        }

        public MainWindow()
        {
            InitializeComponent();

            var inputFilename = @"C:\Users\steve\Desktop\README.airport.json";

            var reports = ReadReports(inputFilename);
            PlotModels.Add(GeneratePlot(reports, "s_sentiment"));
            PlotModels.Add(GeneratePlot(reports, "s_i"));
            PlotModels.Add(GeneratePlot(reports, "n_carousels"));
            PlotModels.Add(GeneratePlot(reports, "ScheduledPassengers_Departures"));
            PlotModels.Add(GeneratePlot(reports, "MissedFlight"));
            PlotModels.Add(GeneratePlot(reports, "BoardedFlight"));
            PlotModels.Add(GeneratePlot(reports, "LuggageLost"));
            PlotModels.Add(GeneratePlot(reports, "LuggageSucceeded"));
            PlotModels.Add(GeneratePlot(reports, "FlightsCount"));
            PlotModels.Add(GeneratePlot(reports, "FlightsCanceled"));
            PlotModels.Add(GeneratePlot(reports, "FlightsDelayed"));
            Application.Current.Shutdown();
            DataContext = this;
        }
    }
}
