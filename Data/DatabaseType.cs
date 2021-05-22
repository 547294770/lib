using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Data
{
    /// <summary>
    /// 数据库类型
    /// </summary>
    public enum DatabaseType : byte
    {
        /// <summary>
        /// SQL SERVER 2005 and high
        /// </summary>
        [Description("System.Data.SqlClient")]
        SqlServer,
        [Description("System.Data.Oledb")]
        Access,
        [Description("MySql.Data.MySqlClient")]
        MySql,
        [Description("System.Data.Sqlite")]
        SQLite,
        Oracle
    }
}
