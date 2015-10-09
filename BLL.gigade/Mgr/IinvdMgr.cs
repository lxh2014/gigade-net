using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Dao.Impl;
using BLL.gigade.Dao;
using BLL.gigade.Model;
using BLL.gigade.Mgr.Impl;
using System.Data;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Mgr
{
    public class IinvdMgr:IinvdImplMgr
    {
        private IinvdImplDao _ivddao;
        private IstockChangeDao _istockdao;
        private IIplasImplDao _iplasdao;
        public IinvdMgr(string connectionString)
        {
            _ivddao = new IinvdDao(connectionString);
            _iplasdao = new IplasDao(connectionString);
            _istockdao = new IstockChangeDao(connectionString);
        }
        public int Insert(Iinvd i)
        {
            try
            {
                //IstockChange m = new IstockChange();
                //m.item_id = i.item_id;
                //m.sc_trans_type = 1;
                //m.sc_num_old = GetProqtyByItemid(int.Parse(i.item_id.ToString()));
                //m.sc_num_chg = i.prod_qty;
                //m.sc_num_new = GetProqtyByItemid(int.Parse(i.item_id.ToString())) + i.prod_qty;
                //m.sc_time = i.create_dtim;
                //m.sc_user = i.create_user;
                return _ivddao.Insert(i);

            }
            catch (Exception ex)
            {
                throw new Exception("IinvdMgr-->Insert-->" + ex.Message, ex);
            }
        }
        public int Upd(Iinvd i)
        {
            try
            {
                //IstockChange m = new IstockChange();
                //m.item_id = i.item_id;
                //m.sc_trans_type = 1;
                //m.sc_num_old = GetProqtyByItemid(int.Parse(i.item_id.ToString()));
                //m.sc_num_chg = i.prod_qty - GetProqtyByItemid(int.Parse(i.item_id.ToString()));
                //m.sc_num_new = i.prod_qty;
                //m.sc_time = i.change_dtim;
                //m.sc_user = i.create_user;
                //if (_istockdao.insert(m) > 0)
                //{
                //    return _ivddao.Upd(i);
                //}
                //else
                //{
                //    return 0;
                //}
                return _ivddao.Upd(i);
            }
            catch (Exception ex)
            {
                throw new Exception("IinvdMgr-->Upd-->" + ex.Message, ex);
            }
        }
        public DataTable Getprodu(int id)
        {
            try
            {
                return _ivddao.Getprodu(id);
            }
            catch (Exception ex)
            {
                throw new Exception("IinvdMgr-->Getprodu-->" + ex.Message, ex);
            }
        }
        public DataTable Getprodubybar(string id)
        {
            try
            {
                return _ivddao.Getprodubybar(id);
            }
            catch (Exception ex)
            {
                throw new Exception("IinvdMgr-->Getprodubybar-->" + ex.Message, ex);
            }
        }
        public int Islocid(string id, string zid, string prod_id)
        {
            try
            {
                return _ivddao.Islocid(id,zid, prod_id);
            }
            catch (Exception ex)
            {
                throw new Exception("IinvdMgr-->Islocid-->" + ex.Message, ex);
            }
        }

        public List<IinvdQuery> GetIinvdList(IinvdQuery ivd, out int totalCount)
        {
            try
            {
                if (!string.IsNullOrEmpty(ivd.serchcontent))
                {
                    switch (ivd.serch_type)
                    {
                        case 1:
                            ivd.serchcontent = _iplasdao.Getlocid(ivd.serchcontent.ToString());
                            break;
                        case 2:
                            ivd.serchcontent = _iplasdao.Getprodbyupc(ivd.serchcontent.ToString()).Rows[0]["item_id"].ToString();
                            break;
                        default:
                            break;
                    }
                }
                return _ivddao.GetIinvdList(ivd,out totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("IinvdMgr-->GetIinvdList-->" + ex.Message, ex);
            }
        }
        public int IsUpd(Iinvd i,int type=0)/*chaojie1124j添加，區分是庫存調整，還是收貨上架*/
        {
            try
            {
                IstockChange m = new IstockChange();
                m.item_id = i.item_id;
                if (type == 1)//收貨上架
                {
                    m.sc_trans_type = 1;//1.收貨上架,2表示庫調
                    m.sc_istock_why = 3;//3.收貨上架，2.庫調，1.庫鎖
                }
                if (type == 0)//庫存調整
                {
                    m.sc_trans_type = 2;
                    m.sc_istock_why = 2;//2表示庫調
                }
                m.sc_num_old = GetProqtyByItemid(int.Parse(i.item_id.ToString()));
                m.sc_num_chg = i.prod_qty;
                m.sc_num_new = GetProqtyByItemid(int.Parse(i.item_id.ToString())) + i.prod_qty;
                m.sc_time = i.change_dtim;
                m.sc_user = i.create_user;
               
                _istockdao.insert(m);
                return _ivddao.IsUpd(i);
            }
            catch (Exception ex)
            {
                throw new Exception("IinvdMgr-->IsUpd-->" + ex.Message, ex);
            }
        }
        public int Selnum(Iinvd m)
        {
            try
            {
                return _ivddao.Selnum(m);
            }
            catch (Exception ex)
            {
                throw new Exception("IinvdMgr-->Selnum-->" + ex.Message, ex);
            }
        }        
        public int UpdateIinvdLock(Iinvd i,IialgQuery q)
        {
            try
            {
                IstockChange m = new IstockChange();
                m.item_id = GetIinvd(i).FirstOrDefault().item_id;
                m.sc_trans_type = 1;
                m.sc_num_old = GetProqtyByItemid(int.Parse(m.item_id.ToString()));
                m.sc_num_chg = GetIinvd(i).FirstOrDefault().prod_qty;
                if (i.ista_id=="H")
                {
                    m.sc_num_chg = -m.sc_num_chg;                
                }
                m.sc_num_new = m.sc_num_old + m.sc_num_chg;
                m.sc_time = i.change_dtim;
                m.sc_user = i.change_user;
                m.sc_trans_id = q.po_id;
                m.sc_note = q.remarks;
                m.sc_istock_why = 1;
                if (_istockdao.insert(m) > 0)
                {
                    return _ivddao.UpdateIinvdLock(i);
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("IinvdMgr-->UpdateIinvdLock-->" + ex.Message, ex);
            }
        }
        public DataTable ExportExcel(IinvdQuery vd)
        {
            try
            {
                return _ivddao.ExportExcel (vd);
            }
            catch (Exception ex)
            {
                throw new Exception("IinvdMgr-->ExportExcel-->" + ex.Message, ex);
            }
        }
        public DataTable PastProductExportExcel(IinvdQuery vd)
        {
            try
            {
                return _ivddao.PastProductExportExcel(vd);
            }
            catch (Exception ex)
            {
                throw new Exception("IinvdMgr-->PastProductExportExcel-->" + ex.Message, ex);
            }
        }
        public List<IinvdQuery> KucunExport(IinvdQuery nvd)
        {
            try
            {
                return _ivddao.KucunExport(nvd);
            }
            catch (Exception ex)
            {
                throw new Exception("IinvdMgr-->KucunExport-->" + ex.Message, ex);
            }
        }
        public string UpdProdqty(Iinvd m)
        {
            try
            {
                return _ivddao.UpdProdqty(m);
            }
            catch (Exception ex)
            {
                throw new Exception("IinvdMgr-->UpdProdqty-->" + ex.Message, ex);
            }
        }
        public string InsertIinvdLog(IinvdLog il)
        {
            try
            {
                return _ivddao.InsertIinvdLog(il);
            }
            catch (Exception ex)
            {
                throw new Exception("IinvdMgr-->InsertIinvdLog-->" + ex.Message, ex);
            }
        }
        public int kucunTiaozheng(Iinvd invd)
        {
            try
            {
                return _ivddao.kucunTiaozheng(invd);
            }
            catch (Exception ex)
            {
                throw new Exception("IinvdMgr-->kucunTiaozheng-->" + ex.Message, ex);
            }
        }
        public DataTable GetRowMsg(Iinvd invd)
        {
            try
            {
                return _ivddao.GetRowMsg(invd);
            }
            catch (Exception ex)
            {
                throw new Exception("IinvdMgr-->GetRowMsg-->" + ex.Message, ex);
            }
        }

        public DataTable CountBook(IinvdQuery m)
        {
            try
            {
                
                return _ivddao.CountBook(m);
            }
            catch (Exception ex)
            {
                throw new Exception("IinvdMgr-->CountBook-->" + ex.Message, ex);
            }
        }
        public DataTable GetIplasCountBook(IinvdQuery m)
        {
            try
            {
                return _ivddao.GetIplasCountBook(m);
            }
            catch (Exception ex)
            {
                throw new Exception("IinvdMgr-->GetIplasCountBook-->" + ex.Message, ex);
            }
        }

        public int AboutItemidLocid(Iinvd invd)
        {
            try
            {
                return _ivddao.AboutItemidLocid(invd);
            }
            catch (Exception ex)
            {
                throw new Exception("IinvdMgr-->AboutItemidLocid-->" + ex.Message, ex);
            }
        }

        public int sum(Iinvd i, string lcat_id)
        {
            try
            {
                return _ivddao.sum(i, lcat_id);
            }
            catch (Exception ex)
            {
                throw new Exception("IinvdMgr-->sum-->" + ex.Message, ex);
            }
        }
        public int Updateiinvdstqty(Iinvd invd)
        {
            try
            {
                return _ivddao.Updateiinvdstqty(invd);
            }
            catch (Exception ex)
            {
                throw new Exception("IinvdMgr-->Updateiinvdstqty-->" + ex.Message, ex);
            }
        }
        public DataTable DifCountBook(IinvdQuery m)
        {
            try
            {
                return _ivddao.DifCountBook(m);
            }
            catch (Exception ex)
            {
                throw new Exception("IinvdMgr-->DifCountBook-->" + ex.Message, ex);
            }
        }
        public DataTable CountBookOBK(IinvdQuery m)
        {
            try
            {
                return _ivddao.CountBookOBK(m);
            }
            catch (Exception ex)
            {
                throw new Exception("IinvdMgr-->CountBookOBK-->" + ex.Message, ex);
            }
        }
        public List<Iinvd> GetIinvd(Iinvd i)
        {
            try
            {
                return _ivddao.GetIinvd(i);
            }
            catch (Exception ex)
            {
                throw new Exception("IinvdMgr-->GetIinvd-->" + ex.Message, ex);
            }
        }


        public List<IinvdQuery> GetIinvdExprotList(IinvdQuery ivd)
        {
            try
            {
                if (!string.IsNullOrEmpty(ivd.serchcontent))
                {
                    switch (ivd.serch_type)
                    {
                        case 1:
                            ivd.serchcontent = _iplasdao.Getlocid(ivd.serchcontent.ToString());
                            break;
                        case 2:
                            ivd.serchcontent = _iplasdao.Getprodbyupc(ivd.serchcontent.ToString()).Rows[0]["item_id"].ToString();
                            break;
                        default:
                            break;
                    }
                }
                return _ivddao.GetIinvdExprotList(ivd);
            }
            catch (Exception ex)
            {
                throw new Exception("IinvdMgr-->GetIinvdExprotList-->" + ex.Message, ex);
            }
        }
        public string remark(IinvdQuery q)
        {
            try
            {
                return _ivddao.remark(q);
            }
            catch (Exception ex)
            {
                throw new Exception("IinvdMgr-->GetIinvdExprotList-->" + ex.Message, ex);
            }
        }
        public string Getcost(string item_id)
        {
            try
            {
                if (!string.IsNullOrEmpty(item_id))
                {
                    return _ivddao.Getcost(item_id);
                }
                else
                {
                    return "";
                }
            }
            catch (Exception ex)
            {
                throw new Exception("IinvdMgr-->Getcost-->" + ex.Message, ex);
            }
        }
        public int SumProd_qty(Iinvd i) 
        {
            try
            {

                return _ivddao.SumProd_qty(i);
               
            }
            catch (Exception ex)
            {
                throw new Exception("IinvdMgr-->SumProd_qty-->" + ex.Message, ex);
            }
        }
        public DataTable Getloc()
        {
            DataTable dt =  _ivddao.Getloc();
            try
            {
                return dt;
            }
            catch (Exception ex)
            {
                throw new Exception("IinvdMgr-->SumProd_qty-->" + ex.Message, ex);
            }
        }
        public DataTable getproduct(IinvdQuery m)
        {
            DataTable dt = _ivddao.getproduct(m);
            try
            {
                return dt;
            }
            catch (Exception ex)
            {
                throw new Exception("IinvdMgr-->getproduct-->" + ex.Message, ex);
            }
        }

        public DataTable GetIinvdCountBook(IinvdQuery m)
        {
            DataTable dt = _ivddao.GetIinvdCountBook(m);
            try
            {
                return dt;
            }
            catch (Exception ex)
            {
                throw new Exception("IinvdMgr-->GetIinvdCountBook-->" + ex.Message, ex);
            }
        }



        public int GetProqtyByItemid(int item_id)
        {
            try
            {
                return  _ivddao.GetProqtyByItemid(item_id);
            }
            catch (Exception ex)
            {
                throw new Exception("IinvdMgr-->GetProqtyByItemid-->" + ex.Message, ex);
            }
        }
    }
}
