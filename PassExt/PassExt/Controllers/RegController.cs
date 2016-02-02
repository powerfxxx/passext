using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Security.AccessControl;
using System.Web.Http;
using System.Web.Http.Results;
using Microsoft.Ajax.Utilities;
using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;
using PassExt.Models;


namespace PassExt.Controllers
{
    public class RegController : ApiController
    {
        //Маршруты
        public IEnumerable<Way> GetWays()
        {
            List<Way> result = new List<Way>();
            Connector connector = new Connector();

            #region SQL

            string commandLine = "SET @@lc_time_names='ru_RU';";
            commandLine +=
                @"select w.id, w.name, w.goback, w.town_from, w.dflag, w.town_to, tt.name town_to_name, tf.name town_from_name
                                from ways w 
                                left join towns tt on tt.id = w.town_to 
                                left join towns tf on tf.id = w.town_from where w.dflag='a'";

            #endregion

            using (MySqlConnection connect = new MySqlConnection(connector.ConnectionString))
            using (MySqlCommand cmd = new MySqlCommand(commandLine, connect))
            {
                connect.Open();
                IDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    result.Add(WayFromReader(rdr));
                }
                connect.Close();
            }
            return result;
        }

        private Way WayFromReader(IDataReader dataReader)
        {
            return new Way()
            {
                ID = (int) dataReader["id"],
                Name = dataReader["name"].ToString(),
                TownFrom_id = dataReader["town_from"].ToString(),
                TownFrom_Name = dataReader["town_from_name"].ToString(),
                TownTo_id = dataReader["town_to"].ToString(),
                TownTo_Name = dataReader["town_to_name"].ToString(),
                GoBack = dataReader["goback"].ToString(),
                dflag = dataReader["dflag"].ToString(),
            };
        }

        //Время отправления
        [Route("api/reg/gettimes/{forDate}/{wayId}")]
        public IEnumerable<Schedule> GetTimes(string forDate, string wayId)
        {
            List<Schedule> result = new List<Schedule>();
            Connector connector = new Connector();

            #region SQL

            string commandLine = "SET @@lc_time_names='ru_RU';";
            commandLine += @"select
                            sch.id,
                            sch.way_id,
                            date_format(sch.start_time, '%H:%i') start_time,
                            date_format(sch.end_time, '%H:%i') end_time,
                            sch.days,
                            date_format(sch.udate, '%d.%m.%Y') udate,
                            sch.sequence_id,
                            sch.dflag,
                            ws.goback
                            from schedule sch
                            left join ways ws on sch.way_id = ws.id
                            where 
                            (sch.udate = STR_TO_DATE('" + forDate + @"', '%d.%m.%Y')
                            or substring(sch.days, (select WEEKDAY(STR_TO_DATE('" + forDate +
                           @"', '%d.%m.%Y')) from dual)+1, 1) = '1') and sch.dflag = 'a' and ws.id = " + wayId;

            #endregion

            using (MySqlConnection connect = new MySqlConnection(connector.ConnectionString))
            using (MySqlCommand cmd = new MySqlCommand(commandLine, connect))
            {
                connect.Open();
                IDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    Schedule schedule = ScheduleFromReader(rdr);
                    if (schedule.Sequence_ID != "")
                    {
                        schedule.Sequence = GetSequence(schedule.Sequence_ID);
                        schedule.Sequence.DayStr = schedule.Days;
                        StartSequence.SeqElem SeqRes = schedule.Sequence.GetSeqTime(DateTime.Parse(forDate));
                        schedule.Start_time = SeqRes.StartTime;
                        schedule.End_time = SeqRes.EndTime;
                    }
                    result.Add(schedule);
                }
                connect.Close();
            }
            return result;
        }

        //Возвращает точки с заполненным временем по ID
        [Route("api/reg/getpointstime")]
        [HttpPost]
        public IEnumerable<Point> GetPointsTime(JObject obj)
        {
            dynamic json = obj;
            List<Point> points = new List<Point>();
            try
            {
                string[] split = json.pointIDs.ToString().Split(',');
                foreach (string s in split)
                {
                    if (s.Trim() != "")
                        points.Add(new Point()
                        {
                            ID = Int32.Parse(s),
                            Name = SharedObjects.Points.First(p => p.ID == Int32.Parse(s)).Name,
                            TakeTime = new Mapper().GetPointTime(json.scheduleId.ToString(), s, json.dateText.ToString())
                        });
                }
            }
            catch
            {
                return points;
            }
            return points;
        }

        private Schedule ScheduleFromReader(IDataReader dataReader)
        {
            return new Schedule()
            {
                ID = dataReader["id"].ToString(),
                WayID = dataReader["way_id"].ToString(),
                Start_time = dataReader["start_time"].ToString(),
                End_time = dataReader["end_time"].ToString(),
                Days = dataReader["days"].ToString(),
                UDate = dataReader["udate"].ToString(),
                GoBack = dataReader["goback"].ToString(),
                Sequence_ID = dataReader["sequence_id"].ToString(),
                dflag = dataReader["dflag"].ToString()
            };
        }

        //Гибкое расписание
        private StartSequence GetSequence(string id)
        {
            StartSequence result = new StartSequence();
            Connector connector = new Connector();

            #region SQL

            string commandLine = "SET @@lc_time_names='ru_RU';";
            commandLine += @"select
                                                    seq.id,
                                                    date_format(seq.first_date, '%d.%m.%Y') first_date,
                                                    seqelem.number,
                                                    date_format(seqelem.start_time, '%H:%i') start_time,
                                                    date_format(seqelem.end_time, '%H:%i') end_time
                                                    from sequences seq
                                                    left join seqelem on seqelem.sequence_id = seq.id
                                                    where seq.id=" + id + @" 
                                                    order by seqelem.number";

            #endregion

            using (MySqlConnection connect = new MySqlConnection(connector.ConnectionString))
            using (MySqlCommand cmd = new MySqlCommand(commandLine, connect))
            {
                bool flag = false;
                connect.Open();
                IDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    if (!flag)
                    {
                        result = SequenceFromReader(rdr);
                        flag = true;
                    }
                    result.Elements.Add(new StartSequence.SeqElem()
                    {
                        Number = (int) rdr["number"],
                        StartTime = rdr["start_time"].ToString(),
                        EndTime = rdr["end_time"].ToString()
                    });
                }
                connect.Close();
            }
            return result;
        }

        private StartSequence SequenceFromReader(IDataReader dataReader)
        {
            return new StartSequence()
            {
                ID = dataReader["id"].ToString(),
                FirstDate = DateTime.Parse(dataReader["first_date"].ToString())
            };
        }

        //Точки сбора
        [Route("api/reg/gettownpoints/{town_id}")]
        public IEnumerable<Point> GetTownPoints(string town_id)
        {
            List<Point> result = new List<Point>();
            Connector connector = new Connector();

            #region SQL

            string commandLine = "SET @@lc_time_names='ru_RU';";
            commandLine +=
                @"select
                    p.id,
                    p.name,
                    p.pgroup,
                    p.sortindex,
                    p.dflag
                 from points p
                 left join pgroups pg on pg.id = p.pgroup
                 where pg.town_id = " + town_id + " and p.dflag = 'a' and p.dflag = 'a'" +
                "order by p.name";

            #endregion

            using (MySqlConnection connect = new MySqlConnection(connector.ConnectionString))
            using (MySqlCommand cmd = new MySqlCommand(commandLine, connect))
            {
                connect.Open();
                IDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    result.Add(PointFromReader(rdr));
                }
                connect.Close();
            }
            return result;
        }

        private Point PointFromReader(IDataReader dataReader)
        {
            try
            {
                return new Point()
                {
                    ID = (int) dataReader["id"],
                    Name = dataReader["name"].ToString(),
                    Group = null,
                    SortIndex = (int) dataReader["sortindex"],
                    dflag = dataReader["dflag"].ToString()
                };
            }
            catch
            {
                return null;
            }
        }

        [Route("api/reg/insertorder/{order}")]
        public Response InsertOrder([FromBody] Order order)
        {
            Response validResult = ValidateOrder(order);
            if (validResult.Status != "ok")
            {
                return validResult;
            }
            
            Connector connector = new Connector();
            string commandLine = @"insert into orders (schedule_id, way_id, comment, customer_fam, customer_tel, tripdate, start_time) values (@schedule_id, @way_id, @comment, @customer_fam, @customer_tel, @tripdate, @start_time);
                                  select last_insert_id();";

            using (MySqlConnection con = new MySqlConnection(connector.ConnectionString))
            {
                con.Open();
                using (MySqlTransaction trans = con.BeginTransaction())
                {
                    string last_id; //ID вставленного заказа для записи точкам
                    try
                    {
                        //Запись заказа
                        using (MySqlCommand cmd = new MySqlCommand(commandLine, con, trans))
                        {
                            
                            cmd.Parameters.Add("@schedule_id", MySqlDbType.Int32).Value = order.ScheduleId;
                            cmd.Parameters.Add("@way_id", MySqlDbType.Int32).Value =  order.WayId;
                            cmd.Parameters.Add("@comment", MySqlDbType.VarChar).Value = "";
                            cmd.Parameters.Add("@customer_fam", MySqlDbType.VarChar).Value = order.Fam;
                            cmd.Parameters.Add("@customer_tel", MySqlDbType.VarChar).Value = order.Tel;
                            DateTime orderDate = DateTime.ParseExact(order.Date, "dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture);
                            cmd.Parameters.Add("@tripdate", MySqlDbType.Date).Value = orderDate;
                            TimeSpan orderTime = TimeSpan.Parse(order.StartTime);
                            cmd.Parameters.Add("@start_time", MySqlDbType.Time).Value = orderTime;

                            string query = cmd.CommandText;

                            foreach (MySqlParameter p in cmd.Parameters)
                            {
                                query = query.Replace(p.ParameterName, p.Value.ToString());
                            }

                            last_id = cmd.ExecuteScalar().ToString();
                            cmd.Parameters.Clear();
                        }
                        //Запись точек

                        commandLine = @"insert into order_points (order_id, point_id, count, time, comment) values(@order_id, @point_id, @count, @time, @comment)";

                        using (MySqlCommand cmd = new MySqlCommand(commandLine, con, trans))
                        {
                            foreach (var p in order.Points)
                            {
                                cmd.Parameters.Clear();
                                cmd.Parameters.Add("@order_id", MySqlDbType.Int32).Value = last_id;
                                cmd.Parameters.Add("@point_id", MySqlDbType.Int32).Value = p.Id;
                                cmd.Parameters.Add("@count", MySqlDbType.Int32).Value = p.Count;
                                TimeSpan time = TimeSpan.Parse(p.Time);
                                cmd.Parameters.Add("@time", MySqlDbType.Time).Value = time;
                                cmd.ExecuteNonQuery();
                            }
                            cmd.Parameters.Clear();
                        }
                        trans.Commit();
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        return new Response() { Message = "Ошибка записи заказа. Пожалуйста проверьте корректность заполнения информации по заказу.", Status = "err" };
                    }
                }
            }

            return new Response() { Status = "ok", Message = "Заказ принят! Вскоре Вы получите СМС уведомление с деталями заказа! Желаем Вам приятного пути!"};
        }

        public Response ValidateOrder(Order order)
        {
            int temp;
            if (!Int32.TryParse(order.WayId, out temp))
            {
                return new Response() { Message = "Укажите направление поездки. Шаг 1.", Status = "err" };
            }
            try { DateTime orderDate = DateTime.ParseExact(order.Date, "dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture); }
            catch { return new Response() { Message = "Укажите дату поездки. Шаг 2.", Status = "err" }; }
            if (!Int32.TryParse(order.ScheduleId, out temp))
            {
                return new Response() { Message = "Укажите время поездки. Шаг 3.", Status = "err" };
            }
            try { TimeSpan time = TimeSpan.Parse(order.StartTime); }
            catch { return new Response() { Message = "Укажите время поездки. Шаг 3.", Status = "err" }; }
            if (!order.Points.Any()) return new Response() { Message = "Укажите хотя бы одну точку сбора и число пассажиров. Шаг 4.", Status = "err" };
            if (order.Fam.Length < 2) return new Response() { Message = "Укажите Вашу фамилию. Шаг 5.", Status = "err" };
            if (order.Tel.Length < 12) return new Response() { Message = "Укажите номер Вашего мобильного телефона. Шаг 5.", Status = "err" };
            return new Response() { Message = "Заказ корректный", Status = "ok" };
            
        }
    }
}