using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
using Client.Classes;
using Object = System.Object;

namespace Client.Pages
{
    /// <summary>
    /// Логика взаимодействия для Platform.xaml
    /// </summary>
    public partial class Platform : UserControl
    {
        public Platform()
        {
            InitializeComponent();
            CBType.ItemsSource = types.Keys;
            GShade.MouseDown += (s, e) => { OPShowHide(); };
        }


        #region Editor

        const int SHAnimationTime = 300;
        bool EIsOpen;
        int editmode = -1;
        Property iop;
        Script ios;
        Classes.Object ioo;
        Model iom;
        readonly Dictionary<string, TypeCode> types = new Dictionary<string, TypeCode> { { "Boolean", TypeCode.Boolean }, { "String", TypeCode.String }, { "Double", TypeCode.Double }, { "Integer", TypeCode.Int32 } };
        Dictionary<TypeCode, int> tid = new Dictionary<TypeCode, int> { { TypeCode.Boolean, 0 }, { TypeCode.String, 1 }, { TypeCode.Double, 2 }, { TypeCode.Int32, 3 } };

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            var name = TBName.Text.Trim(' ');
            if (!string.IsNullOrEmpty(name))
                switch (editmode)
                {
                    case 1:
                        Model model;
                        try
                        {
                            model = new Model(name);
                            model.Id = Classes.Platform.Current.Models.Count == 0 ? -1 : Classes.Platform.Current.Models.Min(i => i.Id) - 1;
                            //model = Network.IoTFactory.CreateModel(new Model(name));
                        }
                        catch
                        {
                            Message.Show((string)Application.Current.Resources["Dialogid9"] + (string)Application.Current.Resources["Viewid1"], (string)Application.Current.Resources["Dialogid5"]); return;
                        }
                        if (model == null)
                        {
                            Message.Show((string)Application.Current.Resources["Dialogid9"] + (string)Application.Current.Resources["Viewid1"], (string)Application.Current.Resources["Dialogid5"]); return;
                        }
                        Classes.Platform.Current.Models.Add(model);
                        break;
                    case 2:
                        Classes.Object obj;
                        try
                        {
                            obj = new Classes.Object(name, (long)(ELmodels.SelectedItem as Model).Id);
                            obj.Id = Classes.Platform.Current.Models.Count == 0 || Classes.Platform.Current.Models.SelectMany(x => x.Objects).Count() == 0 ? -1 : Classes.Platform.Current.Models.SelectMany(x => x.Objects).Min(i => i.Id) - 1;
                            //obj = Network.IoTFactory.CreateObject(new Object(name, (long)(Lmodels.SelectedItem as Model).id));
                        }
                        catch
                        {
                            Message.Show((string)Application.Current.Resources["Dialogid9"] + (string)Application.Current.Resources["Viewid2"], (string)Application.Current.Resources["Dialogid5"]); return;
                        }
                        if (obj == null)
                        {
                            Message.Show((string)Application.Current.Resources["Dialogid9"] + (string)Application.Current.Resources["Viewid2"], (string)Application.Current.Resources["Dialogid5"]); return;
                        }
                        (ELmodels.SelectedItem as Model)?.Objects.Add(obj);
                        break;
                    case 3:
                        if (CBType.SelectedItem != null)
                        {
                            TypeCode type = TypeCode.Object;
                            types.TryGetValue((CBType.SelectedItem as string), out type);
                            if (type != TypeCode.Object)
                            {
                                Property prop;
                                try
                                {
                                    prop = new Property(name, (long)(ELobjects.SelectedItem as Classes.Object).Id, (int)type, TBValue.Text);
                                    prop.Id = Classes.Platform.Current.Models.Count == 0 || Classes.Platform.Current.Models.SelectMany(x => x.Objects).Count() == 0 || Classes.Platform.Current.Models.SelectMany(x => x.Objects).SelectMany(y => y.Properties).Count() == 0 ? -1 : Classes.Platform.Current.Models.SelectMany(x => x.Objects).SelectMany(y => y.Properties).Min(i => i.Id) - 1;
                                    //prop = Network.IoTFactory.CreateProperty(new Property(name, (long)(Lobjects.SelectedItem as Object).id, (int)type, TBValue.Text));
                                }
                                catch
                                {
                                    Message.Show((string)Application.Current.Resources["Dialogid9"] + (string)Application.Current.Resources["Viewid2"], (string)Application.Current.Resources["Dialogid5"]); return;
                                }
                                if (prop == null)
                                {
                                    Message.Show((string)Application.Current.Resources["Dialogid9"] + (string)Application.Current.Resources["Viewid2"], (string)Application.Current.Resources["Dialogid5"]); return;
                                }
                                (ELobjects.SelectedItem as Classes.Object)?.Properties.Add(prop);

                            }
                        }
                        // MessageBox.Show((string)Application.Current.Resources["Errid2"]);
                        break;
                    case 4:
                        Script script;
                        try
                        {
                            script = new Script(name, (long)(ELproperties.SelectedItem as Property).Id, TBScript.Text);
                            script.Id = Classes.Platform.Current.Models.Count == 0 || Classes.Platform.Current.Models.SelectMany(x => x.Objects).Count() == 0 || Classes.Platform.Current.Models.SelectMany(x => x.Objects).SelectMany(y => y.Properties).Count() == 0 || Classes.Platform.Current.Models.SelectMany(x => x.Objects).SelectMany(y => y.Properties).SelectMany(z => z.Scripts).Count() == 0 ? -1 : Classes.Platform.Current.Models.SelectMany(x => x.Objects).SelectMany(y => y.Properties).SelectMany(z => z.Scripts).Min(i => i.Id) - 1;
                            //script = Network.IoTFactory.CreateScript(new Script(name, (long)(Lproperties.SelectedItem as Property).id, TBScript.Text));
                        }
                        catch
                        {
                            Message.Show((string)Application.Current.Resources["Dialogid9"] + (string)Application.Current.Resources["Viewid5"], (string)Application.Current.Resources["Dialogid5"]); return;
                        }
                        if (script == null)
                        {
                            Message.Show((string)Application.Current.Resources["Dialogid9"] + (string)Application.Current.Resources["Viewid5"], (string)Application.Current.Resources["Dialogid5"]); return;
                        }
                        (ELproperties.SelectedItem as Property)?.Scripts.Add(script);
                        break;
                    case 10:
                        if (iop != null)
                        {
                            iop.Name = name;
                            iop.Value = TBValue.Text;
                            //Network.IoTFactory.UpdateProperty(iop);//добавить проверку на 1/0, а лучше сделать другую модель одновления
                            int t = ELobjects.SelectedIndex;
                            ELobjects.SelectedIndex = -1;
                            ELobjects.SelectedItem = ELobjects.Items[t];
                        }
                        break;
                    case 11:
                        if (ios != null)
                        {
                            ios.Name = name;
                            ios.Value = TBScript.Text;
                            //Network.IoTFactory.UpdateScript(ios);
                            int t = ELproperties.SelectedIndex;
                            ELproperties.SelectedIndex = -1;
                            ELproperties.SelectedItem = ELproperties.Items[t];
                        }
                        break;
                    case 12:
                        if (ioo != null)
                        {
                            ioo.Name = name;
                            //Network.IoTFactory.UpdateObject(ioo);
                            int t = ELmodels.SelectedIndex;
                            ELmodels.SelectedIndex = -1;
                            ELmodels.SelectedItem = ELmodels.Items[t];
                        }
                        break;
                    case 13:
                        if (iom != null)
                        {
                            iom.Name = name;
                            try
                            {
                                int t = ELmodels.SelectedIndex == -1 ? 0 : ELmodels.SelectedIndex;
                                ELmodels.SelectedIndex = -1;
                                ELmodels.SelectedItem = ELmodels.Items[t];
                            }
                            catch { }
                            //Network.IoTFactory.UpdateModel(iom);
                        }
                        break;
                }
            TBName.Text = TBScript.Text = TBValue.Text = "";
            Tag = null;
            editmode = -1;
            //TODO Notified(null, null);
            OPShowHide();
        }

        private void button1_Click_1(object sender, RoutedEventArgs e)
        {
            if (EIsOpen) return;
            labelName.Visibility = Visibility.Visible;
            labelType.Visibility = Visibility.Hidden;
            labelValue.Visibility = Visibility.Hidden;
            labelScript.Visibility = Visibility.Hidden;
            TBName.Visibility = Visibility.Visible;
            TBValue.Visibility = Visibility.Hidden;
            TBScript.Visibility = Visibility.Hidden;
            CBType.Visibility = Visibility.Hidden;
            CBType.IsEnabled = false;
            editmode = 1;
            OPShowHide();
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            if (EIsOpen) return;
            labelName.Visibility = Visibility.Visible;
            labelType.Visibility = Visibility.Hidden;
            labelValue.Visibility = Visibility.Hidden;
            labelScript.Visibility = Visibility.Hidden;
            TBName.Visibility = Visibility.Visible;
            TBValue.Visibility = Visibility.Hidden;
            TBScript.Visibility = Visibility.Hidden;
            CBType.Visibility = Visibility.Hidden;
            CBType.IsEnabled = false;
            editmode = 2;
            OPShowHide();
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            if (EIsOpen) return;
            labelName.Visibility = Visibility.Visible;
            labelType.Visibility = Visibility.Visible;
            labelValue.Visibility = Visibility.Visible;
            labelScript.Visibility = Visibility.Hidden;
            TBName.Visibility = Visibility.Visible;
            TBValue.Visibility = Visibility.Visible;
            TBScript.Visibility = Visibility.Hidden;
            CBType.Visibility = Visibility.Visible;
            CBType.IsEnabled = true;
            editmode = 3;
            OPShowHide();
        }

        private void button4_Click(object sender, RoutedEventArgs e)
        {
            if (EIsOpen) return;
            labelName.Visibility = Visibility.Visible;
            labelType.Visibility = Visibility.Hidden;
            labelValue.Visibility = Visibility.Hidden;
            labelScript.Visibility = Visibility.Visible;
            TBName.Visibility = Visibility.Visible;
            TBValue.Visibility = Visibility.Hidden;
            TBScript.Visibility = Visibility.Visible;
            CBType.Visibility = Visibility.Hidden;
            editmode = 4;
            OPShowHide();
        }


        private async void OPShowHide()
        {

            BSave.IsEnabled = false;
            await Task.Run(async () =>
            {
                if (EIsOpen)
                {
                    await Dispatcher.BeginInvoke(new Action(delegate ()
                    {
                        GShade.Visibility = Visibility.Hidden;
                    }));
                    for (double i = 0; i >= -290; i -= 4)
                    {
                        Thread.Sleep(SHAnimationTime * 4 / 290);
                        await Dispatcher.BeginInvoke(new Action(delegate ()
                        {
                            editOptions.Margin = new Thickness(0, 0, i, 0);
                        }));
                    }
                }
                else
                {
                    await Dispatcher.BeginInvoke(new Action(delegate ()
                    {
                        GShade.Visibility = Visibility.Visible;
                    }));
                    for (double i = -290; i <= 0; i += 4)
                    {
                        Thread.Sleep(SHAnimationTime * 4 / 290);
                        await Dispatcher.BeginInvoke(new Action(delegate ()
                        {
                            editOptions.Margin = new Thickness(0, 0, i, 0);
                        }));
                    }
                }

            });
            EIsOpen = editOptions.Margin.Right > -100;
            BSave.IsEnabled = true;
        }

        private void EditEvent(object sender, RoutedEventArgs e)
        {
            if (EIsOpen) return;
            if ((sender as Button).Tag is Property)
            {
                labelName.Visibility = Visibility.Visible;
                labelType.Visibility = Visibility.Hidden;
                labelValue.Visibility = Visibility.Visible;
                labelScript.Visibility = Visibility.Hidden;
                TBName.Visibility = Visibility.Visible;
                TBName.Text = ((sender as Button).Tag as Property).Name;
                TBValue.Visibility = Visibility.Visible;
                TBValue.Text = ((sender as Button).Tag as Property).Value;
                TBScript.Visibility = Visibility.Hidden;
                CBType.Visibility = Visibility.Hidden;
                CBType.SelectedItem = ((sender as Button).Tag as Property).Type;
                CBType.IsEnabled = false;
                ELproperties.SelectedItem = (sender as Button).Tag;
                editmode = 10;
                iop = ((sender as Button).Tag as Property);
                OPShowHide();
            }
            if ((sender as Button).Tag is Script)
            {
                labelName.Visibility = Visibility.Visible;
                labelType.Visibility = Visibility.Hidden;
                labelValue.Visibility = Visibility.Hidden;
                labelScript.Visibility = Visibility.Visible;
                TBName.Visibility = Visibility.Visible;
                TBName.Text = ((sender as Button).Tag as Script).Name;
                TBValue.Visibility = Visibility.Hidden;
                TBScript.Visibility = Visibility.Visible;
                TBScript.Text = ((sender as Button).Tag as Script).Value;
                CBType.Visibility = Visibility.Hidden;
                CBType.IsEnabled = false;
                ELscripts.SelectedItem = (sender as Button).Tag;
                editmode = 11;
                ios = ((sender as Button).Tag as Script);
                OPShowHide();
            }
            if ((sender as Button).Tag is Classes.Object)
            {
                labelName.Visibility = Visibility.Visible;
                labelType.Visibility = Visibility.Hidden;
                labelValue.Visibility = Visibility.Hidden;
                labelScript.Visibility = Visibility.Hidden;
                TBName.Visibility = Visibility.Visible;
                TBName.Text = ((sender as Button).Tag as Classes.Object).Name;
                TBValue.Visibility = Visibility.Hidden;
                TBScript.Visibility = Visibility.Hidden;
                CBType.Visibility = Visibility.Hidden;
                CBType.IsEnabled = false;
                ELobjects.SelectedItem = (sender as Button).Tag;
                editmode = 12;
                ioo = ((sender as Button).Tag as Classes.Object);
                OPShowHide();
            }
            if ((sender as Button).Tag is Model)
            {
                labelName.Visibility = Visibility.Visible;
                labelType.Visibility = Visibility.Hidden;
                labelValue.Visibility = Visibility.Hidden;
                labelScript.Visibility = Visibility.Hidden;
                TBName.Visibility = Visibility.Visible;
                TBName.Text = ((sender as Button).Tag as Model).Name;
                TBValue.Visibility = Visibility.Hidden;
                TBScript.Visibility = Visibility.Hidden;
                CBType.Visibility = Visibility.Hidden;
                CBType.IsEnabled = false;
                ELobjects.SelectedItem = (sender as Button).Tag;
                editmode = 12;
                iom = ((sender as Button).Tag as Model);
                OPShowHide();
            }
        }
        private void DeleteEvent(object sender, RoutedEventArgs e)
        {
            if (!(sender is Button)) return;
            if (((Button)sender).Tag is Model)
            {
                if ((bool)Message.Show((string)Application.Current.Resources["Dialogid1"], (string)Application.Current.Resources["Dialogid3"], true))
                {
                    //if(Network.IoTFactory.DeleteModel((sender as Button).Tag as Model))
                    Classes.Platform.Current.Models.Remove(((Button)sender).Tag as Model);
                }
            }
            if (((Button)sender).Tag is Classes.Object)
            {
                ELobjects.SelectedItem = ((Button)sender).Tag;
                if (ELmodels.SelectedItem != null)
                    if ((bool)Message.Show((string)Application.Current.Resources["Dialogid1"], (string)Application.Current.Resources["Dialogid3"], true))
                    {
                        //if (Network.IoTFactory.DeleteObject((sender as Button).Tag as Object))
                        (ELmodels.SelectedItem as Model).Objects.Remove((sender as Button).Tag as Classes.Object);
                    }
            }
            if (((Button)sender).Tag is Property)
            {
                ELproperties.SelectedItem = ((Button)sender).Tag;
                if (ELobjects.SelectedItem != null)
                    if ((bool)Message.Show((string)Application.Current.Resources["Dialogid1"], (string)Application.Current.Resources["Dialogid3"], true))
                    {
                        //if (Network.IoTFactory.DeleteProperty((sender as Button).Tag as Property))
                        (ELobjects.SelectedItem as Classes.Object)?.Properties.Remove(((Button)sender).Tag as Property);
                    }
            }
            if (((Button)sender).Tag is Script)
            {
                ELscripts.SelectedItem = ((Button)sender).Tag;
                if (ELproperties.SelectedItem != null)
                    if ((bool)Message.Show((string)Application.Current.Resources["Dialogid1"], (string)Application.Current.Resources["Dialogid3"], true))
                    {
                        //if (Network.IoTFactory.DeleteScript((sender as Button).Tag as Script))
                        (ELproperties.SelectedItem as Property).Scripts.Remove((sender as Button).Tag as Script);
                    }
            }
            Main.GetMainWindow().PMonitoring.Notified(null, null);
        }

        private void CopyEvent(object sender, RoutedEventArgs e)
        {
            if ((sender as Button) == null) return;
            if (((Button)sender).Tag is Model)
            {
                Model model;
                try
                {
                    model = new Model((sender as Button).Tag as Model)
                    {
                        Id = !Classes.Platform.Current.Models.Any() ? -1 : Classes.Platform.Current.Models.Min(i => i.Id) - 1
                    };
                    //model = Network.IoTFactory.CreateModel(new Model((sender as Button).Tag as Model));
                }
                catch
                {
                    Message.Show((string)Application.Current.Resources["Dialogid9"] + (string)Application.Current.Resources["Viewid1"], (string)Application.Current.Resources["Dialogid5"]); return;
                }
                if (model == null)
                {
                    Message.Show((string)Application.Current.Resources["Dialogid9"] + (string)Application.Current.Resources["Viewid1"], (string)Application.Current.Resources["Dialogid5"]); return;
                }
                Classes.Platform.Current.Models.Add(model);
            }
            if (((Button)sender).Tag is Classes.Object)
            {
                if (ELmodels.SelectedItem != null)
                {
                    Classes.Object obj;
                    try
                    {
                        obj = new Classes.Object(((Button)sender).Tag as Classes.Object)
                        {
                            Id =
                                !Classes.Platform.Current.Models.SelectMany(x => x.Objects).Any()
                                    ? -1
                                    : Classes.Platform.Current.Models.SelectMany(x => x.Objects).Min(i => i.Id) - 1
                        };
                        //obj = Network.IoTFactory.CreateObject(new Object((sender as Button).Tag as Object));
                    }
                    catch
                    {
                        Message.Show((string)Application.Current.Resources["Dialogid9"] + (string)Application.Current.Resources["Viewid2"], (string)Application.Current.Resources["Dialogid5"]); return;
                    }
                    if (obj == null)
                    {
                        Message.Show((string)Application.Current.Resources["Dialogid9"] + (string)Application.Current.Resources["Viewid2"], (string)Application.Current.Resources["Dialogid5"]); return;
                    }
                        (ELmodels.SelectedItem as Model)?.Objects.Add(obj);
                }
            }
            if (((Button)sender).Tag is Property)
            {
                if (ELobjects.SelectedItem != null)
                {
                    Property prop;
                    try
                    {
                        prop = new Property(((Button)sender).Tag as Property)
                        {
                            Id =
                                !Classes.Platform.Current.Models.SelectMany(x => x.Objects).SelectMany(y => y.Properties).Any()
                                    ? -1
                                    : Classes.Platform.Current.Models.SelectMany(x => x.Objects)
                                          .SelectMany(y => y.Properties)
                                          .Min(i => i.Id) - 1
                        };
                        //prop = Network.IoTFactory.CreateProperty(new Property((sender as Button).Tag as Property));
                    }
                    catch
                    {
                        Message.Show((string)Application.Current.Resources["Dialogid9"] + (string)Application.Current.Resources["Viewid2"], (string)Application.Current.Resources["Dialogid5"]); return;
                    }
                    if (prop == null)
                    {
                        Message.Show((string)Application.Current.Resources["Dialogid9"] + (string)Application.Current.Resources["Viewid2"], (string)Application.Current.Resources["Dialogid5"]); return;
                    }
                                (ELobjects.SelectedItem as Classes.Object)?.Properties.Add(prop);
                }
            }
            if (((Button)sender).Tag is Script)
            {
                if (ELproperties.SelectedItem != null)
                {
                    Script script;
                    try
                    {
                        script = new Script((sender as Button).Tag as Script)
                        {
                            Id =
                                !Classes.Platform.Current.Models.SelectMany(x => x.Objects)
                                    .SelectMany(y => y.Properties)
                                    .SelectMany(z => z.Scripts)
                                    .Any()
                                    ? -1
                                    : Classes.Platform.Current.Models.SelectMany(x => x.Objects)
                                          .SelectMany(y => y.Properties)
                                          .SelectMany(z => z.Scripts)
                                          .Min(i => i.Id) - 1
                        };
                        //script = Network.IoTFactory.CreateScript(new Script((sender as Button).Tag as Script));
                    }
                    catch
                    {
                        Message.Show((string)Application.Current.Resources["Dialogid9"] + (string)Application.Current.Resources["Viewid5"], (string)Application.Current.Resources["Dialogid5"]);
                        return;
                    }
                    if (script == null)
                    {
                        Message.Show((string)Application.Current.Resources["Dialogid9"] + (string)Application.Current.Resources["Viewid5"], (string)Application.Current.Resources["Dialogid5"]);
                        return;
                    }
                        (ELproperties.SelectedItem as Property)?.Scripts.Add(script);
                }
            }
        }

        #endregion


    }
}
