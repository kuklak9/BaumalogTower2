using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;

namespace BTPDataBase
{
    public class DB_WMS_3
    {
        #region FIELDS
        protected String dbconnstr = "";
        protected String dbconnstr_Remote = "";
        protected String dbconnstr_Remote_new = "";

        public DS_ArticlesTableAdapters.p_ArtykulyNaPolkach_SELECTTableAdapter ArticlesOnTrays = new DS_ArticlesTableAdapters.p_ArtykulyNaPolkach_SELECTTableAdapter();
        public DS_ArticlesTableAdapters.p_KatalogArtykulow_SELECTTableAdapter Articles = new DS_ArticlesTableAdapters.p_KatalogArtykulow_SELECTTableAdapter();
        public DS_ArticlesTableAdapters.p_RodzajeArtykulow_SELECTTableAdapter ArticlesTypes = new DS_ArticlesTableAdapters.p_RodzajeArtykulow_SELECTTableAdapter();
        public DS_ArticlcesTransactionsTableAdapters.p_TransakcjeNaArtykulach_SELECTTableAdapter ArticleTransactions = new DS_ArticlcesTransactionsTableAdapters.p_TransakcjeNaArtykulach_SELECTTableAdapter();
              
        public DS_OperatorsTableAdapters.Kar_Operator_SELECTTableAdapter Operators= new DS_OperatorsTableAdapters.Kar_Operator_SELECTTableAdapter();
        public DS_OperatorsTableAdapters.Def_GrupyPrawa_SELECTTableAdapter OperatorsRights= new DS_OperatorsTableAdapters.Def_GrupyPrawa_SELECTTableAdapter();

        public DS_TraysTableAdapters.p_Polki_SELECTTableAdapter TraysOfflinePLC = new DS_TraysTableAdapters.p_Polki_SELECTTableAdapter();
        public DS_LogsTableAdapters.p_Logi_SELECTTableAdapter Logs = new DS_LogsTableAdapters.p_Logi_SELECTTableAdapter();
        public DS_ArticlesTableAdapters.v_WagaIIloscArtykulowNaPolkachTableAdapter WeightAndCountOfArticlesOnTrays = new DS_ArticlesTableAdapters.v_WagaIIloscArtykulowNaPolkachTableAdapter();
        public DS_TraysTableAdapters.p_PobierzListePolek_WagaIlosciTableAdapter TraysWithWeightAndCount = new DS_TraysTableAdapters.p_PobierzListePolek_WagaIlosciTableAdapter();
        public DS_TraysTableAdapters.p_PolkiStatusy_SELECTTableAdapter TrayState = new DS_TraysTableAdapters.p_PolkiStatusy_SELECTTableAdapter();
        public DS_LogsTableAdapters.p_Przeglady_SELECTTableAdapter Maintenance = new DS_LogsTableAdapters.p_Przeglady_SELECTTableAdapter();
        public DS_LogsTableAdapters.QueriesTableAdapter LogQueries = new DS_LogsTableAdapters.QueriesTableAdapter();
        public DS_LogsTableAdapters.p_OPC_ProgramLog_SELECTTableAdapter OpcProgramLog = new DS_LogsTableAdapters.p_OPC_ProgramLog_SELECTTableAdapter();

        public Add_DataTableAdapters.Kar_Artykuly_SELECTTableAdapter Add_Kar_Artykul_ta = new Add_DataTableAdapters.Kar_Artykuly_SELECTTableAdapter();
        public Add_DataTableAdapters.Kar_Artykuly_EditTableAdapter Edit_Kar_Artykul_ta = new Add_DataTableAdapters.Kar_Artykuly_EditTableAdapter();
        public Add_DataTableAdapters.StanyMagazynowe_SELECTTableAdapter Add_StanyMagazynowe_ta = new Add_DataTableAdapters.StanyMagazynowe_SELECTTableAdapter();
        public Add_DataTableAdapters.StanyMagazynowe_EDITTableAdapter Edit_StanyMagazynowe_ta = new Add_DataTableAdapters.StanyMagazynowe_EDITTableAdapter();
        public Add_DataTableAdapters.Kar_Gatunki_SELECTTableAdapter Add_Gatunki_ta = new Add_DataTableAdapters.Kar_Gatunki_SELECTTableAdapter();
        public Add_DataTableAdapters.Kar_Gatunki_EditTableAdapter Edit_Gatunki_ta = new Add_DataTableAdapters.Kar_Gatunki_EditTableAdapter();
        public Add_DataTableAdapters.StanyMagazynowe__Filter_SELECTTableAdapter Add_StanyMagazynowe_Filter = new Add_DataTableAdapters.StanyMagazynowe__Filter_SELECTTableAdapter();
        public Add_DataTableAdapters.Zlecenia_SELECTTableAdapter Add_Zlecenia_Select = new Add_DataTableAdapters.Zlecenia_SELECTTableAdapter();
        public Add_DataTableAdapters.ZleceniaLinie_SELECTTableAdapter Add_ZleceniaLinie_Select = new Add_DataTableAdapters.ZleceniaLinie_SELECTTableAdapter();
        public Add_DataTableAdapters.Kar_Polki_HMI_SELECTTableAdapter Add_Polki_HMI_Select = new Add_DataTableAdapters.Kar_Polki_HMI_SELECTTableAdapter();
        public Add_DataTableAdapters.Kar_Polki_SELECTTableAdapter Add_Polki_Select = new Add_DataTableAdapters.Kar_Polki_SELECTTableAdapter();
        public Add_DataTableAdapters.ObrotyMagazynowe_SELECTTableAdapter Add_ObrotyMagazynowe_Select = new Add_DataTableAdapters.ObrotyMagazynowe_SELECTTableAdapter();
        public Add_DataTableAdapters.Kar_Lokalizacje_SELECTTableAdapter Add_Lokalizacje_Select = new Add_DataTableAdapters.Kar_Lokalizacje_SELECTTableAdapter();
        public Add_DataTableAdapters.Kar_Operator_SELECTTableAdapter Add_Operator_Select = new Add_DataTableAdapters.Kar_Operator_SELECTTableAdapter();
        public Add_DataTableAdapters.KategoriaTree_SELECTTableAdapter kategoriaTree_SELECT = new Add_DataTableAdapters.KategoriaTree_SELECTTableAdapter();
        public Add_DataTableAdapters.ZlecenieRealizacja_PobierzPozycjeTableAdapter ZlecenieRealizacja_PobierzPozycje_ta = new Add_DataTableAdapters.ZlecenieRealizacja_PobierzPozycjeTableAdapter();
        #endregion

        #region CONSTRUSTOR
        public DB_WMS_3(String local, String remote, String remote_new)
        {
            dbconnstr = local;
            dbconnstr_Remote = remote;
            dbconnstr_Remote_new = remote_new;
            SetConnectionString();
        }
        #endregion

        #region METHODS

        public void SetConnectionString()
        {

            ArticlesOnTrays.Connection.ConnectionString = dbconnstr_Remote;
            Articles.Connection.ConnectionString = dbconnstr_Remote;
            ArticlesTypes.Connection.ConnectionString = dbconnstr_Remote;
            ArticleTransactions.Connection.ConnectionString = dbconnstr_Remote;

            Operators.Connection.ConnectionString = dbconnstr_Remote_new;
            OperatorsRights.Connection.ConnectionString = dbconnstr_Remote_new;

            TraysOfflinePLC.Connection.ConnectionString = dbconnstr;
            Logs.Connection.ConnectionString = dbconnstr;
            TraysWithWeightAndCount.Connection.ConnectionString = dbconnstr_Remote;
            WeightAndCountOfArticlesOnTrays.Connection.ConnectionString = dbconnstr_Remote;
            TrayState.Connection.ConnectionString = dbconnstr_Remote;
            Maintenance.Connection.ConnectionString = dbconnstr;
            LogQueries.ConnectionString = dbconnstr;
            OpcProgramLog.Connection.ConnectionString = dbconnstr_Remote;

            Add_Kar_Artykul_ta.Connection.ConnectionString = dbconnstr_Remote_new;
            Edit_Kar_Artykul_ta.Connection.ConnectionString = dbconnstr_Remote_new;
            Add_StanyMagazynowe_ta.Connection.ConnectionString = dbconnstr_Remote_new;
            Add_Gatunki_ta.Connection.ConnectionString = dbconnstr_Remote_new;
            Edit_Gatunki_ta.Connection.ConnectionString = dbconnstr_Remote_new;
            Add_StanyMagazynowe_Filter.Connection.ConnectionString = dbconnstr_Remote_new;
            Add_Zlecenia_Select.Connection.ConnectionString = dbconnstr_Remote_new;
            Add_ZleceniaLinie_Select.Connection.ConnectionString = dbconnstr_Remote_new;
            Add_Polki_HMI_Select.Connection.ConnectionString = dbconnstr_Remote_new;
            Add_Polki_Select.Connection.ConnectionString = dbconnstr_Remote_new;
            Add_ObrotyMagazynowe_Select.Connection.ConnectionString = dbconnstr_Remote_new;
            Add_Lokalizacje_Select.Connection.ConnectionString = dbconnstr_Remote_new;
            Add_Operator_Select.Connection.ConnectionString = dbconnstr_Remote_new;
            kategoriaTree_SELECT.Connection.ConnectionString = dbconnstr_Remote_new;
            ZlecenieRealizacja_PobierzPozycje_ta.Connection.ConnectionString = dbconnstr_Remote_new;
            Edit_StanyMagazynowe_ta.Connection.ConnectionString = dbconnstr_Remote_new;

        }
        #region ArticleOnTray
        public DataTable GetWeight(String[] cond)
        {
            //String[] str;
            //if (!forGetGive)
            //{
            //    str = new String[2];
            //    str[0] = "Indeks <> '" + _SelectedArticle.Art.Index + "'";
            //    str[1] = "Polka=" + _SelectedArticle.Tray;
            //}
            //else
            //{
            //    str = new String[1];
            //    str[0] = "Polka=" + _SelectedArticle.Tray;
            //}

            // obsluga cond - zachowanie kompatybilnosci
            String Index = "";
            int Tray = 0;

            if (cond.Count() == 1)
            {
                int.TryParse(cond[0].Replace("Polka=", ""), out Tray);
            }
            else if (cond.Count() == 2)
            {
                Index = cond[0].Replace("Indeks <> '", "").Replace("'", "");
                int.TryParse(cond[1].Replace("Polka=", ""), out Tray);
            }
            else
                return null;

            String q = "EXEC dbo.p_PobierzWageArtykulowNaPolce ";
            q += Tray.ToString() + ", ";
            q += "N'" + Index + "'";


            //String q = "SELECT IsNull(SUM(IsNull(([Ilosc]*ka.Waga),0)),0) ";
            //q += "FROM [dbo].[ArtykulyNaPolkach] anp ";
            //q += "LEFT JOIN [dbo].KatalogArtykulow ka ON ka.Indeks=anp.Indeks ";


            //if (cond != null && cond.Count() > 0)
            //{
            //    for (int i = 0; i < cond.Count(); i++)
            //    {
            //        if (i == 0)
            //        {
            //            q += " WHERE ";
            //        }
            //        else
            //        {
            //            q += " AND ";
            //        }
            //        q += "anp." + cond[i];
            //    }
            //}

            //return GetTableData(q);
            return GetTableData_Remote(q);

        }
        public DataTable FilterDataTable(DataTable dt, String[] cond)
        {
            try
            {
                if (cond == null || dt == null)
                    return dt;
                String filter = "";

                for (int i = 0; i < cond.Count(); i++)
                {
                    if (i != 0)
                        filter += " AND ";

                    filter += cond[i].Replace("Like N'", "Like '").Replace("= N'", "= '").Replace("Like  N'", "Like '").Replace("=N'", "='");
                }
                //migracja - sprawdzić czy działa było -  DataView dv = dt.AsDataView;
                DataView dv = dt.Copy().DefaultView;
                dv.RowFilter = filter;
                dt = dv.ToTable();
                return dt;
                //var q = dt.AsDataView().RowFilter()
            }
            catch
            {
                return dt;
            }
        }

        #endregion

        public DataTable GetTrays(int tower)
        {
            return TraysWithWeightAndCount.GetData(tower);
        }


        /// <summary>
        /// Zwraca pogrupowane ilosc i wagi artykulow na polkach 
        /// </summary>
        /// <returns>Polka, Waga, Ilosc</returns>
        public DataTable GetArtWeightAndCountGroupedByTray()
        {
            return WeightAndCountOfArticlesOnTrays.GetData();
        }
        private DataTable CreateTrayTableFromTrayConfig(Dictionary<int, BTPUtilities.Tray> trayConfig)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("Polka", typeof(int)));
            dt.Columns.Add(new DataColumn("Wysokosc", typeof(int)));
            dt.Columns.Add(new DataColumn("Pozycja", typeof(int)));
            dt.Columns.Add(new DataColumn("Kolumna", typeof(int)));
            dt.Columns.Add(new DataColumn("Regal", typeof(int)));
            dt.Columns.Add(new DataColumn("Ladownosc", typeof(int)));

            try
            {
                foreach (KeyValuePair<int, BTPUtilities.Tray> kvp in trayConfig)
                {
                    dt.Rows.Add(new object[] { kvp.Value.ID, kvp.Value.Height, kvp.Value.Position, kvp.Value.Column, kvp.Value.Tower, 0 });
                }

                return dt;
            }
            catch
            {
                return null;
            }
        }
        public DataTable GetArticleOnTray(int tray, int Tower)
        {
            DataTable dt = ArticlesOnTrays.GetData(Tower);
            dt = FilterDataTable(dt, new string[] { "Polka=" + tray.ToString() });
            return dt;
            //String q = "SELECT [Indeks],[Ilosc] FROM [dbo].[ArtykulyNaPolkach] WHERE Polka=" + tray.ToString();
            //return base.GetTableData(q);
        }
        public DataTable GetEmptyTrays()//Dictionary<int, BTPUtilities.Tray> trayConfig)
        {
            // String q = "dbo.p_puste_polki";

            DataTable dt = new DataTable();
            SqlConnection sqlc = new SqlConnection(dbconnstr_Remote);
            try
            {

                sqlc.Open();
                SqlCommand sqlCmd = new SqlCommand();
                sqlCmd.Connection = sqlc;
                sqlCmd.CommandText = "dbo.p_PobierzPustePolki";//"dbo.p_puste_polki";
                sqlCmd.CommandType = CommandType.StoredProcedure;
                //SqlParameter param = sqlCmd.Parameters.AddWithValue("@polki", CreateTrayTableFromTrayConfig(trayConfig));
                //param.SqlDbType = SqlDbType.Structured;
                //param.TypeName = "t_Polka";

                SqlDataReader sdr = sqlCmd.ExecuteReader();
                dt.Load(sdr);
                sdr.Close();
                sqlc.Close();
            }
            catch
            {
                dt = null;
            }
            sqlc.Dispose();

            return dt;
        }
        //private void ArticleUsed(String article)
        //{
        //    String q = "EXEC dbo.p_uzycie_artykulow '"+article +"'";
        //    this.GetNonNonQuerry(q);
        //}

        //private void TrayUsed(int tray)
        //{
        //    String q = "EXEC dbo.p_uzycie_polek " + tray.ToString();
        //    this.GetNonNonQuerry(q);
        //}

        #region 4statistic

        public int GetNonNonQuerry(String q)
        {
            int rv = -1;
            SqlConnection sqlc = new SqlConnection(dbconnstr);
            try
            {
                sqlc.Open();
                SqlCommand sqlCmd = new SqlCommand(q, sqlc);
                rv = sqlCmd.ExecuteNonQuery();
                sqlc.Close();
            }
            catch
            {
                rv = -1;
            }
            sqlc.Dispose();
            return rv;
        }

        public int GetNonNonQuerry_Remote(String q)
        {
            int rv = -1;
            SqlConnection sqlc = new SqlConnection(dbconnstr_Remote);
            try
            {
                sqlc.Open();
                SqlCommand sqlCmd = new SqlCommand(q, sqlc);
                rv = sqlCmd.ExecuteNonQuery();
                sqlc.Close();
            }
            catch
            {
                rv = -1;
            }
            sqlc.Dispose();
            return rv;
        }
        public void ClearArticleUse()
        {

            String querry = "EXEC dbo.p_UzycieArtykulow_TRUNCATE";
            GetNonNonQuerry(querry);
        }

        public void ClearTrayUse()
        {

            String querry = "p_UzyciePolek_TRUNCATE";
            GetNonNonQuerry(querry);
        }

        public String GetGroupForArticle(String article)
        {
            String querry = "EXEC dbo.p_uzycie_artykulow_grupa '" + article + "'";
            DataTable dt = GetTableData(querry);
            try
            {
                return dt.Rows[0][0].ToString();
            }
            catch
            {
                return "C";
            }
        }

        public DataTable GetArticleUse()
        {
            UpdateArticleUsedRemoteFromRemote();

            String querry = "EXEC dbo.p_uzycie_artykulow_grupy";
            return GetTableData(querry);
        }

        public DataTable GetTrayUse()
        {

            String querry = "EXEC dbo.p_uzycie_polek_grupy";//"SELECT * FROM [dbo].[UzyciePolek]";
            return GetTableData(querry);
        }

        public void AddTrayUse(int tray)
        {

            String querry = "EXEC dbo.p_uzycie_polek " + tray.ToString();
            GetNonNonQuerry(querry);

        }

        public void AddArticleUse(String article)
        {
            UpdateArticleUsedRemoteFromRemote();
            String querry = "EXEC dbo.p_uzycie_artykulow '" + article.ToString() + "'";
            GetNonNonQuerry(querry);

        }

        private void UpdateArticleUsedRemoteFromRemote()
        {
            String querry = "EXEC dbo.p_uzycie_artykulow_merge";
            DataTable dt = GetTableData_Remote(querry);

            if (dt != null && dt.Rows.Count > 0)
            {
                querry = "DELETE FROM dbo.UzycieArtykulow_Remote";
                GetNonNonQuerry(querry);
                foreach (DataRow dr in dt.Rows)
                {
                    querry = "INSERT INTO dbo.UzycieArtykulow_Remote ([Indeks]) VALUES ('" + (string)dr[0] + "');";
                    GetNonNonQuerry(querry);
                }
            }
        }

        public DataTable GetTableData_Remote(String q)
        {
            DataTable dt = new DataTable();
            SqlConnection sqlc = new SqlConnection(dbconnstr_Remote);
            try
            {

                sqlc.Open();
                SqlCommand sqlCmd = new SqlCommand(q, sqlc);
                SqlDataReader sdr = sqlCmd.ExecuteReader();
                dt.Load(sdr);
                sdr.Close();
                sqlc.Close();
            }
            catch
            {
                dt = null;
            }
            sqlc.Dispose();
            return dt;
        }

        public DataTable GetTableData(String q)
        {
            DataTable dt = new DataTable();
            SqlConnection sqlc = new SqlConnection(dbconnstr);
            try
            {

                sqlc.Open();
                SqlCommand sqlCmd = new SqlCommand(q, sqlc);
                SqlDataReader sdr = sqlCmd.ExecuteReader();
                dt.Load(sdr);
                sdr.Close();
                sqlc.Close();
            }
            catch
            {
                dt = null;
            }
            sqlc.Dispose();
            return dt;
        }
        #endregion

        #region 4eagleDB

        public DataTable GetTraysWithAutoindexInfo(String[] cond)
        {

            String q = "SELECT * FROM [dbo].[fn_Polki_laser] ()";

            return FilterDataTable(GetTableData(q), cond);
        }



        public void WriteTrayConfigToDB(int tower, Dictionary<int, BTPUtilities.Tray> trayConfig)
        {

            DataTable dt = new DataTable();
            SqlConnection sqlc = new SqlConnection(dbconnstr_Remote);
            try
            {

                sqlc.Open();
                SqlCommand sqlCmd = new SqlCommand();
                sqlCmd.Connection = sqlc;
                sqlCmd.CommandText = "dbo.p_AktualizujListePolek";//"dbo.p_puste_polki";
                sqlCmd.CommandType = CommandType.StoredProcedure;
                SqlParameter param = sqlCmd.Parameters.AddWithValue("@regal", tower);
                param.SqlDbType = SqlDbType.Int;

                param = sqlCmd.Parameters.AddWithValue("@polki", CreateTrayTableFromTrayConfig(trayConfig));
                param.SqlDbType = SqlDbType.Structured;
                param.TypeName = "t_Polka";

                sqlCmd.ExecuteNonQuery();
                sqlc.Close();
            }
            catch
            {
                dt = null;
            }
            sqlc.Dispose();



        }


        #endregion 4eagleDB

        #endregion
    }
}
