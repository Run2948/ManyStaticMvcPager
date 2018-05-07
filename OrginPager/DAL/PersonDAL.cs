using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace OrginPager.DAL
{
    public class PersonDAL
    {
        #region ##得到所有人
        /// <summary>
        /// 得到所有人
        /// </summary>
        /// <param name="size"></param>
        /// <param name="index"></param>
        /// <param name="where"></param>
        /// <param name="OutTotalCount"></param>
        /// <returns></returns>
        public List<Models.Person> GetAllPerson(int size, int index, string where, out int OutTotalCount)
        {
            Models.Person person = null;
            List<Models.Person> list = new List<Models.Person>();
            DataTable dt = null;
            //分页参数
            string[] paramValue =
            {
                "Person",//表明
                "*",//返回字段
                "Id",//主键标识列
                where,//where条件
                "Id asc",//排序必须跟有 asc 或 desc
                "1",//排序规则 1:正序asc 2:倒序desc 3:多列排序方法
                "0",//记录总数 0:会返回总记录
                ""+size,//页面大小
                ""+index//当前页
            };
            dt = SqlHelper.SqlGetDataTable("P_AspNetPage", CommandType.StoredProcedure, paramValue, out OutTotalCount);
            foreach (DataRow dr in dt.Rows)
            {
                person = new Models.Person
                {
                    Id = Convert.ToInt32(dr["Id"]),
                    Name = dr["Name"].ToString()
                };
                list.Add(person);
            }
            return list;
        }
        #endregion

        #region##得到总数
        /// <summary>
        /// 得到总数
        /// </summary>
        /// <returns></returns>
        public int GetPersonCount()
        {
            string sql = "select count(*) from Person";
            try
            {
               return (int)SqlHelper.ExecuteScalar(sql, null);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        #endregion
    }
}
