using System;
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
using System.Drawing;
using System.IO;
using System.Collections;
using System.Drawing.Imaging;

namespace hideimg
{
    public partial class MainWindow : Window
    {
        private string sciezka;
        private BitmapImage zdjecie;
        private Bitmap bitmap;

        public static string bindostring(string binText)
        {
            BitArray bit = new BitArray(binText.Length);
            for (int i = 0; i < binText.Length; i++)
            {
                if (binText.Substring(i, 1) == "1") bit[i] = true;
                else
                    bit[i] = false;
            }
            int numBytes = bit.Count / 8;
            if (bit.Count % 8 != 0) numBytes++;
            byte[] bytes = new byte[numBytes];
            int byteIndex = 0, bitIndex = 0;

            for (int i = 0; i < bit.Count; i++)
            {
                if (bit[i]) bytes[byteIndex] |= (byte)(1 << (7 - bitIndex));
                bitIndex++;
                if (bitIndex == 8)
                {
                    bitIndex = 0;
                    byteIndex++;
                }
            }

            return System.Text.Encoding.UTF8.GetString(bytes);
        }

        public static string stringdobin(string strText)
        {
            byte[] btText;
            btText = System.Text.Encoding.UTF8.GetBytes(strText);
            Array.Reverse(btText);
            BitArray bit = new BitArray(btText);
            StringBuilder sb = new StringBuilder();

            for (int i = bit.Length - 1; i >= 0; i--)
            {
                if (bit[i] == true)
                {
                    sb.Append(1);
                }
                else
                {
                    sb.Append(0);
                }
            }

            return sb.ToString();
        }

        void deukryj(){
            string msg="";
            int count = 0;
                for (int i = 0; i < bitmap.Height; i++){
                    for (int a = 0; a < bitmap.Width; a++){
                        
                        string r, g, b;                        

                        System.Drawing.Color pixel;
                        pixel = bitmap.GetPixel(a,i);
                        r = dajbity(pixel.R.ToString());
                        if (r[r.Length-1] == '0') count++; else count = 0;
                        g = dajbity(pixel.G.ToString());
                        if (g[g.Length-1] == '0') count++; else count = 0;
                        b = dajbity(pixel.B.ToString());
                        if (b[b.Length-1] == '0') count++; else count = 0;
                        if (count >= 8) break;
                        
                        msg += (r[r.Length-1].ToString() + g[g.Length-1].ToString() + b[b.Length-1].ToString());
                    }                   
                }

                tekst.Text = bindostring(msg);
                MessageBox.Show("Odszyfrowano tekst ukryty w obrazku");            
        }

        public String bajtnabity(Byte[] data){
            return string.Join("", data.Select(byt =>Convert.ToString(byt, 2).PadLeft(8, '0')));
        }

        string dajbity(string txt)
        {
        byte[] bytes = Encoding.GetEncoding("windows-1250").GetBytes(txt);
        return bajtnabity(bytes);
        }

        void ukryj(string tekst2){
            
            string wiadomosc = stringdobin(tekst2);
            int ile = wiadomosc.Length;
            int index = 0;
            Bitmap nowy = bitmap;
            for (int i = 0; i < bitmap.Height; i++){
                for (int a = 0; a < bitmap.Width; a++){
                    System.Drawing.Color pixel;
                    pixel = bitmap.GetPixel(a,i);
                    string r, g, b;
                    r = dajbity(pixel.R.ToString());
                    StringBuilder pomoc = new StringBuilder(r);
                    if (index < ile){
                        pomoc[r.Length - 1] = wiadomosc[index];
                        
                        index++;
                    }
                    else{
                        pomoc[r.Length - 1] = '0';
                    }
                    r = pomoc.ToString();

                    g = dajbity(pixel.G.ToString());
                    StringBuilder pomoc2 = new StringBuilder(g);
                    if (index < ile){
                        pomoc2[g.Length - 1] = wiadomosc[index];
                        
                        index++;
                    }
                    else{
                        pomoc2[g.Length - 1] = '0';
                    }
                    g = pomoc2.ToString();

                    b = dajbity(pixel.B.ToString());
                    StringBuilder pomoc3 = new StringBuilder(b);
                    if (index < ile){
                        pomoc3[b.Length - 1] = wiadomosc[index];
                        index++;
                    }
                    else{
                        pomoc3[b.Length - 1] = '0';
                    }
                    b = pomoc3.ToString();
                    System.Drawing.Color pixel2 = System.Drawing.Color.FromArgb(int.Parse(bindostring(r)), int.Parse(bindostring(g)), int.Parse(bindostring(b)));
                    nowy.SetPixel(a,i, pixel2);
                }
            }
            System.Drawing.Image nowy2 = (System.Drawing.Image)nowy;
            nowy2.Save("ukryty.png", ImageFormat.Png);
            if (nowy.Width * nowy.Height * 3 < wiadomosc.Length) MessageBox.Show("Obrazek był za mały, aby ukryć cały tekst, ukryto, ile się da.");
            
            MessageBox.Show("Zaszyfrowano do pliku ukryty.png");            
        }

        public MainWindow()
        {
            InitializeComponent();
        }

        private Bitmap dobmp(BitmapImage zdjecie2)
        {
            using(MemoryStream outStream = new MemoryStream()){
            BitmapEncoder enc =new BmpBitmapEncoder();
            enc.Frames.Add(BitmapFrame.Create(zdjecie2));
            enc.Save(outStream);
            System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(outStream);
            return new Bitmap(bitmap);
        }
        }

        private void zaladuj_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".jpg";
            dlg.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png, *.bmp) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png; *.bmp";
            var result = dlg.ShowDialog();
            if (result == true) {
                try { zdjecie = new BitmapImage();
                    sciezka = dlg.FileName;
                    zdjecie.BeginInit();
                    zdjecie.UriSource = new Uri(sciezka);
                    zdjecie.EndInit();
                    image.Source = zdjecie;
                    bitmap = dobmp(zdjecie);
                } catch (Exception ex) {
                    MessageBox.Show(ex.Message); } }
        }

        private void szyfruj_Click(object sender, RoutedEventArgs e)
        {
            if (zdjecie == null || tekst.Text == "") MessageBox.Show("Najpierw wpisz tekst oraz wybierz zdjęcie.");
            else
            {
                string tekst2 = tekst.Text;
                ukryj(tekst2);
            }            
        }

        private void deszyfruj_Click(object sender, RoutedEventArgs e)
        {
            if (zdjecie == null) MessageBox.Show("Najpierw wybierz zdjęcie.");
            else
            {
                deukryj();
            }
        }

        private void pomoc_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("Aby ukryć tekst, wybierz obraz, wpisz tekst i kliknij przycisk Ukryj.\n\nAby odczytać tekst, wybierz obraz z ukrytym tekstem, a następnie kliknij Odczytaj\n\nNie można szyfrować pliku ukryty.png");
        }
    }
}
