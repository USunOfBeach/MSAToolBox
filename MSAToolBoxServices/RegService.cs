using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Text.RegularExpressions;
using MySql.Data.MySqlClient;
using System.Data;

namespace MSAToolBoxServices
{
    public enum RegResult
    {
        RESULT_FAILED_INVALID_REGINFO   = 1,
        RESULT_FAILED_CANT_CONNECT_DB   = 2,
        RESULT_FAILED_USERNAME_TAKEN    = 3,
        RESULT_FAILED_EMAIL_TAKEN       = 4,
        RESULT_SUCCESS                  = 5,
        RESULT_FAILED_WITHOUT_REASON    = 6

    }
    public enum ChangePwResult
    {
        RESULT_FAILED_INFO_NOT_MATCH    = 1,
        RESULT_SUCCESS                  = 2,
        RESULT_FAILED_CANT_CONNECT_DB   = 3,
        RESULT_FAILED_WITHOUT_REASON    = 4
    }
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码和配置文件中的类名“Service1”。
    public class RegService : IRegService
    {
        public RegService()
        {
            db_connection = RegUtil.CreateDBConnection("localhost", "auth_v3", "root", "ascent", "utf8");
            db_connection.Open();
        }
        MySqlConnection db_connection;
        public RegResult TryRegister(byte[] data)
        {
            string decryptedString = RegUtil.DecryptData(data, "MSAToolBoxRegistration");
            RegInfo regInfo = RegUtil.GetRegInfo(decryptedString);
            if (regInfo == null)
                return RegResult.RESULT_FAILED_INVALID_REGINFO;

            if (db_connection.State != ConnectionState.Open)
                RegUtil.ReestablishMySQLConnection(db_connection);
            if (db_connection.State != ConnectionState.Open)
                return RegResult.RESULT_FAILED_CANT_CONNECT_DB;

            // Check Username.
            MySqlCommand cmd = db_connection.CreateCommand();
            cmd.CommandText = String.Format("SELECT * FROM account WHERE username = '{0}'", regInfo.Username);
            if (RegUtil.IsQueryHasRow(cmd))
                return RegResult.RESULT_FAILED_USERNAME_TAKEN;

            // Check Email.
            cmd.CommandText = String.Format("SELECT * FROM account WHERE email = '{0}'", regInfo.Email);
            if (RegUtil.IsQueryHasRow(cmd))
                return RegResult.RESULT_FAILED_EMAIL_TAKEN;

            // everthing looks good, try register.
            cmd.CommandText = String.Format("INSERT INTO account (`username`, `sha_pass_hash`, `email`, `expansion`) VALUES ('{0}', '{1}', '{2}', {3})", regInfo.Username, regInfo.Passhash, regInfo.Email, 2);

            cmd.ExecuteNonQuery();

            return RegResult.RESULT_SUCCESS;
        }
        public ChangePwResult TryChangePw(byte[] data)
        {
            string decryptedString = RegUtil.DecryptData(data, "MSAToolBoxChangePassword");
            ChangePwInfo userInfo = RegUtil.GetChangePwInfo(decryptedString);
            if (userInfo == null)
                return ChangePwResult.RESULT_FAILED_INFO_NOT_MATCH;

            if (db_connection.State != ConnectionState.Open)
                RegUtil.ReestablishMySQLConnection(db_connection);
            if (db_connection.State != ConnectionState.Open)
                return ChangePwResult.RESULT_FAILED_CANT_CONNECT_DB;

            // Try to find a match.
            MySqlCommand cmd = db_connection.CreateCommand();
            cmd.CommandText = String.Format("SELECT * FROM account WHERE username = '{0}' AND sha_pass_hash = '{1}' AND email = '{2}'", userInfo.Username, userInfo.Passhash, userInfo.Email);
            if (!RegUtil.IsQueryHasRow(cmd))
                return ChangePwResult.RESULT_FAILED_INFO_NOT_MATCH;
            else
                // thats a hit. change password for him.
                cmd.CommandText = String.Format("UPDATE account SET sha_pass_hash = '{0}', sessionkey = '', v = '', s = '' WHERE account = '{1}'", userInfo.PasshashNew, userInfo.Username);

            return ChangePwResult.RESULT_SUCCESS;
        }
    }

    public class RegInfo
    {
        public RegInfo(string u, string p, string e)
        {
            Username = u;
            Passhash = p;
            Email = e;
        }
        public string Username { get; set; }
        public string Passhash { get; set; }
        public string Email { get; set; }
    }

    public class ChangePwInfo
    {
        public ChangePwInfo(string u, string p, string e, string pn)
        {
            Username = u;
            Passhash = p;
            Email = e;
            PasshashNew = pn;
        }
        public string Username { get; set; }
        public string Passhash { get; set; }
        public string Email { get; set; }
        public string PasshashNew { get; set; }
    }

    public static class RegUtil
    {
        public static Regex UserNameRegEx = new Regex("^\\w+$");
        public static Regex EmailRegEx = new Regex("^(\\w)+(\\.\\w+)*@(\\w)+((\\.\\w+)+)$");
        public static string DecryptData(byte[] b, string k)
        {
            byte[] key = Encoding.ASCII.GetBytes(k);

            for (int i = 0; i < b.Length; i += 2)
            {
                for (int j = 0; j < key.Length; j += 2)
                    b[i] = Convert.ToByte(b[i] ^ key[j]);
            }

            string s = Encoding.ASCII.GetString(b);
            return s;
        }
        public static RegInfo GetRegInfo(string regString)
        {
            string[] splittedString = regString.Split(':');
            if (splittedString.Length != 4) // USERNAME:PASSHASH:EMAIL:REGCODE
                return null;
            if (splittedString[3] != "MSAToolBoxRegistration")
                return null;
            // Username
            if (splittedString[0].Length < 6 || splittedString[0].Length > 32 || !RegUtil.UserNameRegEx.IsMatch(splittedString[0]))
                return null;
            // Passhash
            if (splittedString[1].Length != 40)
                return null;
            // Email
            if (!RegUtil.EmailRegEx.IsMatch(splittedString[2]))
                return null;

            return new RegInfo(splittedString[0], splittedString[1], splittedString[2]);
        }
        public static ChangePwInfo GetChangePwInfo(string regString)
        {
            string[] splittedString = regString.Split(':');
            if (splittedString.Length != 5) // USERNAME:PASSHASH:EMAIL:NEWPASSHASH:REGCODE
                return null;
            if (splittedString[4] != "MSAToolBoxChangePassword")
                return null;
            // Username
            if (splittedString[0].Length < 6 || splittedString[0].Length > 32 || !RegUtil.UserNameRegEx.IsMatch(splittedString[0]))
                return null;
            // Passhash
            if (splittedString[1].Length != 40 || splittedString[3].Length != 40)
                return null;
            // Email
            if (!RegUtil.EmailRegEx.IsMatch(splittedString[2]))
                return null;

            return new ChangePwInfo(splittedString[0], splittedString[1], splittedString[2], splittedString[3]);
        }
        public static MySqlConnection CreateDBConnection(string host, string db, string uid, string pw, string charset)
        {
            return new MySqlConnection(String.Format("Server={0};Database={1};Uid={2};Pwd={3};Charset={4};", host, db, uid, pw, charset));
        }
        public static void ReestablishMySQLConnection(MySqlConnection connection)
        {
            if (connection.State == ConnectionState.Open)
                return;
            
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
                return;
            }

            if (connection.State == ConnectionState.Broken)
            {
                connection.Close();
                connection.Open();
                return;
            }
        }
        public static bool IsQueryHasRow(MySqlCommand cmd)
        {
            bool hasRow = false;
            MySqlDataReader reader = cmd.ExecuteReader();
            reader.Read();
            hasRow = reader.HasRows;
            reader.Close();
            return hasRow;
        }
    }
}
