#nullable disable

using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SocialMediaDataScraper.Common
{
    public static class DbHelper
    {
        private static string connectionString = @"Filename=D:\04_Practice\SocialMediaDataScraper\SocialMediaDataScraper\database.db;Connection=shared";

        public static T SaveOne<T>(T model, Expression<Func<T, bool>> condition = null) where T : class
        {
            using var db = new LiteDatabase(connectionString);
            var col = db.GetCollection<T>();

            if (condition != null)
            {
                var existing = col.FindOne(condition);
                if (existing != null)
                {
                    CopyProperties<T>(model, existing);
                    return col.Update(existing) ? existing : null;
                }
            }

            var res = col.Insert(model);
            return res == null ? null : model;
        }

        public static bool SaveMany<T>(List<T> models, Expression<Func<T, bool>> condition = null) where T : class
        {
            using var db = new LiteDatabase(connectionString);
            var col = db.GetCollection<T>();

            bool anySaved = false;

            foreach (var model in models)
            {
                if (condition != null)
                {
                    var existing = col.FindOne(condition);
                    if (existing != null)
                    {
                        CopyProperties<T>(model, existing);
                        anySaved |= col.Update(model);
                        continue;
                    }
                }

                var res = col.Insert(model);
                if (res != null) anySaved = true;
            }

            return anySaved;
        }

        public static bool UpdateOne<T>(T model, Expression<Func<T, bool>> condition = null) where T : class
        {
            using var db = new LiteDatabase(connectionString);
            var col = db.GetCollection<T>();
            return col.Update(model);
        }

        public static bool UpdateMany<T>(List<T> models, Expression<Func<T, bool>> condition = null) where T : class
        {
            using var db = new LiteDatabase(connectionString);
            var col = db.GetCollection<T>();

            int updatedCount = 0;

            if (condition == null)
            {
                foreach (var model in models)
                {
                    if (col.Update(model)) updatedCount++;
                }
            }
            else
            {
                var existing = col.Find(condition).ToList();
                foreach (var model in models)
                {
                    if (existing.Any(x => x.Equals(model)) && col.Update(model)) updatedCount++;
                }
            }

            return updatedCount == models.Count;
        }

        public static bool Delete<T>(ObjectId id) where T : class
        {
            using var db = new LiteDatabase(connectionString);
            var col = db.GetCollection<T>();
            return col.Delete(id);
        }

        public static bool DeleteMany<T>(Expression<Func<T, bool>> condition) where T : class
        {
            using var db = new LiteDatabase(connectionString);
            var col = db.GetCollection<T>();
            var res = col.DeleteMany(condition);
            return res >= 0;
        }

        public static List<T> GetAll<T>() where T : class
        {
            using var db = new LiteDatabase(connectionString);
            var col = db.GetCollection<T>();
            return col.FindAll().ToList();
        }

        public static T GetOne<T>(Expression<Func<T, bool>> condition) where T : class
        {
            using var db = new LiteDatabase(connectionString);
            var col = db.GetCollection<T>();
            return col.FindOne(condition);
        }

        public static List<T> Get<T>(Expression<Func<T, bool>> condition) where T : class
        {
            using var db = new LiteDatabase(connectionString);
            var col = db.GetCollection<T>();
            return col.Find(condition).ToList();
        }

        public static void CopyProperties<T>(T source, T target) where T : class
        {
            var props = typeof(T).GetProperties().Where(p => p.CanRead && p.CanWrite && p.Name != "_id");

            foreach (var prop in props)
            {
                var value = prop.GetValue(source, null);
                prop.SetValue(target, value, null);
            }
        }
    }
}
