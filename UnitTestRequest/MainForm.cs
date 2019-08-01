using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using TunstallBL;
using TunstallBL.Services;
using TunstallBL.Helpers;
using TunstallBL.Models;
using TunstallBL.API;
using TunstallBL.API.Model;
namespace UnitTestRequest
{
    public partial class MainForm : Form
    {
        #region Member Variables
        LogHelper _log = null;

        enum TESTMODE
        {
            ON,
            OFF
        }

        #endregion

        #region Private Methods
        void LogMessage(string message)
        {
            if(_log != null)
            {
                _log.LogMessage(LogMessageType.INFO, message);
            }

            //log to ui console window
            txtConsole.Text = string.Format("{0}    {1}", DateTime.Now.ToString("yyyyMMdd HH:mm:ss"), message) + Environment.NewLine + txtConsole.Text;
        }

        void LoadCellDeviceGrid()
        {
            grdDevices.AutoGenerateColumns = false;
            var devices = CellDeviceService.Instance.GetCellDevices();
            grdDevices.DataSource = devices;

            LogMessage("Loading Cell devices from database");
        }

        bool SendToAnelto(TESTMODE mode, string unitId)
        {
            var model = new AneltoSubscriberOverrideRequest();
            model.accounts = unitId;

            switch (mode)
            {
                case TESTMODE.ON:
                    model.number = AppConfigurationHelper.AneltoTestNumber;
                    break;

                case TESTMODE.OFF:
                    model.number = AppConfigurationHelper.AneltoProdNumber;
                    break;
            }
           
            try
            {
                AneltoAPI api = new AneltoAPI();
                var response = api.SubscriberCCOverride(model);
                LogMessage(string.Format("Sent to Anelto CCOverride with response {0} for unit {1}. Data: {2}", response, unitId, model.ToJson()));

                return true;
            }
            catch (Exception ex)
            {
                LogMessage(string.Format("Failed to send to Anelto API CCOverride for unit {0}. DATA: {1}. ERROR: {2}", unitId, model.ToJson(), ex.Message));
                return false;
            }
        }

        bool SendToMytrex(TESTMODE mode, string unitId, string serialNum)
        {
            try
            {
                string testFileName = AppConfigurationHelper.MytrextTestEvents;
                if(mode == TESTMODE.OFF)
                {
                    testFileName = AppConfigurationHelper.MytrexProdEvents;
                }
                // read in the correct file based on mode
                List<MytrexUnitEvent> eventList = JsonConvert.DeserializeObject<List<MytrexUnitEvent>>(File.ReadAllText(testFileName));

                //Change the serialnumber for the events
                eventList.ForEach(e => e.UnitSerNum = serialNum);
                
                var api = new MytrexAPI();
                var response = api.UpdateEvents(eventList);
                LogMessage(string.Format("Sent to Mytrex Update Events with response {0} for unit {1}. Data: {2}", response, unitId, eventList.ToJson()));

                return true;
            }
            catch (Exception ex)
            {
                LogMessage(string.Format("Failed to send to Mytrex API Unit Event for unit {0}.ERROR: {1}", unitId, ex.Message));
                return false;
            }
        }

        void SetUnitTestMode(string unitId)
        {
            if (!string.IsNullOrEmpty(unitId))
            {
                var cellDevice = CellDeviceService.Instance.GetCellDeviceByUnitId(unitId);
                if (cellDevice != null)
                {
                    //change the unit to the opposite of currnet status
                    var mode = TESTMODE.OFF;
                    switch(cellDevice.TEST)
                    {
                        case true:
                            mode = TESTMODE.OFF;
                            break;
                                
                        case false:
                            mode = TESTMODE.ON;
                            var isExisting = HomeService.Instance.SearchExistingUnit(cellDevice);
                            if (isExisting)
                            {
                                MessageBox.Show(string.Format("Unit {0} is an existing unit.", cellDevice.UNIT_ID.ToString()));
                                return;
                            }
                            break;
                    }

                   

                    LogMessage(string.Format("Unit {0} : Test Mode: {1}", unitId, mode.ToString()));


                    bool isSuccess = false;
                    LogMessage(string.Format("Found cell device IMEI {0} for unit {1}.", cellDevice.IMEI, unitId));

                    switch(cellDevice.OTHER.ToLower())
                    {
                        case "mytrex lte":
                            isSuccess = SendToMytrex(mode, unitId, cellDevice.SERIALNO);
                            break;

                        case "anelto lte":
                        case "anelto otg":
                            isSuccess = SendToAnelto(mode, unitId);
                            break;

                        default:
                            LogMessage(string.Format("No process found for {0}", cellDevice.OTHER));
                            break;
                    }

                    if (isSuccess)
                    {
                        CellDeviceService.Instance.UpdateCellDeviceStatus(cellDevice.ID, mode == TESTMODE.ON ? true : false);
                        LogMessage(string.Format("Set cell device {0} test status to {1}.", unitId, mode.ToString()));
                    }
                }
                else
                {
                    MessageBox.Show(string.Format("Found no cell device for unit {0}.", unitId));
                    LogMessage(string.Format("Found no cell device for unit {0}.", unitId));
                }

                LoadCellDeviceGrid();

                txtUnit.Clear();
                //txtUnit.Focus();
            }
        }
        #endregion

        public MainForm()
        {
            InitializeComponent();
            this.Load += MainForm_Load;
            btnOnTest.Click += BtnOnTest_Click;
            btnOffTest.Click += BtnOffTest_Click;
            txtUnit.KeyDown += TxtUnit_KeyDown;
            _log = new LogHelper(AppConfigurationHelper.LogFile);

            this.Text = string.Format("{0}  {1}", Application.ProductName, Application.ProductVersion);
        }

        private void TxtUnit_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                var unitId = txtUnit.Text;
                SetUnitTestMode(unitId);
            }
        }
        

        private void BtnOffTest_Click(object sender, EventArgs e)
        {
            //var unitId = txtUnit.Text;
           // SetUnitTestMode(TESTMODE.OFF, unitId);
        }

        private void BtnOnTest_Click(object sender, EventArgs e)
        {
            //var unitId = txtUnit.Text;
            //SetUnitTestMode(TESTMODE.ON, unitId);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            LoadCellDeviceGrid();

            this.ActiveControl = txtUnit;

        }

        private void toolStripMenuItemSearch_Click(object sender, EventArgs e)
        {
            var search = new frmSearch(_log);
            search.OnFormClosedEvent += _search_OnFormClosedEvent;
            search.ShowDialog();
        }

        private void _search_OnFormClosedEvent(bool isCompleted)
        {
            txtUnit.Clear();
            txtUnit.Focus();
            LoadCellDeviceGrid();
        }
    }
}
