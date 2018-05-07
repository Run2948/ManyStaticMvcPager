using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OrginPager.Models
{
    /// <summary>
    /// 实体类---人
    /// </summary>
    [Serializable]    
    public class Person
    {
        /// <summary>
        /// 编号
        /// </summary>
        public int Id { get; set; }         

        /// <summary>
        /// 名字
        /// </summary>
        public string Name { get; set; }
    }
}
