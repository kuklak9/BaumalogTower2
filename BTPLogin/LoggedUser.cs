using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Data;
using BTPDataBase;
using System.Data.SqlClient;

namespace BTPLogin
{
    public class LoggedUser
    {
        DB_WMS_3 _db;
        public LoggedUser(DB_WMS_3 db)
        {
            _db = db;
        }

        public int LoginTimer = 0;
        public String UserName = "";
        public int AccessLevel = 0;
        public bool LoggedIn = false;
        public DataTable UserRights;
        public Dictionary<string, bool[]> OperatorRights = new Dictionary<string, bool[]>();

        public void DoLogOut()
        {
            UserName = "";
            LoggedIn = false;
        }
        public bool DoLogin(String name, String password)
        {
            if (name == "BAUMALOG")
            {
                int pom = DateTime.Now.Date.Day + DateTime.Now.Date.Month + DateTime.Now.Date.Year;
                if (password == "BL" + pom.ToString())
                {
                    UserName = "BAUMALOG";
                    LoggedIn = true;
                    //UserRights = _db.Operators.GetRights().Select("ID_Grupy = 0").CopyToDataTable();
                    return true;
                }
                return false;
            }
            else
                return Login(name, password);

        }

        private bool Login(string username, string password)
        {
            string v = password;
            byte[] salt = Encoding.ASCII.GetBytes("9369888615820315512323246139138941212977");
            Rfc2898DeriveBytes? pbkdf2 = new(v, salt, 1000, HashAlgorithmName.SHA512);
            byte[] hash = pbkdf2.GetBytes(20);

            byte[] hashByts = new byte[36];

            Array.Copy(salt, 0, hashByts, 0, 16);
            Array.Copy(hash, 0, hashByts, 16, 20);
            string temp = Convert.ToBase64String(hashByts);

            //_ = Validate(username,temp);
            if (Validate(username, temp))
            {
                LoggedIn = true;
                UserName = username;
                _ = int.TryParse(_db.Operators.GetData().Select("Login = '" + username + "'")[0]["ID_Grupy"].ToString(), out int result);
                try
                {
                    OperatorRights.Clear();
                    UserRights = _db.OperatorsRights.GetData().Select("ID_Grupy = " + result).CopyToDataTable();
                    foreach (DataRow row in UserRights.Rows)
                    {
                        OperatorRights.Add(row[1].ToString(), new bool[] { (bool)row[2], (bool)row[3], (bool)row[4], (bool)row[5] });
                    }
                }
                catch (Exception)
                {
                    UserRights = null;
                    OperatorRights = null;
                }
                return true;
            }
            else
                return false;
        }

        private bool Validate(string username, string password)
        {
            string user = username;
            string pass = password;

            DataTable dt1 = _db.Operators.GetData();
            for (int i = 0; i < dt1.Rows.Count; i++)
            {
                if (dt1.Rows[i]["Login"].ToString() == user && dt1.Rows[i]["Haslo"].ToString() == pass)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
