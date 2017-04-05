using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using Newtonsoft.Json.Linq;

namespace SimAirportStatsViewer
{
    internal class GameReport
    {
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
        private List<GameReport> ReadReports(string inputFilename)
        {
            dynamic d = JObject.Parse(File.ReadAllText(inputFilename));
            var gameReports = d["GameReports"];
            var reports = new List<GameReport>();
            foreach (var gameReportKey in gameReports)
            {
                foreach (var gameReport in gameReportKey)
                {
                    var report = new GameReport();
                    foreach (var nameValuePair in gameReport)
                    {
                        var fields = typeof(GameReport).GetFields(BindingFlags.Public | BindingFlags.Instance);
                        var field = fields.FirstOrDefault(f => f.Name == nameValuePair.Name);
                        if (field != null)
                            field.SetValue(report, nameValuePair.Value.Value);
                    }
                    reports.Add(report);
                }
            }
            return reports;
        }

        public MainWindow()
        {
            InitializeComponent();

            var inputFilename = @"C:\Users\steve\Desktop\README.airport.json";
            var reports = ReadReports(inputFilename);
        }
    }
}
