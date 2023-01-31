using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.IO;
using System.Threading;
using System.Collections;

namespace VTYS_Proje
{
    public partial class Form1 : Form
    {
        // VARIABLES
        Boolean girisYapildi = false;
        int sayfaIndex = 1;
        String secilmisUrun;
        String  suankiKategori = null;
        List<Panel> panelList = new List<Panel>();
        SqlConnection baglanti = new SqlConnection("Data Source=SAMET\\SQLEXPRESS;Initial Catalog=Flust;Integrated Security=True");
        #region Methodlar
        private void moveSelectedPNL(Button selectedButton)
        {
            PNL_selectedCategory.Location = new Point(PNL_selectedCategory.Location.X, selectedButton.Location.Y);
        }
        private void kayitOlustur(String isim, String soyIsim, String telNo, String eposta, String parola)
        {
            if(baglanti.State == ConnectionState.Closed) baglanti.Open();
            String komut = "INSERT INTO uyeler (isim,soyisim,telefon,eposta,parola) VALUES (@isim,@soyIsim,@telNo,@eposta,@parola)";
            SqlCommand kayitYap = new SqlCommand(komut, baglanti);
            kayitYap.Parameters.AddWithValue("@isim", isim);
            kayitYap.Parameters.AddWithValue("@soyIsim", soyIsim);
            kayitYap.Parameters.AddWithValue("@telNo", telNo);
            kayitYap.Parameters.AddWithValue("@eposta", eposta);
            kayitYap.Parameters.AddWithValue("@parola", parola);
            kayitYap.ExecuteNonQuery();
            MessageBox.Show("Kayıt olundu", "Kayıt Sonucu:", MessageBoxButtons.OK, MessageBoxIcon.Information);
            TCL_girisVeKayit.SelectTab(0);
            baglanti.Close();

        }

        private void girisYap(String eposta, String sifre)
        {
            if(baglanti.State == ConnectionState.Closed) baglanti.Open();
            
            String komut = "SELECT * FROM uyeler";
            SqlCommand girisYap = new SqlCommand(komut, baglanti);
            SqlDataReader read = girisYap.ExecuteReader();
            while (read.Read())
            {
                if (eposta == read["eposta"].ToString())
                {
                    if (sifre == read["parola"].ToString())
                    {
                        MessageBox.Show("Giriş Başarılı!", "Giriş sonucu:", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        girisYapildi = true;
                        BTN_cikisYap.Visible = true;
                        PNL_girisVeKayit.Visible = false;
                        BTN_girisVeKayit.Text = read["isim"].ToString() + " " + read["soyisim"].ToString();
                        TBX_hesabimIsim.Text = read["isim"].ToString();
                        TBX_hesabimSoyisim.Text = read["soyisim"].ToString();
                        TBX_hesabimTelno.Text = read["telefon"].ToString();
                        TBX_hesabimPosta.Text = read["eposta"].ToString();
                        TBX_hesabimSifre.Text = read["parola"].ToString();
                        break;
                    }else {
                    MessageBox.Show("Yanlış Eposta veya Şifre !", "Giriş sonucu:", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
                } 
                } 
            }
            if (eposta != "" && sifre != "" && !girisYapildi) MessageBox.Show("Giriş Başarısız!", "Giriş sonucu:", MessageBoxButtons.OK, MessageBoxIcon.Error);
           
            baglanti.Close();
        }
        
        private void guncelleHesapBilgileri(String isim, String soyIsim, String telNo, String eposta, String parola)
        {
            if(baglanti.State == ConnectionState.Closed) baglanti.Open();
            String idBulmaKomutu = "SELECT id FROM uyeler WHERE telefon=@telNo";
            SqlCommand idBul = new SqlCommand(idBulmaKomutu,baglanti);
            idBul.Parameters.AddWithValue("@telNo", telNo);
            SqlDataReader read = idBul.ExecuteReader();
            read.Read();
            int id = Convert.ToInt32(read["id"].ToString());
            read.Close();
            String komut = "UPDATE uyeler SET isim=@isim,soyisim=@soyIsim,telefon=@telNo,eposta=@eposta,parola=@parola WHERE id=@id";
            SqlCommand verileriGuncelle = new SqlCommand(komut,baglanti);
            verileriGuncelle.Parameters.AddWithValue("@isim",isim);
            verileriGuncelle.Parameters.AddWithValue("@soyisim", soyIsim);
            verileriGuncelle.Parameters.AddWithValue("@telNo", telNo);
            verileriGuncelle.Parameters.AddWithValue("@eposta", eposta);
            verileriGuncelle.Parameters.AddWithValue("@parola", parola);
            verileriGuncelle.Parameters.AddWithValue("@id", id);
            verileriGuncelle.ExecuteNonQuery();
            MessageBox.Show("Bilgileriniz başarı ile güncellendi.","İşlem sonucu:",MessageBoxButtons.OK, MessageBoxIcon.Information);
            baglanti.Close() ;
        }

        private void urunIlanEkle(String isim, String fiyat,String kategori, String renk, String boyut, Image image)
        {
            if(baglanti.State == ConnectionState.Closed) baglanti.Open();
            
            String komut = "INSERT INTO "+ kategori+"  (isim,fiyat,renk,boyut,satici,resim) VALUES (@isim,@fiyat,@renk,@boyut,@satici,@image)";
            SqlCommand urunIlanEkle = new SqlCommand(komut,baglanti);
            urunIlanEkle.Parameters.AddWithValue("@isim", isim);
            urunIlanEkle.Parameters.AddWithValue("@fiyat", fiyat);
            
            urunIlanEkle.Parameters.AddWithValue("@renk", renk);
            urunIlanEkle.Parameters.AddWithValue("@boyut", boyut);
            urunIlanEkle.Parameters.AddWithValue("@satici",BTN_girisVeKayit.Text);
            urunIlanEkle.Parameters.AddWithValue("@image", ImageToByte(image));
            urunIlanEkle.ExecuteNonQuery();
            MessageBox.Show("İşlem Başarılı!","İşlem sonucu:",MessageBoxButtons.OK, MessageBoxIcon.Information);
            CBX_urunKategori.Text = "";
            TBX_urunIsim.Text = "";
            TBX_urunBoyut.Text = "";
            TBX_urunFiyat.Text = "";
            TBX_urunRenk.Text = "";
            PBX_urunResmi.Image = PBX_urunResmi.Image = Image.FromFile("image-editing.png"); 
            baglanti.Close();

        }
        private byte[] ImageToByte(Image img)
        {
            byte[] byteArray = new byte[0];
            using (MemoryStream stream = new MemoryStream())
            {
                img.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                stream.Close();

                byteArray = stream.ToArray();
            }
            return byteArray;
        }
        private Image ByteToImage(byte[] byteArray)
        {
            using (MemoryStream ms = new MemoryStream(byteArray))
            {
                return Image.FromStream(ms);
            }
        }
        private void urunResmiYukle()
        {
           resimSecici.ShowDialog() ;
            resimSecici.Filter = "Image Files(*.jpg; *.jpeg;)|*.jpg; *.jpeg;";
             
            String resimYolu = resimSecici.FileName;
            PBX_urunResmi.ImageLocation = resimYolu;
            

        }

        private void urunleriGetir(String kategori)
        {
            
            PNL_kategori.Visible = true;
            PNL_kategori.Dock = DockStyle.Fill;
            if(baglanti.State == ConnectionState.Closed) baglanti.Open();
            String komut = "SELECT id,isim,puan,fiyat,resim FROM "+kategori;
            SqlCommand urunleriGetir = new SqlCommand(komut, baglanti);
            
            SqlDataReader read = urunleriGetir.ExecuteReader();
            read.GetOrdinal("resim");
            while (read.Read())
            {
                if (int.Parse(read["id"].ToString()) == sayfaIndex)
                {
                    PBX_urun1Resim.Image = ByteToImage((byte[])read["resim"]);
                    LBL_urun1Isim.Text = read["isim"].ToString();
                    
                    LBL_urun1Fiyat.Text = read["fiyat"].ToString();
                }
                if (int.Parse(read["id"].ToString()) == sayfaIndex+1)
                {
                    PBX_urun2Resim.Image = ByteToImage((byte[])read["resim"]);
                    LBL_urun2Isim.Text = read["isim"].ToString();
                    
                    LBL_urun2Fiyat.Text = read["fiyat"].ToString();
                }
                if (int.Parse(read["id"].ToString()) == sayfaIndex+2 )
                {
                    PBX_urun3Resim.Image = ByteToImage((byte[])read["resim"]);
                    LBL_urun3Isim.Text = read["isim"].ToString();
                    
                    LBL_urun3Fiyat.Text = read["fiyat"].ToString();
                }
            }

            baglanti.Close();

        }

        private void digerPanelleriKapa(Panel panel)
        {
            foreach (var i in panelList)
            {
                if(i != panel) i.Visible = false;
            }
            panel.Visible = true;
            panel.Dock= DockStyle.Fill;
        }

        private void siparisVer(String isim,String soyisim,String odemeYontemi,String kartNo,String SKT,String guvenlikKodu,String sehir,String telefon,String adres,String urun,String sirket,String ucret)
        {
            if(baglanti.State == ConnectionState.Closed) baglanti.Open();
            String komut = "INSERT INTO faturalandirma (isim,soyisim,odemeYontemi,kartNumarasi,sonKullanmaTarihi,guvenlikKodu,sehir,telefonNumarasi,adres,urun) VALUES (@isim,@soyisim,@odemeYontemi,@kartNumarasi,@sonKullanmaTarihi,@guvenlikKodu,@sehir,@telefonNumarasi,@adres,@urun)";
            SqlCommand siparisVer = new SqlCommand(komut, baglanti);
            siparisVer.Parameters.AddWithValue("@isim",isim);
            siparisVer.Parameters.AddWithValue("@soyisim",soyisim);
            siparisVer.Parameters.AddWithValue("@odemeYontemi",odemeYontemi);
            siparisVer.Parameters.AddWithValue("@kartNumarasi",kartNo);
            siparisVer.Parameters.AddWithValue("@sonKullanmaTarihi",SKT);
            siparisVer.Parameters.AddWithValue("@guvenlikKodu",guvenlikKodu);
            siparisVer.Parameters.AddWithValue("@sehir",sehir);
            siparisVer.Parameters.AddWithValue("@telefonNumarasi",telefon);
            siparisVer.Parameters.AddWithValue("@adres",adres);
            siparisVer.Parameters.AddWithValue("@urun", urun);
            siparisVer.ExecuteNonQuery();
           
            MessageBox.Show("Sipariş Verildi!", "İşlem Sonucu:", MessageBoxButtons.OK, MessageBoxIcon.Information);
           
            baglanti.Close();
        }
        private void satinAl(String urun)
        {
            secilmisUrun = urun;
            digerPanelleriKapa(PNL_odeme);
            
        }

        #endregion
        public Form1()
        {
            InitializeComponent();
        }

        private void BTN_kapat_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        #region Sekmeler
        private void BTN_erkek_Click(object sender, EventArgs e)
        {
            moveSelectedPNL(BTN_erkek);
            digerPanelleriKapa(PNL_kategori);
            suankiKategori = "Erkek";
            sayfaIndex= 1;
            urunleriGetir(suankiKategori);
        }

        private void BTN_kadin_Click(object sender, EventArgs e)
        {
            moveSelectedPNL(BTN_kadin);
            
            suankiKategori = "Kadın";
            digerPanelleriKapa(PNL_kategori);
            sayfaIndex = 1;
            urunleriGetir(suankiKategori); 
        }

        private void BTN_elektronik_Click(object sender, EventArgs e)
        {
            moveSelectedPNL(BTN_elektronik);
            digerPanelleriKapa(PNL_kategori);
            suankiKategori = "Elektronik";
            sayfaIndex = 1;
            urunleriGetir(suankiKategori);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            moveSelectedPNL(BTN_anasayfa);
            digerPanelleriKapa(PNL_anasayfa);
            PNL_anasayfa.Visible = true;
            
        }

        private void BTN_kirtasiye_Click(object sender, EventArgs e)
        {
            moveSelectedPNL(BTN_kitapVeKırtasiye);
            suankiKategori = "Kırtasiye";
            digerPanelleriKapa(PNL_kategori);
            sayfaIndex = 1;
            urunleriGetir(suankiKategori);
        }

        private void BTN_ev_Click(object sender, EventArgs e)
        {
            moveSelectedPNL(BTN_ev);
            suankiKategori = "Ev";
            digerPanelleriKapa(PNL_kategori);
            sayfaIndex = 1;
            urunleriGetir(suankiKategori);
        }
        #endregion
        private void Form1_Load(object sender, EventArgs e)
        {

            panelList.Add(PNL_girisVeKayit); panelList.Add(PNL_girisYapildi);
            panelList.Add(PNL_kategori);panelList.Add(PNL_anasayfa);panelList.Add(PNL_arama);panelList.Add(PNL_odeme);
            digerPanelleriKapa(PNL_anasayfa);
            if(baglanti.State == ConnectionState.Closed) baglanti.Open();
            String komut = "SELECT sirket,ucret FROM kargolama";
            SqlCommand kargoSirketleriGetir = new SqlCommand(komut,baglanti);
            SqlDataReader read = kargoSirketleriGetir.ExecuteReader();
            while (read.Read())
            {
                CBX_odemeKargoSirket.Items.Add(read["sirket"].ToString() );
            }
            read.Close();
            komut = "SELECT odemeYontemi FROM odeme";
            SqlCommand odemeYontemleriGetir = new SqlCommand(komut,baglanti);
            read = odemeYontemleriGetir.ExecuteReader();
            while (read.Read())
            {
                CBX_odemeYontem.Items.Add(read["odemeYontemi"].ToString());
            }
            baglanti.Close();
            BTN_cikisYap.Visible = false;
        }



        private void TBX_aramaYap_MouseClick(object sender, MouseEventArgs e)
        {
            TBX_aramaYap.Text = "";
            TBX_aramaYap.ForeColor = Color.Black;
            digerPanelleriKapa(PNL_arama);
        }



        private void BTN_girisVeKayit_Click(object sender, EventArgs e)
        {
            if (girisYapildi == false)
            {
                PNL_girisVeKayit.Visible = true;
                PNL_girisVeKayit.Dock = DockStyle.Fill;
                digerPanelleriKapa(PNL_girisVeKayit);
            }
            else
            {
                PNL_girisYapildi.Visible = true;
                PNL_girisYapildi.Dock = DockStyle.Fill;
                digerPanelleriKapa(PNL_girisYapildi);

            }

            

        }

        private void BTN_kayitYap_Click(object sender, EventArgs e)
        {
            kayitOlustur(TBX_kayitIsim.Text, TBX_kayitSoyisim.Text, TBX_kayitTelNo.Text, TBX_kayitEposta.Text, TBX_kayitSifre.Text);
        }

        private void BTN_girisYap_Click(object sender, EventArgs e)
        {
            girisYap(TBX_girisEposta.Text, TBX_girisSifre.Text);
        }

        private void BTN_hesabimKaydet_Click(object sender, EventArgs e)
        {
            guncelleHesapBilgileri(TBX_hesabimIsim.Text,TBX_hesabimSoyisim.Text,TBX_hesabimTelno.Text,TBX_hesabimPosta.Text,TBX_hesabimSifre.Text);
        }

        private void BTN_urunIlanVer_Click(object sender, EventArgs e)
        {
            urunIlanEkle(TBX_urunIsim.Text,TBX_urunFiyat.Text, CBX_urunKategori.Text,TBX_urunRenk.Text,TBX_urunBoyut.Text,PBX_urunResmi.Image);
        }

        private void PBX_urunResmi_Click(object sender, EventArgs e)
        {
            urunResmiYukle();
        }

        private void BTN_cikisYap_Click(object sender, EventArgs e)
        {
            girisYapildi = false;
            PNL_girisYapildi.Visible = false;
            BTN_girisVeKayit.Text = "Giriş / Kayıt";
            BTN_cikisYap.Visible= false;
            TBX_girisSifre.Text = "";
        }

        private void BTN_sonrakiSayfa_Click(object sender, EventArgs e)
        {
            sayfaIndex += 3;
            urunleriGetir(suankiKategori);
            BTN_oncekiSayfa.Visible = true;
        }

        private void TBX_aramaYap_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                PNL_arama.Visible = true;
                PNL_arama.Dock = DockStyle.Fill;
                if(baglanti.State == ConnectionState.Closed) baglanti.Open();
                String komut = "SELECT * FROM Kadın Where Kadın.isim=@anahtar";
                SqlCommand ara = new SqlCommand(komut, baglanti);
                ara.Parameters.AddWithValue("@anahtar", TBX_aramaYap.Text);
                SqlDataReader read = ara.ExecuteReader();
                read.GetOrdinal("resim");
                while (read.Read())
                {
                    
                        PBX_aramaResim.Image = ByteToImage((byte[])read["resim"]);
                        LBL_aramaIsim.Text = read["isim"].ToString();
                        
                        LBL_aramaFiyat.Text = read["fiyat"].ToString();
                    

                }read.Close();
                komut = "SELECT * FROM Erkek Where Erkek.isim=@anahtar";
                ara = new SqlCommand(komut, baglanti);
                ara.Parameters.AddWithValue("@anahtar", TBX_aramaYap.Text);
                read = ara.ExecuteReader();
                read.GetOrdinal("resim");
                while (read.Read())
                {

                    PBX_aramaResim.Image = ByteToImage((byte[])read["resim"]);
                    LBL_aramaIsim.Text = read["isim"].ToString();
                    
                    LBL_aramaFiyat.Text = read["fiyat"].ToString();


                }
                read.Close();
                komut = "SELECT * FROM Elektronik Where Elektronik.isim=@anahtar";
                ara = new SqlCommand(komut, baglanti);
                ara.Parameters.AddWithValue("@anahtar", TBX_aramaYap.Text);
                read = ara.ExecuteReader();
                read.GetOrdinal("resim");
                while (read.Read())
                {

                    PBX_aramaResim.Image = ByteToImage((byte[])read["resim"]);
                    LBL_aramaIsim.Text = read["isim"].ToString();
                    
                    LBL_aramaFiyat.Text = read["fiyat"].ToString();


                }
                read.Close();
                komut = "SELECT * FROM Kırtasiye Where Kırtasiye.isim=@anahtar";
                ara = new SqlCommand(komut, baglanti);
                ara.Parameters.AddWithValue("@anahtar", TBX_aramaYap.Text);
                read = ara.ExecuteReader();
                read.GetOrdinal("resim");
                while (read.Read())
                {

                    PBX_aramaResim.Image = ByteToImage((byte[])read["resim"]);
                    LBL_aramaIsim.Text = read["isim"].ToString();
                    
                    LBL_aramaFiyat.Text = read["fiyat"].ToString();


                }
                read.Close();
                komut = "SELECT * FROM Ev Where Ev.isim=@anahtar";
                ara = new SqlCommand(komut, baglanti);
                ara.Parameters.AddWithValue("@anahtar", TBX_aramaYap.Text);
                read = ara.ExecuteReader();
                read.GetOrdinal("resim");
                while (read.Read())
                {

                    PBX_aramaResim.Image = ByteToImage((byte[])read["resim"]);
                    LBL_aramaIsim.Text = read["isim"].ToString();
                    
                    LBL_aramaFiyat.Text = read["fiyat"].ToString();


                }
                if (LBL_aramaIsim.Text.Length > 0) BTN_aramaSatin.Visible = true;
                baglanti.Close();
            }
        }

        private void BTN_hesabimSil_Click(object sender, EventArgs e)
        {
            if(baglanti.State == ConnectionState.Closed) baglanti.Open();
            String komut = "DELETE FROM uyeler WHERE isim=@isim AND soyisim=@soyisim AND telefon=@telefon";
            SqlCommand sil = new SqlCommand(komut, baglanti);
            sil.Parameters.AddWithValue("@isim", TBX_hesabimIsim.Text);
            sil.Parameters.AddWithValue("@soyisim", TBX_hesabimSoyisim.Text);
            sil.Parameters.AddWithValue("@telefon",TBX_hesabimTelno.Text);
            sil.ExecuteNonQuery();
            MessageBox.Show("Hesabınız başarı ile silindi!", "İşlem sonucu:", MessageBoxButtons.OK, MessageBoxIcon.Information);
            digerPanelleriKapa(PNL_anasayfa);
            BTN_girisVeKayit.Text = "Giriş / Kayıt";
            TBX_girisEposta.Text = ""; TBX_girisSifre.Text = "";
            girisYapildi = false;
        }

        private void TBX_odemeGonder_Click(object sender, EventArgs e)
        {
            siparisVer(TBX_odemeIsim.Text, TBX_odemeSoyisim.Text,CBX_odemeYontem.Text,TBX_odemeKartNo.Text,CBX_odemeTarihAY.Text+"/"+CBX_odemeTarihYIL.Text,TBX_odemeGuvenlikKod.Text,TBX_odemeSehir.Text,TBX_odemeTelefonNo.Text,TBX_odemeAdres.Text,secilmisUrun,CBX_odemeKargoSirket.Text,TBX_odemeKargoUcret.Text);
        }

        private void CBX_odemeKargoSirket_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CBX_odemeKargoSirket.Text == "PTT") TBX_odemeKargoUcret.Text = "17.99₺"; else { TBX_odemeKargoUcret.Text = "27.99₺"; }
            TBX_odemeTahminiVaris.Text = DateTime.Now.AddDays(+3).ToShortDateString().ToString();
        }

        private void BTN_urun1SatinAl_Click(object sender, EventArgs e)
        {
            satinAl(LBL_urun1Isim.Text);
        }

        private void BTN_urun2SatinAl_Click(object sender, EventArgs e)
        {
            satinAl(LBL_urun2Isim.Text);
        }

        private void BTN_urun3SatinAl_Click(object sender, EventArgs e)
        {
            satinAl(LBL_urun3Isim.Text);
        }

        private void BTN_oncekiSayfa_Click(object sender, EventArgs e)
        {
            sayfaIndex -= 3;
            if(sayfaIndex == 1) BTN_oncekiSayfa.Visible= false;
            urunleriGetir(suankiKategori);
        }
    }
}
