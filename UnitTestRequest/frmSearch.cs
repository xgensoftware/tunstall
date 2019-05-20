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
    public partial class frmSearch : Form
    {
        public delegate void SearchCompletedHandler(bool isCompleted);
        public event SearchCompletedHandler OnFormClosedEvent;

        #region Member Variables

        LogHelper _log = null;
        
        #endregion

        

        #region Private Methods
        void InitializeForm()
        {
            // populate controls
            List<ListItem> modeList = new List<ListItem>();
            modeList.Add(new ListItem { Key = "All", Value = "All" });
            modeList.Add(new ListItem { Key = "On", Value = "True" });
            modeList.Add(new ListItem { Key = "Off", Value = "False" });

            cmbMode.DataSource = modeList;
            cmbMode.DisplayMember = "Key";
            cmbMode.ValueMember = "Value";
          

        }
        #endregion

        public frmSearch(LogHelper log)
        {
            InitializeComponent();

            this.Load += FrmSearch_Load;
            grdSearchResult.CellContentClick += GrdSearchResult_CellContentClick;

            _log = log;

           
        }

        private void GrdSearchResult_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            var senderGrid = (DataGridView)sender;
            var cellDevice = senderGrid.Rows[e.RowIndex].DataBoundItem as CellDeviceModel;
            if(cellDevice != null)
            {
                var frmUpdatePhone = new frmUpdatePhone(cellDevice);
                frmUpdatePhone.OnPhoneNumberUpdated += _frmUpdatePhone_OnPhoneNumberUpdated;
                frmUpdatePhone.ShowDialog();
            }
        }

        private void _frmUpdatePhone_OnPhoneNumberUpdated(bool isCompleted, CellDeviceModel model)
        {
           if(isCompleted)
            {
                MessageBox.Show(string.Format("Successfully updated phone number to {0} for unit {1} ", model.MDN, model.UNIT_ID));

                // clear search
                txtUnitId.Clear();
                txtUnitType.Clear();
                txtIMEI.Clear();
                txtSerialNum.Clear();
                txtUnitId.Focus();

                grdSearchResult.DataSource = null;
            }
        }

        private void FrmSearch_Load(object sender, EventArgs e)
        {
            InitializeForm();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (OnFormClosedEvent != null)
                OnFormClosedEvent(false);


            this.Close();

        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            CellDeviceSearchModel model = new CellDeviceSearchModel();
            model.UnitId = txtUnitId.Text;
            model.IMEI = txtIMEI.Text;
            model.SerialNumber = txtSerialNum.Text;
            model.UnitType = txtUnitType.Text;

            model.TestMode = ((ListItem)cmbMode.SelectedItem).Value;
            var result = CellDeviceService.Instance.SearchCellDevice(model);

            grdSearchResult.AutoGenerateColumns = false;
            grdSearchResult.DataSource = result;

        }
    }
}
