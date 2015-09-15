var CallidForm;
var pageSize = 25;
/**********************************************************************站臺管理主頁面**************************************************************************************/
//料位管理Model
Ext.define('gigade.IlocChangeDetails', {
    extend: 'Ext.data.Model',
    fields: [
         //icd.icd_item_id,icd.icd_old_loc_id,icd.icd_new_loc_id,icd.icd_create_time,mu.user_username,pt.product_name 
         {name:"icd_id",type:"int"},
        { name: "icd_item_id", type: "string" },
        { name: "product_name", type: "string" },
        { name: "product_sz", type: "string" },
        { name: "prepaid", type: "string" },
        { name: "prepa_name", type: "string" },
        { name: "icd_old_loc_id", type: "string" },
        { name: "icd_new_loc_id", type: "string" },
        //{ name: "made_date", type: "string" },
        { name: "cde_dt_incr", type: "string" },
        //{ name: "cde_dt", type: "string" },
        { name: "prod_qty", type: "string" },
        { name: "pwy_dte_ctl", type: "string" },
        //{ name: "isjq", type: "string" },
        //{ name: "isgq", type: "string" },
        { name: "cde_dt_var", type: "string" },
        { name: "cde_dt_shp", type: "string" },
        { name: "icd_create_time", type: "string" },
        { name: "user_username", type: "string" },
        { name: "icd_status", type: "string" },
        { name: "icd_create_time",type:"string" }
    ]
});

var IlocChangeDetailsStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    pageSize: pageSize,
    model: 'gigade.IlocChangeDetails',
    proxy: {
        type: 'ajax',
        url: '/WareHouse/GetIlocChangeDetailList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
    //    autoLoad: true
});

Ext.define("gigade.IupcType", {
    extend: 'Ext.data.Model',
    fields: [
        { name: "ParameterCode", type: "string" },
        { name: "parameterName", type: "string" }
    ]
});
var IlocChangeStatus = Ext.create('Ext.data.Store', {
    model: 'gigade.IupcType',
    autoLoad: true,
    proxy: {
        type: 'ajax',
        url: "/WareHouse/GetIupcType?Type=icd_status",
        //     actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'data'
        }
    }
});

//var sm = Ext.create('Ext.selection.CheckboxModel', {
//    listeners: {
//        selectionchange: function (sm, selections) {
//            Ext.getCmp("gdIlocChangeDetails").down('#MoveTally').setDisabled(selections.length == 0);
//            Ext.getCmp("gdIlocChangeDetails").down('#MoveComplete').setDisabled(selections.length == 0);
//        }
//    }
//});
IlocChangeDetailsStore.on('beforeload', function () {
    Ext.apply(IlocChangeDetailsStore.proxy.extraParams, {
        productids: Ext.getCmp('productids').getValue(),
        oldilocid: Ext.getCmp('oldilocid').getValue(),
        newilocid: Ext.getCmp('newilocid').getValue(),
        start_time: Ext.getCmp('start_time').getValue(),//課程開始時間
        end_time: Ext.getCmp('end_time').getValue(),//課程結束時間
        startloc: Ext.getCmp('startloc').getValue(),
        endloc: Ext.getCmp('endloc').getValue(),
        Icd_status: Ext.getCmp('Icd_Status').getValue()
    });
});

function Query(x) {
    IlocChangeDetailsStore.removeAll();
    Ext.getCmp("gdIlocChangeDetails").store.loadPage(1, {
        params: {
            productids: Ext.getCmp('productids').getValue(),
            oldilocid: Ext.getCmp('oldilocid').getValue(),
            newilocid: Ext.getCmp('newilocid').getValue(),
            start_time: Ext.getCmp('start_time').getValue(),//課程開始時間
            end_time: Ext.getCmp('end_time').getValue(),//課程結束時間
            startloc: Ext.getCmp('startloc').getValue(),
            endloc: Ext.getCmp('endloc').getValue(),
            Icd_status: Ext.getCmp('Icd_Status').getValue()
        }
    });
}

Ext.onReady(function () {
    var frm = Ext.create('Ext.form.Panel', {
        id: 'frm',
        layout: 'anchor',
        height: 120,
        border: 0,
        bodyPadding: 10,
        width: document.documentElement.clientWidth,
        items: [
            {
                xtype: 'fieldcontainer',
                combineErrors: true,
                layout: 'hbox',
                items: [
                    {
                        xtype: 'textfield',
                        margin: '0 0 0 5',
                        allowBlank: true,
                        fieldLabel: "商品編號(可用,，|分割)",
                        id: 'productids',
                        name: 'productids',
                        labelWidth: 150,
                        regex: /^(\d+)([,，|]{1}\d+)*(\d+)*$/,
                        regexText: '輸入格式不符合要求'
                    },
                    {
                        xtype: 'textfield',
                        margin: '0 0 0 5',
                        fieldLabel: "原料位編號",
                        id: 'oldilocid',
                        name: 'oldilocid',
                        labelWidth: 90
                    },
                    {
                        xtype: 'textfield',
                        margin: '0 0 0 5',
                        fieldLabel: "新料位編號",
                        id: 'newilocid',
                        name: 'newilocid',
                        labelWidth: 90
                    },
                ]
            },
            {
                xtype: 'fieldcontainer',
                combineErrors: true,
                layout: 'hbox',
                items: [
                    {
                        xtype: "datefield",
                        editable: false,
                        fieldLabel: "搬移日期區間",
                        margin: '0 0 0 5',
                        id: 'start_time',
                        name: 'start_time',
                        format: 'Y/m/d',
                        listeners: {
                            select: function (a, b, c) {
                                var start = Ext.getCmp("start_time");
                                var end = Ext.getCmp("end_time");
                                
                                if (end.getValue() == null) {
                                    end.setValue(setNextMonth(start.getValue(), 1));
                                } else if (start.getValue() > end.getValue()) {
                                    Ext.Msg.alert(INFORMATION, "開始時間不能大於結束時間");
                                    end.setValue(setNextMonth(start.getValue(), 1));
                                }
                                //else if (end.getValue() > setNextMonth(start.getValue(), 1)) {
                                //    // Ext.Msg.alert(INFORMATION, DATE_LIMIT);
                                //    end.setValue(setNextMonth(start.getValue(), 1));
                                //}
                                //if (end.getValue() == null) {
                                //    end.setValue(setNextMonth(start.getValue(), 1));
                                //} else if (end.getValue() < start.getValue()) {
                                //    Ext.Msg.alert(INFORMATION, DATA_TIP);
                                //    start.setValue(setNextMonth(end.getValue(), -1));
                                //}
                                //else if (end.getValue() > setNextMonth(start.getValue(), 1)) {
                                //    //Ext.Msg.alert(INFORMATION, DATE_LIMIT);
                                //    end.setValue(setNextMonth(start.getValue(), 1));
                                //}
                            },
                            specialkey: function (field, e) {
                                if (e.getKey() == e.ENTER) {
                                    Query();
                                }
                            }
                        }
                    },
                    { xtype: 'displayfield', value: '~ ' },
                    {
                        xtype: "datefield",
                        editable: false,
                        id: 'end_time',
                        name: 'end_time',
                        format: 'Y/m/d',
                        listeners: {
                            select: function (a, b, c) {
                                var start = Ext.getCmp("start_time");
                                var end = Ext.getCmp("end_time");
                                var s_date = new Date(start.getValue());
                                var now_date = new Date(end.getValue());
                                if (start.getValue() != "" && start.getValue() != null) {
                                    if (end.getValue() < start.getValue()) {
                                        Ext.Msg.alert(INFORMATION, "結束時間不能小於開始時間");
                                        end.setValue(setNextMonth(start.getValue(), 1));
                                    }
                                    //else if (end.getValue() > setNextMonth(start.getValue(), 1)) {
                                    //    //Ext.Msg.alert(INFORMATION, DATE_LIMIT);
                                    //    start.setValue(setNextMonth(end.getValue(), -1));
                                    //}

                                } else {
                                    start.setValue(setNextMonth(end.getValue(), -1));
                                }
                                //if (start.getValue() != "" && start.getValue() != null) {
                                //    if (end.getValue() < start.getValue()) {
                                //        Ext.Msg.alert(INFORMATION, DATA_TIP);
                                //        end.setValue(setNextMonth(start.getValue(), 1));
                                //    }
                                //    else if (end.getValue() > setNextMonth(start.getValue(), 1)) {
                                //        //Ext.Msg.alert(INFORMATION, DATE_LIMIT);
                                //        start.setValue(setNextMonth(start.getValue(), -1));
                                //    }
                                //}
                                //else {
                                //    start.setValue(setNextMonth(end.getValue(), -1));
                                //}
                            },
                            specialkey: function (field, e) {
                                if (e.getKey() == e.ENTER) {
                                    Query();
                                }
                            }
                        }
                    },
                    {
                        xtype: 'textfield',
                        margin: '0 0 0 5',
                        fieldLabel: "新料位區間",
                        id: 'startloc',
                        name: 'startloc',
                        labelWidth: 90
                    },
                    { xtype: 'displayfield', margin: '0 5 0 5', value: '~ ' },
                    {
                        xtype: 'textfield',
                        margin: '0 0 0 5',
                        id: 'endloc',
                        name: 'endloc',
                        labelWidth: 90
                    },
                     {
                         xtype: 'combobox', //類型
                         editable: false,
                         queryMode: 'local',
                         id: 'Icd_Status',
                         fieldLabel: "搬移狀態",
                         name: 'Icd_Status',
                         store: IlocChangeStatus,
                         margin: '0 0 0 10',
                         //lastQuery: '',
                         labelWidth: 60,
                         displayField: 'parameterName',
                         valueField: 'ParameterCode',
                        // allowBlank: false,
                         //emptyText: "请选择搬移狀態",
                         listeners: {
                             beforerender: function () {
                                 IlocChangeStatus.load({
                                     callback: function () {
                                         IlocChangeStatus.insert(0, { ParameterCode: '全部', parameterName: '全部' });
                                         Ext.getCmp("Icd_Status").setValue(IlocChangeStatus.data.items[0].data.ParameterCode);
                                     }
                                 });
                             }
                         }
                     },
                   
                ]
            },
            
            {
                xtype: 'fieldcontainer',
                combineErrors: true,
                layout: 'hbox',
                margin: '10 0 0 0',
                items: [
                    {
                        xtype: 'button',
                        margin: '0 8 0 8',
                        iconCls: 'icon-search',
                        text: "查詢",
                        handler: Query
                    },
                    {
                        xtype: 'button',
                        text: '重置',
                        id: 'btn_reset',
                        iconCls: 'ui-icon ui-icon-reset',
                        listeners: {
                            click: function () {
                                Ext.getCmp("Icd_Status").setValue("全部");
                                Ext.getCmp("productids").setValue("");
                                Ext.getCmp('oldilocid').setValue("");
                                Ext.getCmp('newilocid').setValue("");
                                Ext.getCmp('start_time').setValue("");//開始時間--time_start--delivery_date
                                Ext.getCmp('end_time').setValue("");//結束時間--time_end--delivery_date
                                Ext.getCmp('startloc').setValue("");//開始時間--time_start--delivery_date
                                Ext.getCmp('endloc').setValue("");//結束時間--time_end--delivery_date
                            }
                        }
                    }
                ]
            }
        ]
    });
    
    var gdIlocChangeDetails = Ext.create('Ext.grid.Panel', {
        id: 'gdIlocChangeDetails',
        store: IlocChangeDetailsStore,
        flex: 9.4,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        columns: [
            { header: "商品編號", dataIndex: 'icd_item_id', width: 60, align: 'center' },
            { header: "商品名稱", dataIndex: 'product_name', width: 150, align: 'center' },
            { header: "規格", dataIndex: 'product_sz', width: 120, align: 'center' },
            { header: "是否買斷", dataIndex: 'prepa_name', width: 60, align: 'center' },
            { header: "原料位編號", dataIndex: 'icd_old_loc_id', width: 80, align: 'center' },
            { header: "新料位編號", dataIndex: 'icd_new_loc_id', width: 80, align: 'center' },
            //{
            //    header: "製造日期", dataIndex: 'made_date', width: 120, align: 'center',
            //    renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
            //        if (value.substr(0, 10) == "0001-01-01") {
            //            return "N/A";
            //        }
            //        else {
            //            return value;
            //        }
            //    }
            //},
           // { header: "保存期限", dataIndex: 'cde_dt_incr', width: 60, align: 'center' },
            //{
            //    header: "有效日期", dataIndex: 'cde_dt', width: 120, align: 'center',
            //    renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
            //        if (value.substr(0, 10) == "0001-01-01") {
            //            return "N/A";
            //        }
            //        else {
            //            return value;
            //        }
            //    }
            //},
            {header:"搬移日期",dataIndex:'icd_create_time',width:120,align:'center'},
            { header: "庫存", dataIndex: 'prod_qty', width: 80, align: 'center' },
            { header: "是否有效期控管", dataIndex: 'pwy_dte_ctl', width: 100, align: 'center' },
            //{ header: "是否即期", dataIndex: 'isjq', width: 60, align: 'center' },
            //{ header: "是否過期", dataIndex: 'isgq', width: 60, align: 'center' },
            { header: "允收天數", dataIndex: 'cde_dt_var', width: 60, align: 'center' },
            { header: "允出天數", dataIndex: 'cde_dt_shp', width: 60, align: 'center' },
            {
                header: "操作", dataIndex: 'icd_status', width: 60, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (value != "COM") {
                        return '<a href=javascript:TranToDetial("/WareHouse/IlocChangeDetailLink","' + record.data.icd_id + '")>搬移理貨</a>';
                    } else
                    {
                        return "已完成!"
                    }
                   
                }
            }
        ],
        tbar: [
              //{ xtype: 'button', text: "搬移理貨", id: 'MoveTally',  iconCls: 'icon-user-edit', disabled: true, handler: onMoveTallyClick },
              //{ xtype: 'button', text: "搬移完成", id: 'MoveComplete',  iconCls: 'icon-user-edit', disabled: true, handler: onMoveComplete },
              { xtype: 'button', text: "匯出Excel", id: 'ExportOut', icon: '../../../Content/img/icons/excel.gif', handler: onExportOut }
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: IlocChangeDetailsStore,
            pageSize: pageSize,
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
        }
        //,
        //selModel: sm
    });

    Ext.create('Ext.container.Viewport', {
        layout: 'vbox',
        items: [frm,gdIlocChangeDetails],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                gdIlocChangeDetails.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
    ToolAuthority();
    //IlocChangeDetailsStore.load({ params: { start: 0, limit: 25 } });
});


onExportOut = function () {

    var url = "productids=" + Ext.getCmp('productids').getValue() + "&oldilocid=" + Ext.getCmp('oldilocid').getValue() + "&newilocid=" + Ext.getCmp('newilocid').getValue() + "&start_time=" + Ext.Date.format(new Date(Ext.getCmp('start_time').getValue()), 'Y-m-d H:i:s') + "&end_time=" + Ext.Date.format(new Date(Ext.getCmp('end_time').getValue()), 'Y-m-d H:i:s') + "&startloc=" + Ext.getCmp('startloc').getValue() + "&endloc=" + Ext.getCmp('endloc').getValue() + "&icd_status=" + Ext.getCmp('Icd_Status').getValue();
    window.open("/WareHouse/IlocChangeDetailExcelList?" + url);
}
function TranToDetial(url, deliver_id) {
    var urlTran = url + '?icd_id=' + deliver_id;
    var panel = window.parent.parent.Ext.getCmp('ContentPanel');
    var copy = panel.down('#IlocChangeDetailLink');
    if (copy) {
        copy.close();
    }
    copy = panel.add({
        id: 'IlocChangeDetailLink',
        title: "搬移理貨",
        html: window.top.rtnFrame(urlTran),
        closable: true
    });
    panel.setActiveTab(copy);
    panel.doLayout();
}

//onMoveTallyClick = function ()//只能選一個
//{
//    var row = Ext.getCmp("gdIlocChangeDetails").getSelectionModel().getSelection();
//    //alert(row[0]);
//    if (row.length == 0) {
       
//        Ext.Msg.alert(INFORMATION, NO_SELECTION);
//    } else if (row.length > 1) {
//        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
//    } else if (row.length == 1) {
//        //editFunction(row[0], IlocChangeDetailsStore);
      
//        var url = '/WareHouse/IlocChangeDetailLink?icd_id=' + row[0].data.icd_id;
//        var panel = window.parent.parent.Ext.getCmp('ContentPanel');
//        var copy = panel.down('#IlocChangeDetailLink');
//        if (copy) {
//            copy.close();
//        }
//        copy = panel.add({
//            id: 'IlocChangeDetailLink',
//            title: '搬移理貨',
//            html: window.top.rtnFrame(url),
//            closable: true
//        });
//        panel.setActiveTab(copy);
//        panel.doLayout();
//    }
//}
//onMoveComplete = function ()//批量處理
//{
   
//    var icd_idIn='';
//    var row = Ext.getCmp("gdIlocChangeDetails").getSelectionModel().getSelection();
//    Ext.Msg.confirm(CONFIRM, Ext.String.format("是否確定要刪除嗎?", row.length), function (btn) {
//        if (btn == 'yes') {
//            for (var i = 0; i < row.length; i++) {
//                icd_idIn += row[i].data.icd_id + ',';
//            }
//            Ext.Ajax.request({
//                url: '/WareHouse/GetIlocChangeDetailEdit',//執行方法
//                method: 'post',
//                params: {
//                    icd_id_In: icd_idIn
//                },
//                success: function (form, action) {
//                    var result = Ext.decode(form.responseText);
//                    if (result.success) {
//                        Ext.Msg.alert(INFORMATION, "操作成功!");
//                    } else {
//                        Ext.Msg.alert(INFORMATION, "操作失敗!");
//                    }
//                    IlocChangeDetailsStore.load();
//                },
//                failure: function () {
//                    Ext.Msg.alert(INFORMATION, "操作超時!");
//                    IlocChangeDetailsStore.load();
//                }
//            });
//        }
//    });
//}
setNextMonth = function (source, n) {
    var s = new Date(source);
    s.setMonth(s.getMonth() + n);
    if (n < 0) {
        s.setHours(0, 0, 0);
    } else if (n > 0) {
        s.setHours(23, 59, 59);
    }
    return s;
}
function Tomorrow() {
    var d;
    d = new Date();
    d.setDate(d.getDate() + 1);
    return d;
}