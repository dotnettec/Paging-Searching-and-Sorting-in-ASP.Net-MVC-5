using EmployeeManagement.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EmployeeManagement.Logic
{
    public class EmployeeManager : CommonRepository<Employee>
    {
        public EmployeeManager(DbContext context) : base(context) { }

        public List<Employee> GetEmployeeList()
        {
            var lst = this.GetAll().ToList();
            return lst;
        }

        public void AddEmployee(Employee obj)
        {
            this.Add(obj);
        }

        public void UpdateEmployee(Employee obj)
        {
            this.Update(obj);
        }

        public Employee GetEmployeeById(int Id)
        {
            return this.GetById(Id);
        }

        public void DeleteEmployee(int Id)
        {
            this.Delete(Id);
        }

        public string SaveEmployee(Employee objEmployee)
        {
            using (var context = new CommonRepository<Employee>(this.DbContext))
            {
                if (objEmployee.Id == 0)
                {
                    var res = context.FirstOrDefault(d => d.Name == objEmployee.Name);
                    if (res != null) { return "exists"; }
                    else
                    {
                        objEmployee.CreatedDate = DateTime.Now;
                        context.Add(objEmployee);
                        return "success";
                    }
                }
                else
                {
                    var res = context.FirstOrDefault(d => d.Name == objEmployee.Name && d.Id != objEmployee.Id);
                    if (res != null) { return "exists"; }
                    else
                    {
                        context.Update(objEmployee);
                        return "success";
                    }
                }

            }
        }

    }

    public class EmployeeModel : CommonModel<Employee>
    {
        public SelectList CityList { get; set; }
        public string SortNameType { get; set; }      
    }

    public class CityManager : CommonRepository<City>
    {
        public CityManager(DbContext context) : base(context) { }
        public DataTable GetDtCity()
        {
            return GetDataTable("select CityID,CityName from City order by CityName");
        }
    }
}