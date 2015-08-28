var pageSize = 25;


//簡訊查詢Model
Ext.define('gigade.Ipo', {
    extend: 'Ext.data.Model',
    fields: [
    { name: "row_id", type: "int" },
    { name: "po_id", type: "string" },
    { name: "vend_id", type: "string" },
    { name: "buyer", type: "string" },
    { name: "sched_rcpt_dt", type: "string" },
    { name: "po_type", type: "string" },
    { name: "po_type_desc", type: "string" },
    { name: "cancel_dt", type: "string" },
    { name: "msg1", type: "string" },
    { name: "msg2", type: "string" },
    { name: "msg3", type: "string" },
    { name: "create_user", type: "string" },
    { name: "create_dtim", type: "string" },//user_username
    { name: "status", type: "int" },
    { name: "user_username", type: "string" }
    ]
});

//
//簡訊查詢Store
var IpoStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    pageSize: pageSize,
    model: 'gigade.Ipo',
    proxy: {
        type: 'ajax',
        url: '/WareHouse/GetIpoList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
});
IpoStore.on('beforeload', function () {
    Ext.apply(IpoStore.proxy.extraParams, {
        Potype: Ext.getCmp('Poty').getValue(),
        Poid: Ext.getCmp('searchcontent').getValue()
    })
});
Ext.define("gigade.parameter", {
    extend: 'Ext.data.Model',
    fields: [
        { name: "ParameterCode", type: "string" },
        { name: "parameterName", type: "string" }
    ]
});
var PoTypeStore = Ext.create('Ext.data.Store', {
    model: 'gigade.parameter',
    autoLoad: true,
    proxy: {
        type: 'ajax',
        url: "/WareHouse/GetPromoInvsFlgList?Type=po_type",
        //     actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'data'
        }
    }
});
var sm = Ext.create('Ext.selection.CheckboxModel', {
    listeners: {
        selectionchange: function (sm, selections) {
            Ext.getCmp("IpoView").down('#edit').setDisabled(selections.length == 0); 
            Ext.getCmp("IpoView").down('#delete').setDisabled(selections.length == 0);
           // Ext.getCmp("IpoView").down('#ExportEnter').setDisabled(selections.length == 0);
        }
    }
});
function Query(x) {
    var potys = Ext.getCmp('Poty').getValue();
    var Poid = Ext.getCmp('searchcontent').getValue();
    if (potys.trim() == "" && Poid.trim()=="") {
        Ext.Msg.alert("提示", "請輸入採購單號或選擇查詢類別!");
        return;
    }
    IpoStore.removeAll();
    Ext.getCmp("IpoView").store.loadPage(1);
}

Ext.onReady(function () {
   
    var IpoView = Ext.create('Ext.grid.Panel', {
        id: 'IpoView',
        store: IpoStore,
        flex: 8,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        columns: [
        { header: "採購單單號", dataIndex: 'po_id', width: 100, align: 'center' },
        {
            header: "廠商代號", dataIndex: 'vend_id', width: 100, align: 'center'
        },
        {
            header: "採購員編號", dataIndex: 'buyer', width: 130, align: 'center'
            //,
            //renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
            //    return "<a  href='javascript:void(0);' onclick='SecretLogin(" + record.data.id + ")'  >" + value + "</a>"
            //}
        },
        {
            header: "預計到貨日期", dataIndex: 'sched_rcpt_dt', width: 150, align: 'center',
            renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                if (value == "0001-01-01") {
                    return "-";
                } else
                {
                    return value;
                }

            }

        },
        { header: "採購單別", dataIndex: 'po_type', width: 60, align: 'center' },
        { header: "採購單別描述", dataIndex: 'po_type_desc', flex: 1, align: 'center' },
        { header: "備註一", dataIndex: 'msg1', flex: 1, align: 'center' },
        { header: "備註二", dataIndex: 'msg2', flex: 1, align: 'center' },
        { header: "備註三", dataIndex: 'msg3', flex: 1, align: 'center' },
        { header: "創建人", dataIndex: 'user_username', flex: 1, align: 'center' },
        { header: "創建時間", dataIndex: 'create_dtim', flex: 1, align: 'center' },
        {
            header: '功能', dataIndex: 'po_id', width: 60, align: 'center',
            renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
              
                    return '<a href=javascript:TranToIpod("' + record.data.po_id + '")>' + "查看" + '</a>';
              
            }
        }
        ],
        tbar: [
          //{ xtype: 'button', text:"新增", id: 'add', hidden: false, iconCls: 'icon-user-add', handler: onAddClick },
          { xtype: 'button', text: "編輯", id: 'edit', iconCls: 'icon-user-edit', disabled: true, handler: onEditClick },
          { xtype: 'button', text: "刪除", id: 'delete', iconCls: 'icon-user-remove', disabled: true, handler: onDeleteClick },
         
          "->",
           {
               xtype: 'combobox', //類型
               editable: false,
               id: 'Poty',
               fieldLabel: "採購單類別",
               name: 'Poty',
               labelWidth: 70,
               store: PoTypeStore,
               // margin: '20 0 0 0',
               lastQuery: '',
               displayField: 'parameterName',
               valueField: 'ParameterCode',
               emptyText: "请选择採購單類別",
               value:''
           },
            { xtype: 'button', text: "打印採購單", id: 'ExportEnter', icon: '../../../Content/img/icons/excel.gif', handler: onExportEnter },
         {
             id: 'searchcontent',
             xtype: 'textfield',
             fieldLabel: "採購單單號",
             
             labelWidth: 70,
             //regex: /^\d+$/,
             //regexText: '请输入正確的編號,訂單編號,行動電話進行查詢',
             name: 'searchcontent',
             allowBlank: true,
             listeners: {
                 specialkey: function (field, e) {
                     if (e.getKey() == Ext.EventObject.ENTER) {
                         Query();
                     }
                 }
             }
         },
            {
                xtype: 'button',
                text: SEARCH,
               
                margin: '0 0 0 0',
                id: 'btnQuery',
                iconCls: 'icon-search',
                handler: Query
            },
            {
                xtype: 'button',
                text: RESET,
                margin: '0 0 0 0',
                id: 'btn_reset',
                iconCls: 'ui-icon ui-icon-reset',
                listeners: {
                    click: function () {
                        Ext.getCmp("searchcontent").setValue('');
                        Ext.getCmp("Poty").setValue('');
                     
                    }
                }
            }
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: IpoStore,
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
        }, selModel: sm
    });

    Ext.create('Ext.container.Viewport', {
        layout: 'vbox',
        items: [ IpoView],//
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                IpoView.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
    ToolAuthority();
});

/*******************************新增*********************************************/
onAddClick = function () {
    editFunction(null, IpoView);
}
/************************編輯*********************************/
onEditClick = function () {
    var row = Ext.getCmp("IpoView").getSelectionModel().getSelection();
    //alert(row[0]);
    if (row.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    } else if (row.length > 1) {
        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
    } else if (row.length == 1) {
        editFunction(row[0], IpoView);
    }
}
/************************刪除**************************/
onDeleteClick = function () {
    var row = Ext.getCmp("IpoView").getSelectionModel().getSelection();
    if (row.length <= 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    } else {
        Ext.Msg.confirm(CONFIRM, Ext.String.format(DELETE_INFO, row.length), function (btn) {
            if (btn == 'yes') {
                var rowIDs = '';
                for (var i = 0; i < row.length; i++) {
                    rowIDs += row[i].data.row_id + ',';
                }

                Ext.Ajax.request({
                    url: '/WareHouse/DeleteIpo',
                    method: 'post',
                    params: {
                        rowId: rowIDs
                    },
                    success: function (form, action) {
                        Ext.Msg.alert(INFORMATION, SUCCESS);
                        IpoStore.load();

                    },
                    failure: function () {
                        Ext.Msg.alert(INFORMATION, FAILURE);
                        IpoStore.load();
                    }
                });
            }
        });
    }
}
/************************匯入 匯出**************************/
onExportEnter = function () {
    var potys = Ext.getCmp('Poty').getValue();
    var Poid= Ext.getCmp('searchcontent').getValue();
    if (potys.trim() =="") {
        Ext.Msg.alert("提示", "請選擇匯出類別!");
    } else {
        var Potype = Ext.getCmp('Poty').getValue();
        window.open("/WareHouse/WritePdf?Poid=" + Poid + "&Potype=" + Potype);
        
    }
}


function TranToIpod(po_id) {
    var url = '/WareHouse/Ipod?po_id=' + po_id;
    var panel = window.parent.parent.Ext.getCmp('ContentPanel');
    var copy = panel.down('#ipod');
    if (copy) {
        copy.close();
    }
    copy = panel.add({
        id: 'ipod',
        title: '採購單單身',
        html: window.top.rtnFrame(url),
        closable: true
    });
    panel.setActiveTab(copy);
    panel.doLayout();
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


