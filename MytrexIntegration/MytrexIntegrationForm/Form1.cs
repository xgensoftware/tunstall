using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TunstallBL;
using TunstallBL.Helpers;
namespace MytrexIntegrationForm
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Text = string.Format("{0}  {1}", Application.ProductName, Application.ProductVersion);
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            var secret = ConfigurationManager.AppSettings["Secret"].ToString();
            var dealerKey = ConfigurationManager.AppSettings["DealerKey"].ToString();
            var username = ConfigurationManager.AppSettings["Username"].ToString();
            var url = ConfigurationManager.AppSettings["MytrexUrl"].ToString();

            if(!string.IsNullOrEmpty(txtPhoneNum.Text))
            {
                StringBuilder phone = new StringBuilder(txtPhoneNum.Text);

                //1 . check if the first char is a 1. Add 1 
                if(phone.ToString().Substring(0,1) != "1")
                {
                    phone.Insert(0, "1");
                }

                var token = JWTHelper.GetToken(secret, username);
                url = string.Format("{0}?username={1}&phonenumber={2}&dealerkey={3}&token={4}", url, username, phone.ToString(), dealerKey, token);

                try
                {
                    Process.Start("chrome.exe", url);
                }
                catch
                {

                    using (WebBrowser browser = new WebBrowser())
                    {
                        browser.Navigate(url, "_blank", null, null);
                    }
                }

                this.Close();
            }
            else
            {
                MessageBox.Show("You must enter a valid phone number to search.");
                txtPhoneNum.Clear();
                txtPhoneNum.Focus();
            }
        }
    }
}
