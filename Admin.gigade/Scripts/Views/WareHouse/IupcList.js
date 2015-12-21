var CallidForm;
var pageSize = 25;
//商品主料位管理Model
Ext.define('gigade.Iupc', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "row_id", type: "string" },
        { name: "upc_id", type: "string" },
        { name: "item_id", type: "string" },
        { name: "product_name", type: "string" },
        { name: "create_users", type: "string" },
        { name: "create_dtim", type: "string" },
        { name: "upc_type_flg", type: "string" },
         { name: "upc_type_flg_string", type: "string" }
       
    ]
});

var IupcStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    pageSize: pageSize,
    model: 'gigade.Iupc',
    proxy: {
        type: 'ajax',
        url: '/WareHouse/GetIupcList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
    //    autoLoad: true
});
//var SearchStore = Ext.create('Ext.data.Store', {
//    fields: ['txt', 'value'],
//    data: [
//        { "txt": ALL, "value": "0" },
//        { "txt": PRODID, "value": "1" },
//        { "txt": UPCID, "value": "2" }
//    ]
//});

Ext.define("gigade.IupcType", {
    extend: 'Ext.data.Model',
    fields: [
        { name: "ParameterCode", type: "string" },
        { name: "parameterName", type: "string" }
    ] 
});
var IupcTypeStore = Ext.create('Ext.data.Store', {
    model: 'gigade.IupcType',
    autoLoad: true,
    proxy: {
        type: 'ajax',
        url: "/WareHouse/GetIupcType?Type=iupc_type",
        //     actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'data'
        }
    }
});

IupcStore.on('beforeload', function () {
    Ext.apply(IupcStore.proxy.extraParams,
        {
            searchcontent: Ext.getCmp('searchcontent').getValue().trim(),
            time_start: Ext.getCmp('start_time').getValue(),
            time_end: Ext.getCmp('end_time').getValue()
        });
});

var sm = Ext.create('Ext.selection.CheckboxModel', {
    listeners: {
        selectionchange: function (sm, selections) {
            Ext.getCmp("gdIupc").down('#edit').setDisabled(selections.length == 0);
            Ext.getCmp("gdIupc").down('#delete').setDisabled(selections.length == 0);
        }
    }
});

function Query(x) {
    IupcStore.removeAll();
    var search = Ext.getCmp('searchcontent');
    var start = Ext.getCmp('start_time');
    var end = Ext.getCmp('end_time');
    if (search.getValue().trim() == "") {
        if (start.getValue() == null || end.getValue() == null) {
            Ext.Msg.alert("提示", "請輸入查詢時間或查詢內容");
            return;
        }
    }
    Ext.getCmp("gdIupc").store.loadPage(1, {
        params: {
            searchcontent: Ext.getCmp('searchcontent').getValue().trim(),
            time_start: Ext.getCmp('start_time').getValue(),
            time_end: Ext.getCmp('end_time').getValue()
        }
    });
}
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
Ext.onReady(function () {
    IupcTypeStore.load();
    var gdIupc = Ext.create('Ext.grid.Panel', {
        id: 'gdIupc',
        store: IupcStore,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        columns: [
            { header: ROWID, dataIndex: 'row_id', width: 90, align: 'center' },
            { header: PRODID, dataIndex: 'item_id', width: 130, align: 'center' },
            { header: PRODNAME, dataIndex: 'product_name', width: 250, align: 'center' },
            { header: UPCID, dataIndex: 'upc_id', width: 150, align: 'center' },
            { header: CREATEUSER, dataIndex: 'create_users', width: 100, align: 'center' },
            { header: CREATEDTIM, dataIndex: 'create_dtim', width: 150, align: 'center' },
            { header: "條碼類型", dataIndex: 'upc_type_flg_string', width: 150, align: 'center'}
        ],
        tbar: [
            { xtype: 'button', text: ADD, id: 'add', hidden: false, iconCls: 'icon-user-add', handler: onAddClick },
            { xtype: 'button', text: EDIT, id: 'edit', hidden: true, iconCls: 'icon-user-edit', disabled: true, handler: onEditClick },
            { xtype: 'button', text: "刪除", id: 'delete', hidden: true, iconCls: 'icon-user-remove', disabled: true, handler: onDeleteClick },
            { xtype: 'button', id: 'Export', text: "匯入Excel", icon: '../../../Content/img/icons/excel.gif', hidden: false, handler: ImportExcel },
            { xtype: 'button', text: "匯出Excel", id: 'exportExcel', icon: '../../../Content/img/icons/excel.gif', handler: ExportExcel },
            '->',
            {
                xtype: 'datetimefield',
                margin: '0 0 0 10',
                id: 'start_time',
                format: 'Y-m-d H:i:s',
                time: { hour: 00, min: 00, sec: 00 },
                fieldLabel: "創建時間",
                labelWidth: 70,
                editable: false,
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
                    },
                    specialkey: function (field, e) {
                        if (e.getKey() == Ext.EventObject.ENTER) {
                            Query();
                        }
                    }
                }
            },
            {
                xtype: 'displayfield',
                margin: '0 5 0 5',
                value: '~',
            },
            {
                xtype: 'datetimefield',
                id: 'end_time',
                format: 'Y-m-d  H:i:s',
                time: { hour: 23, min: 59, sec: 59 },
                editable: false,
                listeners: {
                    select: function (a, b, c) {
                        var start = Ext.getCmp("start_time");
                        var end = Ext.getCmp("end_time");                       
                        if (start.getValue() != "" && start.getValue() != null) {
                            if (end.getValue() < start.getValue()) {
                                Ext.Msg.alert(INFORMATION, "結束時間不能小於開始時間");
                                start.setValue(setNextMonth(end.getValue(), -1));
                            }
                            //else if (end.getValue() > setNextMonth(start.getValue(), 1)) {
                            //    start.setValue(setNextMonth(end.getValue(), -1));
                            //}
                        } else {
                            start.setValue(setNextMonth(end.getValue(), -1));
                        }
                    },
                    specialkey: function (field, e) {
                        if (e.getKey() == Ext.EventObject.ENTER) {
                            Query();
                        }
                    }
                }
            },
           {
               xtype: 'textfield', allowBlank: true, fieldLabel: "商品品號/條碼編號",
               id: 'searchcontent', name: 'searchcontent', labelWidth: 120, listeners: {
                   specialkey: function (field, e) {
                       if (e.getKey() == e.ENTER) {
                           Query();
                       }
                   }
               }
           },
           {
               xtype: 'button',
               text: SEARCH,
               iconCls: 'icon-search',
               id: 'btnQuery',
               handler: Query
           },
          {
              xtype: 'button',
              text: RESET,
              id: 'btn_reset',
              listeners: {
                  click: function () {
                      Ext.getCmp("searchcontent").setValue("");
                      Ext.getCmp("start_time").setValue(null);
                      Ext.getCmp("end_time").setValue(null);
                  }
              }
          }
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: IupcStore,
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
        },
        selModel: sm
    });

    Ext.create('Ext.container.Viewport', {
        layout: 'fit',
        items: [gdIupc],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                gdIupc.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
    ToolAuthority();
   // IupcStore.load({ params: { start: 0, limit: 25 } });
   
});

function Tomorrow() {
    var d;
    d = new Date();
    d.setDate(d.getDate() + 1);
    return d;
}
/*************************************************************************************新增*************************************************************************************************/
onAddClick = function () {
    //addWin.show();
    editFunction(null, IupcStore);
}
ImportExcel = function ()
{
    ExportFunction();
}

ExportExcel = function () {
  
    var time1 = "";
    var time2 = "";
    var start = Ext.getCmp("start_time").getValue();
    var end = Ext.getCmp("end_time").getValue();
    if(start!=null&&end!=null)
    {
        time1 = Ext.htmlEncode(Ext.Date.format(new Date(start), 'Y-m-d H:i:s'));
        time2 = Ext.htmlEncode(Ext.Date.format(new Date(end), 'Y-m-d H:i:s'))
    }
    window.open("/WareHouse/ReportManagementExcelList?searchcontent=" + Ext.getCmp("searchcontent").getValue() + "&time_start=" + time1 + "&time_end=" + time2);

}

/*************************************************************************************編輯*************************************************************************************************/
onEditClick = function () {
    var row = Ext.getCmp("gdIupc").getSelectionModel().getSelection();
    //alert(row[0]);
    if (row.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    } else if (row.length > 1) {
        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
    } else if (row.length == 1) {
        editFunction(row[0], IupcStore);
    }
}
/*********************************************刪除***************************************/
onDeleteClick = function () {
    var row = Ext.getCmp("gdIupc").getSelectionModel().getSelection();
    if (row.length < 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    } else {
        Ext.Msg.confirm(CONFIRM, Ext.String.format(DELETE_INFO, row.length), function (btn) {
            if (btn == 'yes') {
                var rowIDs = '';
                for (var i = 0; i < row.length; i++) {
                    rowIDs += row[i].data.row_id + '|';
                }
                Ext.Ajax.request({ 
                    url: '/WareHouse/IupcDelete',
                    method: 'post',
                    params: { rowID: rowIDs },
                    success: function (form, action) {
                        var result = Ext.decode(form.responseText);
                        Ext.Msg.alert(INFORMATION, SUCCESS);
                        if (result.success) {
                            IupcStore.load();
                        }
                    },
                    failure: function () {
                        Ext.Msg.alert(INFORMATION, FAILURE);
                    }
                });
            }
        });
    }
}






