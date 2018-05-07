using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;

namespace OrginPager.BLL
{
    public class PersonService
    {
        private static readonly DAL.PersonDAL dal = new DAL.PersonDAL();

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
            return dal.GetAllPerson(size, index, where, out OutTotalCount);
        }
        #endregion

        #region##得到总数
        /// <summary>
        /// 得到总数
        /// </summary>
        /// <returns></returns>
        public int GetPersonCount()
        {
            return dal.GetPersonCount();
        }
        #endregion
    }
}
