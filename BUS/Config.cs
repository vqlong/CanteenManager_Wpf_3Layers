using Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace BUS
{
    public static class Config
    {
        public static UnityContainer Container { get; private set; } //= new UnityContainer();

        public static IDataProvider DataProvider { get; private set; }

        public static string ConnectionString { get; private set; }

        /// <summary>
        /// Dùng ADO.NET với SQLite.
        /// </summary>
        public static void RegisterSQLite()
        {
            Container = new UnityContainer();

            Container.RegisterInstance<IAccountDAO>(Activator.CreateInstance(typeof(SQLiteDataAccess.AccountDAO), true) as SQLiteDataAccess.AccountDAO, InstanceLifetime.Singleton);
            Container.RegisterInstance<IBillDAO>(SQLiteDataAccess.BillDAO.Instance, InstanceLifetime.Singleton);
            Container.RegisterInstance<IBillDetailDAO>(Activator.CreateInstance(typeof(SQLiteDataAccess.BillDetailDAO), true) as SQLiteDataAccess.BillDetailDAO, InstanceLifetime.Singleton);
            Container.RegisterInstance<IBillInfoDAO>(Activator.CreateInstance(typeof(SQLiteDataAccess.BillInfoDAO), true) as SQLiteDataAccess.BillInfoDAO, InstanceLifetime.Singleton);
            Container.RegisterInstance<ICategoryDAO>(Activator.CreateInstance(typeof(SQLiteDataAccess.CategoryDAO), true) as SQLiteDataAccess.CategoryDAO, InstanceLifetime.Singleton);
            Container.RegisterInstance<IFoodDAO>(SQLiteDataAccess.FoodDAO.Instance, InstanceLifetime.Singleton);
            Container.RegisterInstance<ITableDAO>(Activator.CreateInstance(typeof(SQLiteDataAccess.TableDAO), true) as SQLiteDataAccess.TableDAO, InstanceLifetime.Singleton);

            DataProvider = SQLiteDataAccess.DataProvider.Instance;
            ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["SQLiteConnection"].ConnectionString;
        }

        /// <summary>
        /// Dùng SQL Server.
        /// </summary>
        //public static void RegisterSQLServer()
        //{
        //    Container = new UnityContainer();

        //    Container.RegisterInstance<IAccountDAO>(Activator.CreateInstance(typeof(AccountDAO), true) as AccountDAO, InstanceLifetime.Singleton);
        //    Container.RegisterInstance<IBillDAO>(Activator.CreateInstance(typeof(BillDAO), true) as BillDAO, InstanceLifetime.Singleton);
        //    Container.RegisterInstance<IBillDetailDAO>(Activator.CreateInstance(typeof(BillDetailDAO), true) as BillDetailDAO, InstanceLifetime.Singleton);
        //    Container.RegisterInstance<IBillInfoDAO>(Activator.CreateInstance(typeof(BillInfoDAO), true) as BillInfoDAO, InstanceLifetime.Singleton);
        //    Container.RegisterInstance<ICategoryDAO>(Activator.CreateInstance(typeof(CategoryDAO), true) as CategoryDAO, InstanceLifetime.Singleton);
        //    Container.RegisterInstance<IFoodDAO>(Activator.CreateInstance(typeof(FoodDAO), true) as FoodDAO, InstanceLifetime.Singleton);
        //    Container.RegisterInstance<ITableDAO>(Activator.CreateInstance(typeof(TableDAO), true) as TableDAO, InstanceLifetime.Singleton);

        //    DataProvider = CanteenManager.DAO.DataProvider.Instance;
        //    //ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["SqlServerConnection"].ConnectionString.Replace("{ApplicationFolder}", Application.StartupPath);
        //    ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["SqlServerConnection"].ConnectionString;

        //}

        /// <summary>
        /// Dùng EntityFramework Core với SQL Server.
        /// </summary>
        public static void RegisterEntity()
        {
            Container = new UnityContainer();

            Container.RegisterInstance<IAccountDAO>(Activator.CreateInstance(typeof(EntityDataAccess.AccountDAO), true) as EntityDataAccess.AccountDAO, InstanceLifetime.Singleton);
            Container.RegisterInstance<IBillDAO>(Activator.CreateInstance(typeof(EntityDataAccess.BillDAO), true) as EntityDataAccess.BillDAO, InstanceLifetime.Singleton);
            Container.RegisterInstance<IBillDetailDAO>(Activator.CreateInstance(typeof(EntityDataAccess.BillDetailDAO), true) as EntityDataAccess.BillDetailDAO, InstanceLifetime.Singleton);
            Container.RegisterInstance<IBillInfoDAO>(Activator.CreateInstance(typeof(EntityDataAccess.BillInfoDAO), true) as EntityDataAccess.BillInfoDAO, InstanceLifetime.Singleton);
            Container.RegisterInstance<ICategoryDAO>(Activator.CreateInstance(typeof(EntityDataAccess.CategoryDAO), true) as EntityDataAccess.CategoryDAO, InstanceLifetime.Singleton);
            Container.RegisterInstance<IFoodDAO>(Activator.CreateInstance(typeof(EntityDataAccess.FoodDAO), true) as EntityDataAccess.FoodDAO, InstanceLifetime.Singleton);
            Container.RegisterInstance<ITableDAO>(Activator.CreateInstance(typeof(EntityDataAccess.TableDAO), true) as EntityDataAccess.TableDAO, InstanceLifetime.Singleton);

            DataProvider = EntityDataAccess.DataProvider.Instance;
            ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["SqlServerConnection"].ConnectionString;
        }
    }
}
