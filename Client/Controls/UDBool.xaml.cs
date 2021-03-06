﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Client.Controls
{
    /// <summary>
    /// Логика взаимодействия для UDBool.xaml
    /// </summary>
    public partial class UDBool : UserControl
    {
        public UDBool()
        {
            InitializeComponent();
            BChart.Click += (s, e) => { pop.IsOpen = true; };
        }

        private void BExit_Click(object sender, RoutedEventArgs e)
        {
            pop.IsOpen = false;
        }
    }
}
