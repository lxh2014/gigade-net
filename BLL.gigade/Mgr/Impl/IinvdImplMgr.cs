﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BLL.gigade.Model;
using System.Data;
using BLL.gigade.Model.Query;

namespace BLL.gigade.Mgr.Impl
{
    public interface IinvdImplMgr
    {
        int Insert(Iinvd ivd);
        int Upd(Iinvd ivd);
        DataTable Getprodu(int id);
        DataTable Getprodubybar(string id);
        int Islocid(string id, string zid, string prod_id);
        List<Model.Query.IinvdQuery> GetIinvdList(Model.Query.IinvdQuery ivd, out int totalCount);
        int IsUpd(Iinvd m);
        int Selnum(Iinvd m);
        int UpdateIinvdLock(Iinvd nvd,IialgQuery q);
        DataTable ExportExcel(IinvdQuery vd);
        DataTable PastProductExportExcel(IinvdQuery vd);

        List<IinvdQuery> KucunExport(IinvdQuery nvd);
        string UpdProdqty(Iinvd m);
        string InsertIinvdLog(IinvdLog il);
        DataTable GetRowMsg(Iinvd invd);
        int kucunTiaozheng(Iinvd invd);//庫存調整
        DataTable CountBook(IinvdQuery m);
        int AboutItemidLocid(Iinvd invd);//判斷item_id和上架料位是否對應
        int sum(Iinvd i, string lcat_id);

        int Updateiinvdstqty(Iinvd invd);//更新st_qty字段
        DataTable DifCountBook(IinvdQuery m);
        DataTable CountBookOBK(IinvdQuery m);
        List<Iinvd> GetIinvd(Iinvd i);
        DataTable GetIplasCountBook(IinvdQuery m);
        List<IinvdQuery> GetIinvdExprotList(IinvdQuery iq);
        string remark(IinvdQuery q);
        string Getcost(string item_id);
        int SumProd_qty(Iinvd i);
        DataTable Getloc();
        DataTable getproduct(IinvdQuery m);
        DataTable GetIinvdCountBook(IinvdQuery m);
        int GetProqtyByItemid(int item_id);
    }
}
