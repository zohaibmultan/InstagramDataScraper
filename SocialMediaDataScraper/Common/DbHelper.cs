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
        private static string databaseName = @"Filename=D:\04_Practice\SocialMediaDataScraper\SocialMediaDataScraper\database.db;Connection=shared";

        public static bool Save<T>(T model, Expression<Func<T, bool>> condition = null) where T : class
        {
            using var db = new LiteDatabase(databaseName);
            var col = db.GetCollection<T>();

            if (condition != null)
            {
                var existing = col.FindOne(condition);
                if (existing != null)
                {
                    // update -> preserve Id
                    var idProp = typeof(T).GetProperties().FirstOrDefault(p => Attribute.IsDefined(p, typeof(BsonIdAttribute)) || p.Name.Equals("Id", StringComparison.OrdinalIgnoreCase));

                    if (idProp != null)
                    {
                        var idVal = idProp.GetValue(existing);
                        if (idVal != null)
                        {
                            idProp.SetValue(model, idVal);
                        }
                    }

                    return col.Update(model);
                }
            }

            var res = col.Insert(model);
            return res != null;
        }

        public static bool SaveMany<T>(List<T> models, Expression<Func<T, bool>>? condition = null) where T : class
        {
            using var db = new LiteDatabase(databaseName);
            var col = db.GetCollection<T>();

            bool anySaved = false;

            foreach (var model in models)
            {
                if (condition != null)
                {
                    var existing = col.FindOne(condition);
                    if (existing != null)
                    {
                        var idProp = typeof(T).GetProperties()
                                              .FirstOrDefault(p => Attribute.IsDefined(p, typeof(BsonIdAttribute)) || p.Name.Equals("Id", StringComparison.OrdinalIgnoreCase));

                        if (idProp != null)
                        {
                            var idVal = idProp.GetValue(existing);
                            if (idVal != null)
                            {
                                idProp.SetValue(model, idVal);
                            }
                        }

                        anySaved |= col.Update(model);
                        continue;
                    }
                }

                var res = col.Insert(model);
                if (res != null) anySaved = true;
            }

            return anySaved;
        }

        public static bool Delete<T>(ObjectId id) where T : class
        {
            using var db = new LiteDatabase(databaseName);
            var col = db.GetCollection<T>();
            return col.Delete(id);
        }

        public static List<T> GetAll<T>()
        {
            using var db = new LiteDatabase(databaseName);
            var col = db.GetCollection<T>();
            return col.FindAll().ToList();
        }
    }

}
