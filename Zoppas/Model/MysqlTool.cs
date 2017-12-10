namespace Zoppas.Model
{
    using System;
    using System.Collections.Generic;
    using MySql.Data.MySqlClient;
    using System.Data;

    public class coffeepot
    {

    }
    public struct insertRecordStr
    {
        public string insertItemName;//检测项名
        public string insertItemValue;//检测项值            
    }

    public class MySQLTool
    {
        private MySQLTool()
        {
            // MySqlConnection myConn = new MySqlConnection();
        }

        private static MySQLTool MySqlInstance;
        public static MySQLTool MySql
        {
            get
            {
                if (MySqlInstance == null)
                    MySqlInstance = new MySQLTool();
                return MySqlInstance;
            }
        }

        //连接状态;true打开false关闭
        public bool SqlState
        {
            get
            {
                if (myConn != null)
                {
                    if (myConn.State == ConnectionState.Open)
                        return true;
                    else
                        return false;
                }
                else
                    return false;
            }
        }

        //建立连接
        private MySqlConnection myConn;
        /// <summary>
        /// 数据库连接
        /// </summary>
        /// <param name="server"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="database"></param>
        /// <returns>0:连接成功; -1: 连接失败</returns>
        public int MySqlConn(string server, string username, string password, string database)
        {
            string connStr = "server=" + server + ";user Id=" + username + ";password=" + password + ";Database=" + database + "";
            try
            {
                myConn = new MySqlConnection(connStr);
                myConn.Open();
                if (myConn.State == ConnectionState.Open)
                    return 0;
                else
                    return -1;
            }
            catch
            {
                return -1;
            }
        }
        /// <summary>
        /// 插入检测记录
        /// </summary>
        /// <param name="coffeepotTable"></param>
        /// <param name="coffeepotType"></param>
        /// <param name="checkResult"></param>
        /// <param name="record"></param>
        /// <returns>true:插入成功;false:插入失败</returns>
        public bool InsertNewRow(string coffeepotTable, string coffeepotType, string checkResult, List<insertRecordStr> record)
        {
            if (!SqlState)
                return false;
            string str1 = "insert into " + coffeepotTable + " (Type,Result";
            for (int i = 0; i < record.Count; i++)
            {
                string str2 = record[i].insertItemName;
                str2 = "," + str2;
                str1 += str2;
            }
            string commStr = str1 + ")values ('" + coffeepotType + "','" + checkResult + "'";
            for (int j = 0; j < record.Count; j++)
            {
                string str3 = record[j].insertItemValue;
                str3 = ",'" + str3 + "'";
                commStr += str3;
            }
            commStr += ")";
            //字符串拼接完成
            try
            {
                MySqlCommand myComm = new MySqlCommand(commStr, myConn);
                if (myComm.ExecuteNonQuery() > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
        /// <summary>
        /// 插入用户信息
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="userLevel"></param>
        /// <returns>true:插入成功;false:插入失败</returns>
        public bool InsertUser(string userName, string userLevel, string password)
        {

            if (!SqlState)
                return false;
            string commStr = "insert into operator_info (operator,userLevel,password) values ('" + userName + "','" + userLevel + "','" + password + "')";
            try
            {
                MySqlCommand myComm = new MySqlCommand(commStr, myConn);
                if (myComm.ExecuteNonQuery() > 0)
                    return true;
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// 查询登录密码;
        /// </summary>
        /// <param name="coffeepotTable"></param>
        /// <param name="userName"></param>
        /// <returns>-1:查询失败;</returns>
        public string UserInfoQuery(string coffeepotTable, string userName)
        {
            string UserPassword = "";
            if (!SqlState)
                return "-1";
            string commStr = "select operator,password from operator_info where operator='" + userName + "'";
            MySqlDataReader reader = null;
            try
            {
                MySqlCommand myComm = new MySqlCommand(commStr, myConn);
                reader = myComm.ExecuteReader();
                while (reader.Read())
                {
                    UserPassword = reader[1].ToString();
                }
                return UserPassword;
            }
            catch
            {
                return "-1";
            }
            finally
            {
                reader.Close();
            }
        }

        /// <summary>
        /// 获得表最大ID(自增)
        /// </summary>
        /// <param name="coffeepotTable"></param>
        /// <returns>-1:连接关闭;-2查询失败;</returns>
        public int TableMaxIDQuery(string coffeepotTable)
        {
            int tableMaxID = 0;
            if (!SqlState)
                return -1;
            string commStr = "select max(ID) from " + coffeepotTable + "";
            MySqlDataReader reader = null;
            try
            {
                MySqlCommand myComm = new MySqlCommand(commStr, myConn);
                reader = myComm.ExecuteReader();
                while (reader.Read())
                {
                    tableMaxID = Convert.ToInt32(reader["max(ID)"].ToString());
                }
                return tableMaxID;
            }
            catch
            {
                return -2;
            }
            finally
            {
                reader.Close();
            }
        }
        /// <summary>
        /// 按照ID查询
        /// </summary>
        /// <param name="coffeepotTable"></param>
        /// <param name="coffeepotID"></param>
        /// <returns></returns>
        public Dictionary<string, string> CoffeepotIDSelect(string coffeepotID)
        {

            if (!SqlState)
                return null;
            Dictionary<string, string> coffeepotInfo = new Dictionary<string, string>();
            string tableName = coffeepotID.Substring(0, 4);
            string commStr = "select Item1,Item2,Item3,Item4,Item5,Item6,Item7,Item8,Item9,Item10,Item11,Item12,Item13,Item14,Item15,Item16,Item17,Item18,Item19,Item20,Item21,Item22,Item23,Item24,Item25 from coffeepot_info" + tableName + " where ID='" + coffeepotID + "'";
            MySqlDataReader reader = null;
            try
            {
                MySqlCommand myComm = new MySqlCommand(commStr, myConn);
                reader = myComm.ExecuteReader();
                while (reader.Read())
                {
                    for (int i = 1; i < 26; i++)
                        coffeepotInfo.Add("Item" + i + "", reader[i - 1].ToString());
                }
                return coffeepotInfo;
            }
            catch
            {
                return null;
            }
            finally
            {
                reader.Close();
            }
        }
        /// <summary>
        /// 按照类型和时间查询检测情况
        /// </summary>
        /// <param name="coffeepotTable"></param>
        /// <param name="date1"></param>
        /// <param name="date2"></param>
        /// <param name="coffeepotType"></param>
        /// <returns></returns>
        public List<string[]> CheckResultSelect1(DateTime date1, DateTime date2, string coffeepotType)
        {
            int i = DateTime.Compare(date1, date2);
            DateTime dateStop = new DateTime();
            DateTime dateStart = new DateTime();
            if (i > 0)
            {
                //date1 > date2;
                dateStart = date2;
                dateStop = date1;
            }
            else if (i <= 0)
            {
                dateStart = date1;
                dateStop = date2;
            }
            string tableName = dateStart.Year.ToString();
            //dateStop = dateStop.AddDays(1);
            if (!SqlState)
                return null;
            List<string[]> a = new List<string[]>();
            string[] recordStr = new string[5];
            //                string commStr = "select count(1)as 产量,count(case when Result='1' then 1 else null end)as 合格数,sum(Result)/count(1) as 合格率 from " + coffeepotTable + " where Type='" + coffeepotType + "' and DATE_FORMAT(Date,'%Y/%c/%e')='" + tempDT + "'";
            string commStr = "select date_format(Date,'%Y-%m-%d'),'" + coffeepotType + "',count(1),count(case when Result='1' then 1 else null end),concat(truncate(sum(Result)/count(1)*100,2),'%') from coffeepot_info" + tableName + " where Type='" + coffeepotType + "' and DATE_FORMAT(Date,'%Y-%m-%d') between DATE_FORMAT('" + dateStart + "','%Y-%m-%d') and DATE_FORMAT('" + dateStop + "','%Y-%m-%d') group by date_format(Date,'%Y-%m-%d');";
            MySqlDataReader reader = null;
            try
            {
                MySqlCommand myComm = new MySqlCommand(commStr, myConn);
                reader = myComm.ExecuteReader();
                //if (reader.HasRows)
                //{
                while (reader.Read())
                {
                    recordStr = new string[5];
                    for (int j = 0; j < reader.FieldCount; j++)
                    {
                        recordStr[j] = Convert.ToString(reader[j].ToString());
                    }
                    a.Add(recordStr);
                }
                return a;
            }
            catch (Exception)
            {
                reader.Close();
                return null;
            }
            finally
            {
                reader.Close();
            }
        }
        public bool InsertRow(string coffeepotID, string coffeepotType, string checkResult, List<insertRecordStr> record)
        {
            if (!SqlState)
                return false;
            string tableName = coffeepotID.Substring(0, 4);
            //string checkDate = Convert.ToString(coffeepotID,"yyyy-MM-dd HH:mm:ss");
            string str1 = "insert into coffeepot_info" + tableName + " (ID,Type,Date,Result";
            for (int i = 0; i < record.Count; i++)
            {
                string str2 = record[i].insertItemName;
                str2 = "," + str2;
                str1 += str2;
            }
            string commStr = str1 + ")values ('" + coffeepotID + "','" + coffeepotType + "',date_format(" + coffeepotID + ",'%Y-%m-%d %H:%m:%s'),'" + checkResult + "'";
            for (int j = 0; j < record.Count; j++)
            {
                string str3 = record[j].insertItemValue;
                str3 = ",'" + str3 + "'";
                commStr += str3;
            }
            commStr += ")";
            //字符串拼接完成
            try
            {
                MySqlCommand myComm = new MySqlCommand(commStr, myConn);
                if (myComm.ExecuteNonQuery() > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public string[] CheckResultSelect(DateTime date, string coffeepotType)
        {

            string tableName = date.Year.ToString();
            //dateStop = dateStop.AddDays(1);
            if (!SqlState)
                return null;
            string[] recordStr = new string[5];
            string commStr = "select date_format('" + date + "','%Y-%m-%d'),'" + coffeepotType + "',count(1),count(case when Result='1' then 1 else null end),concat(truncate(sum(Result)/count(1)*100,2),'%') from coffeepot_info" + tableName + " where Type='" + coffeepotType + "' and DATE_FORMAT(Date,'%Y-%m-%d')=DATE_FORMAT('" + date + "','%Y-%m-%d');";
            MySqlDataReader reader = null;
            try
            {
                MySqlCommand myComm = new MySqlCommand(commStr, myConn);
                reader = myComm.ExecuteReader();
                //if (reader.HasRows)
                //{
                while (reader.Read())
                {
                    recordStr = new string[5];
                    for (int j = 0; j < reader.FieldCount; j++)
                    {
                        recordStr[j] = Convert.ToString(reader[j].ToString());
                    }
                }
                return recordStr;
            }
            catch (Exception ex)
            {
                reader.Close();
                return null;
            }
            finally
            {
                reader.Close();
            }
        }
        public List<float> SelectItem(DateTime date, string coffeepotType, string itemCode)
        {
            string tableName = date.Year.ToString();
            if (!SqlState)
                return null;
            string commStr = "select " + itemCode + " from coffeepot_info" + tableName + " where Type='" + coffeepotType + "' and DATE_FORMAT(Date,'%Y-%m-%d')=DATE_FORMAT('" + date + "','%Y-%m-%d')  order by ID;";
            MySqlDataReader reader = null;
            List<float> itemRecord = new List<float>();
            float itemData = 0;
            try
            {
                MySqlCommand myComm = new MySqlCommand(commStr, myConn);
                reader = myComm.ExecuteReader();
                while (reader.Read())
                {
                    for (int j = 0; j < reader.FieldCount; j++)
                    {
                        itemData = Convert.ToSingle(reader[j]);
                    }
                    itemRecord.Add(itemData);
                }
                return itemRecord;
            }
            catch (Exception)
            {
                reader.Close();
                return null;
            }
            finally
            {
                reader.Close();
            }
        }
    }
}
