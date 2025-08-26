#nullable disable
using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SocialMediaDataScraper.Common
{
    public static class DbHelper
    {
        private static string databaseName = @"Filename=D:\04_Practice\SocialMediaDataScraper\SocialMediaDataScraper\database.db;Connection=shared";

        public static bool Save<T>(T model) where T : class
        {
            using var db = new LiteDatabase(databaseName);
            var col = db.GetCollection<T>();
            var res = col.Insert(model);
            return res != null;
        }

        public static bool SaveMany<T>(List<T> model) where T : class
        {
            using var db = new LiteDatabase(databaseName);
            var col = db.GetCollection<T>();
            var res = col.InsertBulk(model);
            return res > 0;
        }

        public static bool Delete<T>(ObjectId id) where T : class
        {
            using var db = new LiteDatabase(databaseName);
            var col = db.GetCollection<T>();
            var res = col.Delete(id);
            return res;
        }

        public static List<T> GetAll<T>()
        {
            using var db = new LiteDatabase(databaseName);
            var col = db.GetCollection<T>();
            var res = col.FindAll().ToList();
            return res;
        }
    }
}
