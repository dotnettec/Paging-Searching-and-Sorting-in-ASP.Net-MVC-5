using EmployeeManagement.Logic;
using EmployeeManagement.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Mvc;

namespace EmployeeManagement.Controllers
{
    public class HomeController : Controller
    {
        CommonFunction common = new CommonFunction();             

        #region Sample3 - Custom Grid with Each Column filter, paging and sorting
        public ActionResult Sample3(int page = 1, int pageSize = 10)
        {
            EmployeeModel objModel = new EmployeeModel();
            objModel.StaticPageSize = 10;

            BindSample3(objModel, page, pageSize);
            return View(objModel);
        }

        public ActionResult Sample3FilterSearch(EmployeeModel objModel, int page = 1, int pageSize = 10)
        {
            BindSample3(objModel, page, pageSize);
            return PartialView("Sample3List", objModel);
        }

        public void BindSample3(EmployeeModel objModel, int page, int pageSize)
        {
            CityManager objCityManager = new CityManager(new DataContext());
            EmployeeManager context = new EmployeeManager(new DataContext());

            StringBuilder query = new StringBuilder();
            List<string> colName = common.GetColumns(CommonFunction.module.Employee.ToString());
            query = common.GetSqlTableQuery(CommonFunction.module.Employee.ToString());
            if (objModel != null)
                objModel.StaticPageSize = pageSize;

            objModel.CityList = Extens.ToSelectList(objCityManager.GetDtCity(), "CityID", "CityName");
            context.setModel(query, objModel, colName, "Name", page, pageSize);
        }
        #endregion

      
    }
}