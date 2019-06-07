using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TunstallBL;
using TunstallBL.Services;
using TunstallBL.Helpers;
using TunstallBL.Models;
using TunstallBL.API;
using TunstallBL.API.Model;
namespace AneltoBulkUpdate
{
    class Program
    {
        static void Main(string[] args)
        {
            //AneltoAPI api = new AneltoAPI();
            //var table = FileHelper.ReadCSVFileToTable(@"e:\AneltoUpdate.csv",',', true);
            //Parallel.ForEach(table.AsEnumerable(), row =>
            //{
            //    var model = new AneltoSubscriberUpdateRequest();
            //    model.ani = row[0].ToString();
            //    model.account = row[1].ToString();

            //    var response = api.SubscriberCreateUpdate(model);
            //    Console.WriteLine(response);

            //});

            var cellDevice = new CellDeviceModel();
            //cellDevice.UNIT_ID = 820001;
            cellDevice.MDN = "4172313490";

            var isExisting = HomeService.Instance.SearchExistingUnit(cellDevice);

            Console.WriteLine(isExisting.ToString());

        }
    }
}
