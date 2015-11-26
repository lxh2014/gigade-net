var pageSize = 25;
/*************初盤復盤管理主頁面****************/

//Model
Ext.define('gigade.Ilocs', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "cb_newid", type: "int" },
        { name: "iinvd_id", type: "int" },
        { name: "cb_jobid", type: "string" },
        { name: "made_dt", type: "string" },
        { name: "cde_dt", type: "string" },
        { name: "loc_id", type: "string" },
        { name: "prod_qty", type: "int" },
        { name: "cde_dt", type: "string" },
        { name: "create_datetime", type: "string" },
        { name: "user_username", type: "string" },
        { name: "item_id", type: "int" },
        { name: "st_qty", type: "int" },
        { name: "sta_id", type: "string" },
        { name: "product_name", type:"string" }
    ]
});

var CheckmanageStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    pageSize: pageSize,
    model: 'gigade.Ilocs',
    proxy: {
        type: 'ajax',
        url: '/WareHouse/GetMessage',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
    , autoLoad: false
});

CheckmanageStore.on('beforeload', function () {
    Ext.apply(CheckmanageStore.proxy.extraParams,
        {
            searchcontent: Ext.getCmp('searchcontent').getValue(),
            newid: Ext.getCmp('newid').getValue()
        });
});


Ext.onReady(function () {

    var CheckMange = Ext.create('Ext.grid.Panel', {
        id: 'CheckMange',
        store: CheckmanageStore,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        columns: [
            { header: "編號", dataIndex: 'cb_newid', width: 60, align: 'center' },
            { header: "iinvdrow_id", dateIndex: 'iinvd_id', width: 100, hidden: true, align: 'center' },
            { header: "iinvditem_id", dateIndex: 'item_id', width: 100, hidden: true, align: 'center' },
            { header: "cbjobdetailsta_id", dateIndex: 'sta_id', width: 100, hidden: true, align: 'center' },
            { header: "工作編號", dataIndex: 'cb_jobid', width: 150, align: 'center' },
            { header: "商品名稱", dataIndex: 'product_name', width: 180, align: 'center' },
            {
                header: "製造日期", dataIndex: 'made_dt', width: 120, align: 'center',
                renderer: Ext.util.Format.dateRenderer('Y-m-d'),
                editor: {
                    xtype: 'datefield',
                    format: 'Y-m-d',
                    allowBlank: false
                }
            },
            {
                header: "有效日期", dataIndex: 'cde_dt', width: 120, align: 'center',
                renderer: Ext.util.Format.dateRenderer('Y-m-d'),
                editor: {
                    xtype: 'datefield',
                    format: 'Y-m-d',
                    allowBlank: false
                }
            },
            {
                header: "庫存數量", dataIndex: 'prod_qty', width: 150, align: 'center'
            },
             {
                 header: "復盤數量", dataIndex: 'st_qty', width: 150, align: 'center',
                 editor: {
                     xtype: 'numberfield',
                     allowBlank: false,
                     minValue: 0
                 }
             },

            { header: "對應料位", dataIndex: 'loc_id', width: 150, align: 'center' },
            { header: "創建時間", dataIndex: 'create_datetime', width: 150, align: 'center' },
            { header: "創建人", dataIndex: 'user_username', width: 150, align: 'center' },
        ],
        selType: 'cellmodel',
        plugins: [
            Ext.create('Ext.grid.plugin.CellEditing', {
                clicksToEdit: 1
            })
        ],
        viewConfig: {
            emptyText: '<span>暫無數據！</span>'
        },
        tbar: [
            {
                xtype: 'button',
                text: "復盤完成",
                margin: '0 0 0 5',
                id: 'btnfpcomplate',
                hidden:true,
                handler: Stqtycomplate
            },
            {
                xtype: 'button',
                text: "蓋帳",
                margin: '0 0 0 5',
                hidden:true,
                id: 'btngaizhang',
                handler: Gaizhang
            },
             '->',
                  {
                      xtype: 'textfield', allowBlank: true, margin: '0 5 0 0', fieldLabel: '請輸入工作編號', id: 'searchcontent', name: 'searchcontent', labelWidth: 100
                  },
                   {
                       xtype: 'numberfield', allowBlank: true, minValue: 0, value: 0, fieldLabel: '編號', id: 'newid', name: 'newid', labelWidth: 70
                   },

           {
               xtype: 'button',
               text: "查詢",
               id: 'btnsure',
               handler: QueryChecks
           },
            {
                xtype: 'button',
                text: "刪除",
                id: 'btndeletes',
                handler: DeleteThis
            }

        ],
        dockedItems: [
       {   //類似于tbar
           xtype: 'toolbar',
           dock: 'top',
           items: [
            //  {
            //      xtype: 'button',
            //      text: "復盤完成",
            //      margin: '0 0 0 5',
            //      id: 'btnfpcomplate',
            //      hidden:true,
            //      handler: Stqtycomplate
            //  },
            //{
            //    xtype: 'button',
            //    text: "蓋帳",
            //    margin: '0 0 0 5',
            //    hidden:true,
            //    id: 'btngaizhang',
            //    handler: Gaizhang
            //}
           ]
       }],
        bbar: Ext.create('Ext.PagingToolbar', {
            id: 'pageToolBar',
            store: CheckmanageStore,
            pageSize: pageSize,
            autoDestroy: true,
            displayInfo: true,
            displayMsg: NOW_DISPLAY_RECORD + ': {0} - {1}' + TOTAL + ': {2}',
            emptyMsg: NOTHING_DISPLAY
        }),
        listeners: {
            scrollershow: function (scroller) {
                if (scroller && scroller.scrollEl) {
                    scroller.clearManagedListeners();
                    scroller.mon(scroller.scrollEl, 'scroll', scroller.onElScroll, scroller);
                }
            }

            , edit: function (editor, e) {
                if (e.field == "made_dt" || e.field == "cde_dt") {
                    //如果有效日期更改的話，就更新有效時間
                    if (e.record.data.sta_id == "COM" || e.record.data.sta_id == "END") {//如果已經完成盤點將不能夠再進行編輯修改
                        Ext.Msg.alert(INFORMATION, "該行數據已經完成盤點,不能再進行編輯!");
                        CheckmanageStore.load();
                    }
                    else {
                        var i = 0;
                        if (e.field == "made_dt") {
                            i = 1;//1表示編輯的是
                        }
                        else {
                            i = 2;//1表示編輯的是cde_dt
                        }
                        //var a = new Date(e.value);
                        //a = Ext.util.Format.date(a, 'Y-m-d');
                        ////if (a == e.originalValue)
                        ////{
                        ////    alert("一樣子");
                        ////}
                        if (Ext.Date.format(e.value, 'Y-m-d') != e.originalValue) {
                         
                                Ext.Ajax.request({
                                    url: "/WareHouse/selectproductexttime",
                                    params: {
                                        item_id: e.record.data.item_id
                                    },
                                    success: function (response) {
                                        var result = Ext.decode(response.responseText);
                                        var datetimes = 0;
                                        datetimes = result.msg;
                                        Ext.Ajax.request({
                                            url: "/WareHouse/aboutmadetime",
                                            params: {
                                                cde_dtormade_dt: e.value,
                                                y_cde_dtormade_dt: e.originalValue,
                                                row_id: e.record.data.iinvd_id,
                                                prod_qtys: e.record.data.prod_qty,
                                                type_id: i,
                                                datetimeday: datetimes,
                                                sloc_id: e.record.data.loc_id,
                                                prod_id: e.record.data.item_id,
                                                gzbh: e.record.data.cb_jobid
                                            },
                                            success: function (response) {
                                                var result = Ext.decode(response.responseText);
                                                var message;
                                                switch (result.msg) {
                                                    case 1:
                                                        message = " 製造日期不能大於當前時間!";
                                                        Ext.Msg.alert(INFORMATION, message);
                                                        break;
                                                    case 3:
                                                        message = " 修改失敗!";
                                                        Ext.Msg.alert(INFORMATION, message);
                                                        break;
                                                        CheckmanageStore.load();
                                                }

                                                if (result.msg == 2)//表示修改成功
                                                {
                                                    Ext.Ajax.request({
                                                        url: "/WareHouse/UpdateCbjobMaster",
                                                        params: {
                                                            gzbh: e.record.data.cb_jobid
                                                        },
                                                        success: function (response) {
                                                            var result = Ext.decode(response.responseText);
                                                            if (result.success) {
                                                                CheckmanageStore.load();
                                                            }
                                                        }
                                                    });
                                                }
                                            }
                                        });
                                    }
                                });
                            }
                        else {
                            CheckmanageStore.load();
                        }
                    }
                }
                //如果編輯的是轉移數量
                if (e.field == "st_qty") {
                    if (e.record.data.sta_id == "COM" || e.record.data.sta_id == "END") {//如果已經完成盤點將不能夠再進行編輯修改
                        Ext.Msg.alert(INFORMATION, "該行數據已經完成盤點,不能再進行編輯!");
                        CheckmanageStore.load();
                    }
                    else {
                        if (e.value != e.originalValue) {
                            Ext.Ajax.request({
                                url: "/WareHouse/Updateiinvdstqty",
                                method: 'post',
                                type: 'text',
                                params: {
                                    row_id: e.record.data.iinvd_id,
                                    stqty: e.value
                                },
                                success: function (form, action) {
                                    var result = Ext.decode(form.responseText);
                                    if (result.success) {
                                        Ext.Ajax.request({
                                            url: "/WareHouse/UpdateCbjobMaster",
                                            params: {
                                                gzbh: e.record.data.cb_jobid
                                            },
                                            success: function (response) {
                                                var result = Ext.decode(response.responseText);
                                                if (result.success) {
                                                    CheckmanageStore.load();
                                                }
                                            },
                                            failure: function (form, action) {
                                                Ext.Msg.alert(INFORMATION, "更新失敗!");
                                            }
                                        });
                                    }
                                },
                                failure: function (form, action) {
                                    Ext.Msg.alert(INFORMATION, "更新失敗!");
                                }
                            });
                        }
                    }
                }
            }
        }
    });

    Ext.create('Ext.container.Viewport', {
        layout: 'fit',
        items: [CheckMange],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                CheckMange.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
    ToolAuthority();
    CheckmanageStore.load({ params: { start: 0, limit: 25 } });
});


function QueryChecks() {
    CheckmanageStore.removeAll();
    Ext.getCmp("CheckMange").store.loadPage(1, {
        params: {
            searchcontent: Ext.getCmp('searchcontent').getValue(),
            newid: Ext.getCmp('newid').getValue()
        }
    });
}

function DeleteThis() {
    if (Ext.getCmp('searchcontent').getValue().trim() == "") {
        Ext.Msg.alert(INFORMATION, "請輸入工作編號");
    }
    else {
        Ext.Ajax.request({//調整庫存數量
            url: "/WareHouse/DeleteCbjobmessage",
            params: {
                searchcontent: Ext.getCmp('searchcontent').getValue()
            },
            success: function (response) {
                var result = Ext.decode(response.responseText);
                if (result.success) {
                    if (result.msg == 1) {
                        Ext.Msg.alert(INFORMATION, "刪除成功!");
                        CheckmanageStore.loadPage(1);
                    }
                    else if (result.msg == -2) {
                        Ext.Msg.alert(INFORMATION, "該工作編號不存在或已被刪除!");
                    }
                    else if (result.msg == -1) {
                        Ext.Msg.alert(INFORMATION, "該工作編號已被編輯不能進行刪除!");
                    }
                }
                else {
                    Ext.Msg.alert(INFORMATION, "刪除失敗!");
                }
            }
        });
    }
}

function Stqtycomplate()//復盤完成
{
    addFunction(null, CheckmanageStore);
}

function Gaizhang()//蓋帳
{

    addFunctiongaizhang(null, CheckmanageStore);

}