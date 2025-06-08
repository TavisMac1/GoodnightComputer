using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using System.Collections.Generic;

namespace GoodnightComputer
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            //this.FindControl<ContentControl>("MainContent").Content = new Schedule();

            this.FindControl<Button>("ScheduleButton").Click += ScheduleButton_Click;
        }

        private void ScheduleButton_Click(object? sender, RoutedEventArgs e)
        {
            this.FindControl<ContentControl>("MainContent").Content = new Schedule();
        }
    }
}