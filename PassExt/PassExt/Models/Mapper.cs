using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using MySql.Data.MySqlClient;
using PassExt.Models;
using System.Xml.Linq;
using System.Configuration;

namespace PassExt.Models
{
    public class Mapper : Connector
    {
        public Mapper()
        {

        }

        #region Справочники

        #region Справочник маршрутов

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
                dflag = Functions.TranslateDFlag(dataReader["dflag"].ToString())
            };
        }

        public void GetWays()
        {
            ConnectToDatabase(ConnectionString);
            try
            {
                IDbCommand Command =
                    new MySqlCommand(
                        "select w.id, w.name, w.goback, w.town_from, w.dflag, w.town_to, tt.name town_to_name, tf.name town_from_name  from ways w left join towns tt on tt.id = w.town_to left join towns tf on tf.id = w.town_from",
                        (MySqlConnection) DatabaseConnection);
                IDataReader ResultReader = ExecuteReadCommand(Command);
                SharedObjects.Ways.Clear();
                while (ResultReader.Read())
                {
                    SharedObjects.Ways.Add(WayFromReader(ResultReader));
                }
            }
            catch
            {

            }
            finally
            {
                DisconnectFromDatabase();
            }
        }

        public string InsertWay(string Name, string dflag, string GoBack, string TownFromID, string TownToID)
        {
            this.ConnectToDatabase(ConnectionString);
            MySqlTransaction transaction = ((MySqlConnection) DatabaseConnection).BeginTransaction();
            string insertText =
                String.Format(
                    "insert into ways (name, goback, dflag, town_from, town_to) values ('{0}','{1}','{2}', {3}, {4})",
                    Name, GoBack, dflag, TownFromID, TownToID);
            int insRows = 0;
            try
            {
                IDbCommand InsCom = new MySqlCommand(insertText, (MySqlConnection) DatabaseConnection, transaction);
                insRows = InsCom.ExecuteNonQuery();
                transaction.Commit();
                this.DisconnectFromDatabase();
                this.GetWays();
                return insRows.ToString();
            }
            catch (Exception e)
            {
                transaction.Rollback();
                this.DisconnectFromDatabase();
                return e.Message;
            }
        }

        public string UpdateWay(string id, string Name, string dflag, string GoBack, string TownFromID, string TownToID)
        {
            this.ConnectToDatabase(ConnectionString);
            MySqlTransaction transaction = ((MySqlConnection) DatabaseConnection).BeginTransaction();
            string UpdateText =
                String.Format(
                    "update ways set name = '{0}', goback = '{3}', dflag='{2}', town_from={4}, town_to={5} where id = {1}",
                    Name, id, dflag, GoBack, TownFromID, TownToID);
            int insRows = 0;
            try
            {
                IDbCommand InsCom = new MySqlCommand(UpdateText, (MySqlConnection) DatabaseConnection, transaction);
                insRows = InsCom.ExecuteNonQuery();
                transaction.Commit();
                this.DisconnectFromDatabase();
                this.GetWays();
                return insRows.ToString();
            }
            catch (Exception e)
            {
                transaction.Rollback();
                this.DisconnectFromDatabase();
                return e.Message;
            }
        }

        #endregion

        #region Справочник Точек сбора

        private Point PointFromReader(IDataReader dataReader)
        {
            try
            {
                return new Point()
                {
                    ID = (int) dataReader["id"],
                    Name = dataReader["name"].ToString(),
                    Group = SharedObjects.PGroups.Where(p => p.ID == (int) (dataReader["pgroup"])).Max(),
                    SortIndex = (int) dataReader["sortindex"],
                    CorX = (int) dataReader["corx"],
                    CorY = (int) dataReader["cory"],
                    dflag = Functions.TranslateDFlag(dataReader["dflag"].ToString())
                };
            }
            catch
            {
                return null;
            }
        }

        public void GetPoints()
        {
            ConnectToDatabase(ConnectionString);
            IDbCommand Command = new MySqlCommand("select * from points", (MySqlConnection) DatabaseConnection);
            IDataReader ResultReader = ExecuteReadCommand(Command);
            SharedObjects.Points.Clear();
            while (ResultReader.Read())
            {
                SharedObjects.Points.Add(PointFromReader(ResultReader));
            }
            DisconnectFromDatabase();
        }

        public string InsertPoint(string Name, string Group, string dflag)
        {
            this.ConnectToDatabase(ConnectionString);
            MySqlTransaction transaction = ((MySqlConnection) DatabaseConnection).BeginTransaction();
            string insertText = String.Format("insert into points(name, pgroup, dflag) values('{0}','{1}','{2}')", Name,
                Group, dflag);
            int insRows = 0;
            try
            {
                IDbCommand InsCom = new MySqlCommand(insertText, (MySqlConnection) DatabaseConnection, transaction);
                insRows = InsCom.ExecuteNonQuery();
                transaction.Commit();
                this.DisconnectFromDatabase();
                this.GetPoints();
                return insRows.ToString();
            }
            catch (Exception e)
            {
                transaction.Rollback();
                this.DisconnectFromDatabase();
                return e.Message;
            }
        }

        public string UpdatePoint(string id, string Name, string Group, string dflag)
        {
            this.ConnectToDatabase(ConnectionString);
            MySqlTransaction transaction = ((MySqlConnection) DatabaseConnection).BeginTransaction();
            string UpdateText =
                String.Format("update points set name = '{1}', pgroup = '{2}', dflag='{3}' where id = {0}", id, Name,
                    Group, dflag);
            int insRows = 0;
            try
            {
                IDbCommand InsCom = new MySqlCommand(UpdateText, (MySqlConnection) DatabaseConnection, transaction);
                insRows = InsCom.ExecuteNonQuery();
                transaction.Commit();
                this.DisconnectFromDatabase();
                this.GetPoints();
                return insRows.ToString();
            }
            catch (Exception e)
            {
                transaction.Rollback();
                this.DisconnectFromDatabase();
                return e.Message;
            }
        }

        public string SortPoints(string ids, string Group)
        {
            string[] split = ids.Split(',');
            int count = 0;
            string sql = "UPDATE points SET sortindex = CASE id ";
            foreach (string s in split)
            {
                count++;
                sql += "WHEN " + s + " THEN " + count.ToString() + " ";
            }
            sql += "ELSE sortindex END WHERE pgroup = " + Group;

            this.ConnectToDatabase(ConnectionString);
            MySqlTransaction transaction = ((MySqlConnection) DatabaseConnection).BeginTransaction();
            try
            {
                IDbCommand InsCom = new MySqlCommand(sql, (MySqlConnection) DatabaseConnection, transaction);
                InsCom.ExecuteNonQuery();
                transaction.Commit();
                this.DisconnectFromDatabase();
                this.GetPoints();
                return "ok";
            }
            catch (Exception e)
            {
                transaction.Rollback();
                this.DisconnectFromDatabase();
                return e.Message;
            }
        }

        public string UpdatePointMapCoors(string id, string CorX, string CorY)
        {
            this.ConnectToDatabase(ConnectionString);
            MySqlTransaction transaction = ((MySqlConnection) DatabaseConnection).BeginTransaction();
            string UpdateText = String.Format("update points set corx = {1}, cory = {2} where id = {0}", id, CorX, CorY);
            try
            {
                IDbCommand InsCom = new MySqlCommand(UpdateText, (MySqlConnection) DatabaseConnection, transaction);
                InsCom.ExecuteNonQuery();
                transaction.Commit();
                this.DisconnectFromDatabase();
                this.GetPoints();
                return "ok";
            }
            catch (Exception e)
            {
                transaction.Rollback();
                this.DisconnectFromDatabase();
                return e.Message;
            }
        }

        #endregion

        #region Справочник пунктов отправлений/прибытий

        private Town TownFromReader(IDataReader dataReader)
        {
            return new Town()
            {
                ID = (int) dataReader["id"],
                Name = dataReader["name"].ToString(),
                Map = dataReader["map"].ToString(),
                dflag = Functions.TranslateDFlag(dataReader["dflag"].ToString())
            };
        }

        public void GetTowns()
        {
            try
            {
                ConnectToDatabase(ConnectionString);
                IDbCommand Command = new MySqlCommand("select * from Towns", (MySqlConnection) DatabaseConnection);
                IDataReader ResultReader = ExecuteReadCommand(Command);
                SharedObjects.Towns.Clear();
                while (ResultReader.Read())
                {
                    SharedObjects.Towns.Add(TownFromReader(ResultReader));
                }
            }
            catch (Exception e)
            {
            }
            finally
            {
                DisconnectFromDatabase();
            }
        }

        public string InsertTown(string Name, string dflag)
        {
            this.ConnectToDatabase(ConnectionString);
            MySqlTransaction transaction = ((MySqlConnection) DatabaseConnection).BeginTransaction();
            string insertText = String.Format("insert into Towns (name, dflag) values ('{0}','{1}')", Name, dflag);
            int insRows = 0;
            try
            {
                IDbCommand InsCom = new MySqlCommand(insertText, (MySqlConnection) DatabaseConnection, transaction);
                insRows = InsCom.ExecuteNonQuery();
                transaction.Commit();
                this.DisconnectFromDatabase();
                this.GetTowns();
                return insRows.ToString();
            }
            catch (Exception e)
            {
                transaction.Rollback();
                this.DisconnectFromDatabase();
                return e.Message;
            }
        }

        public string UpdateTown(string id, string Name, string dflag)
        {
            this.ConnectToDatabase(ConnectionString);
            MySqlTransaction transaction = ((MySqlConnection) DatabaseConnection).BeginTransaction();
            string UpdateText = String.Format("update Towns set name = '{0}', dflag='{2}' where id = {1}", Name, id,
                dflag);
            int insRows = 0;
            try
            {
                IDbCommand InsCom = new MySqlCommand(UpdateText, (MySqlConnection) DatabaseConnection, transaction);
                insRows = InsCom.ExecuteNonQuery();
                transaction.Commit();
                this.DisconnectFromDatabase();
                this.GetTowns();
                return insRows.ToString();
            }
            catch (Exception e)
            {
                transaction.Rollback();
                this.DisconnectFromDatabase();
                return e.Message;
            }
        }

        #endregion

        #region Справочник групп точек сбора

        private PGroup PGroupFromReader(IDataReader dataReader)
        {
            return new PGroup()
            {
                ID = (int) dataReader["id"],
                Name = dataReader["name"].ToString(),
                TownID = dataReader["town_id"].ToString(),
                TownName = dataReader["tname"].ToString(),
                dflag = Functions.TranslateDFlag(dataReader["dflag"].ToString())
            };
        }

        public void GetPGroups()
        {
            try
            {
                ConnectToDatabase(ConnectionString);
                IDbCommand Command =
                    new MySqlCommand(
                        "select pg.id, pg.name, pg.dflag, pg.town_id, t.name tname from PGroups pg left join towns t on pg.town_id = t.id",
                        (MySqlConnection) DatabaseConnection);
                IDataReader ResultReader = ExecuteReadCommand(Command);
                SharedObjects.PGroups.Clear();
                while (ResultReader.Read())
                {
                    SharedObjects.PGroups.Add(PGroupFromReader(ResultReader));
                }
            }
            catch (Exception e)
            {
            }
            finally
            {
                DisconnectFromDatabase();
            }
        }

        public string InsertPGroup(string Name, string town_id, string dflag)
        {
            this.ConnectToDatabase(ConnectionString);
            MySqlTransaction transaction = ((MySqlConnection) DatabaseConnection).BeginTransaction();
            string insertText = String.Format("insert into PGroups (name, town_id, dflag) values ('{0}','{1}','{2}')",
                Name, town_id, dflag);
            int insRows = 0;
            try
            {
                IDbCommand InsCom = new MySqlCommand(insertText, (MySqlConnection) DatabaseConnection, transaction);
                insRows = InsCom.ExecuteNonQuery();
                transaction.Commit();
                this.DisconnectFromDatabase();
                this.GetPGroups();
                return insRows.ToString();
            }
            catch (Exception e)
            {
                transaction.Rollback();
                this.DisconnectFromDatabase();
                return e.Message;
            }
        }

        public string UpdatePGroup(string id, string Name, string town_id, string dflag)
        {
            this.ConnectToDatabase(ConnectionString);
            MySqlTransaction transaction = ((MySqlConnection) DatabaseConnection).BeginTransaction();
            string UpdateText = String.Format(
                "update PGroups set name = '{0}', town_id={3}, dflag='{2}' where id = {1}", Name, id, dflag, town_id);
            int insRows = 0;
            try
            {
                IDbCommand InsCom = new MySqlCommand(UpdateText, (MySqlConnection) DatabaseConnection, transaction);
                insRows = InsCom.ExecuteNonQuery();
                transaction.Commit();
                this.DisconnectFromDatabase();
                this.GetPGroups();
                return insRows.ToString();
            }
            catch (Exception e)
            {
                transaction.Rollback();
                this.DisconnectFromDatabase();
                return e.Message;
            }
        }

        #endregion

        #region Справочник "Расписание"

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
                dflag = Functions.TranslateDFlag(dataReader["dflag"].ToString())
            };
        }

        private StartSequence SequenceFromReader(IDataReader dataReader)
        {
            return new StartSequence()
            {
                ID = dataReader["id"].ToString(),
                FirstDate = DateTime.Parse(dataReader["first_date"].ToString())
            };
        }

        public void GetSchedules()
        {
            ConnectToDatabase(ConnectionString);
            IDbCommand Command = new MySqlCommand(@"select 
                                                    sch.id,
                                                    sch.way_id,
                                                    date_format(sch.start_time, '%H:%i') start_time,
                                                    date_format(sch.end_time, '%H:%i') end_time,
                                                    sch.days,
                                                    date_format(sch.udate, '%d.%m.%Y') udate,
                                                    sch.sequence_id,
                                                    sch.dflag,
                                                    sp.schedule_id,
                                                    sp.pgroup_id,
                                                    date_format(sp.taketime, '%H:%i') taketime,
                                                    ws.goback
                                                    from schedule sch
                                                    left join schedule_points sp on sch.id = sp.schedule_id
                                                    left join ways ws on sch.way_id = ws.id
                                                    order by sch.id
                                                    ", (MySqlConnection) DatabaseConnection);
            IDataReader ResultReader = ExecuteReadCommand(Command);
            SharedObjects.Schedules.Clear();
            Schedule curSched = new Schedule();
            bool flag = false;
            while (ResultReader.Read())
            {
                if (curSched.ID != ResultReader["id"].ToString())
                {
                    if (flag) SharedObjects.Schedules.Add(curSched);
                    else flag = true;
                    curSched = ScheduleFromReader(ResultReader);
                }
                if (ResultReader["pgroup_id"].ToString() != "")
                    curSched.PGroups.Add(new PGroup()
                    {
                        ID = (int) ResultReader["pgroup_id"],
                        TakeTime = ResultReader["taketime"].ToString()
                    });
            }
            SharedObjects.Schedules.Add(curSched);

            foreach (Schedule sched in SharedObjects.Schedules)
            {
                ResultReader.Close();
                if (sched.Sequence_ID != null && sched.Sequence_ID != "")
                {
                    Command = new MySqlCommand(@"select
                                                    seq.id,
                                                    date_format(seq.first_date, '%d.%m.%Y') first_date,
                                                    seqelem.number,
                                                    date_format(seqelem.start_time, '%H:%i') start_time,
                                                    date_format(seqelem.end_time, '%H:%i') end_time
                                                    from sequences seq
                                                    left join seqelem on seqelem.sequence_id = seq.id
                                                    where seq.id=" + sched.Sequence_ID + @" 
                                                    order by seqelem.number
                                                    ", (MySqlConnection) DatabaseConnection);

                    StartSequence curSeq = new StartSequence();
                    flag = false;
                    ResultReader = ExecuteReadCommand(Command);
                    while (ResultReader.Read())
                    {
                        if (!flag)
                        {
                            curSeq = SequenceFromReader(ResultReader);
                            curSeq.DayStr = sched.Days;
                            flag = true;
                        }
                        curSeq.Elements.Add(new StartSequence.SeqElem()
                        {
                            Number = (int) ResultReader["number"],
                            StartTime = ResultReader["start_time"].ToString(),
                            EndTime = ResultReader["end_time"].ToString()
                        });
                    }
                    sched.Sequence = curSeq;
                }
            }

            DisconnectFromDatabase();
        }

        public Schedule ScheduleFromXML(string xml)
        {

            XElement element = XElement.Parse(xml);

            Schedule Sched = new Schedule();
            Sched.ID = element.Attribute("ID").Value;
            Sched.WayID = element.Attribute("WayID").Value;
            Sched.Start_time = element.Attribute("StartTime").Value;
            Sched.End_time = element.Attribute("EndTime").Value;
            Sched.Days = element.Attribute("Days").Value;
            Sched.UDate = element.Attribute("UDate").Value;
            Sched.Sequence_ID = element.Attribute("seq_id").Value == "undefined"
                ? ""
                : element.Attribute("seq_id").Value;
            Sched.dflag = element.Attribute("dflag").Value;
            IEnumerable<XElement> PGroups = element.Descendants("pgroup");
            foreach (XElement e in PGroups)
                Sched.PGroups.Add(
                    new PGroup()
                    {
                        ID = Int32.Parse(e.Attribute("PGroupID").Value),
                        TakeTime = e.Attribute("TakeTime").Value
                    });
            if (element.Descendants("seq").Count() != 0)
            {
                XElement Seq = element.Descendants("seq").First();
                Sched.Sequence = new StartSequence() {ID = "", FirstDate = DateTime.Parse(Seq.Attribute("fdate").Value)};
                IEnumerable<XElement> SeqElems = Seq.Descendants("seqel");
                foreach (XElement se in SeqElems)
                    Sched.Sequence.Elements.Add(new StartSequence.SeqElem()
                    {
                        Number = Int16.Parse(se.Attribute("num").Value),
                        StartTime = se.Attribute("st").Value,
                        EndTime = se.Attribute("et").Value
                    });
            }
            return Sched;
        }

        public string InsertSchedule(string xml)
        {
            Schedule Sched = ScheduleFromXML(xml);
            this.ConnectToDatabase(ConnectionString);
            MySqlTransaction transaction = ((MySqlConnection) DatabaseConnection).BeginTransaction();
            try
            {
                int insRows = 0;
                string LastID = "null";
                string insertText = "";
                IDbCommand InsCom;
                int i = 0;
                //График отправлений/прибытий
                if (Sched.Sequence != null)
                {
                    insertText =
                        String.Format(
                            "insert into sequences (first_date) values (STR_TO_DATE('{0}', '%d.%m.%Y')); SELECT LAST_INSERT_ID();",
                            Sched.Sequence.FirstDate.ToString("dd.MM.yyyy"));
                    InsCom = new MySqlCommand(insertText, (MySqlConnection) DatabaseConnection, transaction);
                    LastID = Convert.ToString(InsCom.ExecuteScalar());
                    //Елементы графика отправлений/прибытий
                    insertText = "insert into seqelem (sequence_id, number, start_time, end_time) values";
                    foreach (StartSequence.SeqElem item in Sched.Sequence.Elements)
                    {
                        i++;
                        insertText += "(" + LastID + "," + item.Number.ToString() + ",STR_TO_DATE('" + item.StartTime +
                                      "', '%H:%i'),STR_TO_DATE('" + item.EndTime + "', '%H:%i'))";
                        if (i != Sched.Sequence.Elements.Count)
                        {
                            insertText += ",";
                        }

                    }
                    InsCom = new MySqlCommand(insertText, (MySqlConnection) DatabaseConnection, transaction);
                    insRows = InsCom.ExecuteNonQuery();
                }


                insertText =
                    String.Format(
                        "insert into schedule (way_id, start_time, end_time, days, udate, sequence_id, dflag) values ({0},STR_TO_DATE('{1}', '%H:%i'),STR_TO_DATE('{2}', '%H:%i'), '{5}', STR_TO_DATE('{3}', '%d.%m.%Y'), {6},'{4}'); SELECT LAST_INSERT_ID();",
                        Sched.WayID, Sched.Start_time, Sched.End_time, Sched.UDate, Sched.dflag, Sched.Days, LastID);
                InsCom = new MySqlCommand(insertText, (MySqlConnection) DatabaseConnection, transaction);
                LastID = Convert.ToString(InsCom.ExecuteScalar());

                string InsPGText = "insert into schedule_points (schedule_id, pgroup_id, taketime) values";
                i = 0;
                foreach (PGroup item in Sched.PGroups)
                {
                    i++;
                    InsPGText += "(" + LastID + "," + item.ID.ToString() + ",STR_TO_DATE('" + item.TakeTime +
                                 "', '%H:%i'))";
                    if (i != Sched.PGroups.Count)
                    {
                        InsPGText += ",";
                    }

                }
                InsCom = new MySqlCommand(InsPGText, (MySqlConnection) DatabaseConnection, transaction);
                insRows += InsCom.ExecuteNonQuery();
                transaction.Commit();
                this.DisconnectFromDatabase();
                this.GetSchedules();
                return insRows.ToString();
            }
            catch (Exception e)
            {
                transaction.Rollback();
                this.DisconnectFromDatabase();
                return e.Message;
            }
        }

        public string UpdateSchedule(string xml)
        {
            string result = "";
            string sql = "";
            Schedule Sched = ScheduleFromXML(xml);
            this.ConnectToDatabase(ConnectionString);
            MySqlTransaction transaction = ((MySqlConnection) DatabaseConnection).BeginTransaction();
            IDbCommand Com;
            int irows = 0;
            string LastID = "null";
            int i = 0;
            try
            {
                if (Sched.Sequence_ID != "")
                {
                    sql = "delete from sequences where id=" + Sched.Sequence_ID;
                    Com = new MySqlCommand(sql, (MySqlConnection) DatabaseConnection, transaction);
                    irows = Com.ExecuteNonQuery();
                }
                //График отправлений/прибытий
                if (Sched.Sequence != null)
                {
                    sql =
                        String.Format(
                            "insert into sequences (first_date) values (STR_TO_DATE('{0}', '%d.%m.%Y')); SELECT LAST_INSERT_ID();",
                            Sched.Sequence.FirstDate.ToString("dd.MM.yyyy"));
                    Com = new MySqlCommand(sql, (MySqlConnection) DatabaseConnection, transaction);
                    LastID = Convert.ToString(Com.ExecuteScalar());
                    //Елементы графика отправлений/прибытий
                    sql = "insert into seqelem (sequence_id, number, start_time, end_time) values";

                    foreach (StartSequence.SeqElem item in Sched.Sequence.Elements)
                    {
                        i++;
                        sql += "(" + LastID + "," + item.Number.ToString() + ",STR_TO_DATE('" + item.StartTime +
                               "', '%H:%i'),STR_TO_DATE('" + item.EndTime + "', '%H:%i'))";
                        if (i != Sched.Sequence.Elements.Count)
                        {
                            sql += ",";
                        }

                    }
                    Com = new MySqlCommand(sql, (MySqlConnection) DatabaseConnection, transaction);
                    Com.ExecuteNonQuery();
                }
                sql =
                    String.Format(
                        "update schedule set way_id={6}, start_time=STR_TO_DATE('{0}', '%H:%i'), end_time=STR_TO_DATE('{1}', '%H:%i'), days='{4}', udate=STR_TO_DATE('{2}', '%d.%m.%Y'), dflag='{3}', sequence_id={7} where id={5}",
                        Sched.Start_time, Sched.End_time, Sched.UDate, Sched.dflag, Sched.Days, Sched.ID, Sched.WayID,
                        LastID);
                Com = new MySqlCommand(sql, (MySqlConnection) DatabaseConnection, transaction);
                irows = Com.ExecuteNonQuery();
                result += "Обновлено записей: " + irows.ToString();
                //Удаление Schedule_Points
                sql = String.Format("DELETE FROM schedule_points WHERE schedule_points.schedule_id = {0}", Sched.ID);
                Com = new MySqlCommand(sql, (MySqlConnection) DatabaseConnection, transaction);
                Com.ExecuteNonQuery();
                //Вставка Schedule_Points
                string InsPGText = "insert into schedule_points (schedule_id, pgroup_id, taketime) values";
                i = 0;
                foreach (PGroup item in Sched.PGroups)
                {
                    i++;
                    InsPGText += "(" + Sched.ID + "," + item.ID.ToString() + ",STR_TO_DATE('" + item.TakeTime +
                                 "', '%H:%i'))";
                    if (i != Sched.PGroups.Count)
                    {
                        InsPGText += ",";
                    }

                }
                Com = new MySqlCommand(InsPGText, (MySqlConnection) DatabaseConnection, transaction);
                irows = Com.ExecuteNonQuery();
                transaction.Commit();
                this.GetSchedules();
                return result;
            }
            catch (Exception e)
            {
                transaction.Rollback();
                return e.Message;
            }
            finally
            {
                this.DisconnectFromDatabase();
            }
        }

        #endregion

        #endregion

        //Поиск времени сбора по ID расписания и точки сбора
        public string GetPointTime(string scheduleId, string pointId, string dateText)
        {
            try
            {
                string result = "";
                Schedule sch = SharedObjects.Schedules.First(s => s.ID == scheduleId);
                if (sch != null && sch.Sequence_ID != "")
                {
                    result = sch.Sequence.GetSeqTime(DateTime.Parse(dateText)).StartTime;
                }
                else
                {
                    try
                    {
                        ConnectToDatabase(ConnectionString);
                        IDbCommand command = new MySqlCommand(@"select
                                                        date_format(sp.taketime, '%H:%i') taketime
                                                        from points p
                                                        left join schedule_points sp on p.pgroup = sp.pgroup_id
                                                        where sp.schedule_id = " + scheduleId + " and p.id = " + pointId,
                            (MySqlConnection) DatabaseConnection);
                        IDataReader resultReader = ExecuteReadCommand(command);

                        if (resultReader != null)
                        {
                            while (resultReader.Read())
                            {
                                result = resultReader["taketime"].ToString();
                            }
                            resultReader.Close();
                        }
                        DisconnectFromDatabase();
                    }
                    catch
                    {
                        DisconnectFromDatabase();
                    }
                }
                return result;
            }
            catch
            {
                return "";
            }
        }

        

    }
}