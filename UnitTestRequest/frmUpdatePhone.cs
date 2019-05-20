using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TunstallBL;
using TunstallBL.Services;
using TunstallBL.Helpers;
using TunstallBL.Models;

namespace UnitTestRequest
{
    public partial class frmUpdatePhone : Form
    {
        public delegate void UpdatePhoneCompletedHandler(bool isCompleted, CellDeviceModel model);
        public event UpdatePhoneCompletedHandler OnPhoneNumberUpdated;

        CellDeviceModel _selectedDevice = null;

        public frmUpdatePhone(CellDeviceModel model)
        {
            InitializeComponent();

            this.Load += FrmUpdatePhone_Load;
            btnCancel.Click += BtnCancel_Click;
            btnSave.Click += BtnSave_Click;

            _selectedDevice = model;
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if(!string.IsNullOrEmpty(txtPhoneNum.Text))
            {
                 _selectedDevice.MDN = txtPhoneNum.Text;
                var isUpdated = CellDeviceService.Instance.UpdateDevicePhone(_selectedDevice);
                if(isUpdated)
                {
                    if (OnPhoneNumberUpdated != null)
                    {
                        OnPhoneNumberUpdated(isUpdated, _selectedDevice);
                    }
                }
                else
                {
                    MessageBox.Show(string.Format("Failed to update unit {0} to phone {1}", _selectedDevice.UNIT_ID, _selectedDevice.MDN));
                }
                this.Close();
            }
            else
            {
                MessageBox.Show("Phone number required!");
                this.Close();
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            if(OnPhoneNumberUpdated != null)
            {
                OnPhoneNumberUpdated(false, null);
                this.Close();
            }
        }

        private void FrmUpdatePhone_Load(object sender, EventArgs e)
        {
            txtPhoneNum.Text = _selectedDevice.MDN;
        }
    }
}
