using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCCloudService.BLL.CommonBLL;
using XCCloudService.BLL.Container;
using XCCloudService.BLL.IBLL.XCGameManager;
using XCCloudService.CacheService;
using XCCloudService.Common;
using XCCloudService.Model.CustomModel.XCGame;
using XCCloudService.Model.XCGameManager;

namespace XCCloudService.Business.XCGame
{
    public class MemberCardQueryBusiness
    {
        public static string SetMemberCardQueryBusiness(string iccard, out string name)
        {

            IMemberTokenService memberToken = BLLContainer.Resolve<IMemberTokenService>();
            //根据iccard查询membertoken表strordID
            var menlist = memberToken.GetModels(p => p.ICCardID.Equals(iccard, StringComparison.OrdinalIgnoreCase)).FirstOrDefault<T_MemberToken>();
            if (menlist != null)
            {
                IStoreService storeService = BLLContainer.Resolve<IStoreService>();
                int storeid = Convert.ToInt32(menlist.StoreId);
                //根据stroid查询t_store的对应数据库的名称
                var storelist = storeService.GetModels(p => p.id == storeid).FirstOrDefault<t_store>();
                if (storelist != null)
                {
                    String sql = String.Format("select row_number() over (order by RealTime) as No,CONVERT(varchar(100),f.RealTime , 20)as RealTimes,'{0}' as ICCardID,f.* from (SELECT f1.RealTime, ( CAST(f1.Balance AS decimal) - CAST(f1.Coins AS decimal) ) AS last_coins, f1.Coins AS save_coins, '0' AS used_coins, f1.Balance AS now_coins, '存币机存币' AS type,  f1.Segment+ ' | '+f1.HeadAddress  AS other_info FROM ( SELECT * FROM flw_485_savecoin WHERE ICCardID = {0} ) f1 UNION ALL SELECT f2.RealTime AS RealTime, ( CASE f2.CoinType WHEN '0' THEN '0' WHEN '3' THEN ( f2.Balance - f2.Coins ) ELSE ( f2.Balance + f2.Coins ) END ) AS last_coins, ( CASE f2.CoinType WHEN '3' THEN f2.Coins ELSE '0' END ) AS save_coins, ( CASE f2.CoinType WHEN '1' THEN f2.Coins WHEN '4' THEN f2.Coins ELSE '0' END ) AS used_coins, f2.Balance AS now_coins, ( CASE f2.CoinType WHEN '0' THEN '实物投币' WHEN '1' THEN '刷卡投币' WHEN '2' THEN '实物退币' WHEN '3' THEN 'IC卡出币' WHEN '4' THEN '数字币投币' END ) AS type,  g.GameName+ ' | '+ h.HeadID + case when DeivceName='' then '' else ' | ' + DeivceName end AS other_info FROM ( SELECT s1.*,ISNULL( s2.DeivceName,'') as DeivceName FROM flw_485_coin s1 left join flw_Push_Coin s2 on s1.ID=s2.FlwID WHERE ICCardID = {0} ) f2 LEFT JOIN t_head h ON f2.HeadAddress = h.HeadAddress AND f2.Segment = h.Segment LEFT JOIN t_game g ON h.GameID = g.GameID UNION ALL SELECT f3.RealTime, ( f3.Balance + f3.Coins ) AS last_coins, '0' AS save_coins, f3.Coins AS used_coins, f3.Balance AS now_coins, ( CASE f3.FlowType WHEN '0' THEN '退币' WHEN '1' THEN '退卡' END ) AS type,  CONVERT(varchar(20),f3.UserID)+ ' | '+ f3.WorkStation  AS other_info FROM ( SELECT * FROM flw_coin_exit WHERE ICCardID = {0} ) f3 UNION ALL SELECT f4.RealTime, ( CASE f4.WorkType WHEN '3' THEN ( f4.Balance + f4.Coins ) WHEN '7' THEN ( f4.Balance + f4.Coins ) WHEN '6' THEN ( f4.Balance + f4.Coins ) ELSE ( f4.Balance - f4.Coins ) END ) AS last_coins, ( CASE f4.WorkType WHEN '3' THEN '0' WHEN '7' THEN '0' ELSE f4.Coins END ) AS save_coins, ( CASE f4.WorkType WHEN '3' THEN f4.Coins WHEN '7' THEN f4.Coins ELSE '0' END ) AS used_coins, f4.Balance AS now_coins, ( CASE f4.WorkType WHEN '4' THEN '手工存币' WHEN '3' THEN '手工提币' WHEN '7' THEN '售币机提币' WHEN '6' THEN '自助机提币' WHEN '2' THEN '送币' WHEN '5' THEN '送币' WHEN '8' THEN '送币' END ) AS type,  f4.WorkStation + ' | ' + CONVERT(VARCHAR(20), f4.UserID) + ' | ' + MacAddress AS other_info FROM ( SELECT * FROM flw_coin_sale WHERE ICCardID = {0} ) f4 UNION ALL SELECT f5.RealTime, '0' AS last_coins, f5.CoinQuantity AS save_coins, '0' AS used_coins, f5.CoinQuantity AS now_coins, '数字币销售' AS type,  h5.FoodName+ ' | '+ g5.WorkStation AS other_info FROM ( SELECT * FROM flw_digite_coin_detial WHERE ICCardID = {0} ) f5 LEFT JOIN flw_digite_coin g5 ON f5.ID = g5.ID LEFT JOIN t_foods h5 ON h5.FoodID = g5.FoodID UNION ALL SELECT f6.RealTime, f6.Balance - f6.CoinQuantity AS last_coins, ( CASE f6.FlowType WHEN 'X' THEN '0' ELSE f6.CoinQuantity END ) AS save_coins, ( CASE f6.FlowType WHEN 'X' THEN f6.CoinQuantity ELSE '0' END ) AS used_coins, f6.Balance AS now_coins, ( CASE f6.FlowType WHEN '0' THEN '开户' WHEN '1' THEN '充值' WHEN 'X' THEN '退餐' END ) AS type,  fs6.FoodName+' | '+f6.WorkStation AS other_info FROM ( SELECT * FROM flw_food_sale WHERE ICCardID = {0} ) f6 LEFT JOIN t_foods fs6 ON f6.FoodID = fs6.FoodID UNION ALL SELECT f9.RealTime, ( f9.Balance + f9.Coins ) AS last_coins, '0' AS save_coins, f9.Coins AS used_coins, f9.Balance AS now_coins, '购买商品' AS type, '销售流水号:'+CONVERT(varchar(20),f9.GoodsID) AS other_info FROM ( SELECT * FROM flw_goods WHERE ICCardID = {0} AND PayType = '2' ) f9 UNION ALL SELECT f10.RealTime, ( f10.Balance - f10.PointCoin ) AS last_coins, f10.PointCoin AS save_coins, '0' AS used_coins, f10.Balance AS now_coins, '积分兑币' AS type, fs10.RebateRuleName AS other_info FROM ( SELECT * FROM flw_rebate WHERE ICCardID = {0} ) f10 LEFT JOIN t_rebate fs10 ON f10.RebateRuleID = fs10.RebateRuleID UNION ALL SELECT f11.RealTime, ( f11.BalanceOut + f11.Coins ) AS last_coins, '0' AS save_coins, f11.Coins AS used_coins, f11.BalanceOut AS now_coins, '转出' AS type, CONVERT(varchar(500),f11.ICCardIDIn) AS other_info FROM ( SELECT * FROM flw_transfer WHERE ICCardIDOut = {0} ) f11 UNION ALL SELECT f12.RealTime, ( f12.BalanceIn - f12.Coins ) AS last_coins, f12.Coins AS save_coins, '0' AS used_coins, f12.BalanceIn AS now_coins, '转入' AS type, CONVERT(varchar(500),f12.ICCardIDOut) AS other_info FROM ( select * from flw_transfer where ICCardIDIn = {0} ) f12 UNION ALL SELECT f13.RealTime, ( f13.Balance + f13.Coins ) AS last_coins, '0' AS save_coins, f13.Coins AS used_coins, f13.Balance AS now_coins, '购买门票' AS type, '无' AS other_info FROM ( select * from flw_project_buy where ICCardID = {0} ) f13) f", iccard);
                    System.Data.DataSet ds = XCGameBLL.ExecuteQuerySentence(sql, storelist.store_dbname, null);
                    DataTable dt = ds.Tables[0];
                    if (dt.Rows.Count > 0)
                    {
                        var list = Utils.GetModelList<MenberICCardModel>(ds.Tables[0]).ToList();
                        int time = CacheExpires.CommonPageQueryDataCacheTime;
                        MemberICICard.Add(iccard, list, time);
                        name = "1";
                        return name;
                    }
                }
                
            }
            name = "";
            return name;
        }
    }
}
