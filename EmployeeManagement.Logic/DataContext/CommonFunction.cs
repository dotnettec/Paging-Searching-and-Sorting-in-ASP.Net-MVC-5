using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace EmployeeManagement.Logic
{
    public class CommonFunction
    {
        public static string GetConnectionStringname() { return "DefaultConnection"; }

        public List<string> GetColumns(string moduleName)
        {
            List<string> colName = new List<string>();
            if (moduleName == module.Employee.ToString())
            {
                colName.Add("Id");
                colName.Add("CityID");
                colName.Add("Name");
                colName.Add("Email");                
                colName.Add("CityName");
                colName.Add("Salary");
                colName.Add("BirthDate");
                colName.Add("IsActive");
            }            
            return colName;
        }

        public StringBuilder GetSqlTableQuery(string mod)
        {
            StringBuilder query = new StringBuilder();
            query.Append("SELECT COUNT(1) OVER() AS rwcnt,");
            if (mod == module.Employee.ToString())
            {
                query.Append("Id,City.CityID,Name,Email,City.CityName,Salary,BirthDate,IsActive FROM Employee");
                query.Append(" join City on City.CityID = Employee.CityID");
            }            
            return query;
        }

        public static string GenerateFilterCondition(List<GridFilter> filters)
        {
            string ret = " 1=1 ";
            if (filters != null)
            {
                foreach (var filter in filters)
                {
                    if (!string.IsNullOrWhiteSpace(filter.ColumnName) && !string.IsNullOrEmpty(filter.Condition))
                    {
                        string value = "";
                        if (filter.Condition.Contains("is null") || filter.Condition.Contains("is not null"))
                        {
                        }
                        else
                        {
                            value = filter.Value == null ? string.Empty : filter.Value.Trim();
                        }

                        Regex regex = new Regex("[;\\\\/:*?\"<>|&']");
                        value = regex.Replace(value, "");

                        if (filter.Condition == "No" && !string.IsNullOrEmpty(value))
                            filter.Condition = "Contain";

                        ret += " AND";
                        ret += " (" + filter.ColumnName;
                        if (filter.Condition == "Contain")
                        {
                            ret += " LIKE '%" + value + "%'";
                        }
                        else if (filter.Condition == "Does not contain")
                        {
                            ret += " NOT LIKE '%" + value + "%'";
                        }
                        else if (filter.Condition == "Starts with")
                        {
                            ret += " LIKE '" + value + "%'";
                        }
                        else if (filter.Condition == "Ends with")
                        {
                            ret += " LIKE '%" + value + "' ";
                        }
                        else if (filter.Condition.Contains("is null") || filter.Condition.Contains("is not null"))
                        {
                            ret += " " + filter.Condition + " ";
                        }
                        else
                        {
                            ret += " " + filter.Condition + " '" + value + "'";
                        }
                        ret += ") ";
                    }
                }
            }
            return ret;
        }

        public enum module
        {
            Employee,
            Category,
            SubCategory,
            Status
        }
        
    }


    public class CommonModel<T>
    {
        public T Table { get; set; }
        public List<dynamic> dynamicList { get; set; }
        public IPagedList dynamicListMetaData { get; set; }
        public int? page { get; set; }
        public string sortOrder { get; set; }
        public string fieldName { get; set; }
        public string SearchText { get; set; }
        public List<GridFilter> Filters { get; set; }

        public int StaticPageSize { get; set; }
        public string ViewType { get; set; }
       
    }
    public class GridFilter
    {
        public string Value { get; set; }
        public string ColumnName { get; set; }
        public string Condition { get; set; }
    }
    public class MVCPagerModel
    {
        public List<dynamic> DynamicList { get; set; }
        public IPagedList DynamicListMetaData { get; set; }
        public string ActionName { get; set; }
        public string ControllerName { get; set; }
        public string UpdateTargetId { get; set; }
        public string TableUpdate { get; set; }

        public string sortOrder { get; set; }
        public string fieldName { get; set; }
        public string SearchText { get; set; }
        public string SearchType { get; set; }
        public string OnBegin { get; set; }
        public string OnComplete { get; set; }
        public string OnSuccess { get; set; }

        public int StaticPageSize { get; set; }

        public bool PagingWithoutFilter { get; set; }
    }
}