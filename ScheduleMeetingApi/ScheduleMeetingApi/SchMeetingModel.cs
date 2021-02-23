using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace ScheduleMeetingApi
{
    public class SchMeetingModel
    {
        public int Id { get; set; }
        public string title { get; set; }

        public string start { get; set; }
        public string end { get; set; }


        internal AppDb Db { get; set; }
        public SchMeetingModel()
        {
        }
        internal SchMeetingModel(AppDb db)
        {
            Db = db;
        }

        public async Task InsertAsync()
        {
            using var cmd = Db.Connection.CreateCommand();
            cmd.CommandText = @"INSERT INTO `schmeeting` (`title`, `startStr`, `endStr`) VALUES (@title, @startStr, @endStr);";
            BindParams(cmd);
            await cmd.ExecuteNonQueryAsync();
            Id = (int)cmd.LastInsertedId;
        }

        private void BindParams(MySqlCommand cmd)
        {
            cmd.Parameters.Add(new MySqlParameter
            {
                ParameterName = "@title",
                DbType = DbType.String,
                Value = HttpUtility.HtmlEncode(title),
            });
            cmd.Parameters.Add(new MySqlParameter
            {
                ParameterName = "@startStr",
                DbType = DbType.String,
                Value = HttpUtility.HtmlEncode(start),
            });
            cmd.Parameters.Add(new MySqlParameter
            {
                ParameterName = "@endStr",
                DbType = DbType.String,
                Value = HttpUtility.HtmlEncode(end),
            });

        }

        public async Task<List<SchMeetingModel>> LatestPostsAsync()
        {
            using var cmd = Db.Connection.CreateCommand();
            cmd.CommandText = @"SELECT `Id`, `title`, `startStr`, `endStr` FROM `schmeeting` ORDER BY `Id` ASC;";
            return await ReadAllAsync(await cmd.ExecuteReaderAsync());
        }

        private async Task<List<SchMeetingModel>> ReadAllAsync(DbDataReader reader)
        {
            var posts = new List<SchMeetingModel>();
            using (reader)
            {
                while (await reader.ReadAsync())
                {
                    var post = new SchMeetingModel(Db)
                    {
                        Id = reader.GetInt32(0),
                        title = reader.GetString(1),
                        start = reader.GetString(2),
                        end = reader.GetString(3)


                    };
                    posts.Add(post);
                }
            }
            return posts;
        }
    }
}
