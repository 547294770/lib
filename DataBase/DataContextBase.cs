using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Common.DataBase
{
    public abstract class DataContextBase : DbContext
    {
        protected virtual string DbConnection { get; set; }
        protected DataContextBase(DbContextOptions options) : base(options)
        {
            // 开始禁止追踪
            this.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            this.ChangeTracker.AutoDetectChangesEnabled = false;
        }

        protected DataContextBase() : base()
        {
            // 开始禁止追踪
            this.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            this.ChangeTracker.AutoDetectChangesEnabled = false;
        }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseSqlServer(this.DbConnection);
        //    optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        //}
    }
}
