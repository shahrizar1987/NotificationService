using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;
using NotificationService.App_Start;
using System.Configuration;

namespace NotificationService.Data
{
	public class Mcard
	{
        public string ConnectionStrings = ConfigurationManager.ConnectionStrings["mCard"].ConnectionString;

        public async Task<int> PushNotificationInfo_Add(string key_xCreatedBy, string DeviceID, string DeviceType, string Channel, string DeviceRegToken, string Topic, string LegacyServerKey, string SenderID, string MiscInfo1, string MiscInfo2, string MiscInfo3, string MiscInfo4, string MiscInfo5, string MiscInfo6, string MiscInfo7, string MiscInfo8, string MiscInfo9, string MiscInfo10)
		{
			var Result = 0;
			try
			{
				using (SqlConnection con = new SqlConnection(ConnectionStrings))
				{
					using (SqlCommand cmd = new SqlCommand("App.PushNotificationInfo_Add", con))
					{
						cmd.CommandType = CommandType.StoredProcedure;
						cmd.Parameters.Add("@key_xCreatedBy", SqlDbType.NVarChar, 200).Value = key_xCreatedBy;
						cmd.Parameters.Add("@DeviceID", SqlDbType.VarChar, 250).Value = DeviceID;
						cmd.Parameters.Add("@DeviceType", SqlDbType.NVarChar, 200).Value = DeviceType;
						cmd.Parameters.Add("@Channel", SqlDbType.VarChar, 32).Value = Channel;
						cmd.Parameters.Add("@DeviceRegToken", SqlDbType.NVarChar, -1).Value = DeviceRegToken;
						cmd.Parameters.Add("@Topic", SqlDbType.VarChar, 500).Value = Topic;
						cmd.Parameters.Add("@LegacyServerKey", SqlDbType.VarChar, 250).Value = LegacyServerKey;
						cmd.Parameters.Add("@SenderID", SqlDbType.VarChar, 250).Value = SenderID;
						cmd.Parameters.Add("@MiscInfo1", SqlDbType.VarChar, 250).Value = MiscInfo1;
						cmd.Parameters.Add("@MiscInfo2", SqlDbType.VarChar, 250).Value = MiscInfo2;
						cmd.Parameters.Add("@MiscInfo3", SqlDbType.VarChar, 250).Value = MiscInfo3;
						cmd.Parameters.Add("@MiscInfo4", SqlDbType.VarChar, 250).Value = MiscInfo4;
						cmd.Parameters.Add("@MiscInfo5", SqlDbType.VarChar, 250).Value = MiscInfo5;
						cmd.Parameters.Add("@MiscInfo6", SqlDbType.VarChar, 250).Value = MiscInfo6;
						cmd.Parameters.Add("@MiscInfo7", SqlDbType.VarChar, 250).Value = MiscInfo7;
						cmd.Parameters.Add("@MiscInfo8", SqlDbType.VarChar, 250).Value = MiscInfo8;
						cmd.Parameters.Add("@MiscInfo9", SqlDbType.VarChar, 250).Value = MiscInfo9;
						cmd.Parameters.Add("@MiscInfo10", SqlDbType.VarChar, 250).Value = MiscInfo10;
						cmd.Parameters.Add("@PkID", SqlDbType.Int, 4).Direction = ParameterDirection.Output;
						cmd.Parameters.Add("@ErrorTicket", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;

						con.Open();
						await cmd.ExecuteNonQueryAsync();
						Result = (int)(cmd.Parameters["@ErrorTicket"].Value);
					}
				}
			}
			catch (SqlException ex)
			{
                Result = -1;
                new Tools().Write(ex.Message, "ERROR-SQL");
			}
			return Result;
		}

        public async Task<FCMDetails> GetTerminalDetail(string TID, string MID)
        {
            FCMDetails fcm = new FCMDetails();

            StringBuilder sql = new StringBuilder();
            sql.Append("SELECT TOP 1 A.DeviceRegToken, A.LegacyServerKey, A.Topic ");
            sql.Append("FROM App.PushNotificationInfo AS A ");
            sql.Append("INNER JOIN App.TermAppAcct AS B ON A.DeviceID=B.DeviceSerialNo ");
            sql.Append("WHERE B.TID = @TID AND B.MID = @MID ");
            sql.Append("ORDER BY B.xDateCreated DESC");

            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionStrings))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(sql.ToString(), conn))
                    {
                        cmd.Parameters.Add(new SqlParameter("TID", TID));
                        cmd.Parameters.Add(new SqlParameter("MID", MID));
                        using (SqlDataReader rd = await cmd.ExecuteReaderAsync())
                        {
                            if (await rd.ReadAsync())
                            {
                                fcm = new FCMDetails
                                {
                                    DeviceRegToken = rd.GetString(0),
                                    LegacyServerKey = rd.GetString(1),
                                    Topic = rd.GetString(2)
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return fcm;
        }

        public async Task<OutletDetails> GetOutletDetail(string MID)
        {
            OutletDetails fcm = new OutletDetails();

            StringBuilder sql = new StringBuilder();
            sql.Append("SELECT O.Phone1, O.Name FROM MMS.Outlet AS O ");
            sql.Append("INNER JOIN App.MercAppAcct as M ");
            sql.Append("on O.Id= M.OutletId ");
            sql.Append("WHERE M.MID = @MID");

            try
            {
                using (SqlConnection conn = new SqlConnection(ConnectionStrings))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(sql.ToString(), conn))
                    {
                        cmd.Parameters.Add(new SqlParameter("MID", MID));
                        using (SqlDataReader rd = await cmd.ExecuteReaderAsync())
                        {
                            if (await rd.ReadAsync())
                            {
                                fcm = new OutletDetails
                                {
                                    MobileNo = rd.GetString(0),
                                    MerchantName = rd.GetString(1),
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return fcm;
        }

        public async Task<int> SMSSend(string TxnID, string MobileNo, string Message)
        {
            int Result = -1;
            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionStrings))
                {
                    using (SqlCommand cmd = new SqlCommand("SMSGW.SMS_Send", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@sz_MesgID", SqlDbType.VarChar, 60).Value = TxnID;
                        cmd.Parameters.Add("@sz_DestAdd", SqlDbType.VarChar, 50).Value = MobileNo;
                        cmd.Parameters.Add("@sz_ClearText", SqlDbType.VarChar, 640).Value = Message;
                        cmd.Parameters.Add("@i_Silent", SqlDbType.Int, 1).Value = 0;
                        cmd.Parameters.Add("@ErrorTicket", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;

                        con.Open();
                        await cmd.ExecuteNonQueryAsync();
                        Result = (int)(cmd.Parameters["@ErrorTicket"].Value);
                    }
                }
            }
            catch (SqlException)
            {
                throw;
            }

            return Result;
        }
    }
}

