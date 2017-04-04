﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Security.Cryptography;
using System.Security.RightsManagement;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

// ReSharper disable PossibleNullReferenceException

namespace Client.Classes
{
    internal static class Controller
    {
        private const string Key = "Q1e3@3e344";
        private static readonly byte[] Salt = { 0x15, 0xdc, 0xf5, 0x40, 0xad, 0x5d, 0x7a, 0x0e, 0xc5, 0xae, 0x89, 0xaf, 0x4d, 0xaa, 0xc2, 0x3c };

        public static void Close()
        {
            Settings.Save();
            Application.Current.Shutdown();

        }

        public static void SaveToFile(string data)
        {
            if (Key.Length == 0) return;
            using (var myAes = Aes.Create())
            {
                var pdb = new Rfc2898DeriveBytes(Key, Salt);
                myAes.Key = pdb.GetBytes(32);
                myAes.IV = pdb.GetBytes(16);
                using (StreamWriter sw = new StreamWriter("user.data", false, Encoding.Default))
                {
                    sw.Write(Encoding.Default.GetString(EncryptStringToBytesAes(data, myAes.Key, myAes.IV)));
                }
            }
        }

        public static string LoadFromFile()
        {
            if (!File.Exists("user.data")) return null;
            using (var myAes = Aes.Create())
            {
                var pdb = new Rfc2898DeriveBytes(Key, Salt);
                myAes.Key = pdb.GetBytes(32);
                myAes.IV = pdb.GetBytes(16);
                byte[] data;
                using (StreamReader sr = new StreamReader("user.data", Encoding.Default))
                {
                    data = Encoding.Default.GetBytes(sr.ReadToEnd());
                }
                return DecryptStringFromBytesAes(data, myAes.Key, myAes.IV);
            }
        }

        public static void GenerateTestData()
        {
            //var scripts = new ObservableCollection<Script>()
            //{
            //    new Script(-1,"Name","Name","Some code",-1),
            //    new Script(-2,"Name1","Name1","Some other code",-1)
            //};
            //var props = new ObservableCollection<Property>()
            //{
            //    new Property(-1,"LED","LED",(int)TypeCode.Boolean,-1,scripts,"false"),
            //    new Property(-2,"XPosition","XPosition",(int)TypeCode.Int32,-1,null,"0"),
            //    new Property(-3,"ButtonState","ButtonState",(int)TypeCode.Boolean,-1,null,"false")
            //};

            //var obj = new ObservableCollection<Object> { new Object(-1, "TestObject", "TestObject", -1, props) };

            //var model = new Model(-1, "TestModel", "TestModel", obj);
            //Platform.Current.Models.Add(model);

            var props = new ObservableCollection<Property>()
            {
                new Property(0,"Terminator","Автоматика",(int)TypeCode.Boolean,-1,null,"false"),
                new Property(-1,"VentState","Окно",(int)TypeCode.Boolean,-1,null,"false"),
                new Property(-2,"Temperature","Температура",(int)TypeCode.Double,-1,null,"0"),
                new Property(-3,"Brightness","Освещенность",(int)TypeCode.Double,-1,null,"0"),
               // new Property(-4,"PumpState","Насос",(int)TypeCode.Boolean,-1,null,"false"),
                new Property(-4,"Humidity","Влажность",(int)TypeCode.Double,-1,null,"0"),
                new Property(-5,"LampBrightness","Лампа",(int)TypeCode.Double,-1,null,"0"),
            };

            var obj = new ObservableCollection<Object> { new Object(-1, "Greenhouse", "Теплица", -1, props) };

            var model = new Model(-1, "Greenhouse farming", "Тепличная ферма", obj);
            Platform.Current.Models.Add(model);

            List<Dashboard.PropertyMap> pm=new List<Dashboard.PropertyMap>
            {
                new Dashboard.PropertyMap(-1,props[0],-1,true,null,null),
                new Dashboard.PropertyMap(-2,props[1],-1,false,null,null),
                new Dashboard.PropertyMap(-3,props[2],-1,false,0.0,100.0),
                new Dashboard.PropertyMap(-4,props[3],-1,false,0.0,5600),
                //new Dashboard.PropertyMap(-5,props[4],-1,true,null,null),
                new Dashboard.PropertyMap(-5,props[4],-1,false,0,1024),
                new Dashboard.PropertyMap(-6,props[5],-1,false,0,255),
            }; 

            Client.Current.Dashboards.Add(new Dashboard (-1,-1,pm));
        }


        static byte[] EncryptStringToBytesAes(string plainText, byte[] key, byte[] iv)
        {
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException(nameof(plainText));
            if (key == null || key.Length <= 0)
                throw new ArgumentNullException(nameof(key));
            if (iv == null || iv.Length <= 0)
                throw new ArgumentNullException(nameof(iv));
            byte[] encrypted;
            // Создаем объект класса AES
            // с определенным ключом и IV.
            using (var aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                aesAlg.IV = iv;
                // Создаем объект, который определяет основные операции преобразований.
                var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
                // Создаем поток для шифрования.
                using (var msEncrypt = new MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Записываем в поток все данные.
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }
            //Возвращаем зашифрованные байты из потока памяти.
            return encrypted;

        }

        static string DecryptStringFromBytesAes(byte[] cipherText, byte[] key, byte[] iv)
        {
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException(nameof(cipherText));
            if (key == null || key.Length <= 0)
                throw new ArgumentNullException(nameof(key));
            if (iv == null || iv.Length <= 0)
                throw new ArgumentNullException(nameof(iv));
            string plaintext;
            // Создаем объект класса AES,
            // Ключ и IV
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                aesAlg.IV = iv;
                // Создаем объект, который определяет основные операции преобразований.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                // Создаем поток для расшифрования.
                using (var msDecrypt = new MemoryStream(cipherText))
                {
                    using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (var srDecrypt = new StreamReader(csDecrypt))
                        {
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }

            }
            return plaintext;
        }


        #region LocalWork

        public static SerialPort CurrentPort = SerialPort.GetPortNames().FirstOrDefault() != null ? new SerialPort(SerialPort.GetPortNames().FirstOrDefault(), 115200) : null;
        public static volatile bool Checker = true;

        public static async void TryReadFromPortAsync()
        {
            await Task.Run(async () =>
            {
                Thread.Sleep(500);
                Checker = true;
                CurrentPort.DtrEnable = true;
                //currentPort.ReadTimeout = 2500;
                CurrentPort.Open();
                while (Checker)
                {
                    try
                    {
                        string line = CurrentPort.ReadLine();
                        string[] data = line.Split(';');

                        await Main.GetMainWindow().Dispatcher.BeginInvoke(new Action(delegate ()
                        {
                            Platform.Current.Models.FirstOrDefault().Objects.FirstOrDefault().Properties[2].Value =
                                data[0];
                            Platform.Current.Models.FirstOrDefault().Objects.FirstOrDefault().Properties[3].Value =
                                data[1];
                            Platform.Current.Models.FirstOrDefault().Objects.FirstOrDefault().Properties[4].Value =
                                data[2];

                            Platform.Current.Models.FirstOrDefault().Objects.FirstOrDefault().Properties[1].Value =
                                (data[3]=="1").ToString();
                            Platform.Current.Models.FirstOrDefault().Objects.FirstOrDefault().Properties[5].Value =
                                data[4];
                        }));
                        Thread.Sleep(500);
                        try
                        {
                            CurrentPort?.WriteLine((bool.Parse(Platform.Current.Models.FirstOrDefault()?.Objects.FirstOrDefault()?.Properties[1].Value) ? "venton;" : "ventoff;") + (bool.Parse(Platform.Current.Models.FirstOrDefault()?.Objects.FirstOrDefault()?.Properties[0].Value) ? "autoon;" : "autooff;") + ("led=" + Math.Abs((int)double.Parse(Platform.Current.Models.FirstOrDefault()?.Objects.FirstOrDefault()?.Properties[5].Value.Replace('.', ','))) % 256));
                        }
                        catch
                        {
                        }
                        Network.IsUpdated = false;

                        Thread.Sleep(500);
                    }
                    catch (TimeoutException) { }
                }
                CurrentPort.Close();

            });
        }

        public static void SendVentState()
        {
            try
            {
                CurrentPort?.WriteLine(bool.Parse(Platform.Current.Models.FirstOrDefault()?.Objects.FirstOrDefault()?.Properties[1].Value) ? "venton" : "ventoff");
            }
            catch { }
        }

        public static void SendPumpState()
        {
            try
            {
                CurrentPort?.WriteLine(bool.Parse(Platform.Current.Models.FirstOrDefault()?.Objects.FirstOrDefault()?.Properties[4].Value) ? "pumpon" : "pumpoff");
            }
            catch { }
        }

        public static void SendLampState()
        {
            try
            {
                CurrentPort?.WriteLine("led="+ Math.Abs((int)double.Parse(Platform.Current.Models.FirstOrDefault()?.Objects.FirstOrDefault()?.Properties[5].Value.Replace('.', ',')))%256);
            }
            catch { }
        }

        public static void Terminate()
        {
            try
            {
                CurrentPort?.WriteLine(bool.Parse(Platform.Current.Models.FirstOrDefault()?.Objects.FirstOrDefault()?.Properties[0].Value)?"autoon":"autooff");
            }
            catch { }
        }

        #endregion
    }

    public enum TransliterationType
    {
        Gost,
        // ReSharper disable once InconsistentNaming
        ISO
    }
    public static class Transliteration
    {
        private static Dictionary<string, string> gost = new Dictionary<string, string>(); //ГОСТ 16876-71
        private static Dictionary<string, string> iso = new Dictionary<string, string>(); //ISO 9-95
        public static string Front(string text)
        {
            return Front(text, TransliterationType.ISO);
        }
        public static string Front(string text, TransliterationType type)
        {
            string output = text;

            output = Regex.Replace(output, @"\s|\.|\(", " ");
            output = Regex.Replace(output, @"\s+", " ");
            output = Regex.Replace(output, @"[^\s\w\d-]", "");
            output = output.Trim();

            Dictionary<string, string> tdict = GetDictionaryByType(type);

            foreach (KeyValuePair<string, string> key in tdict)
            {
                output = output.Replace(key.Key, key.Value);
            }
            return output;
        }
        public static string Back(string text)
        {
            return Back(text, TransliterationType.ISO);
        }
        public static string Back(string text, TransliterationType type)
        {
            string output = text;
            Dictionary<string, string> tdict = GetDictionaryByType(type);

            foreach (KeyValuePair<string, string> key in tdict)
            {
                output = output.Replace(key.Value, key.Key);
            }
            return output;
        }

        private static Dictionary<string, string> GetDictionaryByType(TransliterationType type)
        {
            Dictionary<string, string> tdict = iso;
            if (type == TransliterationType.Gost) tdict = gost;
            return tdict;
        }

        static Transliteration()
        {
            gost.Add("Є", "EH");
            gost.Add("І", "I");
            gost.Add("і", "i");
            gost.Add("№", "#");
            gost.Add("є", "eh");
            gost.Add("А", "A");
            gost.Add("Б", "B");
            gost.Add("В", "V");
            gost.Add("Г", "G");
            gost.Add("Д", "D");
            gost.Add("Е", "E");
            gost.Add("Ё", "JO");
            gost.Add("Ж", "ZH");
            gost.Add("З", "Z");
            gost.Add("И", "I");
            gost.Add("Й", "JJ");
            gost.Add("К", "K");
            gost.Add("Л", "L");
            gost.Add("М", "M");
            gost.Add("Н", "N");
            gost.Add("О", "O");
            gost.Add("П", "P");
            gost.Add("Р", "R");
            gost.Add("С", "S");
            gost.Add("Т", "T");
            gost.Add("У", "U");
            gost.Add("Ф", "F");
            gost.Add("Х", "KH");
            gost.Add("Ц", "C");
            gost.Add("Ч", "CH");
            gost.Add("Ш", "SH");
            gost.Add("Щ", "SHH");
            gost.Add("Ъ", "'");
            gost.Add("Ы", "Y");
            gost.Add("Ь", "");
            gost.Add("Э", "EH");
            gost.Add("Ю", "YU");
            gost.Add("Я", "YA");
            gost.Add("а", "a");
            gost.Add("б", "b");
            gost.Add("в", "v");
            gost.Add("г", "g");
            gost.Add("д", "d");
            gost.Add("е", "e");
            gost.Add("ё", "jo");
            gost.Add("ж", "zh");
            gost.Add("з", "z");
            gost.Add("и", "i");
            gost.Add("й", "jj");
            gost.Add("к", "k");
            gost.Add("л", "l");
            gost.Add("м", "m");
            gost.Add("н", "n");
            gost.Add("о", "o");
            gost.Add("п", "p");
            gost.Add("р", "r");
            gost.Add("с", "s");
            gost.Add("т", "t");
            gost.Add("у", "u");

            gost.Add("ф", "f");
            gost.Add("х", "kh");
            gost.Add("ц", "c");
            gost.Add("ч", "ch");
            gost.Add("ш", "sh");
            gost.Add("щ", "shh");
            gost.Add("ъ", "");
            gost.Add("ы", "y");
            gost.Add("ь", "");
            gost.Add("э", "eh");
            gost.Add("ю", "yu");
            gost.Add("я", "ya");
            gost.Add("«", "");
            gost.Add("»", "");
            gost.Add("—", "-");
            gost.Add(" ", "-");

            iso.Add("Є", "YE");
            iso.Add("І", "I");
            iso.Add("Ѓ", "G");
            iso.Add("і", "i");
            iso.Add("№", "#");
            iso.Add("є", "ye");
            iso.Add("ѓ", "g");
            iso.Add("А", "A");
            iso.Add("Б", "B");
            iso.Add("В", "V");
            iso.Add("Г", "G");
            iso.Add("Д", "D");
            iso.Add("Е", "E");
            iso.Add("Ё", "YO");
            iso.Add("Ж", "ZH");
            iso.Add("З", "Z");
            iso.Add("И", "I");
            iso.Add("Й", "J");
            iso.Add("К", "K");
            iso.Add("Л", "L");
            iso.Add("М", "M");
            iso.Add("Н", "N");
            iso.Add("О", "O");
            iso.Add("П", "P");
            iso.Add("Р", "R");
            iso.Add("С", "S");
            iso.Add("Т", "T");
            iso.Add("У", "U");
            iso.Add("Ф", "F");
            iso.Add("Х", "X");
            iso.Add("Ц", "C");
            iso.Add("Ч", "CH");
            iso.Add("Ш", "SH");
            iso.Add("Щ", "SHH");
            iso.Add("Ъ", "'");
            iso.Add("Ы", "Y");
            iso.Add("Ь", "");
            iso.Add("Э", "E");
            iso.Add("Ю", "YU");
            iso.Add("Я", "YA");
            iso.Add("а", "a");
            iso.Add("б", "b");
            iso.Add("в", "v");
            iso.Add("г", "g");
            iso.Add("д", "d");
            iso.Add("е", "e");
            iso.Add("ё", "yo");
            iso.Add("ж", "zh");
            iso.Add("з", "z");
            iso.Add("и", "i");
            iso.Add("й", "j");
            iso.Add("к", "k");
            iso.Add("л", "l");
            iso.Add("м", "m");
            iso.Add("н", "n");
            iso.Add("о", "o");
            iso.Add("п", "p");
            iso.Add("р", "r");
            iso.Add("с", "s");
            iso.Add("т", "t");
            iso.Add("у", "u");
            iso.Add("ф", "f");
            iso.Add("х", "x");
            iso.Add("ц", "c");
            iso.Add("ч", "ch");
            iso.Add("ш", "sh");
            iso.Add("щ", "shh");
            iso.Add("ъ", "");
            iso.Add("ы", "y");
            iso.Add("ь", "");
            iso.Add("э", "e");
            iso.Add("ю", "yu");
            iso.Add("я", "ya");
            iso.Add("«", "");
            iso.Add("»", "");
            iso.Add("—", "-");
            iso.Add(" ", "-");
        }
    }
}
